using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFillSpawner : MonoBehaviour
{
   public Transform screenFillerParent;
   public ScreenFiller screenFillerPrefab;
   
   public Vector2 scaleMinMaxOfFillers = new Vector2(0.8f, 1.2f);

   public float spawnRadiusPerRing = 10f;

   private void Start()
   {
      Vector2 centerPoint = Vector2.zero;

      int numRings = 100;
      for (int r = 0; r < numRings; r++)
      {
         float spawnRadius = r * spawnRadiusPerRing;
         int numFillers = 100;
         for (int i = 0; i < numFillers; i++)
         {
            var radians = 2 * Mathf.PI / numFillers * i;
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);
            var spawnDir = new Vector2(horizontal, vertical);
            var spawnPos = centerPoint + spawnDir * spawnRadius;

            ScreenFiller filler = Instantiate(screenFillerPrefab, spawnPos, Quaternion.identity, screenFillerParent);
            var dir = centerPoint - spawnPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            filler.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            filler.transform.localScale = Vector3.one * Random.Range(scaleMinMaxOfFillers.x, scaleMinMaxOfFillers.y);
         }
      }
   }

   public void OnTriggerEnter2D(Collider2D collider2D)
   {
      Debug.Log("ETNERED");
      
      if (collider2D.gameObject.CompareTag("ScreenFillerTriggerer"))
      {
         Debug.Log("ScreenFiller entered");
         ScreenFiller screenFiller = collider2D.gameObject.GetComponent<ScreenFiller>();
         screenFiller.hideFeedback.PlayFeedbacks();
      }
   }
   
   public void OnTriggerExit2D(Collider2D collider2D)
   {
      Debug.Log("EXITED");
      
      if (collider2D.gameObject.CompareTag("ScreenFillerTriggerer"))
      {
         Debug.Log("ScreenFiller exited");
         ScreenFiller screenFiller = collider2D.gameObject.GetComponent<ScreenFiller>();
         screenFiller.showFeedback.PlayFeedbacks();
      }
   }
}
