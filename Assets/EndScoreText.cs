using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScoreText : MonoBehaviour
{
    // Start is called before the first frame update
    public Image transition;
    void Start()
    {
        transition.DOFade(0, 0.5f);
        GetComponent<TMP_Text>().text = "Score: " + PersistentController.Instance.totalScore;
    }

    public void PlayAgain()
    {
        StartCoroutine(PlayAgain2());
    }

    IEnumerator PlayAgain2()
    {
        transition.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("StartScreen");
    }
}
