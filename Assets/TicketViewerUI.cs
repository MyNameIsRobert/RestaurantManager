using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TicketViewerUI : MonoBehaviour
{

    public CanvasGroup canvasGroup;
   public FoodPickup pickup;
    public TMP_Dropdown dropdown;
    public GameObject foodOrder;
    public TextMeshProUGUI totalTicketCost, ticketName;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        pickup = FindObjectOfType<FoodPickup>();
        StartCoroutine(DropdownUpdator());
    }
    IEnumerator DropdownUpdator()
    {
        while(true)
        {
            if(pickup && pickup.currentTickets != null)
            {
                int lastIndex = dropdown.value;
                dropdown.options.Clear();
                foreach(Ticket t in pickup.currentTickets)
                    if(t)
                        dropdown.options.Add(new TMP_Dropdown.OptionData(t.itemName));
                if(dropdown.options.Count == 1)
                {
                    dropdown.value = 0;
                    UpdateCurrentTicket();
                }   
                else
                {
                    dropdown.value = lastIndex;
                }     
                     
            }

            yield return new WaitForSeconds(.5f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Recipe"))
        {
            if(canvasGroup.alpha == 0)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
    }

    public void UpdateCurrentTicket()
    {
        string selectedTicket = dropdown.options[dropdown.value].text;
        foreach(Ticket t in pickup.currentTickets)
        {
            if(t.itemName == selectedTicket)
            {
                ticketName.text = t.itemName;
                totalTicketCost.text = t.totalCost + "$";

                if (t.foodOrdered.Count > 1)
                {
                    Food f = t.foodOrdered[0];
                    foodOrder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = f.itemName;
                    foodOrder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = t.foodCost[0] + "$";
                    GameObject extraIngredient_Image = foodOrder.transform.GetChild(2).GetChild(0).gameObject;
                    if (t.extraIngredients[0] != null && t.extraIngredients[0].Count > 0)
                    {
                        if (t.extraIngredients[0].Count == 1)
                        {
                            extraIngredient_Image.GetComponent<Image>().sprite = t.extraIngredients[0][0].itemIcon;
                            extraIngredient_Image.GetComponent<Image>().color = Color.white;
                        }
                        else
                        {
                            extraIngredient_Image.GetComponent<Image>().sprite = t.extraIngredients[0][0].itemIcon;
                            for (int i = 1; i < t.extraIngredients[0].Count; i++)
                            {
                                GameObject go = Instantiate(extraIngredient_Image, foodOrder.transform.GetChild(2));
                                go.GetComponent<Image>().sprite = t.extraIngredients[0][i].itemIcon;
                            }
                        }
                    }
                    else
                    {
                        extraIngredient_Image.GetComponent<Image>().color = Color.clear;
                    }

                    for (int i = 1; i < t.foodOrdered.Count; i++)
                    {
                        GameObject go = Instantiate(foodOrder, transform.GetChild(2));
                        Food f1 = t.foodOrdered[i];
                        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = f1.itemName;
                        go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = t.foodCost[i] + "$";
                        GameObject extraIngredient_Image1 = go.transform.GetChild(2).GetChild(0).gameObject;
                        if (t.extraIngredients[i] != null && t.extraIngredients[i].Count > 0)
                        {
                            if (t.extraIngredients[i].Count == 1)
                            {
                                extraIngredient_Image1.GetComponent<Image>().sprite = t.extraIngredients[i][0].itemIcon;
                                extraIngredient_Image1.GetComponent<Image>().color = Color.white;
                            }
                            else
                            {
                                extraIngredient_Image1.GetComponent<Image>().sprite = t.extraIngredients[i][0].itemIcon;
                                for (int j = 1; j < t.extraIngredients[i].Count; j++)
                                {
                                    GameObject go1 = Instantiate(extraIngredient_Image1, foodOrder.transform.GetChild(2));
                                    go1.GetComponent<Image>().sprite = t.extraIngredients[i][j].itemIcon;
                                }
                            }
                        }
                        else
                        {
                            extraIngredient_Image1.GetComponent<Image>().color = Color.clear;
                        }
                    }
                }
                else
                {
                    Food f = t.foodOrdered[0];
                    foodOrder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = f.itemName;
                    foodOrder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = t.foodCost[0] + "$";
                    GameObject extraIngredient_Image = foodOrder.transform.GetChild(2).GetChild(0).gameObject;
                    if (t.extraIngredients[0] != null && t.extraIngredients[0].Count > 0)
                    {
                        if (t.extraIngredients[0].Count == 1)
                        {
                            extraIngredient_Image.GetComponent<Image>().sprite = t.extraIngredients[0][0].itemIcon;
                            extraIngredient_Image.GetComponent<Image>().color = Color.white;
                        }
                        else
                        {
                            extraIngredient_Image.GetComponent<Image>().sprite = t.extraIngredients[0][0].itemIcon;
                            for (int i = 1; i < t.extraIngredients[0].Count; i++)
                            {
                                GameObject go = Instantiate(extraIngredient_Image, foodOrder.transform.GetChild(2));
                                go.GetComponent<Image>().sprite = t.extraIngredients[0][i].itemIcon;
                            }
                        }
                    }
                    else
                    {
                        extraIngredient_Image.GetComponent<Image>().color = Color.clear;
                    }
                }
            }
        }
    }
}
