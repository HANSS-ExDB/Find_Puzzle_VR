using BlockPuzzleGameTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockPuzzleGameTemplate
{
    public class Block : MonoBehaviour
    {
        Level level;
        [SerializeField]
        Transform PrevPos;
        public bool IsDrag = false;
        public bool IsPlaced = false;
        public BlockTile [] blockTiles;
        public bool IsClicked;
        public BlockType type;
        private GameObject marker;
        public bool IsStatic;
        public bool IsOnGrid = false;


        private void Start ()
        {
            if (gameObject.layer == LayerMask.NameToLayer("InventoryOnly"))
            {
                this.enabled = false;
            }

            BlockTarget parentTarget = GetComponentInParent<BlockTarget>();
            if (parentTarget != null)
            {
                PrevPos = parentTarget.transform;
            }
            blockTiles = GetComponentsInChildren<BlockTile>();
            level = GetComponentInParent(typeof(Level)) as Level;
            SetUp();
        }

        public void SetUp ()
        {
            // Create a small sphere as the marker
            marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var levelRef = GetComponentInParent<Level>();
            if (levelRef != null)
                marker.transform.SetParent(levelRef.transform);
            else
                Debug.LogWarning("[Block] 부모에 Level이 없습니다.");

            marker.transform.localScale = new Vector3(1, 0.2f, 1);

            Collider col = marker.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Renderer ren = marker.GetComponent<Renderer>();
            if (ren != null)
                ren.enabled = false;
        }

        private void Update ()
        {
            IsClicked = CheckClick();
        }

        public void Rotate ( bool isClockwise )
        {
            if (IsStatic)
                return;
            if (isClockwise)
            {
                Debug.Log("Rotate clockwise");
                StartCoroutine(Rotation(true));
            }
            else
            {
                Debug.Log("Rotate  counterclockwise");
                StartCoroutine(Rotation(false));
            }

        }

        IEnumerator Rotation ( bool isClockwise )
        {
            Rotator rotator = transform.GetComponentInChildren<Rotator>();
            Quaternion startRot = rotator.transform.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(0 , isClockwise ? 90 : -90 , 0);

            float duration = 0.25f; // Time for the rotation
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Calculate the interpolation factor (t)
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Interpolate the rotation
                rotator.transform.rotation = Quaternion.Slerp(startRot , endRot , t);

                yield return null; // Wait for the next frame
            }

            // Ensure the final rotation is precisely set
            rotator.transform.rotation = endRot;
        }


        public void Drag ( Vector3 Offset )
        {
            if (IsStatic)
                return;
            IsPlaced = false;
            IsDrag = true;
            // Disable colliders on block tiles to prevent interaction while dragging
            foreach (var tile in blockTiles)
            {
                tile.GetComponent<Collider>().enabled = false;
            }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray , out hit))
            {
                //Debug.Log($"Hit: {hit.transform.name}");
                transform.position = ( hit.point - Offset ) + new Vector3(0 , 2 , 0);
            }
            else
            {
                Debug.Log("No object hit by raycast");
            }

            // Clear OccupiedTarget references
            foreach (var tile in blockTiles)
            {
                if (tile.OccupiedTarget != null)
                {
                    tile.OccupiedTarget.GetComponent<Tile>().Owner = null;
                    tile.OccupiedTarget = null;
                }
            }
            transform.localScale = Vector3.one;
        }

        public void Bounce ()
        {
            StartCoroutine(BounceCoroutine());
        }

        private IEnumerator BounceCoroutine ()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a collider
            if (Physics.Raycast(ray , out hit))
            {
                // Save the original scale
                Vector3 originalScale = transform.localScale;

                // Define the scale offset for the bounce
                Vector3 scaleOffset = new Vector3(0.2f , 0.2f , 0.2f);
                Vector3 targetScale = originalScale + scaleOffset;

                float duration = 0.25f; // Total time for the bounce
                float elapsed = 0f;

                // Scale up
                while (elapsed < duration / 2)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / ( duration / 2 );
                    transform.localScale = Vector3.Lerp(originalScale , targetScale , t);
                    yield return null; // Wait for the next frame
                }

                elapsed = 0f;

                // Scale back to original
                while (elapsed < duration / 2)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / ( duration / 2 );
                    transform.localScale = Vector3.Lerp(targetScale , originalScale , t);
                    yield return null; // Wait for the next frame
                }

                // Ensure the final scale is exactly the original scale
                transform.localScale = originalScale;
            }
        }

        public void FinishedDrag ()
        {
            //Debug.Log("Finishing Drag");

            // Re-enable colliders on block tiles after dragging
            foreach (var tile in blockTiles)
            {
                tile.GetComponent<Collider>().enabled = true;
            }

            // Check if placement is correct
            Level level = Level.Instance;
            if (level == null)
            {
                Debug.LogWarning("Level 인스턴스를 찾을 수 없습니다. FinishedDrag 건너뜀.");
                return;
            }

            if (CheckCorrect())
            {
                IsPlaced = true;
                IsOnGrid = true;
                Debug.Log("Placement successful");
                if (transform.GetComponentInParent<BlockTarget>()?.TheOwner)
                {
                    transform.GetComponentInParent<BlockTarget>().TheOwner = null;
                    transform.SetParent(level.transform);
                }
                UnParent();
                level.OnBlockPlacement();
            }
            else
            {
                Debug.Log("Placement failed, clearing tiles");
                foreach (var tile in blockTiles)
                {
                    if (tile.OccupiedTarget != null)
                    {
                        var tileComponent = tile.OccupiedTarget.GetComponent<Tile>();
                        if (tileComponent != null)
                        {
                            tileComponent.Owner = null;
                        }
                        tile.OccupiedTarget = null;
                    }
                }
                if (IsOnGrid)
                {
                    StartCoroutine(ErrorShow(false));
                }
                else
                {
                    StartCoroutine(ErrorShow(true));
                }


            }
        }

        void UnParent ()
        {
            BlockTarget [] allTargets = FindObjectsByType<BlockTarget>(FindObjectsSortMode.None);
            foreach (BlockTarget target in allTargets)
            {
                if (target.TheOwner == this.gameObject)
                {
                    target.TheOwner = null;
                }
            }

        }

        IEnumerator ErrorShow ( bool ShowError )
        {

            MeshRenderer [] Meshes = GetComponentsInChildren<MeshRenderer>();
            Material [] originalMaterials = new Material [Meshes.Length];
            float waitTime = 0.5f;
            Vector3 Offset = new Vector3(0 , .5f , 0);
            if (ShowError)
            {
                // Change materials to indicate error
                for (int i = 0 ; i < Meshes.Length ; i++)
                {
                    originalMaterials [i] = Meshes [i].material;
                    Meshes [i].material = Meshes [i].GetComponentInParent<BlockTile>().ErrorMat;
                }

                // Shake block for error feedback
                ShakeRotation();
                yield return new WaitForSeconds(waitTime);

                // Reset materials and move back to initial position
                for (int i = 0 ; i < Meshes.Length ; i++)
                {
                    Meshes [i].material = originalMaterials [i];
                }
                transform.eulerAngles = new Vector3(0 , transform.eulerAngles.y , 0);


                if (transform.GetComponentInParent<BlockTarget>()?.TheOwner)
                {
                    yield return StartCoroutine(ReturnToPosition());
                }
                else
                {

                    yield return StartCoroutine(ReturnToRandomPos(Offset));
                }


                // Re-enable colliders on block tiles after error show
                foreach (var tile in blockTiles)
                {
                    tile.GetComponent<Collider>().enabled = true;
                }
            }
            else
            {
                if (transform.GetComponentInParent<BlockTarget>()?.TheOwner)
                {
                    yield return StartCoroutine(ReturnToPosition());
                }
                else
                {

                    yield return StartCoroutine(ReturnToRandomPos(Offset));
                }
            }
            IsDrag = false;
            IsPlaced = false;
            // Ensure IsPlaced is set to false for incorrect placements

            yield return null;

        }

        public void ShakeRotation ()
        {
            StartCoroutine(ShakeRotationCoroutine());
        }

        private IEnumerator ShakeRotationCoroutine ()
        {
            float duration = 0.5f;      // Total duration of the shake
            float magnitude = 20f;     // Maximum rotation angle offset
            int vibrato = 10;          // Number of shakes
            float randomness = 45f;    // Maximum randomness in direction
            bool fadeOut = true;       // Whether the shake should fade out

            Quaternion originalRotation = transform.rotation;
            float timePerShake = duration / vibrato; // Time interval between shakes
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Reduce magnitude over time if fading out
                float progress = elapsed / duration;
                float currentMagnitude = fadeOut ? Mathf.Lerp(magnitude , 0 , progress) : magnitude;

                // Generate a random rotation offset
                float randomX = Random.Range(-currentMagnitude , currentMagnitude) * ( randomness / 100f );
                float randomY = Random.Range(-currentMagnitude , currentMagnitude) * ( randomness / 100f );
                float randomZ = Random.Range(-currentMagnitude , currentMagnitude) * ( randomness / 100f );

                // Apply the new rotation offset
                Quaternion offsetRotation = Quaternion.Euler(randomX , randomY , randomZ);
                transform.rotation = originalRotation * offsetRotation;

                // Wait for the next shake
                yield return new WaitForSeconds(timePerShake);

                elapsed += timePerShake;
            }

            // Reset to the original rotation
            transform.rotation = originalRotation;
        }

        bool CheckIfFromGrid ()
        {
            // Find all BlockTarget objects in the scene
            BlockTarget [] allTargets = FindObjectsByType<BlockTarget>(FindObjectsSortMode.None);

            // Iterate over each blockTile
            foreach (var blockTile in blockTiles)
            {
                // Check if any BlockTarget has this blockTile as its owner
                bool foundMatch = false;
                foreach (var target in allTargets)
                {
                    if (blockTile == target.TheOwner)
                    {
                        foundMatch = true;
                        break; // Stop checking once a match is found
                    }
                }

                // If no match was found for this blockTile, return false
                if (!foundMatch)
                {
                    return false;
                }
            }

            // If all blockTiles have at least one matching BlockTarget, return true
            return true;
        }

        public IEnumerator ReturnToPosition ()
        {
            float duration = 0.5f; // Duration of the movement
            Vector3 startPosition = transform.position; // Starting position
            Vector3 endPosition = PrevPos.position; // Target position

            float elapsed = 0f; // Time elapsed

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration); // Interpolation factor (normalized time)

                // Smoothly interpolate the position
                transform.position = Vector3.Lerp(startPosition , endPosition , t);

                yield return null; // Wait for the next frame
            }

            // Ensure the final position is set precisely
            transform.position = endPosition;

            // Additional actions after movement
            transform.localScale = Vector3.one / 2;
            IsOnGrid = false;
        }

        IEnumerator ReturnToRandomPos ( Vector3 offset )
        {
            Debug.Log(Pos().name + " : " + Pos().transform.parent.name);
            Transform targetPosition = Pos();
            transform.SetParent(targetPosition);

            Vector3 startPos = transform.localPosition;
            Vector3 endPos = new Vector3(0 , offset.y , 0);
            float duration = 0.5f;
            float elapsedTime = 0f;

            // Move over time using a while loop
            while (elapsedTime < duration)
            {
                // Interpolate position
                transform.localPosition = Vector3.Lerp(startPos , endPos , elapsedTime / duration);

                // Increment time
                elapsedTime += Time.deltaTime;

                // Yield to next frame
                yield return null;
            }

            // Ensure the final position is exactly the target position
            transform.localPosition = endPos;

            // After movement, apply transformations
            transform.localScale = Vector3.one / 2;
            transform.localPosition = Vector3.zero;

            // Update the target information
            PrevPos = targetPosition;
            targetPosition.GetComponent<BlockTarget>().TheOwner = this.gameObject;
            IsOnGrid = false;
        }

        Transform Pos ()
        {
            Transform ThePos = null;
            List<BlockTarget> RandomAvailablePos = new List<BlockTarget>();
            List<BlockTarget> AvailablePos = new List<BlockTarget>();

            // Find all BlockTarget objects
            BlockTarget [] allTargets = FindObjectsByType<BlockTarget>(FindObjectsSortMode.None);

            // Separate targets based on conditions
            foreach (BlockTarget target in allTargets)
            {
                if (!target.TheOwner)
                {
                    if (!target.IsRandomPos)
                    {
                        AvailablePos.Add(target);
                    }
                    else
                    {
                        RandomAvailablePos.Add(target);
                    }
                }
            }

            // Check if there are available positions
            if (AvailablePos.Count > 0)
            {
                Debug.Log(1); // Indicates a selection from AvailablePos
                int random = Random.Range(0 , AvailablePos.Count);
                ThePos = AvailablePos [random].transform;
            }
            else if (RandomAvailablePos.Count > 0) // Check only if AvailablePos.Count is 0
            {
                Debug.Log(2); // Indicates a selection from RandomAvailablePos
                int random = Random.Range(0 , RandomAvailablePos.Count);
                ThePos = RandomAvailablePos [random].transform;
            }

            return ThePos;
        }

        public bool CheckCorrect ()
        {
            int correctCount = 0;
            Vector3 cumulativeOffset = Vector3.zero;
            List<GameObject> Tiles = new List<GameObject>();
            Tiles.Clear();
            for (int i = 0 ; i < blockTiles.Length ; i++)
            {
                Transform hitTransform = blockTiles [i].GetSingleHit();
                Debug.Log(hitTransform.name);

                if (hitTransform != null && hitTransform.GetComponentInParent<Tile>() is Tile tileComponent)
                {
                    if (tileComponent.AddOwner(blockTiles [i].gameObject))
                    {
                        correctCount++;
                        Debug.Log(correctCount);

                        Tiles.Add(tileComponent.gameObject);
                    }
                }
            }
            if (correctCount == blockTiles.Length)
            {
                IsPlaced = true;
                // Call the method with Tiles.ToArray()
                Vector3 centerPoint = FindCenterPoint(Tiles.ToArray());

                Vector3 targetPos = centerPoint;

                StartCoroutine(MoveAndTriggerEffects(targetPos));
                return true;
            }
            else
            {
                // Reset ownership for incorrect placement
                foreach (var tile in blockTiles)
                {
                    if (tile.OccupiedTarget != null)
                    {
                        var tileComponent = tile.OccupiedTarget.GetComponent<Tile>();
                        if (tileComponent != null)
                        {
                            tileComponent.Owner = null;
                        }
                        tile.OccupiedTarget = null;
                    }
                }
                return false;
            }
        }

        IEnumerator MoveAndTriggerEffects ( Vector3 targetPos )
        {
            // Store initial position
            Vector3 startPos = transform.position;
            float duration = 0.1f;  // Duration for the movement
            float elapsedTime = 0f;

            // Move the object to targetPos using a while loop
            while (elapsedTime < duration)
            {
                // Interpolate position
                transform.position = Vector3.Lerp(startPos , targetPos , elapsedTime / duration);

                // Increment time
                elapsedTime += Time.deltaTime;

                // Yield to next frame
                yield return null;
            }

            // Ensure the final position is exactly the target position
            transform.position = targetPos;

            // Trigger landing smoke effects
            for (int i = 0 ; i < blockTiles.Length ; i++)
            {
                if (blockTiles [i].LandingSmoke)
                {
                    blockTiles [i].LandingSmoke.GetComponent<ParticleSystem>().Play();
                }
            }

            // Shake the camera after movement
            yield return StartCoroutine(ShakeCamera());
        }

        IEnumerator ShakeCamera ()
        {
            float duration = 0.2f;  // Shake duration
            float magnitude = 0.1f;  // Shake intensity
            float elapsedTime = 0f;
            float randomnessFactor = 1f;  // Randomness factor (higher values for more erratic shake)
            Vector3 originalPosition = Camera.main.transform.position;

            // Continue shaking while the duration is not complete
            while (elapsedTime < duration)
            {
                // Generate random shake offsets with the randomness factor influencing the shake range
                float shakeX = Random.Range(-magnitude , magnitude) * randomnessFactor;
                float shakeY = Random.Range(-magnitude , magnitude) * randomnessFactor;

                // Apply the shake to the camera's position
                Camera.main.transform.position = originalPosition + new Vector3(shakeX , shakeY , 0);

                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Yield to the next frame
                yield return null;
            }

            // Reset the camera's position to the original after shaking
            Camera.main.transform.position = originalPosition;
        }


        public Vector3 FindCenterPoint ( GameObject [] gameObjects )
        {
            if (gameObjects.Length == 0)
                return Vector3.zero;
            if (gameObjects.Length == 1)
                return gameObjects [0].transform.position;

            // Initialize bounds at the position of the first blockTile
            Bounds bounds = new Bounds(gameObjects [0].transform.position , Vector3.zero);

            // Expand bounds to include all BlockTile positions
            for (int i = 1 ; i < gameObjects.Length ; i++)
            {
                bounds.Encapsulate(gameObjects [i].transform.position);
            }

            // Return the center point of the bounds
            return bounds.center;
        }
        private void OnTransformParentChanged()
        {
            var parentTarget = GetComponentInParent<BlockTarget>();
            if (parentTarget != null)
                PrevPos = parentTarget.transform;
        }

        public bool CheckClick ()
        {
            for (int i = 0 ; i < blockTiles.Length ; i++)
            {
                if (blockTiles [i].IsClicked)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
