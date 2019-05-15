using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GrillUI : MonoBehaviour
{
    Grill currentGrill;
    public GameObject grillStorage;
    public GameObject ingredientSlotsParent;
    public GameObject grillStorage_Slot;
    public GameObject ingredient_Slot;

    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonPress(int b)
    {
        //urrentGrill.AddIngredient
    }

    public void SetGrill(Grill newGrill)
    {
        Debug.Log("Set Grill triggered");
        currentGrill = newGrill;
        UpdateUI();

    }

    private void UpdateUI()
    {
        foreach(Transform c in ingredientSlotsParent.transform)
        {
            Destroy(c.gameObject);
        }
        foreach (Transform c in grillStorage.transform)
        {
            Destroy(c.gameObject);
        }
        int numOfStorage, numOfSlots;
        numOfSlots = currentGrill.ingredientSlots.Length;
        numOfStorage = currentGrill.grillStorage.Capacity;
        Debug.Log(numOfSlots + " " + numOfStorage);

        for(int i = 0; i < numOfSlots; i++)
        {
            GameObject go = Instantiate(ingredient_Slot, ingredientSlotsParent.transform);
            TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
            if(!currentGrill.ingredients[i])
            {
                text.text = "Empty";

            }
            else
            {
                text.text = currentGrill.ingredients[i].itemName;
            }
        }
        for(int i = 0; i < numOfStorage; i++)
        {
            GameObject go = Instantiate(grillStorage_Slot, grillStorage.transform);
            TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
            if(!currentGrill.grillStorage[i])
            {
                text.text = "Empty";
                go.GetComponent<Button>().interactable = false;
            }
            else
            {
                go.GetComponent<Button>().interactable = true;
                text.text = currentGrill.grillStorage[i].itemName;
            }
        }
    }
}
