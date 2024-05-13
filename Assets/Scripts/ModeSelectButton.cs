using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectButton : MonoBehaviour
{
    public ModeType modeType;

    public Image buttonImage;

    public Color selectedColor = Color.white;
    public Color unselectedColor = Color.gray;

    public void SelectMode()
    {
        GameController.Instance.SelectMode(modeType);

        foreach (ModeSelectButton modeSelectButton in GameController.Instance.modeSelectButtonList)
        {
            modeSelectButton.ToggleSelectedVisual(false);
        }
        
        ToggleSelectedVisual(true);
    }

    public void ToggleSelectedVisual(bool selected)
    {
        if (selected)
        {
            buttonImage.color = selectedColor;
        }
        else
        {
            buttonImage.color = unselectedColor;
        }
    }
}
