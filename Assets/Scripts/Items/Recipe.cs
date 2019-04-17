using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : Item {

    public Ingredient[] coreIngredients;

    public Food foodCreated;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public bool CheckIfCorrect(Ingredient[] other)
    {
        return false;
    }
    public bool CanMake(Ingredient[] ingredients)
    {
        bool[] canMake = new bool[coreIngredients.Length];
        if(ingredients == null)
        {
            Debug.LogError("Ingredients is null!");
            return false;
        }
        //Cycling through the core ingredients
        for(int i = 0; i < coreIngredients.Length; i++)
        {
            for(int j = 0; j < ingredients.Length; j++)
            {
                if (ingredients[j] && coreIngredients[i])
                {
                    if (ingredients[j].itemName == coreIngredients[i].itemName)
                    {
                        canMake[i] = true;
                        break;
                    }
                    else
                    {
                        canMake[i] = false;
                    }
                }
                else
                {
                    canMake[i] = false;
                }
            }
        }
        //If any of the core ingredients is missing from the recipe, then return false
        foreach (bool b in canMake)
            if (!b) return false;
        //Otherwise return true
        return true;
    }
    public Food CreateFood(Ingredient[] ingredients)
    {
        Food returnedFood = foodCreated;
        GameObject go = Instantiate(returnedFood.realWorldObject);
        Food realFood = go.GetComponent<Food>();
        realFood.tasteRating = CalculateFoodTaste(ingredients);
        //realFood.CalculateTasteRating(realFood.baseIngredients);
        List<Ingredient> exIngredients = new List<Ingredient>(), allIngredients = new List<Ingredient>();
        foreach(Ingredient i in ingredients)
        {
            allIngredients.Add(i);
        }
        System.Array.Clear(ingredients, 0, ingredients.Length);
        realFood.baseIngredients = TrimToExtras(allIngredients, out exIngredients);
        realFood.extraIngredients = exIngredients.ToArray();
        realFood.realWorldObject = go;
        realFood.baseRecipe = this;
        if(realFood.ingredientHolster)
        {
            foreach(Ingredient i in realFood.baseIngredients)
            {
                i.realWorldObject.transform.SetParent(realFood.ingredientHolster.transform);
                i.realWorldObject.SetActive(false);
                i.realWorldObject.transform.localPosition = Vector3.zero;
            }
            foreach (Ingredient i in realFood.extraIngredients)
            {
                i.realWorldObject.transform.SetParent(realFood.ingredientHolster.transform);
                i.realWorldObject.SetActive(false);
                i.realWorldObject.transform.localPosition = Vector3.zero;
            }
        }
        return realFood;
    }
    int CalculateFoodTaste(Ingredient[] ingredients)
    {
        int taste = 0;
        foreach (Ingredient i in ingredients)
        {
            if(i)
                taste += (int)i.taste;
            else
                Debug.Log("An ingredient is null");
        }
        return taste;
    }
    Ingredient[] TrimToExtras(List<Ingredient> allIngredients, out List<Ingredient> extraIngredients)
    {
        List<Ingredient> coreList = new List<Ingredient>();
        extraIngredients = new List<Ingredient>();
        //Debug.Log("coreIngredients length: " + coreIngredients.Length + " and allIngredients length: " + allIngredients.Count);
        List<Ingredient> removeIndex = new List<Ingredient>();
        for (int i = 0; i < allIngredients.Count; i++)
        {
            //Debug.Log("Searching for " + allIngredients[i] + " clones now");
            for(int j = 0; j < coreIngredients.Length; j++)
            {
                if(allIngredients[i].itemName.Equals(coreIngredients[j].itemName))
                {
                    //Debug.Log(allIngredients[i].itemName + " equals " + coreIngredients[j].itemName);
                    coreList.Add(allIngredients[i]);
                    removeIndex.Add(allIngredients[i]);
                    break;
                }
                else
                {
                    //Debug.Log(allIngredients[i].itemName + " does not equal " + coreIngredients[i].itemName);
                }
            }            
        }
        foreach(Ingredient i in removeIndex)
        {
            allIngredients.Remove(i);
        }
        foreach (Ingredient i in allIngredients)
        {
            extraIngredients.Add(i);
        }

        return coreList.ToArray();
        
    }
}
