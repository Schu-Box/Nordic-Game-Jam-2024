using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFillSpawner : MonoBehaviour
{
   public Transform screenFillerParent;
   public ScreenFiller screenFillerPrefab;
   
   public Vector2 scaleMinMaxOfFillers = new Vector2(0.8f, 1.2f);

   public float spawnRadiusPerRing = 10f;
   public int numRings = 10;
   public int numFillersPerRing = 10;
   public float fillerPerRingIncrease = 1.5f;

   public float durationBetweenRingReveals = 0.3f;
   
   private void Start()
   {
      Vector2 centerPoint = Vector2.zero;

      for (int r = 0; r < numRings; r++)
      {
         float spawnRadius = r * spawnRadiusPerRing;
         int numFillers = (int)(numFillersPerRing * (fillerPerRingIncrease * (r + 1)));
         
         GameObject screenFillerRing = new GameObject("ScreenFillerRing" + r);
         screenFillerRing.transform.parent = screenFillerParent;
         for (int i = 0; i < numFillers; i++)
         {
            var radians = 2 * Mathf.PI / numFillers * i;
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);
            var spawnDir = new Vector2(horizontal, vertical);
            var spawnPos = centerPoint + spawnDir * spawnRadius;

            ScreenFiller filler = Instantiate(screenFillerPrefab, spawnPos, Quaternion.identity, screenFillerRing.transform);
            var dir = centerPoint - spawnPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            filler.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            filler.transform.localScale = Vector3.one * Random.Range(scaleMinMaxOfFillers.x, scaleMinMaxOfFillers.y);
         }
      }
   }

   public void HideAllScreenFillers()
   {
      StartCoroutine(HideAllScreenFillersCoroutine());
   }
   private IEnumerator HideAllScreenFillersCoroutine()
   {
      //hide all screenfillers in first ring, wait for durationBetweenRingReveals then reveal next ring
      //repeat until all rings are hidden
      
      for(int i = 0; i < screenFillerParent.childCount; i++)
      {
         Transform ring = screenFillerParent.GetChild(i);
         for (int j = 0; j < ring.childCount; j++)
         {
            ScreenFiller filler = ring.GetChild(j).GetComponent<ScreenFiller>();
            filler.hideFeedback.PlayFeedbacks();
         }
         
         yield return new WaitForSeconds(durationBetweenRingReveals);
      }
   }

   public void ShowAllScreenFillers()
   {
      StartCoroutine(ShowAllScreenFillersCoroutine());
   }
   private IEnumerator ShowAllScreenFillersCoroutine()
  {
     for(int i = screenFillerParent.childCount - 1; i >= 0; i--)
     {
        Transform ring = screenFillerParent.GetChild(i);
        for (int j = ring.childCount - 1; j >= 0; j--)
        {
           ScreenFiller filler = ring.GetChild(j).GetComponent<ScreenFiller>();
           filler.showFeedback.PlayFeedbacks();
        }
        
        yield return new WaitForSeconds(durationBetweenRingReveals);
     }
  }
   
   

   // public void OnTriggerEnter2D(Collider2D collider2D)
   // {
   //    Debug.Log("ETNERED");
   //    
   //    if (collider2D.gameObject.CompareTag("ScreenFillerTriggerer"))
   //    {
   //       Debug.Log("ScreenFiller entered");
   //       ScreenFiller screenFiller = collider2D.gameObject.GetComponent<ScreenFiller>();
   //       screenFiller.hideFeedback.PlayFeedbacks();
   //    }
   // }
   //
   // public void OnTriggerExit2D(Collider2D collider2D)
   // {
   //    Debug.Log("EXITED");
   //    
   //    if (collider2D.gameObject.CompareTag("ScreenFillerTriggerer"))
   //    {
   //       Debug.Log("ScreenFiller exited");
   //       ScreenFiller screenFiller = collider2D.gameObject.GetComponent<ScreenFiller>();
   //       screenFiller.showFeedback.PlayFeedbacks();
   //    }
   // }
}
