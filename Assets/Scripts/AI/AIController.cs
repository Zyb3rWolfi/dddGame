using System;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public float lookRadius = 10f;
    public enum AIState { Patrolling, Chasing, Investigating }
    [SerializeField] public AIState previousState; // Add this at the top of your class
    public AIState currentState = AIState.Patrolling;
    [Header("Detection Settings")]
    public Vector3 raycastOffset = new Vector3(0, 0.5f, 0);
    public float maxDetectionDistance = 50f;
    [SerializeField] float fieldOfViewAngle = 45f; // Total cone angle (45 degrees left/right)
    public Vector3 lastKnownPlayerPosition;
    private bool isChasing = false;
    private bool canHearPlayer = false;
    Transform target;
    NavMeshAgent agent;
    public static Action OnPlayerCaught;
    [SerializeField] private GameObject[] respawnPoints;
    [SerializeField] public Animator animator;
    public static Action playSfx;
    public static Action playInvestigationSfx;
    public static Action playAmbientSfx;
    [Header("Search Settings")]
    public float searchWaitTime = 3f; // How long to look around before giving up
    private float searchTimer;
    private bool isSearching = false;
    [Header("Chase Settings")]
    [SerializeField] private float chaseGracePeriod = 3f; // Stay in Chase for 3s after losing LoS
    private float lastSeenTime;
    public static Action<AIState> HAndleStateAudioChange;
    
    
    // I have basically changed some stuff in this script to allow the AI to be more unpredictable and in a way fun. It does need a bit more tweaking
    
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        lastKnownPlayerPosition = transform.position; // Initialize to current position
    }
    
    // These were added to allow the enemy to respawn.
    private void OnEnable()
    {
        UIManager.ResetPosition += Respawn;
    }

    private void OnDisable()
    {
        UIManager.ResetPosition -= Respawn;
    }

    private void Respawn()
    {
        // Choose a random respawn point
        GameObject respawnPoint = respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)];
        transform.position = respawnPoint.transform.position;
        transform.rotation = respawnPoint.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        // This controls when the footsteps sfx should play aka when there is velocity
        if (agent.velocity.magnitude > 0.1f)
        {
            playSfx?.Invoke();
        }
            
        Vector3 startPos = transform.position + raycastOffset;
        Vector3 targetPos = target.position + raycastOffset;
        Vector3 directionToPlayer = (targetPos - startPos).normalized;
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);
        animator.SetFloat("Velocity", agent.velocity.magnitude);

        // Check Line of Sight
        bool hasLineOfSight = false;

        // Detection Logic
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // If we see the player in the cone OR we are already chasing and have line of sight
        if (angleToPlayer < fieldOfViewAngle / 2f)
        {
            if (Physics.Raycast(startPos, directionToPlayer, out RaycastHit hit, maxDetectionDistance))
            {
                if (hit.transform == target) hasLineOfSight = true;
            }
        }
        
        // If the AI has the line of sight we would change the enum to Chasing.
        if (hasLineOfSight)
        {
            currentState = AIState.Chasing;
            isSearching = false;
            agent.SetDestination(target.position);
            FaceTarget();
        }
        
        // Whereas if the enemy no longer sees the player then we would put it under investigating.
        // This allows the AI to do some "smart" finding and not cheat it way into finding the player
        else if (currentState == AIState.Chasing && !hasLineOfSight)
        {
            currentState = AIState.Investigating;
            lastKnownPlayerPosition = target.position;
            agent.SetDestination(lastKnownPlayerPosition);
        }
        
        if (currentState == AIState.Investigating && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)        {
            if (!isSearching)
            {
                isSearching = true;
                searchTimer = searchWaitTime;
            }

            if (isSearching)
            {
                searchTimer -= Time.deltaTime;
        
                if (searchTimer <= 0)
                {
                    currentState = AIState.Patrolling;
                    isSearching = false;
                    // Go back to a random patrol point or stay still
                }
            }
        }

        // return to patrol if he couldnt find anything
        if (currentState == AIState.Investigating && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = AIState.Patrolling;
        }

        if (currentState != previousState)
        {
            Debug.Log($"AI State changed to: {currentState}");
            HAndleStateAudioChange?.Invoke(currentState);
        }
        previousState = currentState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Footsteps" && currentState != AIState.Chasing)
        {
            currentState = AIState.Investigating;
            lastKnownPlayerPosition = target.position; // Take a "Snapshot" of their current spot
            agent.SetDestination(lastKnownPlayerPosition);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Footsteps")
        {
            canHearPlayer = false;
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
