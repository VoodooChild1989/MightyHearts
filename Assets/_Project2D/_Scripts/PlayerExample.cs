using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExample : PlayerCharacter
{

    #region FIELDS

        [Header("CARDS DATA")]
            
            [Header("First Card")]
            public Transform spawnPoint;
            public GameObject projectile;

            [Header("Second Card")]
            public int secondCardData;

            [Header("Third Card")]
            public int thirdCardData;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            base.PlayerCharacterAwake();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            base.PlayerCharacterStart();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            base.PlayerCharacterUpdate();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        private void FixedUpdate()
        {
            base.PlayerCharacterFixedUpdate();
        }

    #endregion

    #region CUSTOM METHODS

        public override void FirstCard()
        {   
            if (curStamina >= cardOne.staminaCost)
            {
                GameObject projectileObj = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
                Projectile projectileScript = projectileObj.GetComponentInChildren<Projectile>();
                Vector2 curDir = projectileScript.dir;
                
                if (curFacingDirection == FacingDirection.Right)
                {
                    projectileScript.dir = new Vector2(curDir.x, curDir.y);
                }
                else if (curFacingDirection == FacingDirection.Left)
                {
                    projectileScript.dir = new Vector2(-curDir.x, curDir.y);
                }

                TurnFinished();
                RemoveStamina(cardOne.staminaCost);
            }
        }

        public override void SecondCard()
        {
            if (curStamina >= cardTwo.staminaCost)
            {
                GameObject projectileObj = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
                Projectile projectileScript = projectileObj.GetComponentInChildren<Projectile>();
                Vector2 curDir = projectileScript.dir;
                
                if (curFacingDirection == FacingDirection.Right)
                {
                    projectileScript.dir = new Vector2(curDir.x, curDir.y);
                }
                else if (curFacingDirection == FacingDirection.Left)
                {
                    projectileScript.dir = new Vector2(-curDir.x, curDir.y);
                }

                TurnFinished();
                RemoveStamina(cardTwo.staminaCost);
            }
        }

        public override void ThirdCard()
        {
            if (curStamina >= cardThree.staminaCost)
            {
                GameObject projectileObj = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
                Projectile projectileScript = projectileObj.GetComponentInChildren<Projectile>();
                Vector2 curDir = projectileScript.dir;
                
                if (curFacingDirection == FacingDirection.Right)
                {
                    projectileScript.dir = new Vector2(curDir.x, curDir.y);
                }
                else if (curFacingDirection == FacingDirection.Left)
                {
                    projectileScript.dir = new Vector2(-curDir.x, curDir.y);
                }

                TurnFinished();
                RemoveStamina(cardThree.staminaCost);
            }
        }

    #endregion

}