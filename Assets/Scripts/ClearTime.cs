using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ClearTime : MonoBehaviour
{
    public TimeManager TimeManager;
    public TextMeshProUGUI TimeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ① 인스펙터에 할당 안 돼 있으면 씬에서 찾아서 연결
        if (TimeManager == null)
            TimeManager = FindObjectOfType<TimeManager>();

        if (TimeManager == null)
        {
            Debug.LogError("ClearTime: 씬에 TimeManager가 없습니다!");
            return;
        }

        float timer = TimeManager.pasedTime;
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        TimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
