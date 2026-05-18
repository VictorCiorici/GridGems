using System.Collections.Generic;
using UnityEngine;

namespace GridGame.Config
{
    /// <summary>
    /// Holds a sequence of levels to be played in order.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCampaign", menuName = "GridGame/Campaign")]
    public class CampaignData : ScriptableObject
    {
        /// <summary>
        /// The list of levels in this campaign.
        /// </summary>
        public List<LevelData> levels = new List<LevelData>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] == null)
                {
                    Debug.LogWarning($"CampaignData '{name}': level at index [{i}] is null.", this);
                }
            }
        }
#endif
    }
}
