// EventSystemCleaner.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemCleaner : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<EventSystem>().Length > 1)
        {
            Destroy(gameObject); // 자신이 중복된 EventSystem이면 파괴
        }
    }
}
