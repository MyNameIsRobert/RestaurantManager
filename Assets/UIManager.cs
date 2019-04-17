using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public CanvasGroup grill, register, staging, pickup;

    public void GrillUI(bool on)
    {
        if (on)
        { 
            grill.alpha = 1;
        grill.blocksRaycasts = true;
        grill.interactable = true;
        }
        else
        {
            grill.alpha = 0;
            grill.blocksRaycasts = false;
            grill.interactable = false;
        }
    }
    public void ToggleRegisterUI()
    {
        register.alpha = (register.alpha == 0) ? 1 : 0;
        register.blocksRaycasts = !register.blocksRaycasts;
        register.interactable = !register.interactable;
    }
    public void ToggleStagingUI()
    {
        staging.alpha = (staging.alpha == 0) ? 1 : 0;
        staging.blocksRaycasts = !staging.blocksRaycasts;
        staging.interactable = !staging.interactable;
    }
    public void TogglePickupUI()
    {
        pickup.alpha = (pickup.alpha == 0) ? 1 : 0;
        pickup.blocksRaycasts = !pickup.blocksRaycasts;
        pickup.interactable = !pickup.interactable;
    }

}
