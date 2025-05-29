using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ClearTime : MonoBehaviour
{
    public TimeManager TimeManager;
    public TextMeshProUGUI TImeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float timer = TimeManager.pasedTime;
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        TImeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
