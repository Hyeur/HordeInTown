using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HordeInTown.Zombies
{
    /// <summary>
    /// Wave configuration for zombie spawning
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        [Tooltip("Number of zombies to spawn in this wave")]
        public int zombieCount = 10;
        
        [Tooltip("Time between each spawn in this wave (in seconds)")]
        public float spawnRate = 2f;
        
        [Tooltip("Delay before this wave starts (in seconds)")]
        public float waveDelay = 0f;
        
        [Tooltip("Name of this wave (for debugging)")]
        public string waveName = "Wave";
    }
    
    /// <summary>
    /// Handles spawning zombies at spawn points using a wave-based system
    /// </summary>
    public class ZombieSpawner : MonoBehaviour
    {
        [Header("Zombie Prefabs & Spawn Points")]
        [SerializeField] private GameObject[] zombiePrefabs; // Multiple zombie types for random spawning
        [SerializeField] private Transform[] spawnPoints; // Multiple spawn points for random spawning
        
        [Header("Wave System")]
        [SerializeField] private Wave[] waves; // Array of waves to spawn
        [SerializeField] private float timeBetweenWaves = 5f; // Time to wait between waves (after previous wave completes)
        [SerializeField] private bool loopWaves = false; // Loop back to first wave after last wave
        
        [Header("Random Spawn Settings")]
        [SerializeField] private bool useRandomZombieTypes = true; // Enable random zombie type selection
        [SerializeField] private bool useRandomSpawnPoints = true; // Enable random spawn point selection
        [SerializeField] private bool allowSameSpawnPoint = true; // Allow spawning at same point consecutively
        
        private int currentWaveIndex = 0;
        private int zombiesSpawnedInCurrentWave = 0;
        private float timeSinceLastSpawn = 0f;
        private float currentWaveStartTime = 0f;
        private bool isSpawning = false;
        private bool isWaitingForNextWave = false;
        private bool isWaitingForWaveClear = false; // Waiting for all zombies in current wave to be killed
        private int lastSpawnPointIndex = -1; // Track last spawn point for variety
        private int wavesCleared = 0; // Counter for cleared waves
        
        private List<GameObject> activeZombies = new List<GameObject>();
        private List<GameObject> currentWaveZombies = new List<GameObject>(); // Track zombies from current wave
        
        private void Start()
        {
            if (waves == null || waves.Length == 0)
            {
                Debug.LogWarning("ZombieSpawner: No waves configured! Please add waves in the Inspector.");
                return;
            }
            
            StartSpawning();
        }
        
        private void Update()
        {
            if (!isSpawning) return;
            
            // Clean up null references from destroyed zombies
            CleanupDestroyedZombies();
            
            // Check if we need to start a new wave
            if (isWaitingForNextWave)
            {
                float timeSinceWaveEnd = Time.time - currentWaveStartTime;
                if (timeSinceWaveEnd >= timeBetweenWaves)
                {
                    StartNextWave();
                }
                return;
            }
            
            // Check if waiting for current wave to be cleared
            if (isWaitingForWaveClear)
            {
                // Check if all zombies from current wave are dead
                if (IsCurrentWaveCleared())
                {
                    // Wave cleared! Increment counter
                    wavesCleared++;
                    
                    // Wave cleared! Wait before next wave
                    isWaitingForNextWave = true;
                    isWaitingForWaveClear = false;
                    currentWaveStartTime = Time.time;
                    Wave currentWave = waves[currentWaveIndex];
                    Debug.Log($"Wave {currentWaveIndex + 1} ({currentWave.waveName}) cleared! Total waves cleared: {wavesCleared}. Waiting {timeBetweenWaves} seconds before next wave...");
                }
                return;
            }
            
            // Check if current wave is complete
            if (currentWaveIndex >= 0 && currentWaveIndex < waves.Length)
            {
                Wave currentWave = waves[currentWaveIndex];
                
                // Check wave delay
                float timeSinceWaveStart = Time.time - currentWaveStartTime;
                if (timeSinceWaveStart < currentWave.waveDelay)
                {
                    return; // Still waiting for wave delay
                }
                
                // Check if all zombies in this wave have been spawned
                if (zombiesSpawnedInCurrentWave >= currentWave.zombieCount)
                {
                    // All zombies spawned, now wait for them to be cleared
                    isWaitingForWaveClear = true;
                    Debug.Log($"Wave {currentWaveIndex + 1} ({currentWave.waveName}) - All zombies spawned. Waiting for wave to be cleared...");
                    return;
                }
                
                // Spawn zombies based on spawn rate
                timeSinceLastSpawn += Time.deltaTime;
                if (timeSinceLastSpawn >= currentWave.spawnRate)
                {
                    SpawnZombie();
                    timeSinceLastSpawn = 0f;
                    zombiesSpawnedInCurrentWave++;
                }
            }
        }
        
        /// <summary>
        /// Clean up null references from destroyed zombies
        /// </summary>
        private void CleanupDestroyedZombies()
        {
            activeZombies.RemoveAll(zombie => zombie == null);
            currentWaveZombies.RemoveAll(zombie => zombie == null);
        }
        
        /// <summary>
        /// Check if all zombies from current wave are dead
        /// </summary>
        private bool IsCurrentWaveCleared()
        {
            // Remove null references first
            CleanupDestroyedZombies();
            
            // Wave is cleared if all zombies from current wave are dead
            return currentWaveZombies.Count == 0;
        }
        
        private void StartNextWave()
        {
            currentWaveIndex++;
            
            // Check if we've completed all waves
            if (currentWaveIndex >= waves.Length)
            {
                if (loopWaves)
                {
                    // Loop back to first wave
                    currentWaveIndex = 0;
                    Debug.Log("All waves complete! Looping back to Wave 1...");
                }
                else
                {
                    // All waves complete, stop spawning
                    StopSpawning();
                    Debug.Log("All waves complete! Spawning stopped.");
                    return;
                }
            }
            
            // Start the new wave
            Wave newWave = waves[currentWaveIndex];
            zombiesSpawnedInCurrentWave = 0;
            timeSinceLastSpawn = 0f;
            currentWaveStartTime = Time.time;
            isWaitingForNextWave = false;
            isWaitingForWaveClear = false;
            currentWaveZombies.Clear(); // Clear previous wave's zombie list
            
            Debug.Log($"Starting Wave {currentWaveIndex + 1}: {newWave.waveName} ({newWave.zombieCount} zombies, spawn rate: {newWave.spawnRate}s)");
        }
        
        private void SpawnZombie()
        {
            // Get random zombie prefab
            GameObject zombiePrefabToSpawn = GetRandomZombiePrefab();
            if (zombiePrefabToSpawn == null)
            {
                Debug.LogWarning("No zombie prefabs assigned! Please assign zombie prefabs in the Inspector.");
                return;
            }
            
            // Get random spawn point
            Transform spawnPoint = GetRandomSpawnPoint();
            if (spawnPoint == null)
            {
                Debug.LogWarning("No spawn points assigned! Please assign spawn points in the Inspector.");
                return;
            }
            
            // Spawn random zombie type at random spawn point
            GameObject zombie = Instantiate(zombiePrefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            activeZombies.Add(zombie);
            currentWaveZombies.Add(zombie); // Track this zombie as part of current wave
            
            // Play zombie walk sound
            if (HordeInTown.Managers.AudioManager.Instance != null)
            {
                HordeInTown.Managers.AudioManager.Instance.PlaySFX("zombie_walk");
            }
        }
        
        /// <summary>
        /// Gets a random zombie prefab from the zombie prefabs array
        /// </summary>
        private GameObject GetRandomZombiePrefab()
        {
            if (zombiePrefabs == null || zombiePrefabs.Length == 0)
            {
                return null;
            }
            
            if (!useRandomZombieTypes)
            {
                // Use first zombie prefab if random is disabled
                return zombiePrefabs[0];
            }
            
            // If only one zombie type, return it
            if (zombiePrefabs.Length == 1)
            {
                return zombiePrefabs[0];
            }
            
            // Select random zombie prefab
            int randomIndex = Random.Range(0, zombiePrefabs.Length);
            return zombiePrefabs[randomIndex];
        }
        
        /// <summary>
        /// Gets a random spawn point from the spawn points array
        /// </summary>
        private Transform GetRandomSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return null;
            }
            
            if (!useRandomSpawnPoints)
            {
                // Use first spawn point if random is disabled
                return spawnPoints[0];
            }
            
            // If only one spawn point, return it
            if (spawnPoints.Length == 1)
            {
                return spawnPoints[0];
            }
            
            // Select random spawn point
            int randomIndex;
            
            if (allowSameSpawnPoint)
            {
                // Can spawn at any point, including the last one
                randomIndex = Random.Range(0, spawnPoints.Length);
            }
            else
            {
                // Avoid spawning at the same point consecutively
                do
                {
                    randomIndex = Random.Range(0, spawnPoints.Length);
                } while (randomIndex == lastSpawnPointIndex && spawnPoints.Length > 1);
            }
            
            lastSpawnPointIndex = randomIndex;
            return spawnPoints[randomIndex];
        }
        
        public void StartSpawning()
        {
            if (waves == null || waves.Length == 0)
            {
                Debug.LogWarning("ZombieSpawner: Cannot start spawning - no waves configured!");
                return;
            }
            
            isSpawning = true;
            currentWaveIndex = 0;
            zombiesSpawnedInCurrentWave = 0;
            timeSinceLastSpawn = 0f;
            currentWaveStartTime = Time.time;
            isWaitingForNextWave = false;
            isWaitingForWaveClear = false;
            wavesCleared = 0; // Reset waves cleared counter
            currentWaveZombies.Clear();
            
            Wave firstWave = waves[0];
            Debug.Log($"Spawning started! First wave: {firstWave.waveName} ({firstWave.zombieCount} zombies, spawn rate: {firstWave.spawnRate}s)");
        }
        
        /// <summary>
        /// Get the number of waves cleared
        /// </summary>
        public int GetWavesCleared()
        {
            return wavesCleared;
        }
        
        public void StopSpawning()
        {
            isSpawning = false;
        }
        
        /// <summary>
        /// Destroy all active zombies (used when resetting game)
        /// </summary>
        public void DestroyAllZombies()
        {
            // Destroy all active zombies
            foreach (GameObject zombie in activeZombies)
            {
                if (zombie != null)
                {
                    Destroy(zombie);
                }
            }
            
            // Clear lists
            activeZombies.Clear();
            currentWaveZombies.Clear();
        }
        
        public void OnZombieDestroyed(GameObject zombie)
        {
            if (activeZombies.Contains(zombie))
            {
                activeZombies.Remove(zombie);
            }
        }
    }
}

