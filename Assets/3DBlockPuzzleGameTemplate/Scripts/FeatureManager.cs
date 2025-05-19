using UnityEngine;

namespace BlockPuzzleGameTemplate
{
    public class FeatureManager : MonoBehaviour
    {
        public bool EnableClockwiseRotation = false;
        public bool EnableCounterClockwiseRotation = false;
        public bool CanRotateClockWise ()
        {
            if (EnableClockwiseRotation)
            {
                return true;
            }
            return false;
        }

        public bool CanRotateCounterClockWise ()
        {
            if (EnableCounterClockwiseRotation)
            {
                return true;
            }
            return false;
        }
    }
}
