using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public Sprite[] runningSprites;
            public Sprite[] attackSprites;
            public Sprite[] damagedSprites;
            [ShowOnly] public float frameLength = 0.1f;
            [ShowOnly] public float damagedFrameLength = 1f;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public GameObject deathVFX;
            private Coroutine animationCrt;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            deathVFX = Resources.Load<GameObject>("Prefabs/VFX/Character_Death");   
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            SetIdleAnimation();
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

    #region ANIMATION

        public void SetIdleAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(idleSprites));
        }

        public void SetRunningAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(runningSprites));
        }

        public void SetAttackAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(attackSprites, true));
        }

        public void SetDamagedAnimation()
        {
            CheckAnimationCoroutine();
            animationCrt = StartCoroutine(Animation(damagedSprites, true, true, damagedFrameLength));
        }

        private void CheckAnimationCoroutine()
        {
            if (animationCrt != null)
            {
                StopCoroutine(animationCrt);
                animationCrt = null;
            }
        }
        
        private IEnumerator Animation(Sprite[] sprites, bool playOnce = false, bool useNewSpeed = false, float newSpeed = 0f)
        {
            while (true)
            {
                foreach (Sprite sprite in sprites)
                {
                    sr.sprite = sprite;

                    if (!useNewSpeed)
                    {
                        yield return new WaitForSeconds(frameLength);
                    }
                    else
                    {
                        yield return new WaitForSeconds(newSpeed);
                    }
                }

                if (playOnce) 
                {
                    SetIdleAnimation();
                    yield break;
                }
            }
        }


    #endregion

}