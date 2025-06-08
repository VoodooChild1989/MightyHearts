using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public enum ProjectileOwner
{
    Player, 
    Enemy
}

public abstract class Projectile : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public float speed;
            public Vector2 dir;
            public int damageAmount;
            public int maxBlocks;
            [ShowOnly] public int curBlocks;
            [ShowOnly] public ProjectileOwner curOwner;
            [ShowOnly] public Collider2D col;
            [ShowOnly] public Rigidbody2D rb;
            private GameObject collidingObj;

        [Header("ANIMATIONS")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public float frameLength;
            private SpriteRenderer sr;
            public GameObject birthVFX;
            public GameObject deathVFX;

    #endregion

    #region SETUP

        public void TurnToEnemy()
        {
            SetOwner(2);
        }

        public void TurnToPlayer()
        {
            SetOwner(1);
        }
        
    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        public void ProjectileAwake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        public void ProjectileStart()
        {
            Instantiate(birthVFX, transform.position, Quaternion.identity);
            StartCoroutine(Animation(idleSprites));
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        public void ProjectileUpdate()
        {
            transform.position += (Vector3)dir.normalized * speed * Time.deltaTime;
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        public void ProjectileFixedUpdate()
        {
            // Add physics-related logic here.    
            // Example: Rigidbody movement, applying forces, or collision detection.
        }

    #endregion

    #region CUSTOM METHODS

        public void SetOwner(int num)
        {
            if (num == 1)
            {
                curOwner = ProjectileOwner.Player;
                gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
            }
            else if (num == 2)
            {
                curOwner = ProjectileOwner.Enemy;
                gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
            }
        }

        public void Flip()
        {
            Vector2 curDir = dir;
            dir = new Vector2(-curDir.x, curDir.y);
            sr.flipX = true;
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Cell"))
                collidingObj = other.gameObject;
        }

        public void TryDamage()
        {
            if (collidingObj != null)
            {
                collidingObj.GetComponent<Character>()?.GetComponent<IDamageable>().TakeDamage(damageAmount); 
                StartDeath();
            }
        }

        public void StartDeath()
        {
            StartCoroutine(Death());
        }

        public IEnumerator Death()
        {
            speed = 0f;
            sr.color = new Color(0f, 0f, 0f, 0f);
            Instantiate(deathVFX, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(1.1f);

            if (collidingObj != null)
            {
                if (collidingObj.GetComponent<Character>() != null && !collidingObj.GetComponent<Character>().isDead)
                {
                    LevelManager.instance.MoveQueue();
                }
                else if (collidingObj.GetComponent<Block>() != null)
                {
                    LevelManager.instance.MoveQueue();
                }
            }
            else
            {
                LevelManager.instance.MoveQueue();
            }

            Destroy(gameObject);
        }

    #endregion

    #region ANIMATION

        private IEnumerator Animation(Sprite[] sprites)
        {
            while (true)
            {
                foreach (Sprite sprite in sprites)
                {
                    sr.sprite = sprite;
                    yield return new WaitForSeconds(frameLength);
                }
            }
        }

    #endregion

}