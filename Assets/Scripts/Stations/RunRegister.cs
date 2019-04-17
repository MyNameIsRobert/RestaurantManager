using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunRegister : Station {

#region Variables
   // public GameObject uiContentHolster;
    public Ticket newTicket = null;
    public bool ticketConfirmed = false;
    bool coroutineRunning = false;
    public GameManager manager;
    FinanceManager fManager;
    public int selectedFood = -1;
    public int selectedIngredient = 1;
    [SerializeField]
    int currentFoodCount = 0;
    bool donewithIngredients = false;
    public RegisterUIController uIController;
    bool createNewTicket = false;
    public GameObject blankTicket;
    public CustomerInStore customer;
    public int ticketNumber = 1;
    public float foodAddTime = 1f, ingredientAddTime = .75f;

    #endregion

    IEnumerator PickFood()
    {
        yield return new WaitUntil(() => (selectedFood != -1 || ticketConfirmed));
        if(!ticketConfirmed)
        {
            newTicket.foodOrdered.Add(manager.tickedFoods[selectedFood].food);
            newTicket.totalCost += manager.tickedFoods[selectedFood].salePrice;
            newTicket.foodCost.Add(manager.tickedFoods[selectedFood].salePrice);
            uIController.SetUIStage(RegisterUIController.UIStage.ClearedFood);
            uIController.SetUIStage(RegisterUIController.UIStage.AddIngredients);
            newTicket.extraIngredients.Add(new List<Ingredient>());

            selectedFood = -1;
            StartCoroutine(PickExtraIngredients());
        }
        else
        {
            ticketConfirmed = false;
            if(newTicket && newTicket.foodOrdered != null && newTicket.foodOrdered.Count > 0)
            {
                currentFoodCount = 0;
                GameObject go = newTicket.gameObject;
                go.GetComponent<Ticket>().CopyFrom(newTicket);
                go.GetComponent<Ticket>().itemName = "Ticket " + newTicket.number;
                player.AddItemToHand(go);
                if(customer)
                {
                    GameObject custGo = Instantiate(go, customer.hand);
                    fManager.AddPurchase(customer.GetMoney(newTicket.totalCost), newTicket.foodOrdered.Count);
                }
                newTicket = null;
                ticketConfirmed = false;
                ticketNumber++;
            }
            else
            {
                newTicket = null;
                currentFoodCount = 0;
                selectedFood = -1;
                selectedIngredient = -1;
            }

            uIController.SetUIStage(RegisterUIController.UIStage.ClearedFood);
            uIController.SetUIStage(RegisterUIController.UIStage.NoTicketYet);
        }
    }
    IEnumerator PickExtraIngredients()
    {
        yield return new WaitUntil(() => (selectedIngredient != -1 || donewithIngredients));
        if(!donewithIngredients)
        {
            if (currentFoodCount > newTicket.extraIngredients.Count)
                newTicket.extraIngredients.Insert(currentFoodCount, new List<Ingredient>());
            newTicket.extraIngredients[currentFoodCount].Add(manager.extraIngredients[selectedIngredient].ingredient);
            newTicket.totalCost += manager.extraIngredients[selectedIngredient].addPrice;
            newTicket.foodCost[currentFoodCount] += manager.extraIngredients[selectedIngredient].addPrice;
            selectedIngredient = -1;
            uIController.SetUIStage(RegisterUIController.UIStage.ClearedFood);
            uIController.SetUIStage(RegisterUIController.UIStage.AddIngredients);
            StartCoroutine(PickExtraIngredients());
        }
        else
        {
            donewithIngredients = false;
            selectedFood = -1;
            currentFoodCount++;
            uIController.SetUIStage(RegisterUIController.UIStage.ClearedFood);
            uIController.SetUIStage(RegisterUIController.UIStage.AddFood);
            StartCoroutine(PickFood());
        }
    }
    private void Update()
    {
    }
    public void CreateNewTicket()
    {
        if (newTicket == null)
        {
            uIController.SetUIStage(RegisterUIController.UIStage.AddFood);
            GameObject go = Instantiate(blankTicket, transform);
            go.transform.localPosition = Vector3.zero;
            newTicket = go.GetComponent<Ticket>();
            newTicket.realWorldObject = newTicket.gameObject;
            newTicket.number = ticketNumber;
            createNewTicket = false;
            currentFoodCount = 0;
            StartCoroutine(PickFood());
        }
    }
    public void ConfirmTicket()
    {
        ticketConfirmed = true;
    }
    public void DoneWithIngredients()
    {
        donewithIngredients = true;
    }
    public void SetSelectedFood(int i)
    {
        Debug.Log("Selected food has been set to " + i);
        selectedFood = i;   
    }
    public void SetSelectedIngredient(int i)
    {
        selectedIngredient = i;
    }
    private void Start()
    {
        fManager = manager.gameObject.GetComponentInChildren<FinanceManager>();
        onComplete = new OnComplete(EmptyFunction);
    }
    public void StepAway()
    {
        
    }
    public void EmptyFunction()
    {

    }
    public void EmployeeCreateTicket()
    {
        Debug.Log("Employee Creating Ticket");
        if (!currentEmployee || !customer)
            return;
        StartCoroutine(EmployeeTicket());
    }
    public void EmployeeCreateTicket(Action createOverride)
    {
        if (!currentEmployee || !customer)
            return;
        onComplete = new OnComplete(createOverride);
        StartCoroutine(EmployeeTicket());
    }
    IEnumerator EmployeeTicket()
    {
        newTicket = null;
        GameObject go = Instantiate(blankTicket, transform);
        go.transform.localPosition = Vector3.zero;
        newTicket = go.GetComponent<Ticket>();
        newTicket.realWorldObject = newTicket.gameObject;
        newTicket.number = ticketNumber;
        newTicket.extraIngredients = new List<List<Ingredient>>();
        //Adding food and ingredients to ticket at a rate of 1 food per (foodAddTime * employeeWorkSpeed) seconds
        for (int i = 0; i < customer.wantedFood.Length; i++)
        {
            Food wanted = customer.wantedFood[i];
            int foodIndex = -1;
            for(int j = 0; j < manager.tickedFoods.Count; j++)
            {
                if(wanted.itemName == manager.tickedFoods[j].food.itemName)
                {
                    foodIndex = j;
                    break;
                }
            }
            if (foodIndex > -1)
            {
                newTicket.foodOrdered.Add(manager.tickedFoods[foodIndex].food);
                newTicket.totalCost += manager.tickedFoods[foodIndex].salePrice;
                if(newTicket.foodCost == null)
                {
                    newTicket.foodCost = new List<int>();
                    newTicket.foodCost.Add(manager.tickedFoods[foodIndex].salePrice);
                }
            }
            if (customer.extraIngredients[i] != null)
            {
                newTicket.extraIngredients.Add(new List<Ingredient>());
                for (int j = 0; j < customer.extraIngredients[i].Length; j++)
                {
                    Ingredient wantedI = customer.extraIngredients[i][j];
                    int ingIndex = -1;
                    for (int k = 0; k < manager.tickedFoods.Count; k++)
                    {
                        if (wantedI.itemName == manager.extraIngredients[j].ingredient.itemName)
                        {
                            ingIndex = j;
                            break;
                        }
                    }
                    if (ingIndex > -1)
                    {
                        newTicket.extraIngredients[i].Add(manager.extraIngredients[ingIndex].ingredient);
                        newTicket.totalCost += manager.extraIngredients[ingIndex].addPrice;
                        newTicket.foodCost[i] += manager.extraIngredients[ingIndex].addPrice;
                    }
                    yield return new WaitForSeconds(ingredientAddTime * currentEmployee.skills.workSpeed);
                }
            }

            yield return new WaitForSeconds(foodAddTime * currentEmployee.skills.workSpeed);
        }
        yield return null;
        currentFoodCount = 0;
        GameObject go1 = newTicket.gameObject;
        go1.GetComponent<Ticket>().CopyFrom(newTicket);
        go1.GetComponent<Ticket>().itemName = "Ticket " + newTicket.number;
        currentEmployee.AddItemToHand(go);
        if (customer)
        {
            GameObject custGo = Instantiate(go, customer.hand);
            fManager.AddPurchase(customer.GetMoney(newTicket.totalCost), newTicket.foodOrdered.Count);
        }
        newTicket = null;
        ticketConfirmed = false;
        ticketNumber++;
        onComplete();
        onComplete = new OnComplete(EmptyFunction);
    }
}
