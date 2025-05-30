using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Collider))]
public class PlayerHitHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("왼손 XR 인터랙터 (Direct/Ray)")]
    public XRBaseInteractor leftHandInteractor;
    [Tooltip("XR Interaction Manager (샘플 씬에선 Locomotion System 옆에 붙어 있습니다)")]
    public XRInteractionManager interactionManager;
    bool isStunned = false;

    void Reset()
    {
        // Collider는 Trigger 로 두는 걸 추천
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            StunAndDrop();
        }
    }

    void StunAndDrop()
    {
        // 2) 왼손 인터랙터가 쥐고 있는 모든 아이템 드롭
        if (leftHandInteractor != null && interactionManager != null)
        {
            // 복제 리스트를 만들어서 SelectExit 호출
            var selected = new List<IXRSelectInteractable>(leftHandInteractor.interactablesSelected);
            foreach (var interactable in selected)
            {
                interactionManager.SelectExit(leftHandInteractor, interactable);
            }
        }
    }
}
