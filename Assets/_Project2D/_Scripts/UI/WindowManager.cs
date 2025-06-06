using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]            
            public GameObject window;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            CloseWindow();
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Opening a window.
        /// </summary>
        public void OpenWindow()
        {
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Closing a window.
        /// </summary>
        public void CloseWindow()
        {      
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = false;
        }

    #endregion

}