using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem; //��� UnityEngine.XR�� ����ص� �����.
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
        // 1) ��� ��������
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);

        // 2) �Է� �б� �õ�
        bool gotInput = device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        // ����̽��� ������ ��ȿ����

        // 3) ����� �α� ���
        Debug.Log($"[DBG] Device valid: {device.isValid}, name: {device.name}, characteristics: {device.characteristics}");

        // �����ϴ� ���ĵ� ����Ʈ (���� �̸���)
        List<InputFeatureUsage> features = new List<InputFeatureUsage>();
        device.TryGetFeatureUsages(features);
        foreach (var f in features)
            Debug.Log($"[DBG]  �� Supports: {f.name} ({f.type})");


        // 4) ���� �̵� ó��
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