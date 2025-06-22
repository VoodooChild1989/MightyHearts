using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class CharacterCards : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            [ShowOnly] public bool choseCard;
            public Vector3 spawnPoint;
            public Cell spawnPointCell;

        [Header("CARDS")]

            [Header("Card One")]
            [ShowOnly] public CardSO cardOne;
            [ShowOnly] public CardSO cardOneOriginal;

            [Header("Card Two")]
            [ShowOnly] public CardSO cardTwo;
            [ShowOnly] public CardSO cardTwoOriginal;

            [Header("Card Three")]
            [ShowOnly] public CardSO cardThree;
            [ShowOnly] public CardSO cardThreeOriginal;

        [Header("REFERENCES")]

            [Header("Scripts")]
            private CharacterAnimation chrAnim;
            private CharacterStatistics chrStats;
            private CharacterMovement chrMove;

        public bool hasSetPushedSomebody;
        public CharacterStatistics pushedChr;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            chrAnim = GetComponent<CharacterAnimation>();
            chrStats = GetComponent<CharacterStatistics>();
            chrMove = GetComponent<CharacterMovement>();
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

    #region CUSTOM METHODS

        public Cell SetSpawnPoint()
        {
            if (chrMove.curFacingDirection == FacingDirection.Right)
            {
                int ?maxX = null;
                int ?minY = null;

                foreach (GameObject cell in chrMove.sizeMatrix)
                {
                    Cell cellToTest = cell.GetComponent<Cell>();

                    if (maxX == null)
                    {
                        maxX = cellToTest.x;
                    }
                    else if (cellToTest.x > maxX)
                    {
                        maxX = cellToTest.x;
                    }
                    
                    if (minY == null)
                    {
                        minY = cellToTest.y;
                    }
                    else if (cellToTest.y < minY)
                    {
                        minY = cellToTest.y;
                    }
                }
                
                Cell spawnPointCell = TilemapManager.instance.collisionMatrixCells[(int)maxX + 1, (int)minY].GetComponent<Cell>();
                return spawnPointCell;
            }
            else if (chrMove.curFacingDirection == FacingDirection.Left)
            {
                int ?minX = null;
                int ?minY = null;

                foreach (GameObject cell in chrMove.sizeMatrix)
                {
                    Cell cellToTest = cell.GetComponent<Cell>();

                    if (minX == null)
                    {
                        minX = cellToTest.x;
                    }
                    else if (cellToTest.x < minX)
                    {
                        minX = cellToTest.x;
                    }
                    
                    if (minY == null)
                    {
                        minY = cellToTest.y;
                    }
                    else if (cellToTest.y < minY)
                    {
                        minY = cellToTest.y;
                    }
                }
                
                Cell spawnPointCell = TilemapManager.instance.collisionMatrixCells[(int)minX - 1, (int)minY].GetComponent<Cell>();
                return spawnPointCell;
            }

            return null;
        }

        public void AddYToSpawnPoint()
        {
            Cell curCell = spawnPointCell;
            int ?maxY = null;

            foreach (GameObject cell in chrMove.sizeMatrix)
            {
                Cell cellToTest = cell.GetComponent<Cell>();

                if (maxY == null)
                {
                    maxY = cellToTest.y;
                }
                else if (cellToTest.y > maxY)
                {
                    maxY = cellToTest.y;
                }
            }

            if (curCell.y + 1 <= (int)maxY)
            {
                TilemapManager.instance.HideTrajectory();
                Cell newSpawnPointCell = TilemapManager.instance.collisionMatrixCells[curCell.x, curCell.y + 1].GetComponent<Cell>();
                spawnPointCell = newSpawnPointCell;
                spawnPoint = newSpawnPointCell.transform.position;
                TilemapManager.instance.ShowSingleBlockTrajectory(chrStats);
            }
        }

        public void RemoveYToSpawnPoint()
        {
            Cell curCell = spawnPointCell;
            int ?minY = null;

            foreach (GameObject cell in chrMove.sizeMatrix)
            {
                Cell cellToTest = cell.GetComponent<Cell>();

                if (minY == null)
                {
                    minY = cellToTest.y;
                }
                else if (cellToTest.y < minY)
                {
                    minY = cellToTest.y;
                }
            }

            if (curCell.y - 1 >= (int)minY)
            {
                TilemapManager.instance.HideTrajectory();
                Cell newSpawnPointCell = TilemapManager.instance.collisionMatrixCells[curCell.x, curCell.y - 1].GetComponent<Cell>();
                spawnPointCell = newSpawnPointCell;
                spawnPoint = newSpawnPointCell.transform.position;
                TilemapManager.instance.ShowSingleBlockTrajectory(chrStats);
            }
        }

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public void ApplyCards()
        {
            LevelManager.instance.ShowCards(cardOne, cardTwo, cardThree);


            Button addY = LevelManager.instance.addY;
            Button removeY = LevelManager.instance.removeY;

            addY.onClick.RemoveAllListeners();
            removeY.onClick.RemoveAllListeners();

            addY.onClick.AddListener(AddYToSpawnPoint);   
            removeY.onClick.AddListener(RemoveYToSpawnPoint);   


            Button card01 = LevelManager.instance.cards[0];
            Button card02 = LevelManager.instance.cards[1];
            Button card03 = LevelManager.instance.cards[2];
            
            card01.onClick.RemoveAllListeners();
            card02.onClick.RemoveAllListeners();
            card03.onClick.RemoveAllListeners();

            card01.onClick.AddListener(FirstCard);   
            card02.onClick.AddListener(SecondCard);   
            card03.onClick.AddListener(ThirdCard);    

            card01.gameObject.GetComponent<CardButtonDisplay>().card = cardOne;
            card02.gameObject.GetComponent<CardButtonDisplay>().card = cardTwo;
            card03.gameObject.GetComponent<CardButtonDisplay>().card = cardThree;
            
            spawnPointCell = SetSpawnPoint();
            spawnPoint = SetSpawnPoint().transform.position;
        }

    #endregion

    #region CARDS

        public void CardStamina(int amount)
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

        public void CardDamage(int amount)
        {
            int cardNum = UnityEngine.Random.Range(1, 4);

            if (cardNum == 1) cardOne.damageAmount += amount;
            if (cardNum == 2) cardTwo.damageAmount += amount;
            if (cardNum == 3) cardThree.damageAmount += amount;
        }

        public void CardWave(int waveAmount)
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
            clone.staminaCost = cardToClone.staminaCost;
            clone.attackWaves = cardToClone.attackWaves;
            clone.curCardType = cardToClone.curCardType;

            clone.damageAmount = cardToClone.damageAmount;
            clone.speed = cardToClone.speed;
            clone.maxBlocks = cardToClone.maxBlocks;
            clone.idleSprites = cardToClone.idleSprites;

            clone.cardDescription += " " + clone.attackWaves + " waves.";

            return clone;
        }

        public void FirstCard()
        {
            if (choseCard) return;

            if (chrStats.curStamina >= cardOne.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardOne, 1));            
                chrStats.CheckCards();
            }   
        }
        
        public void SecondCard()
        {
            if (choseCard) return;

            if (chrStats.curStamina >= cardTwo.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardTwo, 2));         
                chrStats.CheckCards();
            }
        }

        public void ThirdCard()
        {
            if (choseCard) return;

            if (chrStats.curStamina >= cardThree.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardThree, 3));         
                chrStats.CheckCards();
            }
        }

        public IEnumerator CardCoroutine(CardSO card, int cardNum)
        {    
            chrStats.RemoveStamina(card.staminaCost);
            choseCard = true;
            hasSetPushedSomebody = false;
            pushedChr = null;

            int attackWaves = card.attackWaves;
                
            for (int i = 1; i <= attackWaves; i++)
            {
                chrAnim.SetAttackAnimation();

                GameObject projToSpawn = Resources.Load<GameObject>("Prefabs/Prefab_Projectile_01");
                projToSpawn.GetComponent<HandleProjectileSO>().cardSO = card;

                GameObject projectileObj = Instantiate(projToSpawn, spawnPoint, Quaternion.identity);
                Projectile projectileScript = projectileObj.GetComponentInChildren<Projectile>();
                projectileScript.damageAmount = card.damageAmount;
                projectileScript.curCardType = card.curCardType;
                projectileScript.cardsScript = this;

                if (chrStats.curCharacterType == CharacterType.Player)
                {
                    projectileScript.TurnToPlayer();
                }
                else if (chrStats.curCharacterType == CharacterType.Enemy)
                {
                    projectileScript.TurnToEnemy();
                }
                
                if (chrMove.curFacingDirection == FacingDirection.Left)
                {
                    projectileScript.Flip();
                }

                projectileScript.maxNumberInSet = card.attackWaves;
                projectileScript.curNumberInSet = i;

                if (hasSetPushedSomebody)
                {
                    projectileScript.pushedSomebody = true;
                    projectileScript.pushedChr = pushedChr;
                }

                yield return new WaitForSeconds(1.1f);
            }

            chrStats.TurnFinished();
        }

    #endregion 

}