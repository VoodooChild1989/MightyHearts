using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using DG.Tweening;

public class QueueManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public List<CharacterStatistics> charactersOnQueue;
            public List<int> charactersOnQueueToIgnore;
            [ShowOnly] public int curQueueIndex;
            [ShowOnly] public bool isFirstReady;
            public Coroutine cooldownCrt;
            public static QueueManager instance;

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
            charactersOnQueue = new List<CharacterStatistics>();
            charactersOnQueueToIgnore = new List<int>();
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
                StartCooldown();
            }
            else
            {   
                if (charactersOnQueueToIgnore[curQueueIndex] == 1)
                {
                    curQueueIndex++;
                    MoveQueue();
                }
                else
                {
                    if (!isFirstReady)
                    {
                        DOTween.To(() => LevelManager.instance.cinemCamera.Lens.OrthographicSize,
                                    x => LevelManager.instance.cinemCamera.Lens.OrthographicSize = x,
                                    LevelManager.instance.finalOrthoSize,
                                    LevelManager.instance.levelCamIntroDuration)
                                    .SetEase(Ease.InOutSine);
                    }

                    charactersOnQueue[curQueueIndex].Ready();
                    curQueueIndex++;
                    isFirstReady = true;
                }
            }
        }

    #endregion

}