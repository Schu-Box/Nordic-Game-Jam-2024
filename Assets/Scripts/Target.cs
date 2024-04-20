using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
   public void Hit()
   {
      Debug.Log("HIT TARGET");
      
      GameController.Instance.AddScore(1);

      Destroy(gameObject);
   }
}
