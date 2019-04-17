using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item {

    public Ingredient[] extraIngredients;
    public Ingredient[] baseIngredients;
    public Recipe baseRecipe;
    //The total calculated taste rating for this food
    public int tasteRating = 0;

    public float thirstModifier = 0;

    public float hungerModifier = 0;
    public GameObject ingredientHolster;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CalculateTasteRating(Ingredient[] baseIngredients)
    {
        float totalTasteModifier = 1;
        for(int i = 0; i < baseIngredients.Length; i++)
        {
            switch (baseIngredients[i].GetCookedLevel())
            {
                case Ingredient.CookedLevel.Cooked_Perfect:
                    totalTasteModifier += .05f;
                    break;
                case Ingredient.CookedLevel.Cooked_Warm:
                    totalTasteModifier += .025f;
                    break;
                case Ingredient.CookedLevel.Cooking_Hot:
                case Ingredient.CookedLevel.Cooked_Cold:
                case Ingredient.CookedLevel.Cooked_Burnt_Warm:
                    totalTasteModifier += .01f;
                    break;
                case Ingredient.CookedLevel.Cooking_Warm:
                case Ingredient.CookedLevel.Cooked_Burnt_Cold:
                    totalTasteModifier += .005f;
                    break;
                case Ingredient.CookedLevel.Frozen:
                case Ingredient.CookedLevel.Raw_Burnt:
                case Ingredient.CookedLevel.Raw_Warm:
                case Ingredient.CookedLevel.Raw_Cold:
                    totalTasteModifier += 0;
                    break;
            }
        }
        tasteRating = (int)(totalTasteModifier * (float)tasteRating);
    }
    public Food(Food o)
    {
        itemName = o.itemName;
        itemDescription = o.itemDescription;
        itemValue = o.itemValue;
        itemIcon = o.itemIcon;
        itemSize = o.itemSize;
        itemType = o.itemType;
        realWorldObject = o.realWorldObject;

        extraIngredients = new Ingredient[o.extraIngredients.Length];
        int count = 0;
        foreach(Ingredient i in o.extraIngredients)
        {
            extraIngredients[count] = new Ingredient(i);
            count++;
        }

    }
}
