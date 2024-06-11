using UnityEngine;

namespace Manager
{
    public class CameraManager : MonoBehaviour
    {
        public static Camera Camera;

        private const uint MAX_ZOOM_OUT = 40;
        private const uint MAX_ZOOM_IN = 5;

        [SerializeField] private float SpaceUpToGrid;
        [SerializeField] private float SpaceDownToGrid;
        [SerializeField] private float SpaceRightToGrid;
        [SerializeField] private float SpaceLeftToGrid;

        public void SetCamera(in Camera cam) => Camera = cam;

        public void AdjustCameraPositionToGrid(Transform gridTransform, Vector2 gridDimensions)
        {
            var gridCenter = new Vector3(gridDimensions.x / 2, 0, gridDimensions.y / 2);

            var totalWidth = gridDimensions.x + SpaceLeftToGrid + SpaceRightToGrid;
            var totalHeight = gridDimensions.y + SpaceUpToGrid + SpaceDownToGrid;

            var requiredVerticalSize = totalHeight / 2f;
            var requiredHorizontalSize = totalWidth / 2f / Camera.aspect;

            var requiredSize = Mathf.Max(requiredVerticalSize, requiredHorizontalSize);
            var cameraHeight = requiredSize / Mathf.Tan(Mathf.Deg2Rad * Camera.fieldOfView / 2f);
            cameraHeight = Mathf.Clamp(cameraHeight, MAX_ZOOM_IN, MAX_ZOOM_OUT);

            Camera.transform.position = new Vector3(gridCenter.x, cameraHeight, gridCenter.z);
            Camera.transform.position += new Vector3((SpaceRightToGrid - SpaceLeftToGrid) / 2, 0,
                (SpaceUpToGrid - SpaceDownToGrid) / 2);
        }

    }
}
