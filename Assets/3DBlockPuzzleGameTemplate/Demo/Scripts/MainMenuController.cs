using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BlockPuzzleGameTemplate
{
    public class MainMenuController : MonoBehaviour
    {
        LevelManager levelManager;
        InventoryManager inventory;
        TimeManager timeManager;

        void Start()
        {
            levelManager = FindObjectOfType<LevelManager>();
            inventory = FindObjectOfType<InventoryManager>();
            timeManager =FindAnyObjectByType<TimeManager>();
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

        public void ReturnBasic() {
            StartCoroutine(ReturnToBasic());
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
            inventory.items.Clear();
            timeManager.pasedTime = 0f;
            SceneManager.LoadScene("StartScene");
        }

        IEnumerator ReturnToBasic()
        {
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene("BasicScene");

        }
    }
}
