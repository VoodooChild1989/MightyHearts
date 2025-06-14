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
            // if (isTweening) return;

            isTweening = true;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY + 200f);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;

            // Button
            Button button = GetComponent<Button>();
            button.interactable = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
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
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(0.7f, 0.7f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            // Button
            Button button = GetComponent<Button>();
            button.interactable = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY + 200f), tweenDuration).SetEase(Ease.InOutSine));
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
        public void OpenButton()
        {
            initX = 862f;    
            initY = -451f;
            
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX + 200f, initY);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join(rectTransform.DOScale(3.5f, tweenDuration).SetEase(Ease.InOutSine));
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
            initX = 862f; 
            initY = -451f;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(3.5f, 3.5f);

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
        public void OpenStepsWindow()
        {
            // initX = 862f;    
            // initY = -451f;
            
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY - 200f);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
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
        public void CloseStepsWindow()
        {   
            // initX = 862f; 
            // initY = -451f;

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(0.8f, 0.8f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY - 200f), tweenDuration).SetEase(Ease.InOutSine));
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
        public void OpenInformation(CardSO card)
        {
            TilemapManager.instance.ShowTrajectory(card);

            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
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
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(0.8f, 0.8f);

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


        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenStart()
        {
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY + 200f);
            rectTransform.localScale = new Vector2(0.2f, 0.2f);

            // CanvasGroup
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;   
            canvasGroup.blocksRaycasts = false;
                        
            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOAnchorPos(new Vector2(initX, initY), tweenDuration).SetEase(Ease.InOutSine));
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
        public void CloseStart()
        {   
            // RectTransform
            rectTransform.anchoredPosition = new Vector2(initX, initY);
            rectTransform.localScale = new Vector2(0.8f, 0.8f);

            // CanvasGroup
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
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
            });
        }

    #endregion

}