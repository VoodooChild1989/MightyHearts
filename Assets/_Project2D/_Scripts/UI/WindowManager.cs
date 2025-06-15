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
            private bool isTweening;

        [Header("VARIABLES")]
            
            [Header("Opened")]
            private float openedPosX;
            private float openedPosY;
            private float openedScale;

            [Header("Closed")]        
            public float closedPosX;
            public float closedPosY;    
            public float closedScale;

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
            openedPosX = rectTransform.anchoredPosition.x;
            openedPosY = rectTransform.anchoredPosition.y;
            openedScale = rectTransform.localScale.x;

            isTweening = false;
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenWindow()
        {
            isTweening = true;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(closedPosX, closedPosY);
            rectTransform.localScale = new Vector2(closedScale, closedScale);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(openedPosX, openedPosY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(openedScale, tweenDuration).SetEase(Ease.InOutSine));
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
        public void CloseWindow()
        {   
            if (isTweening) return;

            isTweening = true;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(openedPosX, openedPosY);
            rectTransform.localScale = new Vector2(openedScale, openedScale);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(closedPosX, closedPosY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(closedScale, tweenDuration).SetEase(Ease.InOutSine));
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
        public void OpenCard()
        {
            // if (isTweening) return;

            isTweening = true;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(openedPosX, openedPosY + 200f);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;

            // Button
            Button button = GetComponent<Button>();
            button.interactable = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(openedPosX, openedPosY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.7f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(1f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                button.interactable = true;
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
            rectTransform.anchoredPosition = new Vector2(openedPosX, openedPosY);
            rectTransform.localScale = new Vector2(0.7f, 0.7f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            // Button
            Button button = GetComponent<Button>();
            button.interactable = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(openedPosX, openedPosY + 200f), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.2f, tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(canvasGroup.DOFade(0f, tweenDuration).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                button.interactable = false;
                isTweening = false;
            });
        } 
        
        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenInformation(CardSO card)
        {
            TilemapManager.instance.ShowTrajectory(card);

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(openedPosX, openedPosY);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(openedPosX, openedPosY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(0.8f, tweenDuration).SetEase(Ease.InOutSine));
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
        public void CloseInformation(CardSO card)
        {   
            TilemapManager.instance.HideTrajectory(card);

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(openedPosX, openedPosY);
            rectTransform.localScale = new Vector2(0.8f, 0.8f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(openedPosX, openedPosY), tweenDuration).SetEase(Ease.InOutSine));
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