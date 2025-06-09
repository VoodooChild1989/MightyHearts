using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileOwner
{
    Player, 
    Enemy
}

public class Projectile : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public float speed = 5f;
            public int damageAmount = 3;
            public int maxBlocks = 4;
            [ShowOnly] public int curBlocks;
            [ShowOnly] public ProjectileOwner curOwner;
            [ShowOnly] public Vector2 dir;
            [ShowOnly] public Collider2D col;
            [ShowOnly] public Rigidbody2D rb;
            private GameObject collidingObj;
            [ShowOnly] public bool isLast;

        [Header("ANIMATIONS")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            [ShowOnly] public float frameLength = 0.1f;
            private SpriteRenderer sr;
            private GameObject birthVFX;
            private GameObject deathVFX;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            birthVFX = Resources.Load<GameObject>("Prefabs/VFX/Projectile_Birth");
            deathVFX = Resources.Load<GameObject>("Prefabs/VFX/Projectile_Death");
            dir = new Vector2(1f, 0f);
            Instantiate(birthVFX, transform.position, Quaternion.identity);
            StartCoroutine(Animation(idleSprites));
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            transform.position += (Vector3)dir.normalized * speed * Time.deltaTime;
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
            col.enabled = false;
            Instantiate(deathVFX, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(1.1f);

            if (isLast)
            {
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
            }
            
            Destroy(transform.parent);
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