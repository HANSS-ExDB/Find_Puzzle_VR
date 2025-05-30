using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float pasedTime; //소모된 시간
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void saveTime(float time) {
        Instance.pasedTime = time;
    } 
}
