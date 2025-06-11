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

public class Character : MonoBehaviour, IDamageable, ICards, IWait
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("CHARACTER")]
            
            [Header("Data")]
            public int maxHealth = 10;
            [ShowOnly] public int curHealth;
            public int maxStamina = 20;
            [ShowOnly] public int curStamina;
            public int maxCooldown = 3;
            public int waitReward = 5;
            [ShowOnly] public int curCooldown;
            [ShowOnly] public bool isDead;
            [ShowOnly] public CharacterType curCharacterType;
            [ShowOnly] public MovementType curMovementType;
            [ShowOnly] public FacingDirection curFacingDirection;
            private List<GameObject> sizeMatrix;
            private Coroutine movementCrt;
            private bool onTurn;
            private bool choseCard;
            public int moveSteps;
            public int moveStaminaCost;
            private bool canSwitchMoveTypes;

            [Header("UI")]
            private TMP_Text healthTMP;
            private TMP_Text staminaTMP;
            private TMP_Text cooldownTMP;
            private Image moveTypeIcon;

        [Header("ANIMATIONS")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public Sprite[] runningSprites;
            public Sprite[] attackSprites;
            public Sprite[] damagedSprites;
            [ShowOnly] public float frameLength = 0.1f;
            [ShowOnly] public float damagedFrameLength = 1f;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public GameObject deathVFX;
            private Coroutine animationCrt;

        [Header("CARDS")]
            
            [Header("Card One")]
            public CardSO cardOne;
            [ShowOnly] public CardSO cardOneOriginal;
            public Transform cardOneSpawnPoint;
            public int cardOneAttackWaves = 1;

            [Header("Card Two")]
            public CardSO cardTwo;
            [ShowOnly] public CardSO cardTwoOriginal;
            public Transform cardTwoSpawnPoint;
            public int cardTwoAttackWaves = 1;

            [Header("Card Three")]
            public CardSO cardThree;
            [ShowOnly] public CardSO cardThreeOriginal;
            public Transform cardThreeSpawnPoint;
            public int cardThreeAttackWaves = 1;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            if (curCharacterType == CharacterType.Player)
            {
                TurnToPlayer();
            }
            else if (curCharacterType == CharacterType.Enemy)
            {
                TurnToEnemy();
            }

            foreach (Transform child in transform.parent)
            {
                if (child.name == "Character_UI")
                {
                    foreach (Transform childChild in child.transform)
                    {
                        if (childChild.name == "Data")
                        {
                            foreach (Transform childChildChild in childChild.transform)
                            {
                                if (childChildChild.name == "Health")
                                {   
                                    healthTMP = childChildChild.GetComponent<TMP_Text>();
                                }
                                else if (childChildChild.name == "Stamina")
                                {   
                                    staminaTMP = childChildChild.GetComponent<TMP_Text>();
                                }
                                else if (childChildChild.name == "Cooldown")
                                {   
                                    cooldownTMP = childChildChild.GetComponent<TMP_Text>();
                                }
                            }
                        }
                        else if (childChild.name == "MoveTypeIcon")
                        {
                            moveTypeIcon = childChild.GetComponent<Image>();
                        }
                    }
                }
            }

            deathVFX = Resources.Load<GameObject>("Prefabs/VFX/Character_Death");
            sizeMatrix = new List<GameObject>();
            curHealth = maxHealth;
            curStamina = maxStamina;
            curCooldown = 0;

            UpdateHealth(0);
            UpdateStamina(0);
            UpdateCooldown(0);

            SetIdleAnimation();
            
            if (curMovementType == MovementType.Ground)
            {
                Sprite icon = LevelManager.instance.groundIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                moveTypeIcon.sprite = icon;
            }
            else if (curMovementType == MovementType.Air)
            {
                Sprite icon = LevelManager.instance.airIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                moveTypeIcon.sprite = icon;
            }

            canSwitchMoveTypes = false;        
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            if (onTurn)
            {
                if ((curCharacterType == CharacterType.Player && LevelManager.instance.isPlayerAuto) || (curCharacterType == CharacterType.Enemy && LevelManager.instance.isEnemyAuto))
                {
                    onTurn = false;

                    if (LevelManager.instance.areCardsOpen)
                        LevelManager.instance.RemoveCards();

                    DelayedAuto();
                }
            }
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        private void FixedUpdate()
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
        
    #endregion

    #region CUSTOM METHODS

        private void DelayedAuto()
        {
            StartCoroutine(DelayedAutoCoroutine());
        }

        private IEnumerator DelayedAutoCoroutine()
        {
            yield return new WaitForSeconds(1f);
            
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

        // !!!
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
                onTurn = true;
                movementCrt = StartCoroutine(Movement());
                LevelManager.instance.ShowCards(cardOne, cardTwo, cardThree);

                Button card01 = LevelManager.instance.cards[0];
                Button card02 = LevelManager.instance.cards[1];
                Button card03 = LevelManager.instance.cards[2];
                Button waitButton = LevelManager.instance.waitButton;
                Button addStepButton = LevelManager.instance.addStepButton;
                Button removeStepButton = LevelManager.instance.removeStepButton;
                UpdateStepsText();
                Button switchMove = LevelManager.instance.switchMovementTypesButton;
                
                card01.onClick.RemoveAllListeners();
                card02.onClick.RemoveAllListeners();
                card03.onClick.RemoveAllListeners();
                waitButton.onClick.RemoveAllListeners();
                addStepButton.onClick.RemoveAllListeners();
                removeStepButton.onClick.RemoveAllListeners();
                switchMove.onClick.RemoveAllListeners();

                card01.onClick.AddListener(FirstCard);   
                card02.onClick.AddListener(SecondCard);   
                card03.onClick.AddListener(ThirdCard);    
                waitButton.onClick.AddListener(Wait);     
                addStepButton.onClick.AddListener(AddStep);     
                removeStepButton.onClick.AddListener(RemoveStep); 
                switchMove.onClick.AddListener(SwitchMovementTypes);   

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
            moveSteps = 1;
            moveStaminaCost = 0;

            if (curMovementType == MovementType.Ground)
            {
                Sprite icon = LevelManager.instance.groundIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                moveTypeIcon.sprite = icon;
            }
            else if (curMovementType == MovementType.Air)
            {
                Sprite icon = LevelManager.instance.airIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                moveTypeIcon.sprite = icon;
            }    
            
            LevelManager.instance.switchMovementTypesButton.interactable = canSwitchMoveTypes;
        }

        public void TurnFinished()
        {
            onTurn = false;
            choseCard = false;
            CheckCards();
            RemoveCooldown(curCooldown);
            
            // Used with manual movement.
            if (movementCrt != null)
            {
                StopCoroutine(movementCrt);
                movementCrt = null;
            }
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("CardBooster"))
            {
                int num = UnityEngine.Random.Range(1, 4);
                int amount = UnityEngine.Random.Range(5, 11);

                if (num == 1)
                {
                    CardStamina(amount);
                }
                else if (num == 2)
                {
                    CardDamage(amount);
                }
                else if (num == 3)
                {
                    CardWave();
                }
                
                other.gameObject.GetComponent<Booster>().Death();
            }
            else if (other.gameObject.CompareTag("FlyBooster"))
            {            
                canSwitchMoveTypes = true;
                other.gameObject.GetComponent<Booster>().Death();
            }
            else if (other.gameObject.CompareTag("CharacterBooster"))
            {
                int num = UnityEngine.Random.Range(1, 4);

                if (num == 1) IncreaseMaxHealth();
                else if (num == 2) IncreaseMaxStamina();
                else if (num == 3) DecreaseMaxCooldown();
            
                other.gameObject.GetComponent<Booster>().Death();
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

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    newX = 0;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    newX = 1;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    newX = -1;
                    newY = 0;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    newX = 1;
                    newY = 0;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    newX = -1;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {                    
                    newX = 0;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.C))
                { 
                    newX = 1;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY) && curStamina >= moveStaminaCost)
                    {
                        CheckCards();
                        Move(newX, newY);
                        RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    yield return null;
                    CheckCards();
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
                StartCoroutine(MoveToNewPos(newPos, moveX, moveY));
            }
        }

        private IEnumerator MoveToNewPos(Vector3 newPos, int moveX, int moveY)
        {
            SetRunningAnimation();
            moveSteps--;    
            
            while (transform.parent.position != newPos)
            {
                transform.parent.position = Vector2.MoveTowards(transform.parent.position, newPos, 0.05f);

                yield return new WaitForSeconds(0.01f);
            }

            transform.parent.position = newPos;   

            if (curMovementType == MovementType.Ground)
            {
                yield return new WaitForSeconds(0.1f);

                if (moveSteps > 0)
                {
                    if (IsThisNewPositionPossible(moveX, moveY))
                    {
                        yield return new WaitForSeconds(0.05f);

                        Move(moveX, moveY);
                    }
                    else
                    {
                        moveSteps = 0;
                        
                        if (IsThisNewPositionPossible(0, -1))
                        {
                            yield return new WaitForSeconds(0.05f);

                            Move(0, -1);   
                        }
                        else
                        {
                            FinishMovement();
                        }
                    }
                }
                else
                {
                    if (IsThisNewPositionPossible(0, -1))
                    {
                        yield return new WaitForSeconds(0.05f);

                        Move(0, -1);   
                    }
                    else
                    {
                        FinishMovement();
                    }
                }
            }
            else
            {
                if (moveSteps > 0)
                {
                    if (IsThisNewPositionPossible(moveX, moveY))
                    {
                        yield return new WaitForSeconds(0.05f);

                        Move(moveX, moveY);
                    }
                    else
                    {
                        FinishMovement();
                    }
                }
                else
                {
                    FinishMovement();
                }
            }
        }

        public void AddStep()
        {   
            moveSteps++;
            moveStaminaCost += 10;

            UpdateStepsText();
        }

        public void RemoveStep()
        {
            moveSteps--;
            moveStaminaCost -= 10;

            if (moveSteps < 1) 
            {
                moveSteps = 1;
                moveStaminaCost = 0;
            }

            UpdateStepsText();
        }

        public void SwitchMovementTypes()
        {
            if (curMovementType == MovementType.Ground)
            {
                curMovementType = MovementType.Air;
                Sprite icon = LevelManager.instance.airIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                moveTypeIcon.sprite = icon;
            }
            else if (curMovementType == MovementType.Air)
            {
                curMovementType = MovementType.Ground;
                Sprite icon = LevelManager.instance.groundIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                moveTypeIcon.sprite = icon;
            }
        }

        public void UpdateStepsText()
        {
            LevelManager.instance.stepsNumber.text = moveSteps.ToString();
            LevelManager.instance.moveStaminaCost.text = moveStaminaCost.ToString();
        }

        private void FinishMovement()
        {
            TurnFinished();
            SetIdleAnimation();
            LevelManager.instance.MoveQueue();
        }

        private bool IsThisNewPositionPossible(int moveX, int moveY)
        {
            Vector2 curPos = transform.position;
            Vector2 newPos = new Vector2(curPos.x + moveX, curPos.y + moveY);

            return TilemapManager.instance.IsNewPositionPossible(gameObject, sizeMatrix, moveX, moveY, curMovementType);
        }

        public void Flip(bool useTurnFinished = true)
        {
            StartCoroutine(FlipCoroutine(useTurnFinished));
        }

        private IEnumerator FlipCoroutine(bool useTurnFinished = true)
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
                CheckCards();
                yield return new WaitForSeconds(1f);
                TurnFinished();
                LevelManager.instance.MoveQueue();
            }
        }

    #endregion

    #region HEALTH
        
        private void IncreaseMaxHealth()
        {
            int amount = UnityEngine.Random.Range(10, 31);
            maxHealth += amount;
            Heal(maxHealth - curHealth);
        }

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

        private void OnDestroy() 
        {
            if (curCharacterType == CharacterType.Player)
            {
                // LevelManager.instance.RemovePlayer(this);
            }
            else if (curCharacterType == CharacterType.Enemy)
            {
                // LevelManager.instance.RemoveEnemy(this);
            }
        }

    #endregion

    #region STAMINA

        private void IncreaseMaxStamina()
        {
            int amount = UnityEngine.Random.Range(10, 31);
            maxStamina += amount;
            AddStamina(maxStamina - curStamina);
        }

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
            AddStamina(waitReward);
            CheckCards();
            StartCoroutine(WaitCoroutine());
        }

        private IEnumerator WaitCoroutine()
        {
            yield return new WaitForSeconds(1f);
            TurnFinished();
            LevelManager.instance.MoveQueue();
        }

    #endregion
 
    #region COOLDOWN

        private void DecreaseMaxCooldown()
        {
            int amount = UnityEngine.Random.Range(1, 4);

            if (amount < maxCooldown)
            {
                maxCooldown -= amount;
            }
            else
            {
                maxCooldown = 1;
            }

            UpdateCooldown(0);
        }

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

        private void CardStamina(int amount)
        {
            if (cardOne.staminaCost > amount)
            {
                cardOne.staminaCost -= amount;
            }
            else if (cardTwo.staminaCost > amount)
            {
                cardTwo.staminaCost -= amount;
            }
            else if (cardThree.staminaCost > amount)
            {
                cardThree.staminaCost -= amount;
            }
            else
            {
                CardDamage(amount);
            }
        }

        private void CardDamage(int amount)
        {
            int cardNum = UnityEngine.Random.Range(1, 4);

            if (cardNum == 1) cardOne.damageAmount += amount;
            if (cardNum == 2) cardTwo.damageAmount += amount;
            if (cardNum == 3) cardThree.damageAmount += amount;
        }

        private void CardWave()
        {
            int cardNum = UnityEngine.Random.Range(1, 4);

            if (cardNum == 1)
            {
                cardOneAttackWaves++;
                cardOne.cardDescription = cardOneOriginal.cardDescription;
                cardOne.cardDescription += " " + cardOneAttackWaves + " waves.";
            }
            else if (cardNum == 2)
            {
                cardTwoAttackWaves++;
                cardTwo.cardDescription = cardTwoOriginal.cardDescription;
                cardTwo.cardDescription += " " + cardTwoAttackWaves + " waves.";
            }
            else if (cardNum == 3)
            {
                cardThreeAttackWaves++;
                cardThree.cardDescription = cardThreeOriginal.cardDescription;
                cardThree.cardDescription += " " + cardThreeAttackWaves + " waves.";
            }
        }

        public CardSO CloneCard(CardSO cardToClone, int cardNum)
        {
            CardSO clone = ScriptableObject.CreateInstance<CardSO>();

            clone.cardIcon = cardToClone.cardIcon;
            clone.cardName = cardToClone.cardName;
            clone.cardDescription = cardToClone.cardDescription;
            
            if (cardNum == 1) clone.cardDescription += " " + cardOneAttackWaves + " waves.";
            if (cardNum == 2) clone.cardDescription += " " + cardTwoAttackWaves + " waves.";
            if (cardNum == 3) clone.cardDescription += " " + cardThreeAttackWaves + " waves.";
            
            clone.staminaCost = cardToClone.staminaCost;
            clone.damageAmount = cardToClone.damageAmount;
            clone.damageAmount = cardToClone.damageAmount;
            clone.cardProjectile = cardToClone.cardProjectile;

            return clone;
        }

        public void FirstCard()
        {
            if (choseCard) return;

            if (curStamina >= cardOne.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardOne, 1));
            }   
        }
        
        public void SecondCard()
        {
            if (choseCard) return;

            if (curStamina >= cardTwo.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardTwo, 2));
            }
        }

        public void ThirdCard()
        {
            if (choseCard) return;

            if (curStamina >= cardThree.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardThree, 3));
            }
        }

        public IEnumerator CardCoroutine(CardSO card, int cardNum)
        {    
            RemoveStamina(card.staminaCost);
            choseCard = true;

            int attackWaves = 1;
            if (cardNum == 1) attackWaves = cardOneAttackWaves;
            else if (cardNum == 2) attackWaves = cardTwoAttackWaves;
            else if (cardNum == 3) attackWaves = cardThreeAttackWaves;
                
            for (int i = 1; i <= attackWaves; i++)
            {
                SetAttackAnimation();

                GameObject projToSpawn = Resources.Load<GameObject>("Prefabs/Prefab_Projectile_01");
                projToSpawn.GetComponent<HandleProjectileSO>().projSO = card.cardProjectile;

                Vector3 spawnPos = Vector3.zero;
                if (cardNum == 1) spawnPos = cardOneSpawnPoint.position;
                else if (cardNum == 2) spawnPos = cardTwoSpawnPoint.position;
                else if (cardNum == 3) spawnPos = cardThreeSpawnPoint.position;

                GameObject projectileObj = Instantiate(projToSpawn, spawnPos, Quaternion.identity);
                Projectile projectileScript = projectileObj.GetComponentInChildren<Projectile>();
                projectileScript.damageAmount = card.damageAmount;

                if (curCharacterType == CharacterType.Player)
                {
                    projectileScript.TurnToPlayer();
                }
                else if (curCharacterType == CharacterType.Enemy)
                {
                    projectileScript.TurnToEnemy();
                }
                
                if (curFacingDirection == FacingDirection.Left)
                {
                    projectileScript.Flip();
                }

                if (cardNum == 1)
                {
                    if (i == cardOneAttackWaves)
                    {
                        projectileScript.isLast = true;
                    }
                }
                else if (cardNum == 2)
                {
                    if (i == cardTwoAttackWaves)
                    {
                        projectileScript.isLast = true;
                    }
                }
                else if (cardNum == 3)
                {
                    if (i == cardThreeAttackWaves)
                    {
                        projectileScript.isLast = true;
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }

            TurnFinished();
        }

    #endregion

}