using UnityEngine.SceneManagement;
using UnityEngine;
using BlockPuzzleGameTemplate;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PortalSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player;       // 인스펙터에 할당 //Player루트
    [SerializeField] private float requiredTime = 1f;
    [SerializeField] private CountdownXR Timer; //타이머

    // ← Inspector에 할당할 XR 인터랙터(Direct 또는 Ray 중 하나)
    [SerializeField] private XRBaseInteractor leftHandInteractor;
    [SerializeField] private XRBaseInteractor rightHandInteractor;

    private float timer = 0f;
    private bool isPlayerOnPortal = false;
    //private bool hasGrabbed;
    //private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        // 최상위 루트 비교
        if (other.transform.root.gameObject == player)
        {
            isPlayerOnPortal = true;
            //hasGrabbed = false;
            timer = 0f;
            Debug.Log("Player On Portal");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //  hasGrabbed 필요하면 조건에 추가
        if (isPlayerOnPortal && other.transform.root.gameObject == player)
        {
            timer += Time.deltaTime;
            if (timer >= requiredTime)
            {
                //hasGrabbed = true;
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                //GrabedBlock();
                //Debug.Log("Mouse Unlocked");
                StoreGrabbedBlockToInventory();
                SceneManager.LoadScene("PuzzleScene");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.gameObject == player)
        {
            isPlayerOnPortal = false;
            timer = 0f;
        }
    }

    private void GrabedBlock()
    {
        // 1) 최상위 Block 오브젝트 찾기
        Transform top = player.transform
            .GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Block"));
        if (top == null) return;

        // 2) Block 컴포넌트 가져오기
        var blockComp = top.GetComponent<Block>();
        if (blockComp == null) return;

        // 3) 타입 문자열 준비
        string typeName = blockComp.type.ToString();

        // 4) 머티리얼 준비 (렌더러에서 가져오기)
        //    필요에 따라 sharedMaterial 대신 material 사용 가능
        var rend = top.GetComponentInChildren<Renderer>();
        Material mat = rend != null
            ? rend.sharedMaterial
            : null;

        // 5) 인벤토리에 타입과 머티리얼 함께 저장
        if (InventoryManager.Instance.AddItem(typeName, mat))
        {
            // 6) 실제 오브젝트는 씬에서 제거
            Destroy(top.gameObject);
            Debug.Log($"[인벤토리] {typeName} 블록을 보관했습니다.");
        }
    }
    private void StoreGrabbedBlockToInventory()
    {
        // 0) 진행 시간을 저장 한다.
        TimeManager.Instance.saveTime(Timer.timer);

        // 1) 좌/우 컨트롤러 중 하나가 잡고 있는 인터랙터를 가져온다
        var leftSel = leftHandInteractor.GetOldestInteractableSelected();
        var rightSel = rightHandInteractor.GetOldestInteractableSelected();
        XRGrabInteractable grabbed = null;

        if (leftSel is XRGrabInteractable l) grabbed = l;
        else if (rightSel is XRGrabInteractable r) grabbed = r;

        if (grabbed == null)
        {
            Debug.LogWarning("[Portal] 현재 잡고 있는 블록이 없습니다.");
            return;
        }
        // 2) 실제 블록 게임오브젝트
        GameObject blockGO = grabbed.gameObject;

        // 3) Block 컴포넌트, 타입, 머티리얼 추출
        var blockComp = blockGO.GetComponent<Block>();
        if (blockComp == null) return;

        string typeName = blockComp.type.ToString();
        var rend = blockGO.GetComponentInChildren<Renderer>();
        Material mat = rend != null ? rend.sharedMaterial : null;

        // 4) 인벤토리에 저장
        if (InventoryManager.Instance.AddItem(typeName, mat))
        {
            Destroy(blockGO);
            Debug.Log($"[인벤토리] {typeName} 블록을 보관했습니다.");
        }
    }
}
