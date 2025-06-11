using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using Unity.Cinemachine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public List<CharacterStatistics> allCharacters;
            public List<CharacterStatistics> playerCharacters;
            public List<CharacterStatistics> enemyCharacters;
            public List<CharacterStatistics> charactersOnQueue;
            public List<int> charactersOnQueueToIgnore;
            [ShowOnly] public int curQueueIndex;
            public Coroutine cooldownCrt;
            public static LevelManager instance;
            [ShowOnly] public bool areCardsOpen;
            
            [Header("Camera")]
            public CinemachineCamera cinemCamera;
            public Transform camDefaultPos;
            public float initOrthoSize;
            public float finalOrthoSize;
            public float levelCamIntroDuration;
            public bool isFirstReady;

            [Header("Buttons")]
            public Button[] cards; 
            public Button waitButton;
            public Button autoButton;
            public GameObject moveStepsWindow;
            public Button addStepButton;
            public Button removeStepButton;
            public Button switchMovementTypesButton;
            public Sprite groundIcon;
            public Sprite airIcon;
            public TMP_Text stepsNumber;
            public TMP_Text moveStaminaCost;

            [Header("Data")]
            [ShowOnly] public int curTurnNumber;
            public bool isPlayerAuto;
            public bool isEnemyAuto;

        [Header("BOOSTERS")]
            
            [Header("Card")]
            public int minStaminaBonus;
            public int maxStaminaBonus;
            public int minDamageBonus;
            public int maxDamageBonus;
            public int minWaveBonus;
            public int maxWaveBonus;

            [Header("Character")]
            public int minHealthBonus;
            public int maxHealthBonus;
            public int minCharacterStaminaBonus;
            public int maxCharacterStaminaBonus;
            public int minCooldownBonus;
            public int maxCooldownBonus;

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
            allCharacters = FindObjectsByType<CharacterStatistics>(FindObjectsSortMode.None).ToList();
            foreach (CharacterStatistics chr in allCharacters)
            {
                if (chr.curCharacterType == CharacterType.Player)
                {
                    playerCharacters.Add(chr);
                    chr.transform.parent.SetParent(GameObject.Find("Players").transform);
                }
                else if (chr.curCharacterType == CharacterType.Enemy)
                {
                    enemyCharacters.Add(chr);
                    chr.transform.parent.SetParent(GameObject.Find("Enemies").transform);
                }
            }

            RemoveCardsStart();
            StartCoroutine(LevelStart());
                
            charactersOnQueue = new List<CharacterStatistics>();
            charactersOnQueueToIgnore = new List<int>();
            curQueueIndex = 0;
            curTurnNumber = 1;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                TogglePlayerAuto();
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

        private IEnumerator LevelStart()
        {
            cinemCamera.Follow = camDefaultPos;
            cinemCamera.Lens.OrthographicSize = initOrthoSize;

            yield return new WaitForSeconds(1f);

            cooldownCrt = StartCoroutine(Cooldown());
        }

        public void TogglePlayerAuto()
        {
            isPlayerAuto = !isPlayerAuto;

            if (isPlayerAuto)
            {
                autoButton.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
            }
            else
            {
                autoButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }

        private void RemoveCardsStart()
        {
            areCardsOpen = false;

            foreach (Button card in cards)
            {
                card.GetComponent<CanvasGroup>().alpha = 0f;
                card.GetComponent<CanvasGroup>().interactable = false;
                card.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            
            waitButton.GetComponent<CanvasGroup>().alpha = 0f;
            waitButton.GetComponent<CanvasGroup>().interactable = false;
            waitButton.GetComponent<CanvasGroup>().blocksRaycasts = false;
            
            moveStepsWindow.GetComponent<CanvasGroup>().alpha = 0f;
            moveStepsWindow.GetComponent<CanvasGroup>().interactable = false;
            moveStepsWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public IEnumerator Cooldown()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                List<CharacterStatistics> characters = FindObjectsByType<CharacterStatistics>(FindObjectsSortMode.None).ToList();
                List<CharacterStatistics> readyCharacters = new List<CharacterStatistics>();

                foreach (CharacterStatistics character in characters)
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
                    foreach (CharacterStatistics character in readyCharacters)
                    {
                        if (character.curCharacterType == CharacterType.Player)
                        {
                            charactersOnQueue.Add(character);
                            charactersOnQueueToIgnore.Add(0);
                        }
                    }   
                    
                    foreach (CharacterStatistics character in readyCharacters)
                    {
                        if (character.curCharacterType == CharacterType.Enemy)
                        {
                            charactersOnQueue.Add(character);
                            charactersOnQueueToIgnore.Add(0);
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
            if (curQueueIndex >= charactersOnQueue.Count)
            {
                curQueueIndex = 0;
                charactersOnQueue.Clear();
                charactersOnQueueToIgnore.Clear();
                // cinemCamera.Follow = camDefaultPos;
                StartCooldown();
            }
            else
            {   
                if (charactersOnQueueToIgnore[curQueueIndex] == 1)
                {
                    curTurnNumber++;
                    curQueueIndex++;
                    MoveQueue();
                }
                else
                {
                    if (!isFirstReady)
                    {
                        DOTween.To(() => cinemCamera.Lens.OrthographicSize,
                                            x => cinemCamera.Lens.OrthographicSize = x,
                                            finalOrthoSize,
                                            levelCamIntroDuration)
                                            .SetEase(Ease.InOutSine);
                    }

                    charactersOnQueue[curQueueIndex].Ready();
                    curTurnNumber++;
                    curQueueIndex++;
                    isFirstReady = true;
                }
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

        public void RemovePlayer(CharacterStatistics playerCharacter)
        {
            playerCharacters.Remove(playerCharacter);   

            if (playerCharacters.Count == 0)
            {
                GameObject.Find("Back").GetComponent<SceneChanger>().ChangeScene("LevelSelection");
            }

            for (int i = 0; i < charactersOnQueue.Count; i++)
            {
                if (charactersOnQueue[i] == playerCharacter)
                {
                    charactersOnQueueToIgnore[i] = 1;
                    break;   
                }
            }
        }

        public void RemoveEnemy(CharacterStatistics enemyCharacter)
        {
            enemyCharacters.Remove(enemyCharacter);   

            if (enemyCharacters.Count == 0)
            {
                GameObject.Find("Back").GetComponent<SceneChanger>().ChangeScene("LevelSelection");
            }

            for (int i = 0; i < charactersOnQueue.Count; i++)
            {
                if (charactersOnQueue[i] == enemyCharacter)
                {
                    charactersOnQueueToIgnore[i] = 1;
                    break;   
                }
            }
        }

        public void ShowCards(CardSO cardOne, CardSO cardTwo, CardSO cardThree)
        {
            areCardsOpen = true;
            StartCoroutine(ShowCardsAnimation(cardOne, cardTwo, cardThree));
        }

        public IEnumerator ShowCardsAnimation(CardSO cardOne, CardSO cardTwo, CardSO cardThree)
        {
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 3; i++)
            {
                cards[i].GetComponent<WindowManager>().OpenCard();

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

            waitButton.GetComponent<WindowManager>().OpenButton();
            moveStepsWindow.GetComponent<WindowManager>().OpenStepsWindow();
        }

        public void AddDataToCard(CardSO card, Button cardButton)
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
            areCardsOpen = false;

            foreach (Button card in cards)
            {
                card.GetComponent<WindowManager>().CloseCard();
            }
            
            waitButton.GetComponent<WindowManager>().CloseButton();
            moveStepsWindow.GetComponent<WindowManager>().CloseStepsWindow();
        }

    #endregion

}