using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem; //대신 UnityEngine.XR만 사용해도 충분함.
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovementVR : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public XRNode inputSource = XRNode.LeftHand;

    private Vector2 inputAxis;
    private CharacterController characterController;
    private Transform head;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        head = Camera.main.transform;
    }

    void Update()
    {
        // 1) 기기 가져오기
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

        // 2) 입력 읽기 시도
        bool gotInput = device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        // 디바이스가 실제로 유효한지

        // 3) 디버그 로그 출력
        Debug.Log($"[DBG] Device valid: {device.isValid}, name: {device.name}, characteristics: {device.characteristics}");

        // 지원하는 피쳐들 리스트 (피쳐 이름만)
        List<InputFeatureUsage> features = new List<InputFeatureUsage>();
        device.TryGetFeatureUsages(features);
        foreach (var f in features)
            Debug.Log($"[DBG]  └ Supports: {f.name} ({f.type})");


        // 4) 실제 이동 처리
        if (gotInput && inputAxis.sqrMagnitude > 0.01f)
        {
            Vector3 direction = new Vector3(inputAxis.x, 0, inputAxis.y);
            Vector3 headYaw = new Vector3(head.forward.x, 0, head.forward.z).normalized;
            Quaternion rotation = Quaternion.LookRotation(headYaw);
            Vector3 movement = rotation * direction;

            characterController.Move(movement * moveSpeed * Time.deltaTime);
        }
    }
}