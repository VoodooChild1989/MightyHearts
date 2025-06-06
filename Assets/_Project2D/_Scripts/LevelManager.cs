using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public List<PlayerCharacter> playerCharacters;
            public List<EnemyCharacter> enemyCharacters;
            public List<Character> charactersOnQueue;
            private int curQueueIndex;
            public Coroutine cooldownCrt;
            public static LevelManager instance;
            
            [Header("Buttons")]
            public Button[] cards; 
            public Button waitButton;
            public int defaultWaitReward;

            [Header("Data")]
            public int curTurnNumber;
            public bool isPlayerAuto;
            public bool isEnemyAuto;

            [Header("Camera")]
            public CinemachineCamera cinemCamera;
            public Transform camDefaultPos;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this, false);
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            playerCharacters = FindObjectsByType<PlayerCharacter>(FindObjectsSortMode.None).ToList();
            foreach (PlayerCharacter pc in playerCharacters)
            {
                pc.transform.parent.SetParent(GameObject.Find("Players").transform);
            }
            enemyCharacters = FindObjectsByType<EnemyCharacter>(FindObjectsSortMode.None).ToList();
            foreach (EnemyCharacter ec in enemyCharacters)
            {
                ec.transform.parent.SetParent(GameObject.Find("Enemies").transform);
            }
                
            curTurnNumber = 1;
            cooldownCrt = StartCoroutine(Cooldown());
            cinemCamera.Follow = camDefaultPos;
            RemoveCards();
            
            charactersOnQueue = new List<Character>();
            curQueueIndex = 0;
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

        public IEnumerator Cooldown()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                List<Character> characters = FindObjectsByType<Character>(FindObjectsSortMode.None).ToList();
                List<Character> readyCharacters = new List<Character>();

                foreach (Character character in characters)
                {
                    if (character.curCooldown < character.maxCooldown)
                    {
                        character.AddCooldown(1);

                        if (character.curCooldown >= character.maxCooldown)
                        {
                            readyCharacters.Add(character);
                        }
                    }
                }

                if (readyCharacters.Count > 0)
                {
                    // Adding player characters first, so a player moves first.
                    foreach (Character character in readyCharacters)
                    {
                        if (character is PlayerCharacter pc)
                        {
                            charactersOnQueue.Add(character);
                        }
                    }   
                    foreach (Character character in readyCharacters)
                    { 
                        if (character is EnemyCharacter ec)
                        {
                            charactersOnQueue.Add(character);
                        }
                    }   

                    StopCooldown();
                    MoveQueue();
                }
            }
        }

        public void MoveQueue()
        {
            // If it was a last index, then end the queue
            if (curQueueIndex == charactersOnQueue.Count)
            {
                curQueueIndex = 0;
                charactersOnQueue.Clear();
                cinemCamera.Follow = camDefaultPos;
                StartCooldown();
            }
            else
            {   
                charactersOnQueue[curQueueIndex].Ready();
                curTurnNumber++;
                curQueueIndex++;
            }
        }

        public void StartCooldown()
        {
            if (cooldownCrt == null)
                cooldownCrt = StartCoroutine(Cooldown());
        }

        public void StopCooldown()
        {
            if (cooldownCrt != null)
            {
                StopCoroutine(cooldownCrt);
                cooldownCrt = null;
            }
        }

        public void RemovePlayer(PlayerCharacter playerCharacter)
        {
            playerCharacters.Remove(playerCharacter);   
        }

        public void RemoveEnemy(EnemyCharacter enemyCharacter)
        {
            enemyCharacters.Remove(enemyCharacter);   
        }

        public void ShowCards(Card cardOne, Card cardTwo, Card cardThree)
        {
            StartCoroutine(ShowCardsAnimation(cardOne, cardTwo, cardThree));
        }

        public IEnumerator ShowCardsAnimation(Card cardOne, Card cardTwo, Card cardThree)
        {
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 3; i++)
            {
                cards[i].GetComponent<CanvasGroup>().alpha = 1f;
                cards[i].GetComponent<CanvasGroup>().interactable = true;

                if (i == 0)
                {
                    AddDataToCard(cardOne, cards[i]);
                }
                else if (i == 1)
                {
                    AddDataToCard(cardTwo, cards[i]);
                }
                else if (i == 2)
                {
                    AddDataToCard(cardThree, cards[i]);
                }
            }

            waitButton.GetComponent<CanvasGroup>().alpha = 1f;
            waitButton.GetComponent<CanvasGroup>().interactable = true;
        }

        public void AddDataToCard(Card card, Button cardButton)
        {
            foreach (Transform child in cardButton.transform)
            {
                if (child.name == "Name")
                {
                    TMP_Text cardName = child.GetComponent<TMP_Text>();
                    cardName.text = card.cardName;
                }
                else if (child.name == "Cost")
                {
                    TMP_Text staminaCost = child.GetComponent<TMP_Text>();
                    staminaCost.text = card.staminaCost.ToString();
                }
                else if (child.name == "Damage")
                {
                    TMP_Text damageAmount = child.GetComponent<TMP_Text>();
                    damageAmount.text = card.damageAmount.ToString();
                }
                else if (child.name == "Icon")
                {
                    Image cardIcon = child.GetComponent<Image>();
                    cardIcon.sprite = card.cardIcon;
                }
            }
        }

        public void RemoveCards()
        {
            foreach (Button card in cards)
            {
                card.GetComponent<CanvasGroup>().alpha = 0f;
                card.GetComponent<CanvasGroup>().interactable = false;
            }
            
            waitButton.GetComponent<CanvasGroup>().alpha = 0f;
            waitButton.GetComponent<CanvasGroup>().interactable = false;
        }

    #endregion

}