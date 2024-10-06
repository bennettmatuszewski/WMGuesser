using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Map map;
    public SpriteRenderer mapImage;

    public Camera cam;
    public CameraMovement pictureCamera;
    public TMP_Text multiplierText;
    
    public bool canZoomOut;
    public float multiplier;

    public SpriteRenderer[] bounds;
    public Image transition;
    public Image transition2;
    private PersistentController persistentController;
    public int currentPicture;
    // Start is called before the first frame update
    void Start()
    {
        persistentController = PersistentController.Instance;
        multiplierText.text = multiplier + "x";
        transition2.DOFade(0, 0.5f);
        StartGame();
    }

    public void StartGame()
    {
        map.Close();
        mapImage.sprite = persistentController.chosenPictures[currentPicture].picture;
        map.correctPoint.transform.position = new Vector3(persistentController.chosenPictures[currentPicture].correctPosition.x, persistentController.chosenPictures[currentPicture].correctPosition.y, 10);

    }

    public void NextRound()
    {
        StartCoroutine(NextRound2());
    }
    private IEnumerator NextRound2()
    {
        if (currentPicture == 4)
        {
            transition.DOFade(1, 0.5f);
            transition2.DOFade(1, 0.5f);
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("EndScreen");
            yield break;
        }
        
        currentPicture++;
        transition.DOFade(1, 0.5f);
        transition2.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        mapImage.sprite = persistentController.chosenPictures[currentPicture].picture;
        map.correctPoint.transform.position = new Vector3(persistentController.chosenPictures[currentPicture].correctPosition.x, persistentController.chosenPictures[currentPicture].correctPosition.y, 10);
        map.guessPoint.transform.position = new Vector3(1000, 1000, 10);
        map.darkCover.DOFade(0f, 0f);
        map.gameOverlay.SetActive(false);
        map.nextButton.SetActive(false);
        
        map.pictureCam.orthographicSize = 8.75f;
        map.dashedLine.GetComponent<TrailRenderer>().Clear();
        map.correctPoint.SetActive(false);
        map.correctPoint.transform.DOScale(new Vector3(0, 0, 10), 0);
        multiplier = 1.5f;
        map.mainCam.orthographicSize = 35;
        map.cameraMovement.targetCamSize = 35;
        yield return new WaitForSeconds(0.5f);
        transition.DOFade(0, 0.5f);
        transition2.DOFade(0, 0.5f);
        map.cameraMovement.canMove = true;
        map.dashedLine.GetComponent<TrailRenderer>().enabled = false;
        map.canOpenClose = true;
        map.cameraMovement.canMove = true;
        map.cameraMovement.canZoom = true;
        pictureCamera.mapRenderer = bounds[0];
        pictureCamera.CalculateBounds();
        pictureCamera.maxCamSize = 8.75f;
        multiplierText.text = multiplier + "x";
        map.Close();
    }

    public void ZoomOut()
    {
        if (!canZoomOut || multiplier<=1)
        {
            return;   
        }

        pictureCamera.canMove = false;
        pictureCamera.canZoom = false;
        canZoomOut = false;
        if (multiplier>1)
        {
            pictureCamera.maxCamSize *= 2;
            multiplier -= 0.25f;
            multiplierText.text = multiplier + "x";
        }
        
        DOTween.To(()=> cam.orthographicSize, x=> cam.orthographicSize = x, pictureCamera.maxCamSize, 0.5f).SetEase(
            Ease.InOutQuad);
        StartCoroutine(WaitCanZoomOut());
    }

    IEnumerator WaitCanZoomOut()
    {
        yield return new WaitForSeconds(0.5f);
        pictureCamera.mapRenderer = bounds[multiplier >= 1.5 ? 0 : multiplier >= 1.25 ? 1 : 2];
        pictureCamera.CalculateBounds();
        canZoomOut = true;
        pictureCamera.targetCamSize = pictureCamera.maxCamSize;
        pictureCamera.canZoom = true;
        pictureCamera.canMove = true;
    }
}
