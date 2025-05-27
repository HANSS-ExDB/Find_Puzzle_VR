using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoad : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("DescriptionScene");  // GameScene 이름은 이동하고자 하는 Scene
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Unity 에디터에서는 이 코드로 "재생 모드"를 중지시켜서 종료처럼 보이게 함
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
