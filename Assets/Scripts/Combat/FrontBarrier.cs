using UnityEngine;

namespace HordeInTown.Combat
{
    /// <summary>
    /// Front barrier that prevents zombie entry and triggers damage when zombies reach it
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FrontBarrier : MonoBehaviour
    {
        [Header("Barrier Settings")]
        [SerializeField] private bool isTrigger = true;
        
        private void Awake()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = isTrigger;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Zombies will handle barrier interaction in ZombieController
            // This script just marks the barrier location
        }
    }
}

