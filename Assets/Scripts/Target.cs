using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Target : MonoBehaviour
{
   public CircleCollider2D collider;
   public MMF_Player destroyFeedback;
   
   private FMOD.Studio.EventInstance fmodStudioEvent;

   private bool destroyed = false;
   public void Hit()
   {
      if (!destroyed)
      {
         // Debug.Log("HIT TARGET");
         destroyed = true;
         
         GameController.Instance.AddScore(1);
      
         destroyFeedback?.PlayFeedbacks();

         collider.enabled = false;
      
         fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/laser_hit");
         fmodStudioEvent.start();
         fmodStudioEvent.release();
      }
      else //THIS WILL NEVER BE CALLED CUZ COLLIDER DISABLED
      {
         fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/laser_static");
         fmodStudioEvent.start();
         fmodStudioEvent.release();
      }
   }
}
