using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

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

            chrMove.UpdateMovementType();

            if (curCharacterType == CharacterType.Enemy)
            {            
                StartCoroutine(EnemyBuff());
            }
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

                if (Input.GetKeyDown(InputManager.instance.waitKey))
                {
                    Wait();
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

    #region ESSENTIALS

        /// <summary>
        /// Called when a character's turn comes.
        /// </summary>
        public void Ready()
        {
            TurnStarted();

            if ((curCharacterType == CharacterType.Player && LevelManager.instance.isPlayerAuto) || (curCharacterType == CharacterType.Enemy && LevelManager.instance.isEnemyAuto))
            {
                chrMove.StartAuto();
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
        /// Setting up the start of a turn.
        /// </summary>
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
            
            chrCards.spawnPointCell = chrCards.SetSpawnPoint();
            chrCards.spawnPoint = chrCards.SetSpawnPoint().transform.position;
            TilemapManager.instance.ShowSingleBlockTrajectory(this);
        }

        /// <summary>
        /// Setting up the end of a turn.
        /// </summary>
        public void TurnFinished()
        {
            onTurn = false;
            chrCards.choseCard = false;
            CheckCards();
            RemoveCooldown(curCooldown);
            chrMove.StopManualMovement();
            chrCards.HideSpawnPoint();
        }

        /// <summary>
        /// Closing the cards if they are active.
        /// </summary>
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

        /// <summary>
        /// Waiting system.
        /// </summary>
        public void Wait()
        {
            AddStamina(waitReward);
            CheckCards();
            StartCoroutine(WaitCoroutine());
        }

        /// <summary>
        /// Waiting coroutine.
        /// </summary>
        private IEnumerator WaitCoroutine()
        {
            yield return new WaitForSeconds(1f);
            TurnFinished();
            QueueManager.instance.MoveQueue();
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Turns a character to a player.
        /// </summary>
        private void TurnToPlayer()
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            Material mat = Resources.Load<Material>("Materials/Player_Outline");
            chrAnim.sr.material = mat;
        }

        /// <summary>
        /// Turns a character to an enemy.
        /// </summary>
        private void TurnToEnemy()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            Material mat = Resources.Load<Material>("Materials/Enemy_Outline");
            chrAnim.sr.material = mat;
        }

        /// <summary>
        /// Buffs for enemies.
        /// </summary>
        private IEnumerator EnemyBuff()
        {
            yield return null;

            for (int i = 1; i <= 3; i++)
            {
                CharacterBooster();
                CardBooster();
                yield return null;
            }
        }

        private void DelayedAuto()
        {
            StartCoroutine(DelayedAutoCoroutine());
        }

        private IEnumerator DelayedAutoCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            chrMove.StartAuto();
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Booster>() != null)
            {
                Booster boosterScript = other.gameObject.GetComponent<Booster>();

                if (boosterScript.curBoosterType == BoosterType.Character)
                {
                    CharacterBooster();
                }
                else if (boosterScript.curBoosterType == BoosterType.Card)
                {
                    CardBooster();
                }
                else if (boosterScript.curBoosterType == BoosterType.Switch)
                {            
                    EnableMoveTypesSwitch();
                }

                boosterScript.Death();
            }
        }

    #endregion

    #region BOOSTERS

        /// <summary>
        /// Randomly choosing a buff to apply.
        /// </summary>
        public void CharacterBooster()
        {
            int num = UnityEngine.Random.Range(1, 4);

            if (num == 1) IncreaseMaxHealth();
            else if (num == 2) IncreaseMaxStamina();
            else if (num == 3) DecreaseMaxCooldown();
        }

        /// <summary>
        /// Applying a buff to one of the cards.
        /// </summary>
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
        
        /// <summary>
        /// Making the health capacity of a character bigger.
        /// </summary>
        private void IncreaseMaxHealth()
        {
            int amount = UnityEngine.Random.Range(LevelManager.instance.minHealthBonus, LevelManager.instance.maxHealthBonus + 1);
            maxHealth += amount;
            Heal(maxHealth - curHealth);
        }

        /// <summary>
        /// Taking damage.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to take.</param>
        public void TakeDamage(int damageAmount)
        {
            chrAnim.SetDamagedAnimation();
            SFXManager.PlaySFX(Resources.Load<AudioClip>("SFX/Damaged"), transform, 1f);
            GameObject.Find("CinemachineCamera").GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            UpdateHealth(-damageAmount);
        }
    
        /// <summary>
        /// Healing a character.
        /// </summary>
        /// <param name="healAmount">The amount of heal to take.</param>
        public void Heal(int healAmount)
        {
            // To be added: Healing SFX and VFX

            UpdateHealth(healAmount);
        }

        /// <summary>
        /// Updating the health of a character.
        /// It can either get smaller (damage) or get bigger (heal).
        /// </summary>
        /// <param name="amount">The amount of either damage or heal to update.</param>
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
        
        /// <summary>
        /// Called when a character appears.
        /// </summary>
        public void Birth()
        {
            Instantiate(chrAnim.birthVFX, transform.position, Quaternion.identity);
            SFXManager.PlaySFX(Resources.Load<AudioClip>("SFX/Chip"), transform, 1f);
        }

        /// <summary>
        /// Starting the death coroutine.
        /// </summary>
        private void StartDeath()
        {
            StartCoroutine(Death());
        }

        /// <summary>
        /// Death coroutine.
        /// </summary>
        public IEnumerator Death()
        {
            isDead = true;
            CheckCards();

            yield return new WaitForSeconds(1f);

            Instantiate(chrAnim.deathVFX, transform.position, Quaternion.identity);
            Destroy(transform.parent.gameObject);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
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

        /// <summary>
        /// Making the stamina capacity of a character bigger.
        /// </summary>
        private void IncreaseMaxStamina()
        {
            int amount = UnityEngine.Random.Range(LevelManager.instance.minCharacterStaminaBonus, LevelManager.instance.maxCharacterStaminaBonus + 1);
            maxStamina += amount;
            AddStamina(maxStamina - curStamina);
        }

        /// <summary>
        /// Adding stamina.
        /// </summary>
        /// <param name="amount">The amount of stamina to add.</param>
        public void AddStamina(int amount)
        {
            // To be added: Stamina SFX and VFX

            UpdateStamina(amount);
        }

        /// <summary>
        /// Removing stamina.
        /// </summary>
        /// <param name="amount">The amount of stamina to remove.</param>
        public void RemoveStamina(int amount)
        {
            UpdateStamina(-amount);
        }

        /// <summary>
        /// Updating the stamina of a character.
        /// It can either get smaller or get bigger.
        /// </summary>
        /// <param name="amount">The amount to update.</param>
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

    #endregion
 
    #region COOLDOWN

        /// <summary>
        /// Making the maximum cooldown capacity of a character bigger.
        /// </summary>
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

        /// <summary>
        /// Adding cooldown.
        /// </summary>
        /// <param name="amount">The amount of cooldown to add.</param>
        public void AddCooldown(int amount)
        {
            UpdateCooldown(amount);
        }

        /// <summary>
        /// Removing cooldown.
        /// </summary>
        /// <param name="amount">The amount of cooldown to remove.</param>
        public void RemoveCooldown(int amount)
        {
            UpdateCooldown(-amount);
        }

        /// <summary>
        /// Updating the cooldown of a character.
        /// It can either get smaller or get bigger.
        /// </summary>
        /// <param name="amount">The amount to update.</param>
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