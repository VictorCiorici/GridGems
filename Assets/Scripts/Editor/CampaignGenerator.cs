using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GridGame.Config;

namespace GridGame.Editor
{
    /// <summary>
    /// Editor tool to automatically generate 500 levels with progressive difficulty.
    /// </summary>
    public static class CampaignGenerator
    {
        private const string OutputFolder = "Assets/Data/Levels/Campaign";
        private const string CampaignPath = "Assets/Data/Collections/Campaign_500.asset";
        private const int MaxAttempts = 100;

        [MenuItem("GridGame/Generate 500 Levels")]
        public static void GenerateCampaign()
        {
            // 1. Find GemCollection
            string[] guids = AssetDatabase.FindAssets("t:GemCollection");
            if (guids.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No GemCollection asset found in the project! Please create one first.", "OK");
                return;
            }
            string collectionPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            GemCollection gemCollection = AssetDatabase.LoadAssetAtPath<GemCollection>(collectionPath);

            if (gemCollection == null || gemCollection.GemVisuals.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "GemCollection is empty or invalid!", "OK");
                return;
            }

            // 2. Ensure directory exists
            if (!AssetDatabase.IsValidFolder(OutputFolder))
            {
                // Create directory chain
                if (!AssetDatabase.IsValidFolder("Assets/Data/Levels"))
                {
                    AssetDatabase.CreateFolder("Assets/Data", "Levels");
                }
                AssetDatabase.CreateFolder("Assets/Data/Levels", "Campaign");
            }

            // 3. Create CampaignData asset
            CampaignData campaign = ScriptableObject.CreateInstance<CampaignData>();
            campaign.levels = new List<LevelData>();

            // 4. Generate 500 levels
            for (int i = 1; i <= 500; i++)
            {
                EditorUtility.DisplayProgressBar("Generating Levels", $"Creating Level {i} of 500...", i / 500f);

                LevelData level = GenerateSingleLevel(i, gemCollection);
                string levelPath = $"{OutputFolder}/Level_{i:D3}.asset";
                
                AssetDatabase.CreateAsset(level, levelPath);
                campaign.levels.Add(level);
            }

            // 5. Save Campaign
            AssetDatabase.CreateAsset(campaign, CampaignPath);
            AssetDatabase.SaveAssets();

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Generated 500 levels in '{OutputFolder}' and created campaign asset at '{CampaignPath}'", "OK");
        }

        private static LevelData GenerateSingleLevel(int levelIndex, GemCollection collection)
        {
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.gems = new List<GemPlacementData>();

            // Determine difficulty parameters based on level index
            int gridWidth;
            int gridHeight;
            int targetGemCount;

            if (levelIndex <= 100)
            {
                gridWidth = 5;
                gridHeight = 5;
                targetGemCount = Random.Range(1, 3); // 1 or 2 gems
            }
            else if (levelIndex <= 200)
            {
                gridWidth = 6;
                gridHeight = 6;
                targetGemCount = Random.Range(2, 4);
            }
            else if (levelIndex <= 300)
            {
                gridWidth = 7;
                gridHeight = 7;
                targetGemCount = Random.Range(3, 5);
            }
            else if (levelIndex <= 400)
            {
                gridWidth = 8;
                gridHeight = 8;
                targetGemCount = Random.Range(4, 6);
            }
            else
            {
                gridWidth = 10;
                gridHeight = 10;
                targetGemCount = Random.Range(5, 8);
            }

            level.gridWidth = gridWidth;
            level.gridHeight = gridHeight;

            // Simple placement loop (similar to ProceduralLevelGenerator)
            bool[,] occupied = new bool[gridWidth, gridHeight];
            int placedCount = 0;
            int attempts = 0;

            // Pick a pool of valid gems from the collection
            var validGems = collection.GemVisuals;

            while (placedCount < targetGemCount && attempts < MaxAttempts)
            {
                attempts++;
                
                // Pick a random gem config
                var randomGem = validGems[Random.Range(0, validGems.Count)];
                if (randomGem == null) continue;

                bool shouldRotate = randomGem.canRotate && Random.value > 0.5f;
                int w = shouldRotate ? randomGem.height : randomGem.width;
                int h = shouldRotate ? randomGem.width : randomGem.height;

                // Pick random origin
                int rx = Random.Range(0, gridWidth - w + 1);
                int ry = Random.Range(0, gridHeight - h + 1);

                // Check overlap
                bool overlap = false;
                for (int x = rx; x < rx + w; x++)
                {
                    for (int y = ry; y < ry + h; y++)
                    {
                        if (occupied[x, y])
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (overlap) break;
                }

                if (!overlap)
                {
                    // Mark occupied
                    for (int x = rx; x < rx + w; x++)
                    {
                        for (int y = ry; y < ry + h; y++)
                        {
                            occupied[x, y] = true;
                        }
                    }

                    // Add to level
                    GemPlacementData placement = new GemPlacementData
                    {
                        origin = new Vector2Int(rx, ry),
                        width = w,
                        height = h
                    };
                    level.gems.Add(placement);
                    placedCount++;
                }
            }

            return level;
        }
    }
}
