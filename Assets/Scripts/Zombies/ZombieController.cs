using UnityEngine;
using UnityEngine.AI;
using HordeInTown.Combat;
using HordeInTown.UI;

namespace HordeInTown.Zombies
{
    /// <summary>
    /// Controls zombie behavior: movement, health, and AI
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieController : MonoBehaviour
    {
        [Header("Zombie Stats")]
        [SerializeField] private float minHealth = 50f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int scoreValue = 10; // 10 points per kill
        
        [Header("Target")]
        [SerializeField] private Transform target; // Player or defense point
        
        [Header("Barrier Damage")]
        [SerializeField] private float damagePerSecond = 5f; // 5 dmg/sec at barrier
        [SerializeField] private float damageInterval = 1f;
        
        [Header("Hit Reaction")]
        [SerializeField] private float hitReactionDuration = 0.5f; // Time to stay idle after taking damage
        
        [Header("Animations")]
        [SerializeField] private Animator animator;

        [Header("VFX")]
        [SerializeField] private GameObject deathVFX; // "smoke_pop" effect
        
        [Header("Health Bar")]
        [SerializeField] private HordeInTown.UI.ZombieHealthBar healthBar;
        
        private NavMeshAgent navAgent;
        private float currentHealth;
        private float actualMaxHealth; // The actual max health value (random between minHealth and maxHealth)
        private bool isDead;
        private bool isAtBarrier = false;
        private float lastDamageTime;
        private FrontBarrier currentBarrier;
        private bool isInHitReaction = false;
        private float hitReactionEndTime = 0f;
        
        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            // Random health between 50-100
            actualMaxHealth = Random.Range(minHealth, maxHealth);
            currentHealth = actualMaxHealth;
            navAgent.speed = moveSpeed;
            healthBar = GetComponent<ZombieHealthBar>();
            if (healthBar == null)
            {
                healthBar = GetComponentInChildren<ZombieHealthBar>();
            }
            if (healthBar == null)
            {
                Debug.LogWarning("ZombieController: No ZombieHealthBar component found!");
            }
        }
        
        private void Start()
        {
            // Auto-find animator if not assigned
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            
            // Find target if not assigned
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
            
            // Setup health bar
            if (healthBar != null)
            {
                healthBar.SetMaxHealth(actualMaxHealth);
                healthBar.UpdateHealthBar(currentHealth);
            }
        }
        
        private void Update()
        {
            if (isDead) return;
            
            // Check if hit reaction has ended
            if (isInHitReaction && Time.time >= hitReactionEndTime)
            {
                isInHitReaction = false;
            }
            
            // If in hit reaction, stay idle
            if (isInHitReaction)
            {
                if (navAgent != null && navAgent.isActiveAndEnabled)
                {
                    navAgent.isStopped = true;
                }
                
                if (animator != null)
                {
                    animator.SetBool("Attack", false);
                    animator.SetBool("Dead", false);
                }
                return; // Don't process movement or attack during hit reaction
            }
            
            if (isAtBarrier)
            {
                // Stop moving and attack barrier
                if (navAgent != null && navAgent.isActiveAndEnabled)
                {
                    navAgent.isStopped = true;
                }
                
                // Play attack animation
                if (animator != null)
                {
                    animator.SetBool("Dead", false);
                    animator.SetBool("Attack", true);
                }
                
                // Deal damage to player every second
                if (Time.time >= lastDamageTime + damageInterval)
                {
                    if (HordeInTown.Managers.GameManager.Instance != null)
                    {
                        HordeInTown.Managers.GameManager.Instance.PlayerTakeDamage(damagePerSecond);
                    }
                    lastDamageTime = Time.time;
                }
            }
            else
            {
                // Move towards target
                if (target != null && navAgent != null && navAgent.isActiveAndEnabled)
                {
                    navAgent.isStopped = false;
                    navAgent.SetDestination(target.position);
                    
                    // Play walk animation
                    if (animator != null)
                    {
                        animator.SetBool("Dead", false);
                        animator.SetBool("Attack", false);
                    }
                }
            }
        }
        
        /// <summary>
        /// Take damage from arrow or other sources
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            
            currentHealth -= damage;
            
            // Update health bar
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(currentHealth);
            }
            
            // Trigger hit reaction (idle state)
            if (currentHealth > 0)
            {
                isInHitReaction = true;
                hitReactionEndTime = Time.time + hitReactionDuration;
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            if (isDead) return;
            
            isDead = true;
            
            // Hide health bar
            if (healthBar != null)
            {
                healthBar.Hide();
            }
            
            // Add score (10 points per kill)
            if (HordeInTown.Managers.GameManager.Instance != null)
            {
                HordeInTown.Managers.GameManager.Instance.AddScore(scoreValue);
                HordeInTown.Managers.GameManager.Instance.OnZombieKilled();
            }
            
            // Play death animation
            if (animator != null)
            {
                animator.SetBool("Dead", true);
                animator.SetBool("Attack", false);
            }
            
            // Play death sound
            if (HordeInTown.Managers.AudioManager.Instance != null)
            {
                HordeInTown.Managers.AudioManager.Instance.PlaySFX("zombie_dying");
            }
            
            // Spawn death VFX (smoke_pop)
            if (deathVFX != null)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
            }
            
            // Disable nav agent
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }
            
            // Destroy zombie after delay (for death animation)
            Destroy(gameObject, 2f);
        }
        
        /// <summary>
        /// Get current health
        /// </summary>
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
        
        /// <summary>
        /// Get max health
        /// </summary>
        public float GetMaxHealth()
        {
            return actualMaxHealth;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Check if reached front barrier
            FrontBarrier barrier = other.GetComponent<FrontBarrier>();
            if (barrier != null)
            {
                isAtBarrier = true;
                currentBarrier = barrier;
                lastDamageTime = Time.time;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Left the barrier
            FrontBarrier barrier = other.GetComponent<FrontBarrier>();
            if (barrier == currentBarrier)
            {
                isAtBarrier = false;
                currentBarrier = null;
            }
        }
    }
}

