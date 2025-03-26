using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitCircleBehavior : MonoBehaviour
{
    public Transform largestChildTransform;
    public TextMeshProUGUI hitJudgement;

    private SortedDictionary<float, Transform> approachCircleChildrenSorted = new SortedDictionary<float, Transform>();
    private Coroutine updateHitJudgementCoroutine;
    private float maxScale = 1.20f;

    public void ButtonClicked()
    {
        if (largestChildTransform != null)
        {
            DestroyLargestApproachCircleChild();
            DetermineScore();
        }
    }

    public void UpdateApproachCircleChildrenSorted(Transform approachCircleChildTransform)
    {
        if (approachCircleChildTransform == null || approachCircleChildTransform.gameObject == null)
        {
            return;
        }

        float scaleMagnitude = approachCircleChildTransform.localScale.magnitude;

        // log n
        if (approachCircleChildTransform.localScale.x >= maxScale)
        {
            approachCircleChildrenSorted.Remove(scaleMagnitude);
            
            if (largestChildTransform == approachCircleChildTransform)
            {
                largestChildTransform = null;
            }
            
            updateHitJudgementCoroutine = StartCoroutine(UpdateHitJudgement("miss"));
            
            Destroy(approachCircleChildTransform.gameObject);
            return;
        }

        // log n
        if (approachCircleChildrenSorted.ContainsKey(scaleMagnitude))
        {
            approachCircleChildrenSorted.Remove(scaleMagnitude);
        }
        
        approachCircleChildrenSorted.Add(scaleMagnitude, approachCircleChildTransform);

        if (largestChildTransform == null || scaleMagnitude > largestChildTransform.localScale.magnitude)
        {
            largestChildTransform = approachCircleChildTransform;
        }
    }

    void DestroyLargestApproachCircleChild()
    {
        Destroy(largestChildTransform.gameObject);
    }

    void DetermineScore()
    {
        float uniformScale = largestChildTransform.localScale.x;

        if (updateHitJudgementCoroutine != null)
        {
            StopCoroutine(updateHitJudgementCoroutine);
        }

        if (uniformScale < 0.70f || uniformScale >= maxScale)
        {
            updateHitJudgementCoroutine = StartCoroutine(UpdateHitJudgement("hit"));
        }
        else if (uniformScale >= 0.70 && uniformScale < 0.80)
        {
            updateHitJudgementCoroutine = StartCoroutine(UpdateHitJudgement("miss"));
            // TODO MINUS HEART
        }
    }

    IEnumerator UpdateHitJudgement(string newHitJudgement)
    {
        hitJudgement.text = newHitJudgement;

        yield return new WaitForSeconds(0.35f);

        hitJudgement.text = "";

        updateHitJudgementCoroutine = null;
    }

    class ApproachCircleChildrenComparer : IComparer<Transform>
    {
        public int Compare(Transform a, Transform b)
        {
            return b.localScale.magnitude.CompareTo(a.localScale.magnitude);
        }
    }
}
