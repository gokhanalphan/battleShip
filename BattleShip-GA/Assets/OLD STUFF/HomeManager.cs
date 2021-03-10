using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    public int whichChapter;
    public Button[] chapterButtons;


    // Start is called before the first frame update
    void Start()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        whichChapter = data.level;
        whichChapter -= 1;
        CheckChapterButtons();
    }


    void CheckChapterButtons()
    {   
        for(int i = 0; i <= whichChapter; i++)
        {
            chapterButtons[i].interactable = true;
        }
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }







}
