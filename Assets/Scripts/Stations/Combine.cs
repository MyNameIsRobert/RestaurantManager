using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Combine : Station {

    public Dictionary<int, Ingredient>[] ingredients = new Dictionary<int, Ingredient>[3];

    public List<Recipe> knownRecipies = new List<Recipe>();
    public Recipe[] recipes = new Recipe[3];
    public Transform objectHolster;
    CombineUI ui;
    private void Start()
    {
        knownRecipies = FindObjectOfType<GameManager>().knownRecipes;
        ui = GetComponentInChildren<CombineUI>();
    }
    public void AddIngredient(Ingredient ingredient, int ticketIndex)
    {
        bool correct = false;
        if(recipes[ticketIndex])
        {
            for(int i= 0; i < recipes[ticketIndex].coreIngredients.Length; i++)
            {
                if (ingredient == recipes[ticketIndex].coreIngredients[i])
                {
                    if (ingredients[ticketIndex] == null)
                    {
                        ingredients[ticketIndex] = new Dictionary<int, Ingredient>();
                    }
                    if (!ingredients[ticketIndex].ContainsKey(i))
                    {
                        ingredients[ticketIndex].Add(i, ingredient);
                        correct = true;
                        break;
                    }
                }
            }
        }
        if (!correct)
        {
            if(ingredients[ticketIndex] == null)
            {
                ingredients[ticketIndex] = new Dictionary<int, Ingredient>();
            }
            ingredients[ticketIndex].Add(ingredients[ticketIndex].Keys.Max() + 1, ingredient);
        }
        ingredient.realWorldObject.transform.SetParent(objectHolster);
        ingredient.realWorldObject.transform.localPosition = Vector3.zero;
    }
    public Recipe[] RecipesWithIngredients()
    {
        
        return null;
    }

    public bool UseRecipe(Recipe r, Ingredient[] ingredients, out Food food)
    {
        food = null;
        if (!knownRecipies.Contains(r)) return false;
        bool hasIngredients = false;
        foreach(Recipe recipe in RecipesWithIngredients())
        {
            if (r.itemName == recipe.itemName) hasIngredients = true; break;
        }
        if(hasIngredients)
        {
            food = r.CreateFood(ingredients);
        }


        return hasIngredients;
    }
    public bool UseRecipe(int recipeIndex)
    {

        Debug.Log("UseRecipe(index) was called");
        if (!player)
            return false;
        Recipe r = recipes[recipeIndex];
        List<Ingredient> temp = new List<Ingredient>();
        for(int i = 0; i < ingredients[recipeIndex].Count; i++)
        {
            Ingredient ing;
            if(ingredients[recipeIndex].TryGetValue(i, out ing))
            {
                temp.Add(ing);
            }
        }
        Food returnedFood;
        returnedFood = r.CreateFood(temp.ToArray());
        player.AddItemToHand(returnedFood.realWorldObject);
        ingredients[recipeIndex].Clear();
        ui.UpdatePlayerInventory();
        ui.UpdateCurrentRecipes(recipeIndex);
        return true;
    }
    public GameObject RemoveIngredient(int recipeIndex, int ingredientIndex)
    {
        if (!player)
            return null;
        Ingredient ingredient = ingredients[recipeIndex][ingredientIndex];
        ingredient.realWorldObject.transform.SetParent(player.hand.transform);
        ingredient.realWorldObject.transform.localPosition = Vector3.zero;
        ingredients[recipeIndex][ingredientIndex] = null;
        return ingredient.realWorldObject;
    }
    public void AddChef(PlayerController c)
    {
        player = c;
        ui.player = c;
        ui.UpdatePlayerInventory();
        ui.FillAllRecipes();
    }
}
