using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoveUIController : MonoBehaviour
{
    public GameObject defaultIngredientDisplay;
    public Transform contentHolster;
    Grill grillParent;
    // Start is called before the first frame update
    void Start()
    {
        grillParent = transform.parent.GetComponent<Grill>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ImageController(int index)
    {
        GameObject go = Instantiate(defaultIngredientDisplay, contentHolster);
        TextMeshProUGUI s1, s2, total;
        Image icon = go.transform.GetChild(0).GetComponent<Image>();
        s1 = go.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        s2 = go.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        total = go.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        
        while(grillParent.ingredients[index] != null)
        {
            Ingredient ingredient = grillParent.ingredients[index];
            icon.sprite = ingredient.itemIcon;
            string s1Result = "", s2Result ="", totalResult ="";
            s1Result = ingredient.getSide1Temp() + "* F";
            s2Result = ingredient.getSide2Temp() + "* F";
            ingredient.CalculateTotalTemp();
            totalResult = (int)ingredient.totalCurrentTemp + "* F";

            s1.text = s1Result;
            s2.text = s2Result;
            total.text = totalResult;
            yield return new WaitForSecondsRealtime(.5f);
        }
        Destroy(go);


        yield return null;
    }

    public void AddIngredient(Ingredient ingredient, int index)
    {
        StartCoroutine(ImageController(index));
    }
}
