using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class CustomerInStore : Customer_StoreFront
{
    public string customerName;
    public Food[] wantedFood;
    public Ingredient[][] extraIngredients;
    public float willingToPayMore_Percent = 10;
    public bool inLine;
    public bool atRegister;
    public bool moving;
    public bool displayingOrder;
    public bool ticketCalled;
    public bool hasFood;
    public NavMeshAgent agent;
    public Transform hand;
    public Text uiText;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        customerName = NameGenerator.firstNames[firstNameIndex] + " " + NameGenerator.lastNames[lastNameIndex];
        uiText.text = customerName;
    }

    // Update is called once per frame
    void Update()
    {
        if (atRegister)
            moving = false;
        else if (agent.isStopped)
            moving = false;
        else
            moving = true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if the customer has a ticket in their inventory</returns>
    public bool hasTicket()
    {
        foreach(Transform c in hand)
        {
            if(c.GetComponent<Ticket>())
            {
                return true;
            }
        }
        return false;
    }
    public Ticket getTicket()
    {
        foreach (Transform c in hand)
        {
            if (c.GetComponent<Ticket>())
            {
                return c.GetComponent<Ticket>();
            }
        }
        return null;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>0 if the food in the ticket is 100 percent correct, -5 for every food item missing, and -1 for any missing
    /// ingredients or one to many ingredients</returns>
    public int hasCorrectFood()
    {
        int total = 0;
        Ticket ticket = null;
        if(hasTicket())
        {
            foreach(Transform c in hand)
            {
                if(c.GetComponent<Ticket>())
                {
                    ticket = c.GetComponent<Ticket>();
                }
                
            }
            foreach(Food wF in wantedFood)
            {
                if(!ticket.foodOrdered.Contains(wF))
                {
                    total -= 5;
                }
            }
            for(int i = 0; i < extraIngredients.Length; i++)
            {
                for (int j = 0; j < extraIngredients[i].Length; j++)
                {
                    if (!ticket.extraIngredients[i].Contains(extraIngredients[i][j]))
                    {
                        total -= 1;
                    }
                }
            }
        }

        return total;
    }

    public void GiveOrder()
    {
        displayingOrder = true;
        string order = "I would like a ";
        for(int i = 0; i < wantedFood.Length; i ++)
        {
            order += wantedFood[i].itemName;
            if(extraIngredients[i].Length > 0)
            {
                order += " with an extra ";
                for(int j = 0; j < extraIngredients[i].Length; j++)
                {
                    order += extraIngredients[i][j].itemName;
                    if(j != extraIngredients[i].Length - 1)
                    {
                        order += " and ";
                    }
                }
            }

            if(i != wantedFood.Length - 1)
            {
                order += " and  a ";
            }
        }
        uiText.text = order;

    }
    public int GetMoney(int amount)
    {
        if(amount > amountOfMoney)
        {
            return amountOfMoney;
        }
        else
        {
            amountOfMoney -= amount;
            return amount;
        }
    }
    public override void Randomize()
    {
        customerName = "Random Bitch Cunt";
        GameManager manager = GameObject.FindObjectOfType<GameManager>();
        //Filling up wantedFood with random assortment of food from ticketFood and ingredients from
        //extra ingredients
        int foodAmount = Random.Range(1, 2);
        wantedFood = new Food[foodAmount];
        extraIngredients = new Ingredient[foodAmount][];
        for(int i = 0; i < foodAmount; i++)
        {
            wantedFood[i] = manager.tickedFoods[Random.Range(0, manager.tickedFoods.Count - 1)].food;
            int extraAmount = Random.Range(0, 2);
            extraIngredients[i] = new Ingredient[extraAmount];
            for(int j = 0; j < extraAmount; j++)
            {
                extraIngredients[i][j] = manager.extraIngredients[Random.Range(0, manager.extraIngredients.Count - 1)].ingredient;
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Register"))
        {
            Debug.Log("Hitting register");
            if (!hasTicket())
            {
                Transform currentTransform = other.transform;
                RunRegister register = currentTransform.GetComponent<RunRegister>();
                if (!register)
                {
                    while (currentTransform.parent != null)
                    {
                        currentTransform = currentTransform.parent;
                        if (currentTransform.GetComponent<RunRegister>())
                        {
                            register = currentTransform.GetComponent<RunRegister>();
                            break;
                        }
                    }
                }
                if (register)
                {
                    register.customer = this;
                    //              RegisterUIController uIController = register.uIController;
                    atRegister = true;
                }
            }
        }
        if (other.CompareTag("Pickup"))
        {

            if (!hasTicket())
            {
                Debug.Log("Doesnt have ticket");
            }
            else
            {
                int ticketNumber = getTicket().number;
                Transform currentTransform = other.transform;
                FoodPickup pickup = currentTransform.GetComponent<FoodPickup>();
                if (!pickup)
                {
                    while (currentTransform.parent != null)
                    {
                        currentTransform = currentTransform.parent;
                        if (currentTransform.GetComponent<FoodPickup>())
                        {
                            pickup = currentTransform.GetComponent<FoodPickup>();
                            break;
                        }
                    }
                }
                if (pickup)
                {
                    foreach (Transform child in pickup.completedHandle)
                    {
                        if (child.GetComponent<Ticket>())
                        {
                            if (child.GetComponent<Ticket>().number == ticketNumber)
                            {
                                Destroy(getTicket().gameObject);
                                child.SetParent(hand);
                                child.localPosition = Vector3.zero;
                                hasFood = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        Debug.Log("Hit something");

        if (other.CompareTag("Register"))
        {
            Debug.Log("Hitting register");
            if(!hasTicket())
            {
                Transform currentTransform = other.transform;
                RunRegister register = currentTransform.GetComponent<RunRegister>();
                if (!register)
                {
                    while (currentTransform.parent != null)
                    {
                        currentTransform = currentTransform.parent;
                        if (currentTransform.GetComponent<RunRegister>())
                        {
                            register = currentTransform.GetComponent<RunRegister>();
                            break;
                        }
                    }
                }
                if (register)
                {
                    register.customer = this;
                    //              RegisterUIController uIController = register.uIController;
                    atRegister = true;
                }
            }
        }
        if(other.CompareTag("Pickup"))
        {
            Debug.Log("hit pickup");
            if(!hasTicket())
            {

            }
            else
            {
                int ticketNumber = getTicket().number;
                Transform currentTransform = other.transform;
                FoodPickup pickup = currentTransform.GetComponent<FoodPickup>();
                if (!pickup)
                {
                    while (currentTransform.parent != null)
                    {
                        currentTransform = currentTransform.parent;
                        if (currentTransform.GetComponent<RunRegister>())
                        {
                            pickup = currentTransform.GetComponent<FoodPickup>();
                            break;
                        }
                    }
                }
                if (pickup)
                {
                    foreach (Transform child in pickup.completedHandle)
                    {
                        if (child.GetComponent<Ticket>())
                        {
                            if (child.GetComponent<Ticket>().number == ticketNumber)
                            {
                                child.SetParent(hand);
                                child.localPosition = Vector3.zero;
                                hasFood = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Register"))
        {
            Transform currentTransform = other.transform;
            RunRegister register = currentTransform.GetComponent<RunRegister>();
            if (!register)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<RunRegister>())
                    {
                        register = currentTransform.GetComponent<RunRegister>();
                        break;
                    }
                }
            }
            if (register)
            {
                register.customer = null;
                atRegister = false;
            }
        }
    }

    public override void CopyFrom(Person p)
    {
        base.CopyFrom(p);
        if(p is CustomerInStore)
        {
            CustomerInStore c = p as CustomerInStore;
            customerName = c.customerName;
            wantedFood = c.wantedFood;
            extraIngredients = c.extraIngredients;
            willingToPayMore_Percent = c.willingToPayMore_Percent;
            inLine = c.inLine;
            atRegister = c.atRegister;
            moving = c.moving;
            displayingOrder = c.displayingOrder;
            ticketCalled = c.ticketCalled;
            hasFood = c.hasFood;
            agent = c.agent;
        }
    }
}
