using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using DG.Tweening;

public enum BoosterType
{
    Character,
    Card,
    Switch
}

public class Booster : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public GameObject birthVFX;
            public GameObject deathVFX;
            public float initScale;
            public float finalScale;
            public float duration;
            public BoosterType curBoosterType;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            // Code...
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            birthVFX = Resources.Load<GameObject>("Prefabs/VFX/Bright_Sparkle");
            deathVFX = Resources.Load<GameObject>("Prefabs/VFX/Yellow_Sparkle");

            transform.localScale = new Vector3(initScale, initScale, initScale);
            transform.DOScale(finalScale, duration).SetLoops(-1, LoopType.Yoyo);
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

        public void Birth()
        {
            Instantiate(birthVFX, transform.position, Quaternion.identity);
            SFXManager.PlaySFX(Resources.Load<AudioClip>("SFX/Chip"), transform, 1f);
        }

        public void Death()
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
            SFXManager.PlaySFX(Resources.Load<AudioClip>("SFX/PowerupCollected"), transform, 1f);
            Destroy(gameObject);
        }

    #endregion

}