using UnityEngine.SceneManagement;
using UnityEngine;
using BlockPuzzleGameTemplate;
using System.Linq;

public class PortalSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player;       // �ν����Ϳ� �Ҵ�
    [SerializeField] private float requiredTime = 1f;

    private float timer = 0f;
    private bool isPlayerOnPortal = false;
    private bool hasGrabbed;

    private void OnTriggerEnter(Collider other)
    {
        // �ֻ��� ��Ʈ ��
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
        // 1) �ֻ��� Block ������Ʈ ã��
        Transform top = player.transform
            .GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.CompareTag("Block"));
        if (top == null) return;

        // 2) Block ������Ʈ ��������
        var blockComp = top.GetComponent<Block>();
        if (blockComp == null) return;

        // 3) Ÿ�� ���ڿ� �غ�
        string typeName = blockComp.type.ToString();

        // 4) ��Ƽ���� �غ� (���������� ��������)
        //    �ʿ信 ���� sharedMaterial ��� material ��� ����
        var rend = top.GetComponentInChildren<Renderer>();
        Material mat = rend != null
            ? rend.sharedMaterial
            : null;

        // 5) �κ��丮�� Ÿ�԰� ��Ƽ���� �Բ� ����
        if (InventoryManager.Instance.AddItem(typeName, mat))
        {
            // 6) ���� ������Ʈ�� ������ ����
            Destroy(top.gameObject);
            Debug.Log($"[�κ��丮] {typeName} ����� �����߽��ϴ�.");
        }
    }
}
