using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectButton : MonoBehaviour
{
   public MapType mapType;

   public void SelectMap()
   {
      GameController.Instance.SelectMap(mapType);
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
