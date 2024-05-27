using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLinePoints : MonoBehaviour
{
    [SerializeField] LineRenderer Line;

    [SerializeField] float height;

    private Vector3 ptposition1;
    private Vector3 ptposition2;


    private void LateUpdate()
    {
        ptposition1 = new Vector3 (Line.GetPosition(2).x, height, Line.GetPosition(2).z);
        ptposition2 = new Vector3(Line.GetPosition(3).x, height, Line.GetPosition(3).z);
        Line.SetPosition(2, ptposition1);
        Line.SetPosition(3, ptposition2);
    }
}
