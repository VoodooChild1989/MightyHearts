using UnityEngine;

public class HandleCharacterSO : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public CharacterSO chrSO;
            public CharacterType curCharacterType;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Character")
                {
                    /*
                    Character chr = child.GetComponent<Character>();

                    chr.maxHealth = chrSO.maxHealth;
                    chr.maxStamina = chrSO.maxStamina;
                    chr.maxCooldown = chrSO.maxCooldown;
                    chr.waitReward = chrSO.waitReward;
                    chr.curCharacterType = curCharacterType;
                    chr.curMovementType = chrSO.curMovementType;

                    chr.idleSprites = chrSO.idleSprites;
                    chr.runningSprites = chrSO.runningSprites;
                    chr.attackSprites = chrSO.attackSprites;
                    chr.damagedSprites = chrSO.damagedSprites;
                    
                    chr.cardOneOriginal = chrSO.cardOne;
                    chr.cardOneAttackWaves = chrSO.cardOneAttackWaves;
                    chr.cardOne = chr.CloneCard(chr.cardOneOriginal, 1);
                    
                    chr.cardTwoOriginal = chrSO.cardTwo;
                    chr.cardTwoAttackWaves = chrSO.cardTwoAttackWaves;
                    chr.cardTwo = chr.CloneCard(chr.cardTwoOriginal, 2);

                    chr.cardThreeOriginal = chrSO.cardThree;
                    chr.cardThreeAttackWaves = chrSO.cardThreeAttackWaves;
                    chr.cardThree = chr.CloneCard(chr.cardThreeOriginal, 3);
                    */

                    
                    CharacterStatistics chrStats = child.GetComponent<CharacterStatistics>();
                    chrStats.maxHealth = chrSO.maxHealth;
                    chrStats.maxStamina = chrSO.maxStamina;
                    chrStats.maxCooldown = chrSO.maxCooldown;
                    chrStats.waitReward = chrSO.waitReward;
                    chrStats.curCharacterType = curCharacterType;
                    chrStats.cardOneOriginal = chrSO.cardOne;
                    chrStats.cardOne = chrStats.CloneCard(chrSO.cardOne);
                    chrStats.cardTwoOriginal = chrSO.cardTwo;
                    chrStats.cardTwo = chrStats.CloneCard(chrSO.cardTwo);
                    chrStats.cardThreeOriginal = chrSO.cardThree;
                    chrStats.cardThree = chrStats.CloneCard(chrSO.cardThree);

                    CharacterMovement chrMove = child.GetComponent<CharacterMovement>();
                    chrMove.curMovementType = chrSO.curMovementType;
                    
                    CharacterAnimation chrAnim = child.GetComponent<CharacterAnimation>();
                    chrAnim.idleSprites = chrSO.idleSprites;
                    chrAnim.runningSprites = chrSO.runningSprites;
                    chrAnim.attackSprites = chrSO.attackSprites;
                    chrAnim.damagedSprites = chrSO.damagedSprites;
                }
            }
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            // Perform initial setup that occurs when the game starts.
            // Example: Initialize game state, start coroutines, load resources, etc.
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            // Add your per-frame logic here.
            // Example: Move objects, check user input, update animations, etc.
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

}