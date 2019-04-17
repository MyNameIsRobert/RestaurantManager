using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticket : Item {

    public int number = -1;
    public List<Food> foodOrdered = new List<Food>();
    public List<List<Ingredient>> extraIngredients = new List<List<Ingredient>>();
    public List<int> foodCost = new List<int>();
    public int totalCost = 0;

    public List<GameObject> actualFoodWithTicket = new List<GameObject>();
	// Use this for initialization
	void Start () {
		realWorldObject = (GameObject)Resources.Load("Paper Bag");
	}
	
    public Ticket()
    {
        number = 0;
        totalCost = 0;
        itemName = "Blank Ticket";
        foodOrdered = new List<Food>();
        extraIngredients = new List<List<Ingredient>>();
        foodCost = new List<int>();
    }
    public void CopyFrom(Ticket t)
    {
        totalCost = t.totalCost;
        number = t.number;
        foodOrdered = t.foodOrdered;
        extraIngredients = t.extraIngredients;
        foodCost = t.foodCost;
        actualFoodWithTicket = t.actualFoodWithTicket;
        itemName = t.itemName;
    }

    private void Awake()
    {
        realWorldObject = (GameObject)Resources.Load("Paper Bag");
    }
    public override string ToString()
    {
        string toReturn = "Current Order:";
        int count = 0;
        foreach(Food f in foodOrdered)
        {
            toReturn += "\n  " + f.itemName;
            //if(extraIngredients != null && extraIngredients[count] != null && extraIngredients[count].Count > 0)
            //{
            //    toReturn += "\nWith extra:";
            
            //    foreach(Ingredient i in extraIngredients[count])
            //    {
            //        toReturn += "\n     -" + i.itemName;
            //    }
            //}
            count++;
        }
        return toReturn;
    }
}
