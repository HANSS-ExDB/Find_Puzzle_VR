using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRSceneController : MonoBehaviour
{
    private InputDevice rightHand;

    void Start()
    {
        // 오른손 컨트롤러 디바이스 검색
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand
                                   | InputDeviceCharacteristics.Right
                                   | InputDeviceCharacteristics.Controller;
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);

        if (devices.Count > 0)
            rightHand = devices[0];
        else
            Debug.LogWarning("오른손 컨트롤러를 찾지 못했습니다.");
    }

    void Update()
    {
        if (!rightHand.isValid)
            return;

        // A 버튼(Primary Button) 체크
        if (rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed)
            && aPressed)
        {
            // 현재 씬 재시작
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // B 버튼(Secondary Button) 체크
        if (rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed)
            && bPressed)
        {
            // BasicScene 로드
            SceneManager.LoadScene("BasicScene");
        }
    }
}
