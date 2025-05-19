using UnityEngine;

namespace BlockPuzzleGameTemplate
{
    public enum BlockType
    {
        Square,
        MediumSquare,
        RectangularHorizontal,
        RectangularVertical,
        LargeRectangularVertical,
        LargeRectangularHorizontal,
        T_shaped_Left,
        T_shaped_Right,
        T_shaped_Top,
        T_shaped_Bottom,
        L_shaped_LeftTop,
        L_shaped_RightTop,
        L_shaped_RightBottom,
        L_shaped_LeftBottom,
        Z_Shaped_LeftHorizontal,
        Z_shaped_RightHorizontal,
        Z_shaped_RightVertical,
        Z_shaped_LeftVertical,
        Large_L_shaped_LeftTop,
        Large_L_shaped_RightTop,
        Large_L_shaped_RightBottom,
        Large_L_shaped_LeftBottom,
    }

    [System.Serializable]
    public class TheBlock
    {
        public BlockType blockType;
        public GameObject BlockPref;
    }

    public class BlockLib : MonoBehaviour
    {
        public TheBlock [] _blocks;

        public TheBlock GetBlock ( BlockType blockType )
        {
            if (_blocks == null) return null;

            for (int i = 0 ; i < _blocks.Length ; i++)
            {
                if (_blocks [i].blockType == blockType)
                {
                    return _blocks [i];
                }
            }
            return null;
        }

        public GameObject GetBlockPref ( BlockType blockType )
        {
            TheBlock block = GetBlock(blockType);
            return block?.BlockPref;
        }
    }
}
