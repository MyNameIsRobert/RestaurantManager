using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RegisterUIController : MonoBehaviour {


    public enum UIStage
    {
        Off,
        NoTicketYet,
        AddFood,
        AddIngredients,
        ViewFood,
        ViewTicket,
        ClearedFood
    }

    public UIStage currentStage = UIStage.Off;
    public GameObject scrollViewContent;
    public CanvasGroup canvasGroup;
    RunRegister register;
    public Button confirmButton;
    public GameObject addFoodButton;
    public Button doneButton;
    public int selectedIndex = -1;
    public TextMeshProUGUI orderText;
	// Use this for initialization
	void Start () {
        register = transform.parent.GetComponent<RunRegister>();
        register.uIController = this;
        UpdateUI();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void TestButton()
    {
        Debug.Log("Button was hit");
    }
    void TestButton(int i)
    {
        Debug.Log("Button " + i.ToString() + " was hit");
    }
    void UpdateUI()
    {
        switch(currentStage)
        {
            case UIStage.Off:
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                break;
            case UIStage.NoTicketYet:
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                break;
            case UIStage.AddFood:
                confirmButton.gameObject.SetActive(true);
                for(int i = 0; i < register.manager.tickedFoods.Count; i++)
                {
                    Food item = register.manager.tickedFoods[i].food;
                    int salePrice = register.manager.tickedFoods[i].salePrice;
                    GameObject go = Instantiate(addFoodButton, scrollViewContent.transform);
                    Button b = go.GetComponent<Button>();
                    b.image.sprite = item.itemIcon;
                    go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
                    go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + salePrice;
                    int temp = i;
                    b.onClick.AddListener(() => register.SetSelectedFood(temp));
                }
                break;
            case UIStage.AddIngredients:
                confirmButton.gameObject.SetActive(false);
                doneButton.gameObject.SetActive(true);
                for (int i = 0; i < register.manager.extraIngredients.Count; i++)
                {
                    Ingredient item = register.manager.extraIngredients[i].ingredient;
                    int salePrice = register.manager.extraIngredients[i].addPrice;
                    GameObject go = Instantiate(addFoodButton, scrollViewContent.transform);
                    Button b = go.GetComponent<Button>();
                    b.image.sprite = item.itemIcon;
                    go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
                    go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + salePrice;
                    int temp = i;
                    b.onClick.AddListener(() => register.SetSelectedIngredient(temp));
                }
                break;
            case UIStage.ViewFood:

                break;
            case UIStage.ViewTicket:

                break;
            case UIStage.ClearedFood:
                foreach(Transform child in scrollViewContent.transform)
                {
                    Destroy(child.gameObject);
                }
                break;

        }
        if(orderText && register.newTicket)
        {
            orderText.text = register.newTicket.ToString();
        }
        else if(orderText)
        {
            orderText.text = "";
        }
    }

    public void SetUIStage(UIStage stage)
    {
        currentStage = stage;
        UpdateUI();
    }
    void AddFood(int i)
    {
        Debug.Log("Food at " + i + " was added");
        selectedIndex = i;
    }
}
