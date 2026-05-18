using UnityEngine;
using GridGame.Domain;
using System.Collections.Generic;

namespace GridGame.Presentation
{
    /// <summary>
    /// Manages the generation and positioning of cell and gem views.
    /// </summary>
    public class GridView : MonoBehaviour
    {
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private GemView gemPrefab;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Camera targetCamera;

        private GridSystem _gridSystem;

        /// <summary>
        /// Initializes the grid view based on the domain grid system.
        /// </summary>
        /// <param name="gridSystem">The domain grid system.</param>
        /// <param name="gemCollection">The collection of gem visuals.</param>
        public void Initialize(GridSystem gridSystem, GridGame.Config.GemCollection gemCollection)
        {
            _gridSystem = gridSystem;
            // Clear existing if any
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Spawn Cells
            for (int x = 0; x < gridSystem.Width; x++)
            {
                for (int y = 0; y < gridSystem.Height; y++)
                {
                    CellNode node = gridSystem.GetCell(x, y);
                    if (node != null)
                    {
                        CellView cellView = Instantiate(cellPrefab, transform);
                        cellView.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0);
                        cellView.Setup(node);
                    }
                }
            }

            // Spawn Gems
            foreach (var gem in gridSystem.ActiveGems)
            {
                if (gem.OccupiedCells.Count > 0)
                {
                    GemView gemView = Instantiate(gemPrefab, transform);
                    
                    int minX = int.MaxValue;
                    int minY = int.MaxValue;
                    foreach (var cell in gem.OccupiedCells)
                    {
                        if (cell.Coordinate.X < minX) minX = cell.Coordinate.X;
                        if (cell.Coordinate.Y < minY) minY = cell.Coordinate.Y;
                    }
                    
                    Vector3 gemPos = new Vector3(
                        minX * cellSize + (gem.Width - 1) * cellSize * 0.5f,
                        minY * cellSize + (gem.Height - 1) * cellSize * 0.5f,
                        0
                    );
                    
                    gemView.transform.localPosition = gemPos;
                    Sprite gemSprite = gemCollection != null ? gemCollection.GetSpriteForSize(gem.Width, gem.Height) : null;
                    gemView.Setup(gem, gemSprite);

                    // Auto scale gem to fit its grid dimensions
                    if (gemSprite != null)
                    {
                        Vector2 spriteSize = gemSprite.bounds.size;
                        if (spriteSize.x > 0 && spriteSize.y > 0)
                        {
                            float targetWidth = gem.Width * cellSize;
                            float targetHeight = gem.Height * cellSize;
                            
                            gemView.transform.localScale = new Vector3(
                                targetWidth / spriteSize.x,
                                targetHeight / spriteSize.y,
                                1f
                            );
                        }
                    }
                }
            }

            // Center Camera and scale to fit
            Camera cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam != null)
            {
                float centerX = (gridSystem.Width - 1) * cellSize * 0.5f;
                float centerY = (gridSystem.Height - 1) * cellSize * 0.5f;
                cam.transform.position = new Vector3(centerX, centerY, -10f);

                float orthoSizeHeight = (gridSystem.Height * cellSize) * 0.5f;
                float orthoSizeWidth = (gridSystem.Width * cellSize) * 0.5f / cam.aspect;
                
                // Add some padding (e.g. 10%)
                float padding = 1.1f;
                cam.orthographicSize = Mathf.Max(orthoSizeHeight, orthoSizeWidth) * padding;
            }
        }


        private void OnDrawGizmos()
        {
            if (_gridSystem == null) return;

            float halfCell = cellSize * 0.5f;

            // Draw grid lines
            Gizmos.color = Color.gray;
            for (int x = 0; x <= _gridSystem.Width; x++)
            {
                float xPos = x * cellSize - halfCell;
                Gizmos.DrawLine(
                    transform.TransformPoint(new Vector3(xPos, -halfCell, 0)),
                    transform.TransformPoint(new Vector3(xPos, _gridSystem.Height * cellSize - halfCell, 0))
                );
            }
            for (int y = 0; y <= _gridSystem.Height; y++)
            {
                float yPos = y * cellSize - halfCell;
                Gizmos.DrawLine(
                    transform.TransformPoint(new Vector3(-halfCell, yPos, 0)),
                    transform.TransformPoint(new Vector3(_gridSystem.Width * cellSize - halfCell, yPos, 0))
                );
            }

            // Draw gems
            Gizmos.color = Color.green;
            foreach (var gem in _gridSystem.ActiveGems)
            {
                if (gem.OccupiedCells.Count == 0) continue;

                int minX = int.MaxValue;
                int minY = int.MaxValue;
                foreach (var cell in gem.OccupiedCells)
                {
                    if (cell.Coordinate.X < minX) minX = cell.Coordinate.X;
                    if (cell.Coordinate.Y < minY) minY = cell.Coordinate.Y;
                }

                // Center matches the placement logic in Initialize
                Vector3 center = new Vector3(
                    minX * cellSize + (gem.Width - 1) * halfCell,
                    minY * cellSize + (gem.Height - 1) * halfCell,
                    0
                );
                
                Vector3 size = new Vector3(gem.Width * cellSize - 0.1f, gem.Height * cellSize - 0.1f, 0.1f);
                
                Gizmos.DrawWireCube(transform.TransformPoint(center), size);
            }
        }
    }
}
