using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Target : MonoBehaviour
{
   public CircleCollider2D collie;
   
   public MMF_Player destroyFeedback;
   public MMF_Player restoreFeedback;

   public Vector2 timeUntilRestorationRange = new Vector2(8f, 12f);

   public Vector2 restorationTimeIncreaseWhenRestored = new Vector2(4f, 6f);

   public float timeAwardedWhenHit = 0.5f;

   private int timesDestroyed = 0;

   public bool destroyed = false;
   
   private void Start()
   {
      GameController.Instance.targetList.Add(this);
   }

   private void OnDestroy()
   {
      if(GameController.Instance.targetList.Contains(this))
         GameController.Instance.targetList.Remove(this);
   }
   
   public void Hit()
   {
      if (!destroyed)
      {
         // Debug.Log("HIT TARGET");
         destroyed = true;
         timesDestroyed++;
         
         GameController.Instance.AddScore(1);
      
         destroyFeedback?.PlayFeedbacks();

         // collie.enabled = false;
      
         AudioManager.Instance.PlayEvent("event:/laser_hit");

         if (GameController.Instance.currentMode == ModeType.Endless) //If it's endless, add time and restore the target over time
         {
            StartCoroutine(RestoreCoroutine());
         
            GameController.Instance.AddTime(timeAwardedWhenHit);
         } 
         else if (GameController.Instance.currentMode == ModeType.Timed) //If it's timed, just check if all targets have been hit
         {
            GameController.Instance.CheckIfAllTargetsHit();
         }
      }
      else //THIS WILL NEVER BE CALLED CUZ COLLIDER DISABLED
      {
         AudioManager.Instance.PlayEvent("event:/laser_static");
      }
   }

   public IEnumerator RestoreCoroutine()
   {
      float randomDelay = Random.Range(timeUntilRestorationRange.x, timeUntilRestorationRange.y) + (Random.Range(restorationTimeIncreaseWhenRestored.x, restorationTimeIncreaseWhenRestored.y) * timesDestroyed);
      yield return new WaitForSeconds(randomDelay);
      
      restoreFeedback.PlayFeedbacks();

      yield return new WaitForSeconds(restoreFeedback.TotalDuration);

      destroyed = false;
      // collie.enabled = true;
   }
}
