using UnityEngine;
using BlockPuzzleGameTemplate;
public class BlockGrabber : MonoBehaviour
{
    public Transform leftHandTransform; // 손 위치
    private GameObject currentCollidingBlock; // 손 근처에 감지된 블록
    private GameObject heldBlock;

    private Quaternion Block_rot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentCollidingBlock != null && heldBlock == null)
            {
                Grab(currentCollidingBlock);
                Debug.Log("Grab");
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (heldBlock != null)
            {
                Release();
                Debug.Log("Release");
            }
        }
    }

    private void Grab(GameObject block)
    {
        // 현재 충돌한 오브젝트 기준으로 가장 가까운 Block 컴포넌트를 가진 부모 찾기
        Block blockComponent = block.GetComponentInParent<Block>();
        if (blockComponent == null) return;

        heldBlock = blockComponent.gameObject;
        Block_rot = blockComponent.transform.rotation;

        //heldBlock.transform.SetParent(null, true);
        heldBlock.transform.SetParent(leftHandTransform);
        heldBlock.transform.localPosition = Vector3.zero;
        heldBlock.transform.localRotation = Quaternion.identity;

        foreach (var rb in heldBlock.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void Release()
    {
        // 1) 손에서 떼어내기 (부모 해제)
        heldBlock.transform.SetParent(null, true);
        // 1.5) 손에서 놓을떄 회전
        heldBlock.transform.rotation = Block_rot;

        // 2) Block 컴포넌트의 ReturnToPosition 코루틴 호출
        var blockScript = heldBlock.GetComponent<Block>();
        if (blockScript != null)
        {
            // MonoBehaviour.StartCoroutine 을 이용해 코루틴 실행
            blockScript.StartCoroutine(blockScript.ReturnToPosition());
        }

        // 3) heldBlock 레퍼런스 초기화
        heldBlock = null;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (heldBlock != null) return; // 이미 잡고 있으면 무시
        if (other.CompareTag("Block"))
        {
            currentCollidingBlock = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentCollidingBlock == other.gameObject)
        {
            currentCollidingBlock = null;
        }
    }
}
