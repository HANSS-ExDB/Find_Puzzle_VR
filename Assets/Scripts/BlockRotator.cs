using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using BlockPuzzleGameTemplate;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BlockRotator : MonoBehaviour
{
    [Header("Left Hand/Input")]
    public XRBaseInteractor leftHandInteractor;
    public XRRayInteractor leftRayInteractor;

    // 회전 쿨타임 (초)
    public float rotationCooldown = 0.5f;
    private float cooldownTimer = 0f;

    private InputDevice leftHandDevice;

    void Start()
    {
        // 왼손 디바이스 가져오기
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0)
            leftHandDevice = devices[0];
    }

    void Update()
    {
        // 쿨타임 감소
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f && leftHandDevice.isValid)
        {
            // X 버튼 (Primary) → 시계방향 회전
            if (leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed) && xPressed)
            {
                if (TryGetTarget(out Transform target))
                {
                    Rotate90Y(target);
                    cooldownTimer = rotationCooldown;
                }
            }
            // Y 버튼 (Secondary) → 반시계방향 회전
            else if (leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool yPressed) && yPressed)
            {
                if (TryGetTarget(out Transform target))
                {
                    Rotate90Y_Rv(target);
                    cooldownTimer = rotationCooldown;
                }
            }
        }
    }

    // 현재 잡고 있거나 레이에 맞은 블록을 찾아 리턴
    bool TryGetTarget(out Transform target)
    {
        if (leftHandInteractor.interactablesSelected.Count > 0)
        {
            target = leftHandInteractor.interactablesSelected[0].transform;
            return true;
        }
        else if (leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            var hitGO = hit.transform;
            if (hitGO.CompareTag("Block") || hitGO.GetComponent<Block>() != null)
            {
                target = hitGO;
                return true;
            }
        }

        target = null;
        return false;
    }

    void Rotate90Y(Transform t) => t.Rotate(0f, 90f, 0f, Space.World);
    void Rotate90Y_Rv(Transform t) => t.Rotate(0f, -90f, 0f, Space.World);
}
