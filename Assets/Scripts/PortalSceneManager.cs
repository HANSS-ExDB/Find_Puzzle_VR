using UnityEngine.SceneManagement;
using UnityEngine;
using BlockPuzzleGameTemplate;
using System.Linq;

public class PortalSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player;       // 인스펙터에 할당
    [SerializeField] private float requiredTime = 1f;

    private float timer = 0f;
    private bool isPlayerOnPortal = false;
    private bool hasGrabbed;

    private void OnTriggerEnter(Collider other)
    {
        // 최상위 루트 비교
        if (other.transform.root.gameObject == player)
        {
            isPlayerOnPortal = true;
            hasGrabbed = false;
            timer = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!hasGrabbed && isPlayerOnPortal && other.transform.root.gameObject == player)
        {
            timer += Time.deltaTime;
            if (timer >= requiredTime)
            {
                hasGrabbed = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GrabedBlock();
                Debug.Log("Mouse Unlocked");
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
}
