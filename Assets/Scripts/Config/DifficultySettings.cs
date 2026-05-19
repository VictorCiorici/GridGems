using System;
using System.Collections.Generic;
using UnityEngine;
using GridGame.Application;

namespace GridGame.Config
{
    [Serializable]
    public struct DifficultySetting
    {
        [Tooltip("The human-readable name of this difficulty level.")]
        public string Name;
        
        [Tooltip("If true, the player can make unlimited mistakes.")]
        public bool IsUnlimited;
        
        [Tooltip("Percentage of empty cells allowed to be revealed before losing (e.g., 0.35 = 35%). Minimum 1 mistake is always granted if not unlimited.")]
        [Range(0f, 1f)] 
        public float MistakeRatio;
    }

    /// <summary>
    /// Configures the mistake allowances and behaviors for each difficulty tier.
    /// Acts as the single source of truth for available difficulty levels.
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultySettings", menuName = "GridGame/Difficulty Settings")]
    public class DifficultySettings : ScriptableObject, IDifficultyConfig
    {
        [SerializeField] private List<DifficultySetting> settings = new List<DifficultySetting>();

        public int GetDifficultyCount()
        {
            return settings.Count;
        }

        public string GetName(int difficultyIndex)
        {
            if (difficultyIndex < 0 || difficultyIndex >= settings.Count)
                return "Unknown";
            
            return settings[difficultyIndex].Name;
        }

        public bool IsUnlimited(int difficultyIndex)
        {
            if (difficultyIndex < 0 || difficultyIndex >= settings.Count)
                return false;
                
            return settings[difficultyIndex].IsUnlimited;
        }

        public float GetMistakeRatio(int difficultyIndex)
        {
            if (difficultyIndex < 0 || difficultyIndex >= settings.Count)
                return 0f;
                
            return settings[difficultyIndex].MistakeRatio;
        }
    }
}
