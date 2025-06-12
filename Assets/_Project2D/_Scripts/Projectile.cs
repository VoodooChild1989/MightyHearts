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

            public CardType curCardType;
            public bool pushedSomebody;
            public int maxNumberInSet;
            public int curNumberInSet;
            public CharacterStatistics pushedChr;
            public CharacterCards cardsScript;

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
            Instantiate(birthVFX, transform.position, Quaternion.identity);
            StartCoroutine(Animation(idleSprites));
            dir = new Vector2(1f, 0f);
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
            speed = -speed;
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
                speed = 0f;
                sr.color = new Color(0f, 0f, 0f, 0f);
                col.enabled = false;
                Instantiate(deathVFX, transform.position, Quaternion.identity);

                CharacterStatistics chr = collidingObj.GetComponent<CharacterStatistics>();
                
                if (chr != null)
                {
                    chr.GetComponent<IDamageable>().TakeDamage(damageAmount); 

                    if (curCardType == CardType.Poison)
                    {   
                        chr.isPoisoned = true;
                        chr.poisonedTurnsLeft = 3;
                    }
                    else if (curCardType == CardType.PushBack)
                    {
                        if (sr.flipX)
                        {
                            if (chr.chrMove.IsThisNewPositionPossible(-1, 0))
                            {                            
                                cardsScript.hasSetPushedSomebody = true;
                                cardsScript.pushedChr = chr;
                                chr.chrMove.Move(-1, 0, false);
                            }
                        }
                        else
                        {
                            if (chr.chrMove.IsThisNewPositionPossible(1, 0))
                            {                            
                                cardsScript.hasSetPushedSomebody = true;
                                cardsScript.pushedChr = chr;
                                chr.chrMove.Move(1, 0, false);
                            }
                        }
                    }
                    else if (curCardType == CardType.Pull)
                    {
                        if (sr.flipX)
                        {
                            if (chr.chrMove.IsThisNewPositionPossible(1, 0))
                            {                            
                                cardsScript.hasSetPushedSomebody = true;
                                cardsScript.pushedChr = chr;
                                chr.chrMove.Move(1, 0, false);
                            }
                        }
                        else
                        {
                            if (chr.chrMove.IsThisNewPositionPossible(-1, 0))
                            {                            
                                cardsScript.hasSetPushedSomebody = true;
                                cardsScript.pushedChr = chr;
                                chr.chrMove.Move(-1, 0, false);
                            }
                        }
                    }
                }

                StartDeath();
            }
        }

        public void StartDeath()
        {
            StartCoroutine(Death());
        }

        public IEnumerator Death()
        {
            yield return new WaitForSeconds(1.1f);

            if (curNumberInSet == maxNumberInSet)
            {
                if (collidingObj != null)
                {
                    if (collidingObj.GetComponent<CharacterStatistics>() != null && !collidingObj.GetComponent<CharacterStatistics>().isDead)
                    {
                        if (pushedSomebody)
                        {
                            pushedChr.chrMove.Move(0, 0, true, true);
                        }
                        else
                        {
                            QueueManager.instance.MoveQueue();   
                        }
                    }
                    else if (collidingObj.GetComponent<Block>() != null)
                    {
                        QueueManager.instance.MoveQueue();
                    }
                }
                else
                {
                    QueueManager.instance.MoveQueue();
                }
            }
            
            Destroy(transform.parent.gameObject);
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