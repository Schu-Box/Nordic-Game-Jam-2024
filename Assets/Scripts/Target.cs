using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Target : MonoBehaviour
{
   public CircleCollider2D collider;
   public MMF_Player destroyFeedback;

   private bool destroyed = false;
   public void Hit()
   {
      Debug.Log("HIT TARGET");
      destroyed = true;
      
      GameController.Instance.AddScore(1);
      
      destroyFeedback?.PlayFeedbacks();

      collider.enabled = false;

      // Destroy(gameObject);
   }
}
