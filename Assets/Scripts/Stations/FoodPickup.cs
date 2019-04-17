using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : Station
{

    public Ticket[] currentTickets = new Ticket[3];
    public Transform ticketHandle;
    public Transform foodHandle;
    public Transform completedHandle;
    public Dictionary<int, Food>[] foodToBeAdded = new Dictionary<int, Food>[3];
    public GameManager manager;
    PickupUIController uiController;
    // Start is called before the first frame update
    void Start()
    {
        uiController = GetComponentInChildren<PickupUIController>();
        manager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CombineTicket(int ticketIndex)
    {
        GameObject tempGameObject;
        Ticket currentTicket = currentTickets[ticketIndex];
        tempGameObject = currentTicket.gameObject;
        int count = 0;
        if(foodToBeAdded[ticketIndex] == null)
        { 
            Debug.Log("Food dictionary for that index is null!");
            return;
        }
        Dictionary<int, Food> food = foodToBeAdded[ticketIndex];

        foreach (Food f in food.Values)
        {
            currentTicket.actualFoodWithTicket.Add(f.realWorldObject);
            f.realWorldObject.transform.SetParent(currentTicket.transform);
            f.realWorldObject.transform.localPosition = Vector3.zero;
            currentTicket.extraIngredients.Add(new List<Ingredient>());
            foreach (Ingredient i in f.extraIngredients)
            {
                currentTicket.extraIngredients[count].Add(i);
            }
            count++;
        }

        tempGameObject.transform.SetParent(completedHandle.transform);
        tempGameObject.transform.localPosition = Vector3.zero;
        manager.CallTicketNumber(currentTicket.number);
        foodToBeAdded[ticketIndex].Clear();
        currentTickets[ticketIndex] = null;
        uiController.UpdateTicketFoods(ticketIndex);

    }

    public void AddFood(Food food, int ticketIndex)
    {
        
        if(foodToBeAdded[ticketIndex] == null)
        {
            foodToBeAdded[ticketIndex] = new Dictionary<int, Food>();
        }
        int addIndex = 0;
        for(int i = 0; i < currentTickets[ticketIndex].foodOrdered.Count; i++)
        {
            Food f = currentTickets[ticketIndex].foodOrdered[i];
            if(f.itemName == food.itemName)
            {
                addIndex = i;
                break;
            }
        }
        foodToBeAdded[ticketIndex].Add(addIndex, food);


        food.realWorldObject.transform.SetParent(foodHandle);
        Vector3 pos = new Vector3(ticketIndex, 0, 0);
        food.realWorldObject.transform.localPosition = pos;
    }

    public GameObject RemoveFood(int ticketIndex, int foodIndex)
    {
        Food f;
        foodToBeAdded[ticketIndex].TryGetValue(foodIndex, out f);
        GameObject g = f.realWorldObject;
        foodToBeAdded[ticketIndex].Remove(foodIndex);
        return g;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns>The gameObject of the ticket that was removed. Null if there is no ticket</returns>
    public GameObject AddTicket(Ticket ticket, int i)
    {
        if(currentTickets[i] == null)
        {
            GameObject go = ticket.gameObject;
            currentTickets[i] = ticket;
            currentTickets[i].realWorldObject = go;
            if (!go.GetComponent<Ticket>())
                go.AddComponent<Ticket>();
            go.GetComponent<Ticket>().CopyFrom(currentTickets[i]);
            currentTickets[i] = go.GetComponent<Ticket>();
            go.transform.SetParent(ticketHandle);
            go.transform.localPosition = Vector3.zero;
            uiController.UpdateTicketFoods(i);
            return null;
        }
        else
        {
            GameObject g = currentTickets[i].realWorldObject;
            currentTickets[i] = ticket;
            currentTickets[i].realWorldObject.transform.SetParent(ticketHandle);
            currentTickets[i].realWorldObject.transform.localPosition = Vector3.zero;
            uiController.UpdateTicketFoods(i);
            return g;
        }
        
    }
    public void AddTicket(Ticket ticket)
    {

        onComplete();
    }
    public void EmployeeAddAllTickets()
    {
        Ticket ticket1 = null, ticket2 = null, ticket3 = null;

        Debug.Log(currentEmployee);
        Debug.Log(currentEmployee.hand);
        foreach(Transform child in currentEmployee.hand)
        {
            if(child.GetComponent<Item>() is Ticket)
            {
                if(ticket1 == null)
                {
                    ticket1 = child.GetComponent<Ticket>();
                }
                else if(ticket2 == null)
                {
                    ticket2 = child.GetComponent<Ticket>();
                }
                else if(ticket3 == null)
                {
                    ticket3 = child.GetComponent<Ticket>();
                }
            }
        }
        if(currentTickets[0] == null && ticket1 != null)
        {
            AddTicket(ticket1, 0);
        }
        if (currentTickets[1] == null && ticket2 != null)
        {
            AddTicket(ticket2, 1);
        }
        if (currentTickets[2] == null && ticket3 != null)
        {
            AddTicket(ticket3, 2);
        }
        onComplete();
    }
    public void AddChef(PlayerController c)
    {
        uiController.player = c;
        uiController.UpdatePlayerInventory();
    }
}
