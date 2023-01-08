using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare52_2
{
    [CreateAssetMenu(menuName = "LudumDare52/Difficulty Settings", fileName = "DifficultySettings", order = 0)]
    public class DifficultySettings : ScriptableObject
    {
        [SerializeField] private int maxLives;
        [SerializeField] private float tractorMoveSpeed = 0.5f;
        [SerializeField] private float tractorDamageCooldown = 3f;
        [SerializeField] private float obstacleSpawnChance = 0.25f;
        [SerializeField] private int maxObstaclesOnScreen = 5;
        [SerializeField] private List<float> wordListWeights;
        
        [Space] [Header("Crops")] 
        [SerializeField] private List<Crop> cropPrefabs;
        [SerializeField] private List<float> cropPrefabWeights;
        
        [Space][Header("Obstacles")]
        [SerializeField] private List<Obstacle> obstaclePrefabs;
        [SerializeField] private List<float> obstaclePrefabWeights;

        public int MaxLives => maxLives;
        public float TractorMoveSpeed => tractorMoveSpeed;
        public float TractorDamageCooldown => tractorDamageCooldown;
        public float ObstacleSpawnChange => obstacleSpawnChance;
        public int MaxObstaclesOnScreen => maxObstaclesOnScreen;
        public List<float> WordListWeights => wordListWeights;
        public List<Crop> CropPrefabs => cropPrefabs;
        public List<float> CropPrefabWeights => cropPrefabWeights;
        public List<Obstacle> ObstaclePrefabs => obstaclePrefabs;
        public List<float> ObstaclePrefabWeights => obstaclePrefabWeights;
    }
}
