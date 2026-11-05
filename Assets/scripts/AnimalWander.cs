
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalWander : MonoBehaviour
{
    [Header("Movement")]
    public float walkRadius = 10f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    [Header("Speed")]
    public float walkSpeed = 2f;

    private NavMeshAgent agent;
    private bool isWaiting;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // Losowanie nowego celu
            Vector3 randomDirection =
                Random.insideUnitSphere * walkRadius;

            randomDirection.y = 0;

            randomDirection += transform.position;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomDirection,
                out hit,
                walkRadius,
                NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);

                // Czekanie a¿ zwierzê dojdzie do celu
                while (agent.pathPending ||
                       agent.remainingDistance > agent.stoppingDistance)
                {
                    yield return null;
                }
            }

            // Odpoczynek przed kolejnym ruchem
            yield return new WaitForSeconds(
                Random.Range(minWaitTime, maxWaitTime)
            );
        }
    }
}
