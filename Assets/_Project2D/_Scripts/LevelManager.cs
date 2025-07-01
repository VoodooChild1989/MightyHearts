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
            public static LevelManager instance;
            
            [Header("Camera")]
            public CinemachineCamera cinemCamera;
            public Transform camDefaultPos;
            public float initOrthoSize;
            public float finalOrthoSize;
            public float levelCamIntroDuration;

            [Header("Data")]
            [ShowOnly] public bool areCardsOpen;
            public bool isPlayerAuto;
            public bool isEnemyAuto;
            public bool isLevelInfinite;
            public List<GameObject> chrsSpawnPoints;
            public List<CharacterSO> chrSOs;

            [Header("Buttons")]
            public Button[] cards; 
            public Button waitButton;
            public Button autoButton;
            public GameObject moveStepsWindow;
            public GameObject spawnPointWindow;
            public Button addStepButton;
            public Button removeStepButton;
            public Button switchMovementTypesButton;
            public Sprite groundIcon;
            public Sprite airIcon;
            public TMP_Text stepsNumber;
            public TMP_Text moveStaminaCost;
            public GameObject startWindow;
            public Button addY;
            public Button removeY;

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
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(InputManager.instance.playerAutoKey))
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
            
            startWindow.GetComponent<CanvasGroup>().alpha = 0f;
            startWindow.GetComponent<CanvasGroup>().interactable = false;
            startWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;

            foreach (CharacterStatistics chr in allCharacters)
            {
                chr.transform.parent.gameObject.SetActive(false);
            }

            List<Booster> allBoosters = new List<Booster>();
            allBoosters = FindObjectsByType<Booster>(FindObjectsSortMode.None).ToList();
            foreach (Booster booster in allBoosters)
            {
                booster.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(1f);

            foreach (CharacterStatistics chr in allCharacters)
            {
                chr.transform.parent.gameObject.SetActive(true);
                chr.chrAnim.SetIdleAnimation();
                chr.Birth();

                yield return new WaitForSeconds(0.2f);
            }
            
            yield return new WaitForSeconds(0.4f);

            foreach (Booster booster in allBoosters)
            {
                booster.gameObject.SetActive(true);
                // booster.Birth();

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.5f);

            startWindow.GetComponent<WindowManager>().OpenWindow();
            QueueManager.instance.StartCooldown();

            yield return new WaitForSeconds(1f);
            startWindow.GetComponent<WindowManager>().CloseWindow();
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

            spawnPointWindow.GetComponent<CanvasGroup>().alpha = 0f;
            spawnPointWindow.GetComponent<CanvasGroup>().interactable = false;
            spawnPointWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void RemovePlayer(CharacterStatistics playerCharacter)
        {
            playerCharacters.Remove(playerCharacter);   

            if (playerCharacters.Count == 0)
            {
                GameObject.Find("Back")?.GetComponent<SceneChanger>().ChangeScene("LevelSelection");
            }

            for (int i = 0; i < QueueManager.instance.charactersOnQueue.Count; i++)
            {
                if (QueueManager.instance.charactersOnQueue[i] == playerCharacter)
                {
                    QueueManager.instance.charactersOnQueueToIgnore[i] = 1;
                    break;   
                }
            }
        }

        public void RemoveEnemy(CharacterStatistics enemyCharacter)
        {
            enemyCharacters.Remove(enemyCharacter);   

            if (!isLevelInfinite)
            {
                if (enemyCharacters.Count == 0)
                {
                    GameObject.Find("Back")?.GetComponent<SceneChanger>().ChangeScene("LevelSelection");
                }
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Prefab_Character_01");   
                
                int rndIndexSpawn = UnityEngine.Random.Range(0, chrsSpawnPoints.Count);
                Vector3 spawnPos = chrsSpawnPoints[rndIndexSpawn].transform.position;

                int rndIndexSO = UnityEngine.Random.Range(0, chrSOs.Count);
                if (chrSOs[rndIndexSO].sizeX % 2 != 0 && chrSOs[rndIndexSO].sizeY % 2 != 0)
                {
                    spawnPos = new Vector3(spawnPos.x + 0.5f, spawnPos.y, spawnPos.z);
                }

                GameObject spawnedChr = Instantiate(prefab, spawnPos, Quaternion.identity);
                HandleCharacterSO handler = spawnedChr.GetComponent<HandleCharacterSO>();
                handler.chrSO = chrSOs[rndIndexSO];
                handler.curCharacterType = CharacterType.Enemy;
                handler.Setup();
            }
            
            for (int i = 0; i < QueueManager.instance.charactersOnQueue.Count; i++)
            {
                if (QueueManager.instance.charactersOnQueue[i] == enemyCharacter)
                {
                    QueueManager.instance.charactersOnQueueToIgnore[i] = 1;
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

            waitButton.GetComponent<WindowManager>().OpenWindow();
            moveStepsWindow.GetComponent<WindowManager>().OpenWindow();
            spawnPointWindow.GetComponent<WindowManager>().OpenWindow();
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
            
            waitButton.GetComponent<WindowManager>().CloseWindow();
            moveStepsWindow.GetComponent<WindowManager>().CloseWindow();
            spawnPointWindow.GetComponent<WindowManager>().CloseWindow();
        }

    #endregion

}