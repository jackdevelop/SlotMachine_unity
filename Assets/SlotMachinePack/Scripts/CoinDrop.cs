using UnityEngine;
using System.Collections;

public class CoinDrop : MonoBehaviour {
    public float delayTime = 1f;
    SpriteRenderer sr;
    Animator animator;
    public GameObject fxTitleObject;

	void Start () {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        DisplayFxTitle(false);
	}

    public void ShowMotion()
    {
        sr.enabled = true;
        animator.Play("CoinDrop");
        DisplayFxTitle(true);
        StartCoroutine(DelayAction(delayTime, () =>
        {
            StopMotion();
            DisplayFxTitle(false);
        }));
    }

    public void StopMotion()
    {
        sr.enabled = false;
        animator.Play("Stop");
    }

    void DisplayFxTitle(bool isON)
    {
        NGUITools.SetActive(fxTitleObject, isON);
    }

    IEnumerator DelayAction(float dTime, System.Action callback)
    {
        yield return new WaitForSeconds(dTime);
        callback();
    }
}
