using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [System.Serializable]
    public struct ticketFood
    {
        public Food food;
        public int salePrice;
    }
    [System.Serializable]
    public struct TicketIngredient
    {
        public Ingredient ingredient;
        public int addPrice;
    }
    public List<ticketFood> tickedFoods = new List<ticketFood>();
    public List<Recipe> knownRecipes = new List<Recipe>();
    public List<TicketIngredient> extraIngredients = new List<TicketIngredient>();
    public int averageOpinion = 0;
    int totalOpionion = 0, numOfOpinions;
    LevelManager lManager;
    CustomerManager cManager;

    [HideInInspector]
    public int averageFoodItem_Cost = 20;
	// Use this for initialization
	void Start () {
        lManager = GetComponentInChildren<LevelManager>();
        cManager = GetComponentInChildren<CustomerManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CallTicketNumber(int number)

    {
        Debug.Log("Ticket number " + number + " was called");
        Debug.Log(cManager.currentCustomers.Count);
        foreach (CustomerInStore c in cManager.currentCustomers)
        {
            Ticket ticket = null;
            if (c.hasTicket())
            {
                Debug.Log(c.name + " has a ticket");
                ticket = c.getTicket();
                Debug.Log("Does " + number + " = " + ticket.number + "?");
                if (ticket.number == number)
                {
                    c.ticketCalled = true;
                }
            }
        }
    }

    public void LeaveRestaurant(CustomerInStore customer)
    {

    }

}
