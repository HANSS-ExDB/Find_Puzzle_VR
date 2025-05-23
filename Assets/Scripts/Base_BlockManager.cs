using BlockPuzzleGameTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base_BlockPuzzleGameTemplate
{
    [System.Serializable]
    public class BlockDetails
    {
        public BlockType Type_;
        public Colors Color_;
    }
    public class Base_BlockManager : MonoBehaviour
    {
        ColorLib colorLib_;
        BlockLib blockLib_;
        public Transform spawnPos;
        public BlockTarget [] blockTargets;
        public List<BlockDetails> blocks = new List<BlockDetails>();

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

        private IEnumerator SpawnBlocksSequentially ()
        {
            isSpawningBlocks = true;

            for (int i = 0 ; i < blockTargets.Length ; i++)
            {
                if (blockTargets [i].TheOwner == null && blockTargets [i].gameObject.activeSelf)
                {
                    if (blocks.Count == 0)
                    {
                        //Debug.LogWarning("No more blocks to spawn.");
                        break;
                    }
                    // Instantiate block from the first in the list


                    GameObject block = blockLib_.GetBlockPref(blocks [0].Type_);
                    GameObject go = Instantiate(block , spawnPos.position , Quaternion.identity);

                    //var click = go.AddComponent<ClickableBlock>();
                    //click.blockId = blocks[0].Type_.ToString(); ; // 예: "L_block", "T_block"
                    //블록 클릭 상호작용, 삭제됨


                    go.transform.Rotate(0f, 0f, -90f);
                    go.GetComponent<Block>().type = blocks [0].Type_;
                    MaterialSwap(go , colorLib_.GetMaterial(blocks [0].Color_));
                    blockTargets [i].TheOwner = go;
                    go.transform.SetParent(blockTargets [i].transform);
                    blocks.RemoveAt(0);
                    go.transform.localScale = Vector3.one / 2;
                    yield return moveToPosition(go , blockTargets [i]);

                }

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
