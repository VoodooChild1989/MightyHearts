using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WindowManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]            
            public GameObject window;
            public float tweenDuration;
            private CanvasGroup canvasGroup;
            private RectTransform rectTransform;
            private float initX;
            private float initY;
            private bool isTweening;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            rectTransform = window.GetComponent<RectTransform>();
            canvasGroup = window.GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            // Setup
            initX = rectTransform.anchoredPosition.x;
            initY = rectTransform.anchoredPosition.y;

            isTweening = false;

            // CloseCard();
            // CloseButton()
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenCard()
        {
            if (isTweening) return;

            isTweening = true;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY + 200f);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY - 10f), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.8f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(1f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                isTweening = false;
            });
        }

        /// <summary>
        /// Closing a window.
        /// </summary>
        public void CloseCard()
        {   
            if (isTweening) return;

            isTweening = true;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY - 10f);
            rectTransform.localScale = new Vector2(0.8f, 0.8f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY + 200f), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.2f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(0f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                isTweening = false;
            });
        }

        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenButton()
        {
            initX = 834f;    
            initY = -429f;
            
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX + 200f, initY);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(3f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(1f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
        }

        /// <summary>
        /// Closing a window.
        /// </summary>
        public void CloseButton()
        {   
            initX = 834f; 
            initY = -429f;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(3f, 3f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX + 200f, initY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.2f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(0f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
        }

        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenInformation()
        {
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(1f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(1f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
        }

        /// <summary>
        /// Closing a window.
        /// </summary>
        public void CloseInformation()
        {   
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(1f, 1f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.2f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(0f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
        }

    #endregion

}