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

public interface IDamageable
{
    public void TakeDamage(int damageAmount);
    public void Heal(int healAmount);
}

public class CharacterStatistics : MonoBehaviour, IDamageable
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Health")]
            public int maxHealth = 10;
            [ShowOnly] public int curHealth;
            [ShowOnly] public bool isDead;

            [Header("Stamina")]
            public int maxStamina = 20;
            [ShowOnly] public int curStamina;

            [Header("Cooldown")]
            public int maxCooldown = 3;
            [ShowOnly] public int curCooldown;

            [Header("Miscellaneous")]
            public int waitReward = 5;
            [ShowOnly] public CharacterType curCharacterType;
            [ShowOnly] public bool onTurn;
            [ShowOnly] public bool choseCard;

        [Header("CARDS")]
            
            [Header("Card One")]
            public Transform cardOneSpawnPoint;
            [ShowOnly] public CardSO cardOne;
            [ShowOnly] public CardSO cardOneOriginal;

            [Header("Card Two")]
            public Transform cardTwoSpawnPoint;
            [ShowOnly] public CardSO cardTwo;
            [ShowOnly] public CardSO cardTwoOriginal;

            [Header("Card Three")]
            public Transform cardThreeSpawnPoint;
            [ShowOnly] public CardSO cardThree;
            [ShowOnly] public CardSO cardThreeOriginal;

        [Header("DISPLAY")]

            [Header("UI")]
            private TMP_Text healthTMP;
            private TMP_Text staminaTMP;
            private TMP_Text cooldownTMP;
            [ShowOnly] public Image moveTypeIcon;

        [Header("REFERENCES")]

            [Header("SCRIPTS")]
            [ShowOnly] public CharacterAnimation chrAnim;
            [ShowOnly] public CharacterMovement chrMove;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            chrAnim = GetComponent<CharacterAnimation>();
            chrMove = GetComponent<CharacterMovement>();
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
            
            curHealth = maxHealth;
            curStamina = maxStamina;
            curCooldown = 0;

            UpdateHealth(0);
            UpdateStamina(0);
            UpdateCooldown(0);
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

                    if (LevelManager.instance.areCardsOpen) LevelManager.instance.RemoveCards();

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

    #region CUSTOM METHODS

        private void TurnToPlayer()
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            Material mat = Resources.Load<Material>("Materials/Player_Outline");
            chrAnim.sr.material = mat;
        }

        private void TurnToEnemy()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            Material mat = Resources.Load<Material>("Materials/Enemy_Outline");
            chrAnim.sr.material = mat;
            chrMove.Flip(false);
        }

        public void TurnStarted()
        {
            LevelManager.instance.cinemCamera.Follow = gameObject.transform;
            chrMove.moveSteps = 1;
            chrMove.moveStaminaCost = 0;

            chrMove.UpdateMovementType();
        }

        public void TurnFinished()
        {
            onTurn = false;
            choseCard = false;
            CheckCards();
            RemoveCooldown(curCooldown);
            chrMove.StopManualMovement();
        }

        private void DelayedAuto()
        {
            StartCoroutine(DelayedAutoCoroutine());
        }

        private IEnumerator DelayedAutoCoroutine()
        {
            yield return new WaitForSeconds(1f);
            
            int rndNum = UnityEngine.Random.Range(1, 3);
            
            if (rndNum == 1) chrMove.StartAutoAttack();
            else chrMove.StartAutoMove();
        }

        public void CheckCards()
        {
            if ((curCharacterType == CharacterType.Player && !LevelManager.instance.isPlayerAuto) || (curCharacterType == CharacterType.Enemy && !LevelManager.instance.isEnemyAuto))
            {
                if (LevelManager.instance.areCardsOpen)
                {
                    LevelManager.instance.RemoveCards();
                }
            }   
        }

        public void Ready()
        {
            TurnStarted();

            if ((curCharacterType == CharacterType.Player && LevelManager.instance.isPlayerAuto) || (curCharacterType == CharacterType.Enemy && LevelManager.instance.isEnemyAuto))
            {
                int rndNum = UnityEngine.Random.Range(1, 3);
                
                if (rndNum == 1)
                {
                    chrMove.StartAutoAttack();
                }
                else
                {
                    chrMove.StartAutoMove();
                }
            }
            else
            {
                onTurn = true;
                chrMove.StartManualMovement();
                LevelManager.instance.ShowCards(cardOne, cardTwo, cardThree);

                Button card01 = LevelManager.instance.cards[0];
                Button card02 = LevelManager.instance.cards[1];
                Button card03 = LevelManager.instance.cards[2];
                Button waitButton = LevelManager.instance.waitButton;
                Button addStepButton = LevelManager.instance.addStepButton;
                Button removeStepButton = LevelManager.instance.removeStepButton;
                chrMove.UpdateStepsText();
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
                addStepButton.onClick.AddListener(chrMove.AddStep);     
                removeStepButton.onClick.AddListener(chrMove.RemoveStep); 
                switchMove.onClick.AddListener(chrMove.SwitchMovementTypes);   

                card01.gameObject.GetComponent<CardButtonDisplay>().card = cardOne;
                card02.gameObject.GetComponent<CardButtonDisplay>().card = cardTwo;
                card03.gameObject.GetComponent<CardButtonDisplay>().card = cardThree;
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
                int staminaAmount = UnityEngine.Random.Range(LevelManager.instance.minStaminaBonus, LevelManager.instance.maxStaminaBonus);
                int damageAmount = UnityEngine.Random.Range(LevelManager.instance.minDamageBonus, LevelManager.instance.maxDamageBonus);
                int waveAmount = UnityEngine.Random.Range(LevelManager.instance.minWaveBonus, LevelManager.instance.maxWaveBonus);

                if (num == 1)
                {
                    CardStamina(staminaAmount);
                }
                else if (num == 2)
                {
                    CardDamage(damageAmount);
                }
                else if (num == 3)
                {
                    CardWave(waveAmount);
                }
                
                other.gameObject.GetComponent<Booster>().Death();
            }
            else if (other.gameObject.CompareTag("FlyBooster"))
            {            
                chrMove.canSwitchMoveTypes = true;
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

    #region HEALTH
        
        private void IncreaseMaxHealth()
        {
            int amount = UnityEngine.Random.Range(LevelManager.instance.minHealthBonus, LevelManager.instance.maxHealthBonus);
            maxHealth += amount;
            Heal(maxHealth - curHealth);
        }

        public void TakeDamage(int damageAmount)
        {
            chrAnim.SetDamagedAnimation();
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

            Instantiate(chrAnim.deathVFX, transform.position, Quaternion.identity);
            Destroy(transform.parent.gameObject);
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

    #region STAMINA

        private void IncreaseMaxStamina()
        {
            int amount = UnityEngine.Random.Range(LevelManager.instance.minCharacterStaminaBonus, LevelManager.instance.maxCharacterStaminaBonus);
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
            int amount = UnityEngine.Random.Range(LevelManager.instance.minCooldownBonus, LevelManager.instance.maxCooldownBonus);

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

        private void CardWave(int waveAmount)
        {
            int cardNum = UnityEngine.Random.Range(1, 4);

            if (cardNum == 1)
            {
                cardOne.attackWaves += waveAmount;
                cardOne.cardDescription = cardOneOriginal.cardDescription;
                cardOne.cardDescription += " " + cardOne.attackWaves + " waves.";
            }
            else if (cardNum == 2)
            {
                cardTwo.attackWaves += waveAmount;
                cardTwo.cardDescription = cardTwoOriginal.cardDescription;
                cardTwo.cardDescription += " " + cardTwo.attackWaves + " waves.";
            }
            else if (cardNum == 3)
            {
                cardThree.attackWaves += waveAmount;
                cardThree.cardDescription = cardThreeOriginal.cardDescription;
                cardThree.cardDescription += " " + cardThree.attackWaves + " waves.";
            }
        }

        public CardSO CloneCard(CardSO cardToClone)
        {
            CardSO clone = ScriptableObject.CreateInstance<CardSO>();

            clone.cardIcon = cardToClone.cardIcon;
            clone.cardName = cardToClone.cardName;
            clone.cardDescription = cardToClone.cardDescription;
            clone.cardDescription += " " + clone.attackWaves + " waves.";

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

            int attackWaves = card.attackWaves;
                
            for (int i = 1; i <= attackWaves; i++)
            {
                chrAnim.SetAttackAnimation();

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
                
                if (chrMove.curFacingDirection == FacingDirection.Left)
                {
                    projectileScript.Flip();
                }

                if (cardNum == 1)
                {
                    if (i == cardOne.attackWaves)
                    {
                        projectileScript.isLast = true;
                    }
                }
                else if (cardNum == 2)
                {
                    if (i == cardTwo.attackWaves)
                    {
                        projectileScript.isLast = true;
                    }
                }
                else if (cardNum == 3)
                {
                    if (i == cardThree.attackWaves)
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