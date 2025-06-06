using UnityEngine;

/// <summary>
/// Manages sound effects in the game.
/// </summary>
public class SFXManager : MonoBehaviour
{

    #region CUSTOM METHODS

        /// <summary>
        /// Plays a sound effect at a specified position with a given volume.
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        /// <param name="spawnTransform">The position to play the sound effect.</param>
        /// <param name="volume">The volume of the sound effect (0.0 - 1.0).</param>
        public static void PlaySFX(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            if (audioClip == null)
            {
                MyGame.Utils.SystemComment("AudioClip is null! Cannot play sound effect!");
                return;
            }

            AudioSource tempSource = new GameObject("TemporaryAudioSource").AddComponent<AudioSource>();
            tempSource.clip = audioClip;
            tempSource.volume = Mathf.Clamp01(volume);
            tempSource.transform.position = spawnTransform.position;
  
            tempSource.Play();
            Object.Destroy(tempSource.gameObject, audioClip.length);
        }

    #endregion
}