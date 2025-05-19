using BlockPuzzleGameTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEditor.UIElements;
using UnityEngine;
namespace BlockPuzzleGameTemplate
{
    [System.Serializable]
    public class BlockDetails
    {
        public BlockType Type_;
        public Colors Color_;
    }
    public class BlockManager : MonoBehaviour
    {
        ColorLib colorLib_;
        BlockLib blockLib_;
        public Transform spawnPos;
        public BlockTarget [] blockTargets;
        //public List<BlockDetails> blocks = new List<BlockDetails>();
        //미리 정해놓는 블록 리스트 // 인벤토리를 불러오면 필요 X

        private bool isSpawningBlocks = false;
        private void Start ()
        {
            colorLib_ = FindObjectOfType<ColorLib>();
            blockLib_ = FindObjectOfType<BlockLib>();
        }
        void Update ()
        {
            if (!isSpawningBlocks)
            {
                CheckAndSpawnBlocksSequentially();
            }
        }

        private void CheckAndSpawnBlocksSequentially ()
        {
            StartCoroutine(SpawnBlocksSequentially());
        }

        private IEnumerator SpawnBlocksSequentially()
        {
            isSpawningBlocks = true;

            // 1) 인벤토리 데이터 읽어오기
            var entries = InventoryManager.Instance.items;
            int spawnCount = Mathf.Min(blockTargets.Length, entries.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                var entry = entries[i];              // InventoryEntry
                string typeName = entry.Type;        // 저장된 블록 타입 문자열

                // 2) 문자열 → BlockType enum 변환
                if (!Enum.TryParse<BlockType>(typeName, out var blockType))
                {
                    Debug.LogWarning($"BlockType 변환 실패: {typeName}");
                    continue;
                }

                var slot = blockTargets[i];
                if (slot.TheOwner != null || !slot.gameObject.activeSelf)
                    continue;

                // 3) 프리팹 가져와 Instantiate (부모에 바로 붙이기)
                GameObject prefab = blockLib_.GetBlockPref(blockType);
                if (prefab == null)
                {
                    Debug.LogWarning($"프리팹을 못 찾음: {typeName}");
                    continue;
                }
                GameObject go = Instantiate(
                    prefab,
                    spawnPos.position,
                    Quaternion.identity,
                    slot.transform
                );
                go.transform.localScale = Vector3.one * 0.5f;
                go.transform.localRotation = Quaternion.identity;

                // 4) 인벤토리에 저장된 머티리얼(entry.Mat) 적용
                if (entry.Mat != null)
                    MaterialSwap(go, entry.Mat);
                else
                    Debug.LogWarning($"머티리얼이 없습니다: {typeName}");

                // 5) 슬롯 소유권 표시
                slot.TheOwner = go;

                // 6) 이동 애니메이션
                yield return moveToPosition(go, slot);
            }

            isSpawningBlocks = false;
        }


        private IEnumerator moveToPosition ( GameObject block , BlockTarget target )
        {
            Vector3 startPos = block.transform.localPosition;
            Vector3 targetPos = Vector3.zero;
            float duration = 0.35f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                block.transform.localPosition = Vector3.Lerp(startPos , targetPos , elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            block.transform.localPosition = targetPos;
            yield return DoPunchEffect(block , targetPos);
        }

        private IEnumerator DoPunchEffect ( GameObject block , Vector3 targetPos )
        {
            float punchDuration = 0.2f;
            float punchMagnitude = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < punchDuration)
            {
                float punchProgress = elapsedTime / punchDuration;
                float dampenedPunch = Mathf.Sin(punchProgress * Mathf.PI * 2) * ( 1 - punchProgress ) * punchMagnitude;
                block.transform.localPosition = targetPos + new Vector3(dampenedPunch , 0 , 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            block.transform.localPosition = targetPos;

        }

        private void MaterialSwap ( GameObject block , Material Mat )
        {
            MeshRenderer [] rend = block.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0 ; i < rend.Length ; i++)
            {
                rend [i].material = Mat;
            }
        }
    }
}
