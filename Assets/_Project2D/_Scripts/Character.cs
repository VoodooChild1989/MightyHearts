using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CharacterType
{
    Player, 
    Enemy
}

public enum MovementType
{
    Ground,
    Air
}

public enum FacingDirection
{
    Right,
    Left
}

public interface IDamageable
{
    public void TakeDamage(int damageAmount);
    public void Heal(int healAmount);
    // public void Death();
}

public interface ICards
{
    public void FirstCard();
    public void SecondCard();
    public void ThirdCard();
}

public interface IWait
{
    public void Wait();
}

public abstract class Character : MonoBehaviour, IDamageable, ICards, IWait
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("CHARACTER")]
            
            [Header("Data")]
            public int maxHealth;
            [ShowOnly] public int curHealth;
            public int maxStamina;
            [ShowOnly] public int curStamina;
            public int maxCooldown;
            [ShowOnly] public int curCooldown;
            public CharacterType curCharacterType;
            public MovementType curMovementType;
            public FacingDirection curFacingDirection;
            public List<GameObject> sizeMatrix;
            private Coroutine movementCrt;
            public bool isDead;

            [Header("UI")]
            public TMP_Text healthTMP;
            public TMP_Text staminaTMP;
            public TMP_Text cooldownTMP;

        [Header("ANIMATIONS")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public Sprite[] runningSprites;
            public Sprite[] attackSprites;
            public Sprite[] damagedSprites;
            public float frameLength;
            public float damagedFrameLength;
            public SpriteRenderer sr;
            public GameObject deathVFX;
            public Coroutine animationCrt;

        [Header("CARDS")]
            
            [Header("First Card")]
            public CardSO cardOne;

            [Header("Second Card")]
            public CardSO cardTwo;

            [Header("Third Card")]
            public CardSO cardThree;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        public void CharacterAwake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        public void CharacterStart()
        {
            if (curCharacterType == CharacterType.Player)
            {
                TurnToPlayer();
            }
            else if (curCharacterType == CharacterType.Enemy)
            {
                TurnToEnemy();
            }

            sizeMatrix = new List<GameObject>();

            curHealth = maxHealth;
            curStamina = maxStamina;
            curCooldown = 0;

            UpdateHealth(0);
            UpdateStamina(0);
            UpdateCooldown(0);

            SetIdleAnimation();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        public void CharacterUpdate()
        {
            // Add your per-frame logic here.
            // Example: Move objects, check user input, update animations, etc.
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        public void CharacterFixedUpdate()
        {
            // Add physics-related logic here.
            // Example: Rigidbody movement, applying forces, or collision detection.
        }

    #endregion

    #region SETUP

        private void TurnToPlayer()
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            Material mat = Resources.Load<Material>("Materials/Player_Outline");
            sr.material = mat;
        }

        private void TurnToEnemy()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            Material mat = Resources.Load<Material>("Materials/Enemy_Outline");
            sr.material = mat;
            Flip(false);
        }

        private void CheckCards()
        {
            if ((curCharacterType == CharacterType.Player && !LevelManager.instance.isPlayerAuto) || (curCharacterType == CharacterType.Enemy && !LevelManager.instance.isEnemyAuto))
            {
                if (LevelManager.instance.areCardsOpen)
                {
                    LevelManager.instance.RemoveCards();
                }
            }   
        }

        private void OnDestroy() 
        {
            if (curCharacterType == CharacterType.Player)
            {
                LevelManager.instance.RemovePlayer(this);
            }
            else if (curCharacterType == CharacterType.Enemy)
            {
                LevelManager.instance.RemoveEnemy(this);
            }
        }
        
    #endregion

    #region CUSTOM METHODS

        public void Ready()
        {
            TurnStarted();

            if ((curCharacterType == CharacterType.Player && LevelManager.instance.isPlayerAuto) || (curCharacterType == CharacterType.Enemy && LevelManager.instance.isEnemyAuto))
            {
                int rndNum = UnityEngine.Random.Range(1, 3);
                
                if (rndNum == 1)
                {
                    StartAutoAttack();
                }
                else
                {
                    StartAutoMove();
                }
            }
            else
            {
                movementCrt = StartCoroutine(Movement());
                LevelManager.instance.ShowCards(cardOne, cardTwo, cardThree);

                Button card01 = LevelManager.instance.cards[0];
                Button card02 = LevelManager.instance.cards[1];
                Button card03 = LevelManager.instance.cards[2];
                Button waitButton = LevelManager.instance.waitButton;
                
                card01.onClick.RemoveAllListeners();
                card02.onClick.RemoveAllListeners();
                card03.onClick.RemoveAllListeners();
                waitButton.onClick.RemoveAllListeners();

                card01.onClick.AddListener(FirstCard);   
                card02.onClick.AddListener(SecondCard);   
                card03.onClick.AddListener(ThirdCard);    
                waitButton.onClick.AddListener(Wait);   

                card01.gameObject.GetComponent<CardButtonDisplay>().card = cardOne;
                card02.gameObject.GetComponent<CardButtonDisplay>().card = cardTwo;
                card03.gameObject.GetComponent<CardButtonDisplay>().card = cardThree;
            }
        }

        public void AddCellToSizeMatrix(GameObject cell)
        {
            if (!sizeMatrix.Contains(cell))
            {
                sizeMatrix.Add(cell);
            }
        }

        public void RemoveCellFromSizeMatrix(GameObject cell)
        {
            if (sizeMatrix.Contains(cell))
            {
                sizeMatrix.Remove(cell);
            }   
        }

        public void TurnStarted()
        {
            LevelManager.instance.cinemCamera.Follow = gameObject.transform;
        }

        public void TurnFinished()
        {
            CheckCards();
            RemoveCooldown(curCooldown);
            
            // Used with manual movement.
            if (movementCrt != null)
            {
                StopCoroutine(movementCrt);
                movementCrt = null;
            }
        }

    #endregion

    #region AUTO

        private void StartAutoAttack()
        {
            StartCoroutine(AutoAttack());
        }

        private IEnumerator AutoAttack()
        {
            yield return new WaitForSeconds(1f);

            bool canAttack = false;

            for (int i = 1; i <= 3; i++)
            {
                if (i == 1)
                {
                    if (curStamina >= cardOne.staminaCost)
                    {
                        FirstCard();
                        canAttack = true;
                        break;
                    }
                }
                else if (i == 2)
                {
                    if (curStamina >= cardTwo.staminaCost)
                    {
                        SecondCard();
                        canAttack = true;
                        break;
                    }
                }
                else if (i == 3)
                {
                    if (curStamina >= cardThree.staminaCost)
                    {
                        ThirdCard();
                        canAttack = true;
                        break;
                    }
                }
            }

            if (!canAttack) 
            {
                int rndNum = UnityEngine.Random.Range(1, 3);
                
                if (rndNum == 1)
                {
                    Flip();
                }
                else
                {
                    Wait();
                }
            }
        }

        private void StartAutoMove()
        {
            StartCoroutine(AutoMove());
        }

        private IEnumerator AutoMove()
        {
            yield return new WaitForSeconds(1f);

            bool canMove = false;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    if (IsThisNewPositionPossible(i, j))
                    {
                        Move(i, j);   
                        canMove = true;
                        break;
                    }
                }

                if (canMove) break;
            }

            if (!canMove)
            {   
                int rndNum = UnityEngine.Random.Range(1, 3);
                
                if (rndNum == 1)
                {
                    Flip();
                }
                else
                {
                    Wait();
                }
            }
        }

    #endregion

    #region MANUAL
    
        private IEnumerator Movement() 
        {
            while (true)
            {   
                int newX = 0;
                int newY = 0;

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    newX = -1;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    newX = 0;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    newX = 1;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    newX = -1;
                    newY = 0;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    newX = 1;
                    newY = 0;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    newX = -1;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {                    
                    newX = 0;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.C))
                { 
                    newX = 1;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY))
                    {
                        CheckCards();
                        Move(newX, newY);
                        yield break;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    yield return null;
                    Flip();
                    yield break;
                }

                yield return null;
            }
        }
        
    #endregion

    #region ANIMATION

        private IEnumerator Animation(Sprite[] sprites, bool playOnce = false, bool useNewSpeed = false, float newSpeed = 0f)
        {
            while (true)
            {
                foreach (Sprite sprite in sprites)
                {
                    sr.sprite = sprite;

                    if (!useNewSpeed)
                    {
                        yield return new WaitForSeconds(frameLength);
                    }
                    else
                    {
                        yield return new WaitForSeconds(newSpeed);
                    }
                }

                if (playOnce) 
                {
                    SetIdleAnimation();
                    yield break;
                }
            }
        }

        private void SetIdleAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(idleSprites));
        }

        private void SetRunningAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(runningSprites));
        }

        public void SetAttackAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(attackSprites, true));
        }

        public void SetDamagedAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(damagedSprites, true, true, damagedFrameLength));
        }

        private void CheckAnimationCoroutine()
        {
            if (animationCrt != null)
            {
                StopCoroutine(animationCrt);
                animationCrt = null;
            }
        }

    #endregion
    
    #region MOVEMENT

        public void Move(int moveX, int moveY)
        {
            Vector2 curPos = transform.position;
            Vector2 newPos = new Vector2(curPos.x + moveX, curPos.y + moveY);

            if (TilemapManager.instance.IsNewPositionPossible(gameObject, sizeMatrix, moveX, moveY, curMovementType))
            {
                StartCoroutine(MoveToNewPos(newPos));
            }
        }

        private IEnumerator MoveToNewPos(Vector3 newPos)
        {
            SetRunningAnimation();
            
            while (transform.parent.position != newPos)
            {
                transform.parent.position = Vector2.MoveTowards(transform.parent.position, newPos, 0.05f);

                yield return new WaitForSeconds(0.01f);
            }

            transform.parent.position = newPos;   

            if (curMovementType == MovementType.Ground)
            {
                yield return new WaitForSeconds(0.1f);

                if (IsThisNewPositionPossible(0, -1))
                {
                    yield return new WaitForSeconds(0.05f);

                    Move(0, -1);   
                }
                else
                {
                    TurnFinished();
                    SetIdleAnimation();
                    LevelManager.instance.MoveQueue();
                }
            }
            else
            {
                TurnFinished();
                SetIdleAnimation();
                LevelManager.instance.MoveQueue();
            }
        }

        private bool IsThisNewPositionPossible(int moveX, int moveY)
        {
            Vector2 curPos = transform.position;
            Vector2 newPos = new Vector2(curPos.x + moveX, curPos.y + moveY);

            return TilemapManager.instance.IsNewPositionPossible(gameObject, sizeMatrix, moveX, moveY, curMovementType);
        }

        public void Flip(bool useTurnFinished = true)
        {
            if (curFacingDirection == FacingDirection.Right)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                curFacingDirection = FacingDirection.Left;
            }
            else 
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                curFacingDirection = FacingDirection.Right;
            }

            if (useTurnFinished) 
            {
                LevelManager.instance.MoveQueue();
                TurnFinished();
            }
        }

    #endregion

    #region HEALTH
        
        public void TakeDamage(int damageAmount)
        {
            SetDamagedAnimation();
            UpdateHealth(-damageAmount);
        }
    
        public void Heal(int healAmount)
        {
            UpdateHealth(healAmount);
        }

        void UpdateHealth(int amount)
        {
            curHealth += amount;

            if (curHealth > maxHealth) 
            {
                curHealth = maxHealth;
            }
            else if (curHealth <= 0) 
            {
                StartDeath();
            }

            healthTMP.text = curHealth.ToString() + "/" + maxHealth.ToString();            
        }
        
        private void StartDeath()
        {
            StartCoroutine(Death());
        }

        public IEnumerator Death()
        {
            isDead = true;
            CheckCards();

            yield return new WaitForSeconds(1f);

            Instantiate(deathVFX, transform.position, Quaternion.identity);
            Destroy(transform.parent.gameObject);
        }

    #endregion

    #region STAMINA

        public void AddStamina(int amount)
        {
            UpdateStamina(amount);
        }

        public void RemoveStamina(int amount)
        {
            UpdateStamina(-amount);
        }

        public void UpdateStamina(int amount)
        {
            curStamina += amount;

            if (curStamina > maxStamina)
            {
                curStamina = maxStamina;
            }
            else if (curStamina < 0)
            {
                curStamina = 0;
            }

            staminaTMP.text = curStamina.ToString() + "/" + maxStamina.ToString();   
        }

        public void Wait()
        {
            AddStamina(LevelManager.instance.defaultWaitReward);
            LevelManager.instance.MoveQueue();
            LevelManager.instance.RemoveCards();
            LevelManager.instance.MoveQueue();
            TurnFinished();
        }

    #endregion
 
    #region COOLDOWN

        public void AddCooldown(int amount)
        {
            UpdateCooldown(amount);
        }

        public void RemoveCooldown(int amount)
        {
            UpdateCooldown(-amount);
        }

        public void UpdateCooldown(int amount)
        {
            curCooldown += amount;

            if (curCooldown > maxCooldown)
            {
                curCooldown = maxCooldown;
            } 
            else if (curCooldown < 0) 
            {
                curCooldown = 0;
            }

            cooldownTMP.text = curCooldown.ToString() + "/" + maxCooldown.ToString();   
        }

    #endregion

    #region CARDS

        public abstract void FirstCard();
        public abstract void SecondCard();
        public abstract void ThirdCard();

    #endregion

}