using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class Cell : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int x;
            public int y;
            public GameObject collidesWith;

            [Header("Visuals")]
            public Sprite fullCell;
            public Sprite trajectoryCell;
            public Sprite emptyCell;
            public float fullCellTransparency;
            public float trajectoryCellTransparency;
            public float emptyCellTransparency;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public bool isTrajectory;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            // Perform initial setup that occurs when the game starts.
            // Example: Initialize game state, start coroutines, load resources, etc.
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            CheckVisibility();
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

        public void SetAsTrajectory()
        {
            isTrajectory = true;
        }

        public void RemoveAsTrajectory()
        {
            isTrajectory = false;
        }

        public void CheckVisibility()
        {
            if (TilemapManager.instance.areCellsVisible)
            {
                if (isTrajectory)
                {
                    sr.color = new Color(0f, 0f, 1f, trajectoryCellTransparency);   
                    sr.sprite = trajectoryCell;
                }
                else
                {
                    if (collidesWith != null)
                    {
                        sr.color = new Color(0f, 1f, 0f, fullCellTransparency);   
                        sr.sprite = fullCell;
                    }
                    else
                    {
                        sr.color = new Color(1f, 0f, 0f, emptyCellTransparency);   
                        sr.sprite = emptyCell;
                    }
                }
            }
            else
            {   
                Color origColor = sr.color;  
                Color transColor = new Color(origColor.r, origColor.g, origColor.b, 0f);
                sr.color = transColor;
            }
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            GameObject collidedObj = other.gameObject;
         
            if (collidedObj.GetComponent<Projectile>() != null)
            {   
                collidesWith = collidedObj;
                Projectile projScript = collidedObj.GetComponent<Projectile>();

                if (projScript.curBlocks == projScript.maxBlocks)
                {
                    projScript.StartDeath();
                }
                else
                {
                    projScript.curBlocks++;
                    projScript.TryDamage();
                }
            }   
        }

        /// <summary>
        /// Sent each frame where a collider on another object is touching
        /// this object's collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionStay2D(Collision2D other)
        {
            GameObject collidedObj = other.gameObject;
            TilemapManager.instance.UpdateCollisionMatrix(x, y);
            
            if (collidedObj.GetComponent<IDamageable>() != null)
            {
                collidedObj.GetComponent<CharacterMovement>().AddCellToSizeMatrix(gameObject);
                collidesWith = collidedObj;
            }
            else if (collidedObj.GetComponent<Block>() != null)
            {
                collidesWith = collidedObj;
            }
            else if (collidedObj.GetComponent<Projectile>() != null)
            {
                collidesWith = collidedObj;
            }
        }

        /// <summary>
        /// Sent when a collider on another object stops touching this
        /// object's collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionExit2D(Collision2D other)
        {
            GameObject collidedObj = other.gameObject;
            TilemapManager.instance.UpdateCollisionMatrix(x, y);
            
            if (collidedObj.GetComponent<IDamageable>() != null)
            {
                collidedObj.GetComponent<CharacterMovement>().RemoveCellFromSizeMatrix(gameObject);
                collidesWith = null;
            }
            else if (collidedObj.GetComponent<Block>() != null)
            {
                collidesWith = null;
            }
            else if (collidedObj.GetComponent<Projectile>() != null)
            {
                collidesWith = null;
            }   
        }

    #endregion

}