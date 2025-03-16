using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{   //
    public float viewRadius = 10f;
    public float viewAngle = 90f;
    public LayerMask playerLayer;
    public LayerMask obstacleMask;

    private Transform player;
    private bool isChasing;

    //Devriye ile ilgili
    public Transform[] patrolPoints;
    private int currentPatrolIndex;
    private NavMeshAgent agent;

    //Oyuncunu son gordugu noktada kac saniye beklesin.
    public float waitTimeAtLastSeen;
    //Oyuncunu son gordugu kunum
    private Vector3 lastSeenPosition;
    //Oyuncunu son gordugu noktada bekliyor mu?
    private bool waitingAtLastSeen;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

    }
    
    private void CheckForPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

        foreach (var hit in hits)
        {
            Vector3 directionToPlayer = (hit.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < viewAngle / 2)
            {
                if(!Physics.Linecast(transform.position, hit.transform.position, obstacleMask))
                {
                    lastSeenPosition = hit.transform.position;
                    ChasePlayer(hit.transform);
                    return;
                }
            }

        }
        
        if(isChasing)
        {
            isChasing = false;
            StartCoroutine (GoToLestSeenPosition());
        }

    }

    private void ChasePlayer(Transform playerTransform)
    {
        agent.SetDestination(playerTransform.position);
        isChasing=true;
    }

    private Transform FindClosestPatrolPoint()
    {
        Transform ClosestPatrolPoint = null;
        float minDistance = Mathf .Infinity;

        foreach (Transform point in patrolPoints)
        {
            float distance = Vector3.Distance(transform.position, point.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                ClosestPatrolPoint= point;
            }
        }
        return ClosestPatrolPoint;

    }

    private void GoToNearestPatrolPoint()
    {
        Transform closestPoint   = FindClosestPatrolPoint();
        if (closestPoint != null)
        {
            agent.SetDestination(closestPoint.position);
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void Update()
    {
        CheckForPlayer();

        if (!isChasing && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        } 
    }


   
    private IEnumerator GoToLestSeenPosition()
    {   //Eger son gorulen noktada bekliyorsa methodu calistirma
        if (waitingAtLastSeen) yield break;

        //Son gorulen noktaya dogru hareket et va son gorulen noktada bwekle
        agent.SetDestination(lastSeenPosition);
        waitingAtLastSeen = true;

        //Hedefe ulasmasini bekle
        while (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            yield return null;
        }

        float elapsedTiam = 0f;
        agent.isStopped = true;
        //Belirli bir saniya bekla
        while(elapsedTiam < waitTimeAtLastSeen) ;
        {
            CheckForPlayer() ;
            if (isChasing)
            {
                agent.isStopped = false;
                waitingAtLastSeen = false;
                yield break;
            }
            elapsedTiam += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        //Belirli zAMAN BEKLEDIKten sonre beklemeyi ve en yakin devriye noktasina don.
        waitingAtLastSeen=false;
        GoToNearestPatrolPoint();
    }

}
