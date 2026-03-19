using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public float lookRadius = 10f;
    
    [Header("Detection Settings")]
    public Vector3 raycastOffset = new Vector3(0, 0.5f, 0);
    public float maxDetectionDistance = 50f;
    [SerializeField] float fieldOfViewAngle = 45f; // Total cone angle (45 degrees left/right)
    private bool isChasing = false;
    Transform target;
    NavMeshAgent agent;
    
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startPos = transform.position + raycastOffset;
        Vector3 targetPos = target.position + raycastOffset;
        Vector3 directionToPlayer = (targetPos - startPos).normalized;
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);

        // Check Line of Sight
        bool hasLineOfSight = false;
        if (Physics.Raycast(startPos, directionToPlayer, out RaycastHit hit, maxDetectionDistance))
        {
            if (hit.transform == target)
            {
                hasLineOfSight = true;
            }
        }

        // Detection Logic
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // If we see the player in the cone OR we are already chasing and have line of sight
        if ((angleToPlayer < fieldOfViewAngle / 2f && hasLineOfSight) || (isChasing && hasLineOfSight))
        {
            isChasing = true;
            agent.SetDestination(target.position);
            
            // Always face the player while chasing so the FOV stays on them
            FaceTarget(); 
        }
        else
        {
            // If the player goes behind a wall, stop chasing
            isChasing = false;
        }
    }
    
    // The AI will be required to face the player in order to end the game
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        
        // Rotates on the Y axis
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 leftRayDirection = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up) * transform.forward;
        Vector3 rightRayDirection = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up) * transform.forward;
        Gizmos.DrawRay(transform.position + raycastOffset, leftRayDirection * maxDetectionDistance);
        Gizmos.DrawRay(transform.position + raycastOffset, rightRayDirection * maxDetectionDistance);
    }
}
