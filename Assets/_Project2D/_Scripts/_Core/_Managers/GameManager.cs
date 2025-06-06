using System.Collections;
using UnityEngine;

/// <summary>
/// The one of the most important scripts in the game.
/// Used to manage the game state.
/// </summary>
public class GameManager : MonoBehaviour
{    

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public bool isGamePlayed;
            public static GameManager instance;

            [Header("Data")]
            public int totalPlayTimeDuration;
            public int gameSessionDuration;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this);
            CheckGameState();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            StartCoroutine(GameTimeCounter());
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

    #region CUSTOM METHODS
        
        /// <summary>
        /// Checks whether game was lanched for the first time or not by getting the value from the PlayerPrefs.
        /// </summary>
        public void CheckGameState()
        {
            isGamePlayed = PlayerPrefs.GetInt("IsGamePlayed", 0) == 1; 

            if (isGamePlayed)
            {
                totalPlayTimeDuration = PlayerPrefs.GetInt("TotalPlayTimeDuration");

                MyGame.Utils.SystemComment("The game was opened not for the first time!");
            }
            else
            {
                MyGame.Utils.SystemComment("The game was opened for the first time!");
            }

            PlayerPrefs.SetInt("IsGamePlayed", 1);
        }

        /// <summary>
        /// Restarts the game by reloading the current scene.
        /// </summary>
        [ContextMenu("Restart The Game")]
        public void RestartTheGame()
        {
            PlayerPrefs.DeleteAll();

            MyGame.Utils.SystemComment("The game has been restarted!");
        }

        /// <summary>
        /// The method triggered when the user closes the game.
        /// </summary>
        public void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("TotalPlayTimeDuration", totalPlayTimeDuration);

            MyGame.Utils.SystemComment("The game has been closed!");
        }

        /// <summary>
        /// Counts seconds in the game.
        /// </summary>
        private IEnumerator GameTimeCounter()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                gameSessionDuration++;   
                totalPlayTimeDuration++;   
            }
        }

    #endregion

}