using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class MaggyCharacter : Character
{

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            base.CharacterAwake();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            base.CharacterStart();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            base.CharacterUpdate();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        private void FixedUpdate()
        {
            base.CharacterFixedUpdate();
        }

    #endregion

    #region CUSTOM METHODS

        public override void FirstCard()
        {   
            StartCoroutine(CardCoroutine(cardOne, 1));
        }
        
        public override void SecondCard()
        {
            StartCoroutine(CardCoroutine(cardTwo, 2));
        }

        public override void ThirdCard()
        {
            StartCoroutine(CardCoroutine(cardThree, 3));
        }

    #endregion

}