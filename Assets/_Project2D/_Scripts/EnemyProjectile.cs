using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public abstract class EnemyProjectile : Projectile
{

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        public void EnemyProjectileAwake()
        {
            base.ProjectileAwake();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        public void EnemyProjectileStart()
        {
            base.ProjectileStart();

            SetOwner(2);
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        public void EnemyProjectileUpdate()
        {
            base.ProjectileUpdate();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        public void EnemyProjectileFixedUpdate()
        {
            base.ProjectileFixedUpdate();
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        private void ExampleMethod()
        {
            // Implement custom functionality here.
            // Example: Execute game-specific behavior or helper logic.
        }

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        private IEnumerator ExampleCoroutine()
        {
            // Wait for 2 seconds before executing further code.
            yield return new WaitForSeconds(2f);

            Debug.Log("Action after 2 seconds.");
        }

    #endregion

}