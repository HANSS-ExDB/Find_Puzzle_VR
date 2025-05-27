using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DescriptionSceneLoad : MonoBehaviour
{

    public void LoadGameScene()
    {
        SceneManager.LoadScene("BasicScene");  // GameScene 이름은 본 게임 씬 이름으로
    }

}
