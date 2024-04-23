using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Target : MonoBehaviour
{
   public CircleCollider2D collie;
   public MMF_Player destroyFeedback;

   private bool destroyed = false;
   public void Hit()
   {
      if (!destroyed)
      {
         // Debug.Log("HIT TARGET");
         destroyed = true;
         
         GameController.Instance.AddScore(1);
      
         destroyFeedback?.PlayFeedbacks();

         collie.enabled = false;
      
         AudioManager.Instance.PlayEvent("event:/laser_hit");
      }
      else //THIS WILL NEVER BE CALLED CUZ COLLIDER DISABLED
      {
         AudioManager.Instance.PlayEvent("event:/laser_static");
      }
   }
}
