using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentController : MonoBehaviour
{
    // Singleton instance
    public static PersistentController Instance { get; private set; }
    
    public int mapIndex;
    public Picture[] currentDataset;
    public List<Picture> chosenPictures = new List<Picture>();

    public Picture[] easyMode;
    private bool cantPress;
    public int totalScore;
    public Image transition;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateCurrentPictures(int mapp)
    {
        if (cantPress)
        {
            return;
        }
        cantPress = true;
        mapIndex = mapp;
        if (mapIndex == 0)
        {
            currentDataset = easyMode.Where(item => !item.hard).ToArray();
        }
        else if (mapIndex == 1)
        {
            currentDataset = easyMode.Where(item => item.hard).ToArray();
        }
        else if (mapIndex == 2)
        {
            currentDataset = easyMode;
        }

        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(0, currentDataset.Length);
            while (chosenPictures.Contains(currentDataset[r]))
            {
                r = Random.Range(0, currentDataset.Length);
            }   
            chosenPictures.Add(currentDataset[r]);
        }

        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        transition.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Game");
    }

    [System.Serializable]
    public class Picture
    {
        public Sprite picture;
        public Vector2 correctPosition;
        public bool hard;
    }
}