using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public Material hitMat;
    public int health = 100;
    private Rigidbody rb;
    private Renderer rend;
    private Material originalMaterial;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject weaponFlash;
    public float bloom;
    public float fireRate;
    public float lastShotTime =0f;

    //AI Setting
    public int currentPointIndex =0;
    public Vector3 currentTarget;
    public float positionThreshold ;
    public float idealTime =5f;
    public float AttackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth  =30f;
    public AudioClip shootSound;

    public Transform[] PatrolPoints;
    private float idealTimeCounter;
    private Transform playerTransform;
    private bool canSeePlayer ;
    private Vector3 lastKnownPlayerPosition;

    private NavMeshAgent agent;

    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Idle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject patrolPointParent = GameObject.FindGameObjectWithTag("PatrolPoint");
        PatrolPoints = patrolPointParent == null
            ? new Transform[0]
            : patrolPointParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointParent.transform).ToArray();

        if (PatrolPoints.Length > 0)
        {
            currentTarget = PatrolPoints[0].position;
            state = State.Patrolling;
        }

        rb.isKinematic = true;

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit navMeshHit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(navMeshHit.position);
            }
            else
            {
                Debug.LogError($"{name} is not on a NavMesh and no nearby NavMesh was found.", this);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            health -= 10;
            if (health <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(Blink());
            }
            
        }
    }

    void Die()
    {
        Destroy(gameObject);

    }

    IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = originalMaterial;
    }

    private void Update()
    {
        LookForPlayer();

       switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }
        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }

    private void LookForPlayer()
    {
        canSeePlayer = false;
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        if( Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform);
            if (canSeePlayer && state != State.Attacking)
            {
                state = State.Chasing;
            }
            
        }
        
    }

    private void Idle()
    {
        agent.ResetPath();
        idealTimeCounter -= Time.deltaTime;
        if(idealTimeCounter <0)
        {
            state = State.Patrolling;
            idealTimeCounter = idealTime;
        }
    }

    private void Patrol()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            return;
        }

        if (PatrolPoints == null || PatrolPoints.Length == 0)
        {
            agent.ResetPath();
            state = State.Idle;
            return;
        }

        if (Vector3.Distance( currentTarget, transform.position) <= positionThreshold)
        {
            float chance = Random.Range(0 ,100);
            if(chance<10)
            {
                state = State.Idle;
                return;
            }
            currentPointIndex++;
            currentTarget = PatrolPoints[currentPointIndex % PatrolPoints.Length].position;
        }
        else
        {
            agent.SetDestination(currentTarget);
        }
    }

    private void Chase()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            return;
        }

        idealTimeCounter = idealTime;
        agent.SetDestination(lastKnownPlayerPosition);

        if(health < minChasingHealth )
        {
            state = State.Patrolling;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) <= AttackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        else  if (Vector3.Distance(transform.position, playerTransform.position) > maxVisionDistance || !canSeePlayer)
        {
            state = State.Patrolling;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) < positionThreshold && !canSeePlayer)
        {
            state = State.Patrolling;
        }
    }

    private void Attack()
    {
        idealTimeCounter = idealTime;
        agent.ResetPath();

        Shoot();
        
        if (Vector3.Distance(transform.position, playerTransform.position) > AttackDistance || !canSeePlayer )
        {
            if( health < minChasingHealth)
            {
                state =State.Patrolling;
            }
            else
            {
                state = State.Chasing;
            }
        }
    }

    private void LookAtPlayer()
    {
       if(canSeePlayer)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
        
    }

    private void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
        {
            lastKnownPlayerPosition = playerTransform.position;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null || weaponFlash == null) return;
        if (Time.time < lastShotTime + fireRate) return;

        Vector3 shootDirection = (playerTransform.position - bulletSpawnPoint.position).normalized;
        shootDirection += new Vector3(Random.Range(-bloom, bloom), Random.Range(-bloom, bloom), 0f);
        shootDirection.Normalize();


        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);
        bulletRotation *= Quaternion.Euler(0f, 90f, 0f);
        AudioManager.Instance.PlaySFX(shootSound, 0.25f);
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        lastShotTime = Time.time;
    }
}
