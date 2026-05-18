using System;
using UnityEngine;
using GridGame.Domain;
using GridGame.Config;

namespace GridGame.Presentation
{
    /// <summary>
    /// Pure visual component. Spawns and positions cell and gem views.
    /// Raises <see cref="OnCellViewSpawned"/> per cell so that a dedicated input handler
    /// can subscribe without <see cref="GridView"/> knowing anything about input or domain interaction.
    /// </summary>
    public class GridView : MonoBehaviour
    {
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private GemView gemPrefab;
        [SerializeField] private Camera targetCamera;

        [Header("Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float cameraFitPadding = 1.1f;
        [SerializeField] private float cameraDepth = -10f;
        [SerializeField] private float gizmoShrink = 0.1f;

        private const float GemRotationAngle = 90f;

        private GridSystem _gridSystem;

        /// <summary>
        /// Raised for each <see cref="CellView"/> spawned during <see cref="Initialize"/>.
        /// Subscribe before calling Initialize to receive all spawned cells.
        /// </summary>
        public event Action<CellView> OnCellViewSpawned;

        /// <summary>
        /// Initializes the grid view. Clears any existing children, then spawns cells and gems.
        /// </summary>
        public void Initialize(GridSystem gridSystem, IGemSpriteResolver spriteResolver)
        {
            _gridSystem = gridSystem;
            ClearChildren();
            SpawnCells(gridSystem);
            SpawnGems(gridSystem, spriteResolver);
            FitCamera(gridSystem);
        }

        private void ClearChildren()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        private void SpawnCells(GridSystem gridSystem)
        {
            for (int x = 0; x < gridSystem.Width; x++)
            {
                for (int y = 0; y < gridSystem.Height; y++)
                {
                    CellNode node = gridSystem.GetCell(x, y);
                    if (node == null) continue;

                    CellView cellView = Instantiate(cellPrefab, transform);
                    cellView.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0f);
                    cellView.Setup(node);
                    OnCellViewSpawned?.Invoke(cellView);
                }
            }
        }

        private void SpawnGems(GridSystem gridSystem, IGemSpriteResolver spriteResolver)
        {
            foreach (var gem in gridSystem.ActiveGems)
            {
                if (gem.OccupiedCells.Count == 0) continue;

                GemView gemView = Instantiate(gemPrefab, transform);
                gemView.transform.localPosition = GetGemPosition(gem);

                var (gemSprite, needsRotation) = spriteResolver != null
                    ? spriteResolver.GetSpriteForSize(gem.Width, gem.Height)
                    : (null, false);

                gemView.Setup(gem, gemSprite);

                if (needsRotation)
                    gemView.transform.localRotation = Quaternion.Euler(0f, 0f, GemRotationAngle);

                ApplyGemScale(gemView, gem, gemSprite, needsRotation);
            }
        }

        private Vector3 GetGemPosition(GemEntity gem)
        {
            float halfCell = cellSize * 0.5f;
            return new Vector3(
                gem.Origin.X * cellSize + (gem.Width  - 1) * halfCell,
                gem.Origin.Y * cellSize + (gem.Height - 1) * halfCell,
                0f
            );
        }

        private void ApplyGemScale(GemView gemView, GemEntity gem, Sprite gemSprite, bool needsRotation)
        {
            if (gemSprite == null) return;
            Vector2 spriteSize = gemSprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f) return;

            float sWidth  = needsRotation ? spriteSize.y : spriteSize.x;
            float sHeight = needsRotation ? spriteSize.x : spriteSize.y;

            gemView.transform.localScale = new Vector3(
                gem.Width  * cellSize / sWidth,
                gem.Height * cellSize / sHeight,
                1f
            );
        }

        private void FitCamera(GridSystem gridSystem)
        {
            Camera cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam == null) return;

            cam.transform.position = new Vector3(
                (gridSystem.Width  - 1) * cellSize * 0.5f,
                (gridSystem.Height - 1) * cellSize * 0.5f,
                cameraDepth
            );

            float orthoHeight = gridSystem.Height * cellSize * 0.5f;
            float orthoWidth  = gridSystem.Width  * cellSize * 0.5f / cam.aspect;
            cam.orthographicSize = Mathf.Max(orthoHeight, orthoWidth) * cameraFitPadding;
        }

        private void OnValidate()
        {
            if (targetCamera == null)
                Debug.LogWarning("GridView: 'Target Camera' is not assigned. Camera.main will be used as fallback.", this);
        }

        private void OnDrawGizmos()
        {
            if (_gridSystem == null) return;
            float halfCell = cellSize * 0.5f;

            Gizmos.color = Color.gray;
            for (int x = 0; x <= _gridSystem.Width; x++)
            {
                float xPos = x * cellSize - halfCell;
                Gizmos.DrawLine(
                    transform.TransformPoint(new Vector3(xPos, -halfCell, 0f)),
                    transform.TransformPoint(new Vector3(xPos, _gridSystem.Height * cellSize - halfCell, 0f)));
            }
            for (int y = 0; y <= _gridSystem.Height; y++)
            {
                float yPos = y * cellSize - halfCell;
                Gizmos.DrawLine(
                    transform.TransformPoint(new Vector3(-halfCell, yPos, 0f)),
                    transform.TransformPoint(new Vector3(_gridSystem.Width * cellSize - halfCell, yPos, 0f)));
            }

            Gizmos.color = Color.green;
            foreach (var gem in _gridSystem.ActiveGems)
            {
                if (gem.OccupiedCells.Count == 0) continue;
                Vector3 center = GetGemPosition(gem);
                Vector3 size   = new Vector3(gem.Width * cellSize - gizmoShrink, gem.Height * cellSize - gizmoShrink, 0.1f);
                Gizmos.DrawWireCube(transform.TransformPoint(center), size);
            }
        }
    }
}
