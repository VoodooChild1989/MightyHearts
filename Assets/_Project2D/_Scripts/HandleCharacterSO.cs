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
                    
                    chr.cardOne = chrSO.cardOne;
                    chr.cardTwo = chrSO.cardTwo;
                    chr.cardThree = chrSO.cardThree;
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