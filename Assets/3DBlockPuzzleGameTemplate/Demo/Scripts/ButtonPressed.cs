using System.Collections;
using UnityEngine;
namespace BlockPuzzleGameTemplate
{
    public class ButtonPressed : MonoBehaviour
    {
        private Coroutine bounceCoroutine;
        private bool isBouncing = false;

        [SerializeField] private float targetScale = 1.25f; // The scale to bounce up to
        [SerializeField] private float originalScale = 1f;  // The original scale
        [SerializeField] private float duration = 2f;       // Duration for scaling up or down
        public void BounceBtn()
        {
            if (this.gameObject.activeSelf)
            {
                StartBouncing();
            }
            else
            {
                StopBouncing();
            }
        }
        public void StartBouncing()
        {
            if (isBouncing) return; // Prevent multiple instances
            isBouncing = true;

            if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
            bounceCoroutine = StartCoroutine(BounceCoroutine());
        }

        public void StopBouncing()
        {
            isBouncing = false;

            if (bounceCoroutine != null)
            {
                StopCoroutine(bounceCoroutine);
                bounceCoroutine = null;
            }

            // Reset the scale to the original value
            if (transform != null)
                transform.localScale = Vector3.one * originalScale;
        }

        private IEnumerator BounceCoroutine()
        {
            float elapsedTime = 0f;

            // Scale up
            while (elapsedTime < duration)
            {
                if (transform == null) yield break; // Safeguard for destroyed GameObject

                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                transform.localScale = Vector3.one * Mathf.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (transform == null) yield break; // Safeguard for destroyed GameObject

                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                transform.localScale = Vector3.one * Mathf.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            // Stop the coroutine after completing the bounce
            StopBouncing();
        }
    }
}
