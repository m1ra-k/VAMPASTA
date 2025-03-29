using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitCircleBehavior : MonoBehaviour
{
    public Transform closestChildTransform;
    public TextMeshProUGUI hitJudgement;
    public string circleType;

    private HitCircle hitCircle;
    private CookingGameManager cookingGameManager;
    private Coroutine updateHitJudgementCoroutine;
    private Queue<GameObject> approachCirclesQueue;

    private object lockObject = new object(); 

    public void Start()
    {
        cookingGameManager = FindObjectOfType<CookingGameManager>();

        switch (circleType)
        {
            case "food":
                approachCirclesQueue = cookingGameManager.approachCircleFoodQueue;
                break;

            case "garlic":
                approachCirclesQueue = cookingGameManager.approachCircleGarlicQueue;
                break;
        }
    }

    public void ButtonClicked()
    {
        if (closestChildTransform != null)
        {
            // print("tried to destroy the closest child");
            DestroyLargestApproachCircleChild();
        }
    }

    public void Update()
    {
        if (approachCirclesQueue.Count != 0)
        {
            UpdateApproachCircleChildrenSorted();
        }
    }

    public void UpdateApproachCircleChildrenSorted()
    {
        if (approachCirclesQueue.Peek().transform.position.y < 120f)
        {
            GameObject missed = approachCirclesQueue.Dequeue();
            // print($"due to too low: missed {missed.name}");
            StartCoroutine(cookingGameManager.RaviHurt());
            updateHitJudgementCoroutine = StartCoroutine(UpdateHitJudgement("miss"));
            RemoveHeart();
            closestChildTransform = null;
        }
        else
        {
            closestChildTransform = approachCirclesQueue.Peek().transform;
        }
    }

    void DestroyLargestApproachCircleChild()
    {
        Destroy(closestChildTransform.gameObject);
        GameObject got = approachCirclesQueue.Dequeue();
        if (got.transform.position.y < 190f) // y is
        {
            // print($"got {got.name}");
            updateHitJudgementCoroutine = StartCoroutine(UpdateHitJudgement("hit"));
        }
        else
        {
            // print($"due to early press: missed {got.name} at {got.transform.position.y}");
            StartCoroutine(cookingGameManager.RaviHurt());
            updateHitJudgementCoroutine = StartCoroutine(UpdateHitJudgement("miss"));
            RemoveHeart();
        }
        closestChildTransform = null;
    }

    private IEnumerator UpdateHitJudgement(string newHitJudgement)
    {
        hitJudgement.text = newHitJudgement;

        yield return new WaitForSeconds(0.35f);

        hitJudgement.text = "";

        updateHitJudgementCoroutine = null;
    }

    private void RemoveHeart()
    {
        try
        {
            lock (lockObject)
            {
                if (cookingGameManager.hearts.Count > 0)
                {
                    GameObject heart = cookingGameManager.hearts[cookingGameManager.hearts.Count - 1];
                    cookingGameManager.hearts.RemoveAt(cookingGameManager.hearts.Count - 1);
                    Destroy(heart);
                }
            }
        }
        catch (Exception)
        {
            Debug.Log("not letting happen");
        }
    }
}
