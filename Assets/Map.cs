using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using DG.Tweening;
using TMPro;

public class Map : MonoBehaviour
{
    public Camera mainCam;
    public Camera pictureCam;
    public GameObject map;
    public GameObject guessPoint;
    public GameObject correctPoint;
    public CameraMovement cameraMovement;
    public Image darkCover;
    public GameObject dashedLine;
    public GameObject gameOverlay;
    public GameObject nextButton;
    public GameObject makeGuessButton;
    public GameObject picture;
    public GameObject zoomStuff;
    public TMP_Text scoreText;
    public Slider scoreSlider;
    private GameController gameController;
    public GameObject openMapButtonPicture;
    public GameObject openMapButtonMap;

    public bool isOpen;
    public bool canGuess;
    public bool canOpenClose;
    private Vector3 mouseDownPos;
    private Vector3 mouseUpPos;
    private float clickThreshold = 0.1f; // Small threshold to differentiate between click and drag
    private float maxClickDuration = 0.2f; // Maximum time considered a "click"

    private float clickStartTime;
    private bool clickedOnce;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenClose();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Input.mousePosition;
            clickStartTime = Time.time; // Record the time the click started
        }

        // Mouse button released
        if (Input.GetMouseButtonUp(0))
        {
            mouseUpPos = Input.mousePosition;
            float clickDuration = Time.time - clickStartTime;

            // Check if the duration was short enough and the mouse didn't move much
            if (canGuess && Vector3.Distance(mouseDownPos, mouseUpPos) < clickThreshold && clickDuration < maxClickDuration)
            {
                var pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                var pos2 = mainCam.ScreenToViewportPoint(Input.mousePosition);
                if (pos2.x >0.87f && pos2.y < 0.21f)
                {
                    return;
                }
                guessPoint.transform.position = new Vector3(pos.x,pos.y, 10);
                clickedOnce = true;
            }
        }
    }
    

    public void OpenClose()
    {
        if (isOpen && canOpenClose)
        {
            Close();
        }
        else if (!isOpen && canOpenClose)
        {
            Open();
        }
    }

    public void Open()
    {
        canGuess = true;
        isOpen = true;
        transform.position = Vector3.zero;
        pictureCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        map.SetActive(true);
        picture.SetActive(false);
        zoomStuff.SetActive(false);
        makeGuessButton.SetActive(true);
        openMapButtonPicture.SetActive(false);
        openMapButtonMap.SetActive(true);
    }

    public void Close()
    {
        canGuess = false;
        isOpen = false;
        pictureCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);
        map.SetActive(false);
        picture.SetActive(true);
        zoomStuff.SetActive(true);
        makeGuessButton.SetActive(false);
        openMapButtonPicture.SetActive(true);
        openMapButtonMap.SetActive(false);
    }

    public void MakeGuess()
    {
        if (!canGuess || !clickedOnce)
        {
            return;
        }

        canOpenClose = false;
        cameraMovement.canMove = false;
        cameraMovement.canZoom = false;
        canGuess = false;
        clickedOnce = false;
        StartCoroutine(Guess());
    }

    IEnumerator Guess()
    {
        makeGuessButton.SetActive(false);
        correctPoint.SetActive(true);
        float dist = Vector3.Distance(correctPoint.transform.position, guessPoint.transform.position);
        float distX = Mathf.Abs(guessPoint.transform.position.x - correctPoint.transform.position.x);
        float distY = Mathf.Abs(guessPoint.transform.position.y - correctPoint.transform.position.y);
        float camSize = distY < distX
            ? 0.000002f * Mathf.Pow(dist, 3) - 0.0006f * Mathf.Pow(dist, 2) + 0.4f * dist + 1f
            : 0.000006f * Mathf.Pow(dist, 3) - 0.002f * Mathf.Pow(dist, 2) + 0.62f * dist + 1f;
        float t = Mathf.InverseLerp(1, 35, camSize);
        float calculatedScale = Mathf.Lerp(correctPoint.GetComponent<GuessPointScale>().minScale, correctPoint.GetComponent<GuessPointScale>().maxScale, t);
        correctPoint.GetComponent<GuessPointScale>().noUpdate = true;
        correctPoint.transform.DOScale(new Vector3(calculatedScale, calculatedScale, 10), 0.5f).SetEase(Ease.OutBack).OnComplete(()=>correctPoint.GetComponent<GuessPointScale>().noUpdate = false);
        Vector3 midpoint = (correctPoint.transform.position + guessPoint.transform.position) / 2;
        mainCam.transform.DOMove(new Vector3(midpoint.x, midpoint.y, mainCam.transform.position.z), 0.5f).SetEase(Ease.InOutQuad);
        DOTween.To(()=> mainCam.orthographicSize, x=> mainCam.orthographicSize = x, camSize, 0.5f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.5f);
        dashedLine.transform.position =
            new Vector3(guessPoint.transform.position.x, guessPoint.transform.position.y, 10);

        dashedLine.GetComponent<TrailRenderer>().startWidth = dist / 90;
        
        dashedLine.GetComponent<TrailRenderer>().enabled = true;
        dashedLine.transform
            .DOMove(new Vector3(correctPoint.transform.position.x, correctPoint.transform.position.y, 10), 1.5f)
            .SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(2f);
        CalculateScore();
        darkCover.DOFade(0.93f, 0.5f);
        gameOverlay.SetActive(true);
        if (gameController.currentPicture == 4)
        {
            nextButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Finish";
        }
        nextButton.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        cameraMovement.canMove = true;





        yield return new WaitForSeconds(10000);
        dashedLine.GetComponent<TrailRenderer>().enabled = false;
    }

    void CalculateScore()
    {
        float dist = Vector3.Distance(correctPoint.transform.position, guessPoint.transform.position);
        float maxScore = 1000f;
        float scaleFactor = 25f;  // Adjust based on the expected range of dist
        int score = Mathf.Clamp(Mathf.RoundToInt(Mathf.Max(0f, maxScore - (dist * scaleFactor)))+100, 0, 1000);
        score = Mathf.RoundToInt(score * gameController.multiplier);
        scoreText.text = score.ToString();
        scoreSlider.value = score;
        PersistentController.Instance.totalScore += score;
    }
}