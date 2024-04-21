using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenFillerTriggerer : MonoBehaviour
{
    public float maxSize = 10f;
    public float duration = 1f;

    public void StartGrow()
    {
        StartCoroutine(Grow());
    }
    
    private IEnumerator Grow()
    {
        //change size of localScale from 0 to maxSize over duration 
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Lerp(0, maxSize, t / duration);
            yield return null;
        }
    }

    public void StartShrink()
    {
        StartCoroutine(Shrink());
    }
    private IEnumerator Shrink()
    {
        //change size of localScale from maxSize to 0 over duration 
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Lerp(maxSize, 0, t / duration);
            yield return null;
        }
    }
}
