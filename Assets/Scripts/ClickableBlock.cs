using UnityEngine;

public class ClickableBlock : MonoBehaviour
{
    public GameObject blockId; // 또는 타입 이름 등
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
        Debug.Log("[ClickableBlock] 클릭됨: " + blockId.name);
        InventoryManager.Instance.AddItem(blockId.ToString(),mat);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        Debug.Log("[클릭됨] 블록: " + blockId);

        // 인벤토리에 추가
        InventoryManager.Instance.AddItem(blockId);     

        // 오브젝트 삭제
        Destroy(gameObject);
    }
    */
}
