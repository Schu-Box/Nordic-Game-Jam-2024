using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectButton : MonoBehaviour
{
   public MapType mapType;

   private Animator animator;

   private void Awake()
   {
      animator = GetComponent<Animator>();
   }

   public void SelectMap()
   {
      GameController.Instance.SelectMap(this);

      animator.Play("Expand");
   }

   public void DeselectMap()
   {
      animator.Play("Contract");
   }
}

public enum MapType
{
   Circle,
   Donut,
   
}

public enum ModeType
{
   Timed,
   Endless,
   
}
