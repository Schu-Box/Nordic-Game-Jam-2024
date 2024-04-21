using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ScreenFiller : MonoBehaviour
{
    public MMF_Player hideFeedback;
    public MMF_Player showFeedback;
    
    public void Hide()
    {
        hideFeedback.PlayFeedbacks();
    }

    public void Show()
    {
        showFeedback.PlayFeedbacks();
    }
}
