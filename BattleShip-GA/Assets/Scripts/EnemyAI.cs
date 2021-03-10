using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    // NOTE:
    // Proje Player ve EnemyAI movementlar için BFS veya A* kullanmak daha mantıklı olacaktı ama bu iki yapıya fazla hakim olmadığım ve yeterli zamanım olmadığı için
    // kullanamadım. NavMesh Agent üzerinde ve IEnum üzerinden oynamalar ile Grid movement yapıyı uygulamaya çalıştım.


    // SHIP INFO...
    public string shipName;
    public int shipLevel;
    public int takingDamage;
    public int maxHp;
    public int currentHp;
    //
    [SerializeField]
    private NavMeshAgent navMesh;
    public GameObject player;
    private GameObject nearest = null;
    public Slider slider;
    private Transform lastTile;
    public GameObject cannonBall;
    public GameObject explosionOB;
    // VAL
    private float distanceFromEnemy;
    public float distance;
    public Vector3 whichDir = Vector3.zero;
    Vector3[] vectors = new[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
    
    public void Start()
    {
        navMesh = this.GetComponent<NavMeshAgent>();
        currentHp = maxHp;
        slider.maxValue = maxHp;
        slider.value = maxHp;

    }

    // Update is called once per frame
    public void Update()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), Vector3.forward * 20f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), -Vector3.forward * 20f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), Vector3.right * 20f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), -Vector3.right * 20f, Color.red);
    }

    public void FirstPlayerInDistanceCheck()
    {
        foreach (Vector3 vector in vectors)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), vector, out hit, 20f))
            {
                if (hit.transform.tag == "Player")
                {
                    whichDir = vector;
                }
            }
        }
    }

    public void PlayerInDistance()
    {
        foreach (Vector3 vector in vectors)
        {
            RaycastHit hit;
            //Debug.Log("LETS FIND");
            if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), vector, out hit, 20f))
            {
                if (hit.transform.tag == "Player")
                {
                    whichDir = vector;
                    EnemyAttack();
                }
            }
        }

        if(whichDir == Vector3.zero)
        {
            MoveTowardsThePlayer();
        }
    }

    public void EnemyAttack()
    {
        var cb = cannonBall.GetComponent<CannonBall>();
        cb.whichWay = whichDir;

        Instantiate(cannonBall, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
    }


    public void MoveTowardsThePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("NO PLAYER" + gameObject.name);
        }
        else
        {
            navMesh.stoppingDistance = 3;
            FindNearestPlayer();
            navMesh.SetDestination(nearest.transform.position);
            StartCoroutine(FixThePosition());
        }
    }

    IEnumerator FixThePosition()
    {
        if(distance < 15)
        {
            yield return new WaitForSeconds(1f);
        }
        else if(distance < 30)
        {
            yield return new WaitForSeconds(1.5f);
        }
        else if (distance < 45)
        {
            yield return new WaitForSeconds(2f);
        }
        else if (distance > 45 )
        {
            yield return new WaitForSeconds(3f);
        }

        RaycastHit tileInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out tileInfo, 5f))
        {
            if (tileInfo.collider.tag == "Tile")
            {
                lastTile = tileInfo.transform;
            }
        }
        Debug.Log("fixing");
        navMesh.stoppingDistance = 0;
        navMesh.SetDestination(lastTile.transform.position);

        if(Vector3.Distance(transform.position, lastTile.transform.position) < 10)
        {
            yield return new WaitForSeconds(1.5f);
        }
        else if(Vector3.Distance(transform.position, lastTile.transform.position) > 10)
        {
            yield return new WaitForSeconds(3f);
        }

        //yield return new WaitForSeconds(3f);
        LastTurnforAttack();
        yield return null;
    }

    public void LastTurnforAttack()
    {
        RaycastHit hit;

        foreach (Vector3 vector in vectors)
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), vector, out hit, 20f))
            {
                if (hit.transform.tag == "Player")
                {
                    whichDir = vector;
                    EnemyAttack();
                }
            }
        }
        Debug.Log("MY TURN IS OVERRRR" + gameObject.name);
    }

    public void FindNearestPlayer()
    {
        GameObject[] playerTarget = GameObject.FindGameObjectsWithTag("Player");

        distance = Mathf.Infinity;

        foreach(GameObject player in playerTarget)
        {
            distanceFromEnemy = Vector3.Distance(transform.position, player.transform.position);

            if(distanceFromEnemy < distance)
            {
                distance = distanceFromEnemy;
                nearest = player;
            }
        }
    }

    public void OnTriggerEnter(Collider enemyOther)
    {
        if(enemyOther.gameObject.tag == "PlayerCannon")
        {
            TakeDamageEnemyAI();
        }
    }

    public void TakeDamageEnemyAI()
    {
        currentHp -= takingDamage;

        slider.value = currentHp;

        if (currentHp <= 0)
        {
            Instantiate(explosionOB, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
