using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class SceneChanger : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public PlayableDirector levelOutroDirector;

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Method used to trigger the coroutine with the level outro animation.
        /// </summary>
        public void ChangeScene(string sceneName)
        {
            if (levelOutroDirector != null)
            {
                StartCoroutine(StartChangingScene(levelOutroDirector, sceneName));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        /// <summary>
        /// The coroutine with the level outro animation.
        /// </summary>
        private IEnumerator StartChangingScene(PlayableDirector levelOutroDir, string sceneName)
        {
            levelOutroDir.Play();

            yield return new WaitForSeconds((float)levelOutroDir.duration);

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

    #endregion

}