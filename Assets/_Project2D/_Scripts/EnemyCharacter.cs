using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class EnemyCharacter : Character
{

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        public void EnemyCharacterAwake()
        {
            base.CharacterAwake();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        public void EnemyCharacterStart()
        {
            base.CharacterStart();
            
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            Flip(false);
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        public void EnemyCharacterUpdate()
        {
            base.CharacterUpdate();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        public void EnemyCharacterFixedUpdate()
        {
            base.CharacterFixedUpdate();
        }

    #endregion

    #region CUSTOM METHODS

        private void OnDestroy() 
        {
            LevelManager.instance.RemoveEnemy(this);
        }

    #endregion

}