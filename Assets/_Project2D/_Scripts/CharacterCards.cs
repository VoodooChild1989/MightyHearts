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

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public void ApplyCards()
        {
            LevelManager.instance.ShowCards(cardOne, cardTwo, cardThree);

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
            clone.attackWaves = cardToClone.attackWaves;
            clone.cardDescription += " " + clone.attackWaves + " waves.";

            clone.staminaCost = cardToClone.staminaCost;
            clone.damageAmount = cardToClone.damageAmount;
            clone.damageAmount = cardToClone.damageAmount;
            clone.cardProjectile = cardToClone.cardProjectile;

            clone.curCardType = cardToClone.curCardType;

            return clone;
        }

        public void FirstCard()
        {
            if (choseCard) return;

            if (chrStats.curStamina >= cardOne.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardOne, 1));
            }   
        }
        
        public void SecondCard()
        {
            if (choseCard) return;

            if (chrStats.curStamina >= cardTwo.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardTwo, 2));
            }
        }

        public void ThirdCard()
        {
            if (choseCard) return;

            if (chrStats.curStamina >= cardThree.staminaCost)
            {
                StartCoroutine(CardCoroutine(cardThree, 3));
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
                projToSpawn.GetComponent<HandleProjectileSO>().projSO = card.cardProjectile;

                Vector3 spawnPos = Vector3.zero;
                if (cardNum == 1) spawnPos = cardOneSpawnPoint.position;
                else if (cardNum == 2) spawnPos = cardTwoSpawnPoint.position;
                else if (cardNum == 3) spawnPos = cardThreeSpawnPoint.position;

                GameObject projectileObj = Instantiate(projToSpawn, spawnPos, Quaternion.identity);
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