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

        [Header("DISPLAY")]

            [Header("UI")]
            private TMP_Text healthTMP;
            private TMP_Text staminaTMP;
            private TMP_Text cooldownTMP;
            [ShowOnly] public Image moveTypeIcon;

        [Header("REFERENCES")]

            [Header("Scripts")]
            [ShowOnly] public CharacterAnimation chrAnim;
            [ShowOnly] public CharacterMovement chrMove;
            [ShowOnly] public CharacterCards chrCards;

        public bool isPoisoned;
        public int poisonedTurnsLeft;

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
            chrCards = GetComponent<CharacterCards>();
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
        }

        public void TurnStarted()
        {
            if (isPoisoned)
            {
                TakeDamage(10);
                poisonedTurnsLeft--;
                if (poisonedTurnsLeft == 0) isPoisoned = false;
            }

            LevelManager.instance.cinemCamera.Follow = gameObject.transform;
            chrMove.moveSteps = 1;
            chrMove.moveStaminaCost = 0;

            chrMove.UpdateMovementType();
        }

        public void TurnFinished()
        {
            onTurn = false;
            chrCards.choseCard = false;
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
                chrCards.ApplyCards();

                Button waitButton = LevelManager.instance.waitButton;
                Button addStepButton = LevelManager.instance.addStepButton;
                Button removeStepButton = LevelManager.instance.removeStepButton;
                chrMove.UpdateStepsText();
                Button switchMove = LevelManager.instance.switchMovementTypesButton;
                
                waitButton.onClick.RemoveAllListeners();
                addStepButton.onClick.RemoveAllListeners();
                removeStepButton.onClick.RemoveAllListeners();
                switchMove.onClick.RemoveAllListeners();

                waitButton.onClick.AddListener(Wait);     
                addStepButton.onClick.AddListener(chrMove.AddStep);     
                removeStepButton.onClick.AddListener(chrMove.RemoveStep); 
                switchMove.onClick.AddListener(chrMove.SwitchMovementTypes);   
            }
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("CharacterBooster"))
            {
                CharacterBooster();
                other.gameObject.GetComponent<Booster>().Death();
            }
            else if (other.gameObject.CompareTag("CardBooster"))
            {
                CardBooster();
                other.gameObject.GetComponent<Booster>().Death();
            }
            else if (other.gameObject.CompareTag("FlyBooster"))
            {            
                EnableMoveTypesSwitch();
                other.gameObject.GetComponent<Booster>().Death();
            }
        }

    #endregion

    #region BOOSTERS

        public void CharacterBooster()
        {
            int num = UnityEngine.Random.Range(1, 4);

            if (num == 1) IncreaseMaxHealth();
            else if (num == 2) IncreaseMaxStamina();
            else if (num == 3) DecreaseMaxCooldown();
        }

        public void CardBooster()
        {
            int num = UnityEngine.Random.Range(1, 4);
            int staminaAmount = UnityEngine.Random.Range(LevelManager.instance.minStaminaBonus, LevelManager.instance.maxStaminaBonus + 1);
            int damageAmount = UnityEngine.Random.Range(LevelManager.instance.minDamageBonus, LevelManager.instance.maxDamageBonus + 1);
            int waveAmount = UnityEngine.Random.Range(LevelManager.instance.minWaveBonus, LevelManager.instance.maxWaveBonus + 1);

            if (num == 1)
            {
                chrCards.CardStamina(staminaAmount);
            }
            else if (num == 2)
            {
                chrCards.CardDamage(damageAmount);
            }
            else if (num == 3)
            {
                chrCards.CardWave(waveAmount);
            }
        }

        public void EnableMoveTypesSwitch()
        {
            chrMove.canSwitchMoveTypes = true;
        }

    #endregion

    #region HEALTH
        
        private void IncreaseMaxHealth()
        {
            int amount = UnityEngine.Random.Range(LevelManager.instance.minHealthBonus, LevelManager.instance.maxHealthBonus + 1);
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
            int amount = UnityEngine.Random.Range(LevelManager.instance.minCharacterStaminaBonus, LevelManager.instance.maxCharacterStaminaBonus + 1);
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
            chrMove.StartCheckFall();
        }

        private IEnumerator WaitCoroutine()
        {
            yield return new WaitForSeconds(1f);
            TurnFinished();
            QueueManager.instance.MoveQueue();
        }

    #endregion
 
    #region COOLDOWN

        private void DecreaseMaxCooldown()
        {
            int amount = UnityEngine.Random.Range(LevelManager.instance.minCooldownBonus, LevelManager.instance.maxCooldownBonus + 1);

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

}