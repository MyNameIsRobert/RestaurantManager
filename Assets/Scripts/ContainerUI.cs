using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ContainerUI : MonoBehaviour
{
    TextMeshProUGUI text;
    Container container;

    bool updating = false;
    IEnumerator UpdateText()
    {
        updating = true;
        while(container.itemsStored.Count > 0)
        {
            string tempString = "";
            tempString = "Grab " + container.itemsStored[container.itemsStored.Count - 1].GetComponent<Item>().itemName +
                " with Left click";


            text.text = tempString;
            yield return new WaitForSecondsRealtime(.1f);
        }

        yield return null;
        updating = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        container = transform.parent.GetComponent<Container>();
        
    }
    private void Update()
    {
        if(!updating && container.itemsStored.Count > 0)
        {
            StartCoroutine(UpdateText());

        }
    }
}
