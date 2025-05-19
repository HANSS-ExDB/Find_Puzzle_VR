using UnityEngine;

public class ClickableBlock : MonoBehaviour
{
    public GameObject blockId; // �Ǵ� Ÿ�� �̸� ��
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ClickableBlock cb = hit.collider.GetComponentInParent<ClickableBlock>();
                if (cb != null)  cb.OnClicked();
            }
        }
    }

    private void OnClicked() { }

    /*
    public void OnClicked()
    {
        Debug.Log("[ClickableBlock] Ŭ����: " + blockId.name);
        InventoryManager.Instance.AddItem(blockId.ToString(),mat);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        Debug.Log("[Ŭ����] ���: " + blockId);

        // �κ��丮�� �߰�
        InventoryManager.Instance.AddItem(blockId);     

        // ������Ʈ ����
        Destroy(gameObject);
    }
    */
}
