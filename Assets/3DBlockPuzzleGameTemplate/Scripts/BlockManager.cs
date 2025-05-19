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
        //�̸� ���س��� ��� ����Ʈ // �κ��丮�� �ҷ����� �ʿ� X

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

            // 1) �κ��丮 ������ �о����
            var entries = InventoryManager.Instance.items;
            int spawnCount = Mathf.Min(blockTargets.Length, entries.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                var entry = entries[i];              // InventoryEntry
                string typeName = entry.Type;        // ����� ��� Ÿ�� ���ڿ�

                // 2) ���ڿ� �� BlockType enum ��ȯ
                if (!Enum.TryParse<BlockType>(typeName, out var blockType))
                {
                    Debug.LogWarning($"BlockType ��ȯ ����: {typeName}");
                    continue;
                }

                var slot = blockTargets[i];
                if (slot.TheOwner != null || !slot.gameObject.activeSelf)
                    continue;

                // 3) ������ ������ Instantiate (�θ� �ٷ� ���̱�)
                GameObject prefab = blockLib_.GetBlockPref(blockType);
                if (prefab == null)
                {
                    Debug.LogWarning($"�������� �� ã��: {typeName}");
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

                // 4) �κ��丮�� ����� ��Ƽ����(entry.Mat) ����
                if (entry.Mat != null)
                    MaterialSwap(go, entry.Mat);
                else
                    Debug.LogWarning($"��Ƽ������ �����ϴ�: {typeName}");

                // 5) ���� ������ ǥ��
                slot.TheOwner = go;

                // 6) �̵� �ִϸ��̼�
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
