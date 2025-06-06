using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class PlayerCharacter : Character
{

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        public void PlayerCharacterAwake()
        {
            base.CharacterAwake();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        public void PlayerCharacterStart()
        {
            base.CharacterStart();
            
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        public void PlayerCharacterUpdate()
        {
            base.CharacterUpdate();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        public void PlayerCharacterFixedUpdate()
        {
            base.CharacterFixedUpdate();
        }

    #endregion

    #region CUSTOM METHODS

        private void OnDestroy() 
        {
            LevelManager.instance.RemovePlayer(this);
        }

    #endregion

}