using UnityEngine;
using UnityEngine.SceneManagement;
using GridGame.Application;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Handles interactions in the Main Menu scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Campaign Configuration")]
        [SerializeField] private CampaignData campaignData;

        /// <summary>
        /// Starts a random game by loading the game scene with procedural mode.
        /// </summary>
        public void PlayRandom()
        {
            GameSessionConfig.Reset();
            GameSessionConfig.Mode = GameMode.Procedural;
            SceneManager.LoadScene("Playground");
        }

        /// <summary>
        /// Starts the campaign by loading the game scene with predefined mode.
        /// </summary>
        public void PlayCampaign()
        {
            if (campaignData == null)
            {
                Debug.LogError("MainMenuController: Campaign Data is not assigned!", this);
                return;
            }

            if (campaignData.levels.Count == 0)
            {
                Debug.LogError("MainMenuController: Campaign has no levels!", this);
                return;
            }

            GameSessionConfig.Reset();
            GameSessionConfig.Mode = GameMode.Predefined;
            GameSessionConfig.CurrentCampaign = campaignData;
            GameSessionConfig.CurrentLevelIndex = 0;
            SceneManager.LoadScene("Playground");
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
