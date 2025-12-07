using UnityEngine;
using HordeInTown.Zombies;

namespace HordeInTown.Combat
{
    /// <summary>
    /// Arrow projectile that damages zombies on impact
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour
    {
        [Header("Arrow Settings")]
        [SerializeField] private float damage = 20f; // Default, will be set by BowController
        [SerializeField] private float lifetime = 10f;
        [SerializeField] private LayerMask zombieLayer;
        
        [Header("Visual")]
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private Vector3 rotationOffset = Vector3.zero; // Rotation offset if arrow model is oriented differently
        
        [Header("Attachment Settings")]
        [SerializeField] private float randomRotationRange = 30f; // Random rotation range in degrees when attaching to body
        
        [SerializeField] private Rigidbody rb;
        
        private bool hasHit = false;
        private float spawnTime;
        
        /// <summary>
        /// Set arrow damage (called by BowController with random 10-30 damage)
        /// </summary>
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            spawnTime = Time.time;
            
            // Disable gravity so arrow flies straight without dropping
            if (rb != null)
            {
                rb.useGravity = false;
            }
        }
        
        private void Start()
        {
            // Set initial rotation based on velocity if available
            if (rb != null && rb.linearVelocity.magnitude > 0.1f)
            {
                UpdateRotation();
            }
        }
        
        private void Update()
        {
            if (hasHit) return;
            
            // Rotate arrow to face velocity direction
            if (rb != null && rb.linearVelocity.magnitude > 0.1f)
            {
                UpdateRotation();
            }
            
            // Destroy after lifetime
            if (Time.time - spawnTime > lifetime)
            {
                Destroy(gameObject);
            }
        }
        
        private void UpdateRotation()
        {
            if (rb == null || rb.linearVelocity.magnitude < 0.1f) return;
            
            // Rotate arrow to face velocity direction
            Vector3 velocityDirection = rb.linearVelocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(velocityDirection);
            
            // Apply rotation offset if needed (for arrow models that point up instead of forward)
            if (rotationOffset != Vector3.zero)
            {
                targetRotation = targetRotation * Quaternion.Euler(rotationOffset);
            }
            
            transform.rotation = targetRotation;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            
            // Check if hit zombie
            ZombieController zombie = other.GetComponent<ZombieController>();
            if (zombie != null)
            {
                hasHit = true;
                zombie.TakeDamage(damage);
                
                // Get the exact hit point on the zombie's collider
                Vector3 hitPoint = GetHitPoint(other);
                
                // Stop arrow movement
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
                
                // Find the closest bone/body part to attach to
                Transform attachmentPoint = FindClosestBone(zombie, hitPoint);
                
                // Move arrow to hit point
                transform.position = hitPoint;
                
                // Apply random rotation for more natural attachment
                ApplyRandomAttachmentRotation();
                
                // Attach to the closest bone or zombie root if no bone found
                if (attachmentPoint != null)
                {
                    transform.SetParent(attachmentPoint);
                }
                else
                {
                    transform.SetParent(other.transform);
                }
                
                // Spawn hit effect at hit point
                if (hitEffect != null)
                {
                    GameObject effect = Instantiate(hitEffect, hitPoint, Quaternion.identity);
                    
                    // Orient effect to face the hit normal (optional - can be improved with raycast normal)
                    // For now, just face away from zombie center
                    Vector3 directionAway = (hitPoint - zombie.transform.position).normalized;
                    if (directionAway != Vector3.zero)
                    {
                        effect.transform.rotation = Quaternion.LookRotation(directionAway);
                    }
                }
                
                // Destroy arrow after delay
                Destroy(gameObject, 5f);
            }
            else if (other.CompareTag("Ground") || other.CompareTag("Environment"))
            {
                // Hit ground or environment
                hasHit = true;
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
                
                Destroy(gameObject, 5f);
            }
        }
        
        /// <summary>
        /// Gets the exact hit point on the collider using raycast
        /// </summary>
        private Vector3 GetHitPoint(Collider hitCollider)
        {
            if (rb == null || hitCollider == null)
            {
                return transform.position;
            }
            
            // Use the arrow's current position and velocity direction to find exact hit point
            Vector3 arrowPosition = transform.position;
            Vector3 arrowDirection = rb.linearVelocity.normalized;
            
            // Raycast backward from arrow position to find where it entered the collider
            // Or use the closest point on the collider
            Vector3 closestPoint = hitCollider.ClosestPoint(arrowPosition);
            
            // If the closest point is very close, use it
            if (Vector3.Distance(arrowPosition, closestPoint) < 0.5f)
            {
                return closestPoint;
            }
            
            // Otherwise, raycast from arrow position in velocity direction
            RaycastHit hit;
            if (Physics.Raycast(arrowPosition - arrowDirection * 0.5f, arrowDirection, out hit, 1f))
            {
                if (hit.collider == hitCollider)
                {
                    return hit.point;
                }
            }
            
            // Fallback: use closest point on collider
            return hitCollider.ClosestPoint(arrowPosition);
        }
        
        /// <summary>
        /// Applies random rotation to the arrow when attaching to zombie body
        /// </summary>
        private void ApplyRandomAttachmentRotation()
        {
            // Get current rotation
            Quaternion baseRotation = transform.rotation;
            
            // Add random rotation offsets
            float randomX = Random.Range(-randomRotationRange, randomRotationRange);
            float randomY = Random.Range(-randomRotationRange, randomRotationRange);
            float randomZ = Random.Range(-randomRotationRange, randomRotationRange);
            
            // Apply random rotation
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);
            transform.rotation = baseRotation * randomRotation;
        }
        
        /// <summary>
        /// Finds the closest bone/body part to the hit point
        /// </summary>
        private Transform FindClosestBone(ZombieController zombie, Vector3 hitPoint)
        {
            if (zombie == null) return null;
            
            // Try to get animator for bone access
            Animator animator = zombie.GetComponent<Animator>();
            if (animator != null && animator.isHuman)
            {
                // Use humanoid rig bones
                return FindClosestHumanoidBone(animator, hitPoint);
            }
            
            // Try to find body part colliders (if zombie has tagged body parts)
            return FindClosestBodyPartCollider(zombie.transform, hitPoint);
        }
        
        /// <summary>
        /// Finds closest humanoid bone using Animator
        /// </summary>
        private Transform FindClosestHumanoidBone(Animator animator, Vector3 hitPoint)
        {
            Transform closestBone = null;
            float closestDistance = float.MaxValue;
            
            // Common body parts to check (in order of priority)
            HumanBodyBones[] bodyParts = new HumanBodyBones[]
            {
                HumanBodyBones.Head,
                HumanBodyBones.Neck,
                HumanBodyBones.Chest,
                HumanBodyBones.Spine,
                HumanBodyBones.LeftUpperArm,
                HumanBodyBones.RightUpperArm,
                HumanBodyBones.LeftLowerArm,
                HumanBodyBones.RightLowerArm,
                HumanBodyBones.LeftHand,
                HumanBodyBones.RightHand,
                HumanBodyBones.LeftUpperLeg,
                HumanBodyBones.RightUpperLeg,
                HumanBodyBones.LeftLowerLeg,
                HumanBodyBones.RightLowerLeg,
                HumanBodyBones.LeftFoot,
                HumanBodyBones.RightFoot,
                HumanBodyBones.Hips
            };
            
            foreach (HumanBodyBones bone in bodyParts)
            {
                try
                {
                    Transform boneTransform = animator.GetBoneTransform(bone);
                    if (boneTransform != null)
                    {
                        float distance = Vector3.Distance(hitPoint, boneTransform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestBone = boneTransform;
                        }
                    }
                }
                catch
                {
                    // Bone doesn't exist, skip it
                    continue;
                }
            }
            
            // Only return bone if it's reasonably close (within 2 units)
            if (closestBone != null && closestDistance < 2f)
            {
                return closestBone;
            }
            
            return null;
        }
        
        /// <summary>
        /// Finds closest body part using colliders with specific tags or names
        /// </summary>
        private Transform FindClosestBodyPartCollider(Transform zombieRoot, Vector3 hitPoint)
        {
            Transform closestPart = null;
            float closestDistance = float.MaxValue;
            
            // Search for child colliders that might be body parts
            Collider[] allColliders = zombieRoot.GetComponentsInChildren<Collider>();
            
            foreach (Collider col in allColliders)
            {
                // Check if it's a body part (by tag or name)
                if (col.CompareTag("BodyPart") || 
                    col.name.ToLower().Contains("head") ||
                    col.name.ToLower().Contains("chest") ||
                    col.name.ToLower().Contains("arm") ||
                    col.name.ToLower().Contains("leg") ||
                    col.name.ToLower().Contains("hand") ||
                    col.name.ToLower().Contains("foot"))
                {
                    float distance = Vector3.Distance(hitPoint, col.ClosestPoint(hitPoint));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPart = col.transform;
                    }
                }
            }
            
            // Only return if reasonably close (within 1.5 units)
            if (closestPart != null && closestDistance < 1.5f)
            {
                return closestPart;
            }
            
            return null;
        }
    }
}

