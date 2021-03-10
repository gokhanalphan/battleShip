using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public GameObject battleSystem;
    private int whichChapter;

    private void Start()
    {
        battleSystem = GameObject.FindGameObjectWithTag("BattleSystem");
        PlayerData data = SaveSystem.LoadPlayer();

        whichChapter = data.level;
    }


    public void NextChapter()
    {
        // NEXT SCENE
        //Debug.Log(chapterInt);
        SceneManager.LoadScene(battleSystem.GetComponent<BattleSystem>().lvl);
    }

    public void Home()
    {
        // BACK TO HOME
        //Debug.Log("GoingHome");
        SceneManager.LoadScene(0);
    }

    public void Retry()
    {
        // RESTART THE LEVEL
        //Debug.Log("GAME RESTART");
        SceneManager.LoadScene(battleSystem.GetComponent<BattleSystem>().lvl);
    }

    public void Play()
    {
        SceneManager.LoadScene(whichChapter);
    }


}
