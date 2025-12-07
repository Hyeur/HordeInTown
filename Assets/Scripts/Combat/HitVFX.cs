using UnityEngine;

namespace HordeInTown.Combat
{
    /// <summary>
    /// Hit VFX effect that plays when arrow hits a target
    /// </summary>
    public class HitVFX : MonoBehaviour
    {
        [Header("VFX Settings")]
        [SerializeField] private float lifetime = 1f; // How long the effect lasts
        [SerializeField] private bool autoDestroy = true; // Automatically destroy after lifetime
        
        [Header("Particle System")]
        [SerializeField] private ParticleSystem hitParticles; // Main particle effect
        
        [Header("Optional Effects")]
        [SerializeField] private GameObject sparkEffect; // Optional spark effect
        [SerializeField] private GameObject impactDecal; // Optional impact decal
        
        private void Start()
        {
            // Auto-find particle system if not assigned
            if (hitParticles == null)
            {
                hitParticles = GetComponent<ParticleSystem>();
            }
            
            // Play particle effect
            if (hitParticles != null)
            {
                hitParticles.Play();
            }
            
            // Play spark effect if available
            if (sparkEffect != null)
            {
                sparkEffect.SetActive(true);
            }
            
            // Auto-destroy after lifetime
            if (autoDestroy)
            {
                Destroy(gameObject, lifetime);
            }
        }
        
        /// <summary>
        /// Set the color of the hit effect (optional, for customization)
        /// </summary>
        public void SetColor(Color color)
        {
            if (hitParticles != null)
            {
                var main = hitParticles.main;
                main.startColor = color;
            }
        }
        
        /// <summary>
        /// Set the scale of the hit effect
        /// </summary>
        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
    }
}

