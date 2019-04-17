using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public CanvasGroup restaurantOverview;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(restaurantOverview.alpha == 1)
            {
                restaurantOverview.alpha = 0;
                restaurantOverview.blocksRaycasts = false;
                restaurantOverview.interactable = false;
            }
            else
            {
                restaurantOverview.alpha = 1;
                restaurantOverview.blocksRaycasts = true;
                restaurantOverview.interactable = true;
            }
        }
    }
}
