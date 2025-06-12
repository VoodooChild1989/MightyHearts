using System.Collections;
using System.Collections.Generic;
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
            public bool useFlip;
            [ShowOnly] public CharacterStatistics curChr;

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
                    CharacterStatistics chrStats = child.GetComponent<CharacterStatistics>();
                    curChr = chrStats;
                    
                    chrStats.maxHealth = chrSO.maxHealth;
                    chrStats.maxStamina = chrSO.maxStamina;
                    chrStats.maxCooldown = chrSO.maxCooldown;
                    chrStats.waitReward = chrSO.waitReward;
                    chrStats.curCharacterType = curCharacterType;

                    CharacterCards chrCards = child.GetComponent<CharacterCards>();
                    chrCards.cardOneOriginal = chrSO.cardOne;
                    chrCards.cardOne = chrCards.CloneCard(chrSO.cardOne);
                    chrCards.cardTwoOriginal = chrSO.cardTwo;
                    chrCards.cardTwo = chrCards.CloneCard(chrSO.cardTwo);
                    chrCards.cardThreeOriginal = chrSO.cardThree;
                    chrCards.cardThree = chrCards.CloneCard(chrSO.cardThree);

                    CharacterMovement chrMove = child.GetComponent<CharacterMovement>();
                    chrMove.curMovementType = chrSO.curMovementType;
                    
                    CharacterAnimation chrAnim = child.GetComponent<CharacterAnimation>();
                    chrAnim.idleSprites = chrSO.idleSprites;
                    chrAnim.runningSprites = chrSO.runningSprites;
                    chrAnim.attackSprites = chrSO.attackSprites;
                    chrAnim.damagedSprites = chrSO.damagedSprites;
                    
                    BoxCollider2D col = child.GetComponent<BoxCollider2D>();
                    col.offset = new Vector2(chrSO.offsetX, chrSO.offsetY);
                    col.size = new Vector2(chrSO.sizeX, chrSO.sizeY);

                    foreach (Transform childChild in child)
                    {
                        if (childChild.name == "Sprite")
                        {                    
                            childChild.transform.localPosition = new Vector3(chrSO.offsetX, chrSO.offsetY, 0f);
                        } 
                    }
                    
                    if (useFlip) chrMove.Flip(false);
                }
            }
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
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

    private IEnumerator EnemyBuff()
    {
        yield return null;

        for (int i = 1; i <= 3; i++)
        {
            curChr.CharacterBooster();
            curChr.CardBooster();
            yield return null;
        }
    }

}