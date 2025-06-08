using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DefaultButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]

            [Header("Animation")]
            [SerializeField] private float hoverScaleBonus = 0.2f;
            [SerializeField] private float pressScaleBonus = -0.2f;
            [SerializeField] private float animationDuration = 0.2f;
            private float defaultScale = 1.3f;
            private RectTransform rectTransform;
            private Tween currentTween;
            
            [Header("SFX")]
            public AudioClip buttonSFX;
            public float volume = 1f;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        
            buttonSFX = Resources.Load<AudioClip>("ButtonClick");
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            defaultScale = rectTransform.localScale.x;
        }

    #endregion

    #region CUSTOM METHODS

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateScale(defaultScale + hoverScaleBonus);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateScale(defaultScale);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AnimateScale(defaultScale + pressScaleBonus);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AnimateScale(defaultScale + hoverScaleBonus);
        }

        private void AnimateScale(float targetScale)
        {
            currentTween?.Kill();
            currentTween = rectTransform.DOScale(Vector3.one * targetScale, animationDuration).SetEase(Ease.OutBack);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // SFXManager.PlaySFX(buttonSFX, transform, volume);
        }

    #endregion
    
}