using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum BattleState { START, ENEMYTURN, PLAYERTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public GameObject[] playerShip;
    public GameObject[] enemyShip;
    public Button[] shipButtons;
    public GameObject[] tiles;
    public GameObject selectSeperator;
    public GameObject lostUI;
    public GameObject wonUI;
    public GameObject[] lvlUI;
    public GameObject[] chapterUI;
    GameObject[] whichEnemy;
    //MAX SHIP NUMBER FOR INSTANTIATE...
    public int maxShipNumber;
    //INSTANTIATED SHIP NUMBER...
    private int playerShipCount;
    //FOR CLOSEST ENEMY DISTANCE...
    public float d = Mathf.Infinity;
    //LIST FOR NEAREST PLAYER SHIP DISTANCES...
    public List<float> shipDistanceList = new List<float>();
    public int whichPlayerShip;

    [HideInInspector]
    public bool enemyTurn = false;
    [HideInInspector]
    public bool playerTurn = false;
    //CHAPTER AND PLAYER LVL...
    public int lvl;

    public BattleState state;

    public void Awake()
    {
        // FOR RESET LVL ENABLE THIS...
        //SaveSystem.SavePlayer(this);

        // FOR RESET LVL DISABLE THIS...
        LoadPlayer();
    }
    // Start is called before the first frame update
    public void Start()
    {
        CheckShipButtons();
        ShowLvlandChapOnTop();
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        lvl = data.level;
    }

    void CheckShipButtons()
    {
        if(lvl < 5)
        {
            shipButtons[0].interactable = true;
        }
        else if(lvl >= 5 && lvl < 10)
        {
            shipButtons[0].interactable = true;
            shipButtons[1].interactable = true;
        }
        else if(lvl >= 10)
        {
            shipButtons[0].interactable = true;
            shipButtons[1].interactable = true;
            shipButtons[2].interactable = true;
        }
    }

    public void SelectedShip(int index)
    {
        state = BattleState.START;
        whichPlayerShip = index;
    }

    // PLAY BUTTON...
    public void PlayNoMoreInstantiate()
    {
        if(playerShipCount > 0)
        {
            selectSeperator.SetActive(false);
            EnemyGenerator();
            CloseOtherButtons();
            state = BattleState.ENEMYTURN;
            Debug.Log("ENEMY STATE");
        }
    }

    public void Update()
    {
        if(state == BattleState.START)
        {
            if (Input.GetMouseButtonDown(0) && (playerShipCount < maxShipNumber))
            {
                RaycastHit hit = new RaycastHit();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform obj = hit.transform;

                    if (obj.tag == "Tile")
                    {
                        Instantiate(playerShip[whichPlayerShip], obj.transform.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
                        playerShipCount++;
                        Debug.Log("instantiate");
                    }
                    else
                        return;
                }
            }
            else if(playerShipCount >= maxShipNumber)
            {
                selectSeperator.SetActive(false);
                EnemyGenerator();
                CloseOtherButtons();
                state = BattleState.ENEMYTURN;
                Debug.Log("ENEMY STATE");
            }
        }

        if(state == BattleState.ENEMYTURN)
        {
            whichEnemy = GameObject.FindGameObjectsWithTag("Enemy");

            if(whichEnemy.Length == 1)
            {
                Debug.Log("LAST ENEMY IS " + whichEnemy[0]);
            }

            // 1st CHECK atak yapılabilecek yakında player var mı?
            foreach(GameObject aEnemy in whichEnemy)
            {
                aEnemy.GetComponent<EnemyAI>().FirstPlayerInDistanceCheck();
                Debug.Log(aEnemy.name);
                if(aEnemy.GetComponent<EnemyAI>().whichDir != Vector3.zero)
                {
                    Debug.Log("PLAYER IN DISTANCE");
                    aEnemy.GetComponent<EnemyAI>().EnemyAttack();
                    StartCoroutine(CheckAfterEnemy());

                    playerTurn = true;
                    aEnemy.GetComponent<EnemyAI>().whichDir = Vector3.zero;
                    return;
                }
            }

            // 2nd CHECK yakında yoksa en yakını bul ve setdest.
            foreach(GameObject wEnemy in whichEnemy)
            {
                wEnemy.GetComponent<EnemyAI>().FindNearestPlayer();
                if (wEnemy.GetComponent<EnemyAI>().distance < d)
                {
                    Debug.Log("NEAREST FINDING");
                    d = wEnemy.GetComponent<EnemyAI>().distance;
                    shipDistanceList.Add(d);

                }
            }

            foreach(GameObject sEnemy in whichEnemy)
            {
                if (sEnemy.GetComponent<EnemyAI>().distance == d)
                {
                    Debug.Log("NEAREST ACTIVE..");
                    sEnemy.GetComponent<EnemyAI>().PlayerInDistance();

                    StartCoroutine(CheckAfterEnemy());



                    playerTurn = true;
                    d = Mathf.Infinity;
                }
            }
        }

        if(state == BattleState.PLAYERTURN)
        {
            if(enemyTurn)
            {
                shipDistanceList.Clear();
                StartCoroutine(CheckAfterPlayer());
                enemyTurn = false;
                playerTurn = false;
                return;
            }


            if(Input.GetMouseButtonDown(0) && playerTurn)
            {
                RaycastHit playerHit = new RaycastHit();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out playerHit))
                {
                    var selectedPlayer = playerHit.transform;
                    if (playerHit.transform.tag == "Player")
                    {
                        var GO = selectedPlayer.GetComponent<Move>();
                        var otherShips = GameObject.FindGameObjectsWithTag("Player");
                        var notSelectableShips = GameObject.FindGameObjectsWithTag("NotSelectableShip");

                        if(!GO.selected)
                        {
                            GO.selected = !GO.selected;
                            GO.SelectedHighLight();

                            foreach(GameObject oS in otherShips)
                            {
                                oS.GetComponent<Move>().CheckiSAnyBodySelected();
                            }
                        }
                        else if(GO.selected)
                        {
                            GO.selected = !GO.selected;
                            GO.SelectedHighLight();

                            foreach(GameObject pS in notSelectableShips)
                            {
                                pS.GetComponent<Move>().CheckChooseOther();
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator CheckAfterPlayer()
    {
        state = BattleState.WON;

        yield return new WaitForSeconds(2f);
        whichEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        if (whichEnemy.Length == 0)
        {
            lvl++;
            if(lvl >= 12)
            {
                lvl = 1;
            }
            wonUI.SetActive(true);
            SaveSystem.SavePlayer(this);
            Debug.Log("YOU WIN");
        }
        else
        {
            state = BattleState.ENEMYTURN;
        }
        yield return null;
        //yield return new WaitForSeconds(2f);

    }

    IEnumerator CheckAfterEnemy()
    {
        state = BattleState.LOST;

        yield return new WaitForSeconds(3f);
        playerShip = GameObject.FindGameObjectsWithTag("Player");
        if (playerShip.Length == 0)
        {
            lostUI.SetActive(true);
            Debug.Log("YOU LOST");
        }
        else
        {
            state = BattleState.PLAYERTURN;
        }
        yield return new WaitForSeconds(1f);
    }

    void CloseOtherButtons()
    {
        for(int i = 0; i < shipButtons.Length; i++)
        {
            shipButtons[i].interactable = false;
        }
    }

    void EnemyGenerator()
    {
        if(lvl < 5)
        {
            for(int i = 0; i < playerShipCount; i++)
            {
                int insTile = Mathf.RoundToInt(Random.Range(0, tiles.Length));
                Instantiate(enemyShip[0], tiles[insTile].transform.position, Quaternion.identity);
            }
        }
        else if(lvl >= 5 && lvl < 10)
        {
            for (int i = 0; i < playerShipCount; i++)
            {
                int insTile = Random.Range(0, tiles.Length);
                int randomShip = Random.Range(0, 2);
                Instantiate(enemyShip[randomShip], tiles[insTile].transform.position, Quaternion.identity);
            }
        }
        else if(lvl >= 10)
        {
            for (int i = 0; i < playerShipCount; i++)
            {
                int insTile = Random.Range(0, tiles.Length);
                int randomShip = Random.Range(0, 3);
                Instantiate(enemyShip[randomShip], tiles[insTile].transform.position, Quaternion.identity);
            }
        }
    }

    void ShowLvlandChapOnTop()
    {
        lvlUI[lvl-1].SetActive(true);
        chapterUI[SceneManager.GetActiveScene().buildIndex - 1].SetActive(true);
    }

}
