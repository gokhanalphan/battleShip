using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    // NOTE:
    // Proje Player ve EnemyAI movementlar için BFS veya A* kullanmak daha mantıklı olacaktı ama bu iki yapıya fazla hakim olmadığım ve yeterli zamanım olmadığı için
    // kullanamadım. NavMesh Agent üzerinde ve IEnum üzerinden oynamalar ile Grid movement yapıyı uygulamaya çalıştım.



    // SHIP VAL...
    //[SerializeField]
    public string shipName;
    public int shipLevel;
    public int takingDamage;
    public int maxHp;
    public int currentHp;
    // OBJ AND SC...
    private NavMeshAgent agent;
    private BattleSystem bS;
    public GameObject playerCannon;
    public GameObject explosionOB;
    public Slider slider;
    private Color whatColor;
    // VAR...
    public bool selected = false;
    private bool fixRot = false;
    // VEC...
    Vector3[] vectors = new[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
    private Vector3 whichDir;



    // EXPERIMENTAL //
    /*
    private Vector3 lastPos;
    private int rotDirY = 0;
    private bool rotate = false;
    float threshold = 0.0f;
    */
    // EXPERIMENTAL // 


    // Start is called before the first frame update
    public void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        bS = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        currentHp = maxHp;
        slider.maxValue = maxHp;
        slider.value = maxHp;

        // EXPERIMENTAL
        //lastPos = transform.position;
        // EXPERIMENTAL
    }

    // Update is called once per frame
    public void Update()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), Vector3.forward * 20f, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), -Vector3.forward * 20f, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), Vector3.right * 20f, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0, 2, 0), -Vector3.right * 20f, Color.green);

        if (Input.GetMouseButtonDown(0) && selected)
        {
            CheckTheEnemyShip();
        }

        if(!agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete && fixRot)
        {
            fixRot = false;
            StartCoroutine(WaitForTilePos());
        }
        
        // EXPERIMENTAL //
        /*
        Vector3 offSet = transform.position - lastPos;
        if (offSet.x > threshold && rotate)
        {
            // SAĞA DÖN...
            //gameObject.transform.Rotate(Vector3.left, Space.World);
            lastPos = transform.position;
            //transform.Rotate(0, 90, 0);
            rotDirY = 90;
            rotate = false;
            Debug.Log("TURN RIGHT");
        }
        if(offSet.x < -threshold && rotate)
        {
            // SOLA DÖNN..
            lastPos = transform.position;
            //transform.Rotate(0, -90, 0);
            rotDirY = -90;
            rotate = false;
            Debug.Log("TURN LEFT");
        }
        if(offSet.z < threshold && rotate)
        {
            lastPos = transform.position;
            rotDirY = 180;
            rotate = false;
            Debug.Log("TURN BACK");
        }
        */
        // EXPERIMENTAL //
    }


    void CheckTheEnemyShip()
    {
        RaycastHit hit;

        foreach (Vector3 vector in vectors)
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), vector, out hit, 20f))
            {
                if (hit.transform.tag == "Enemy")
                {
                    whichDir = vector;
                    PlayerAttack();
                }
            }
            else
            {
                SetDestination();
            }
        }
    }


    public void PlayerAttack()
    {
        Debug.Log("PLAYER ATTACKKKK" + gameObject.name);

        var pCb = playerCannon.GetComponent<PlayerCannonBall>();
        pCb.whichWayforPlayer = whichDir;

        Instantiate(playerCannon , transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        Reset();
        bS.enemyTurn = true;
    }

    void SetDestination()
    {
        RaycastHit hit = new RaycastHit();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform obj = hit.transform;

            if(obj.transform.tag == "Tile")
            {
                agent.SetDestination(obj.transform.position);
                fixRot = true;
                // EXPERIMENTAL // 
                //rotate = true;
            }
        }
    }

    IEnumerator WaitForTilePos()
    {
        //Debug.Log("WAITINGGGG");
        yield return new WaitForSeconds(5f);
        LastTurnForAttack();

    }

    public void LastTurnForAttack()
    {
        // EXPERIMENTAL // 
        //transform.Rotate(0, rotDirY, 0);
        //rotDirY = 0;
        // EXPERIMENTAL //

        Debug.Log("PLAYER LAST TURN FOR ATTACK" + gameObject.name);
        RaycastHit hit;

        foreach (Vector3 vector in vectors)
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), vector, out hit, 20f))
            {
                if (hit.transform.tag == "Enemy")
                {
                    whichDir = vector;
                    PlayerAttack();
                }
            }
            else
            {
                Reset();
                bS.enemyTurn = true;
                Debug.Log("My turn is Over TOOO" + gameObject.name);
            }
        }
    }

    public void Reset()
    {
        selected = false;
        GameObject[] playerShips = GameObject.FindGameObjectsWithTag("NotSelectableShip");

        foreach(GameObject pS in playerShips)
        {
            pS.GetComponent<Move>().selected = false;
            pS.transform.tag = "Player";
        }
        SelectedHighLight();
    }

    public void CheckiSAnyBodySelected()
    {
        if (!selected)
        {
            this.gameObject.transform.tag = "NotSelectableShip";
        }
        else
            return;
    }

    public void CheckChooseOther()
    {
        if (!selected)
        {
            this.gameObject.transform.tag = "Player";
        }
        else
            return;
    }

    public void SelectedHighLight()
    {
        if (selected)
        {
            var renderer = gameObject.GetComponent<MeshRenderer>();
            whatColor = renderer.materials[0].color;
            renderer.materials[0].color = Color.green;
        }
        else
        {
            var renderer = gameObject.GetComponent<MeshRenderer>();
            renderer.materials[0].color = whatColor;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyCannon")
        {
            Debug.Log("PLAYER TAKE DAMAGE" + gameObject.name);
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        currentHp -= takingDamage;

        slider.value = currentHp;

        if(currentHp <= 0)
        {
            Instantiate(explosionOB, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
