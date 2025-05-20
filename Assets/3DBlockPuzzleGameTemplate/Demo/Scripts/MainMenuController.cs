using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BlockPuzzleGameTemplate
{
    public class MainMenuController : MonoBehaviour
    {
        LevelManager levelManager;

        void Start()
        {
            levelManager = FindObjectOfType<LevelManager>();
            StartGame();
        }

        public void StartGame()
        {
            StartCoroutine(LevelLoader());
        }

        IEnumerator LevelLoader()
        {
            yield return new WaitForSeconds(.5f);
            levelManager.ActivateLevels();
            yield return null;
        }

        public void Restart()
        {
            StartCoroutine(RestartLevel(false));
        }

        public void NextLevel()
        {
            StartCoroutine(GoToNextLevel());
        }

        IEnumerator GoToNextLevel()
        {
            yield return new WaitForSeconds(1f);
            levelManager.EndLevelScreen();
            yield return null;
        }

        IEnumerator RestartLevel(bool isFromPause)
        {
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
