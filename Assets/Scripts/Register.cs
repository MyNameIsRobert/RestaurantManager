using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : Station
{
    UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            uiManager.ToggleRegisterUI();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            uiManager.ToggleRegisterUI();
        }
    }
}
