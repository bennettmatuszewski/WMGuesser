using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public RectTransform parent1;
    public RectTransform parent2;
    private bool cantMove;
    public Image transition;

    private void Start()
    {
        transition.DOFade(0, 0.5f);
    }

    public void MoveTutorial()
    {
        if (cantMove)
        {
            return;
        }
        StartCoroutine(MoveTutorial2());
    }
    IEnumerator MoveTutorial2()
    {
        cantMove = true;
        parent1.DOAnchorPosX(-2000, 0.5f).SetEase(Ease.InOutQuad);
        parent2.DOAnchorPosX(0, 0.5f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.5f);
        cantMove = false;
    }

    public void MoveBack()
    {
        if (cantMove)
        {
            return;
        }
        StartCoroutine(MoveBack2());
    }
    IEnumerator MoveBack2()
    {
        cantMove = true;
        parent2.DOAnchorPosX(2000, 0.5f).SetEase(Ease.InOutQuad);
        parent1.DOAnchorPosX(0, 0.5f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.5f);
        cantMove = false;
    }
}
