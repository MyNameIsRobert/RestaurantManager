using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CustomerManager : MonoBehaviour
{
    public List<CustomerInStore> currentCustomers = new List<CustomerInStore>();
    public Vector3 doorPosition;
    public Vector3 registerPosition;
    [Tooltip("How many customers will pass by the shop (and possible go in)")]
    public int customers_StoreFront_Amount = 100;
    public int maxCustomers = 5;
    public List<Customer_StoreFront> storeFront_Customers;
    [Tooltip("All of them")]
    public List<Customer> allCustomers = new List<Customer>();
    [SerializeField]
    LevelManager.Node[] spacesInLine_Nodes;
    [SerializeField]
    LevelManager lManager;
    TimeManager timeManager;
    GameManager manager;
    public GameObject blankCustomer;
    [SerializeField]
    List<CustomerInStore> customersInLine = new List<CustomerInStore>();
    bool spawnerRunning = false;
    public GameObject customer;
    public GameObject customerSF;
    public Transform emptyObjectStorage;
    IEnumerator CustomerSpawner()
    {
        spawnerRunning = true;
        while(true)
        {
            float randomStep = Random.Range((timeManager.realTime_ToHour_Seconds / 8), timeManager.realTime_ToHour_Seconds / 2);
            //Select amout of customers to spawn and fill list with them
            int numOfStoreFront = storeFront_Customers.Count;
            int numToSpawn = Random.Range(1, customers_StoreFront_Amount / 50);
            Customer_StoreFront[] storeFront_Cs = new Customer_StoreFront[numToSpawn];

            for(int i = 0; i < numToSpawn; i++)
            {
                Customer_StoreFront cSF;
                int cIndex = 0;
                cIndex = Random.Range(0, numOfStoreFront - 1);
                cSF = storeFront_Customers[cIndex];

                while(cSF.opinionOfRestaurant < -50 && cSF.amountOfMoney <= manager.averageFoodItem_Cost)
                {
                    cIndex = Random.Range(0, numOfStoreFront - 1);
                    cSF = storeFront_Customers[cIndex];
                }
                storeFront_Cs[i] = cSF;

            }

            foreach(Customer_StoreFront c in storeFront_Cs)
            {
                if(currentCustomers.Count >= maxCustomers)
                    break;

                if(c)
                    SpawnCustomer(c);
            }
            yield return new WaitForSecondsRealtime(randomStep);
        }
    }
    IEnumerator StoreFrontController()
    {
        if (storeFront_Customers == null)
        {
            storeFront_Customers = new List<Customer_StoreFront>();
        }
        if (allCustomers == null)
        {
            allCustomers = new List<Customer>();
        }
        int randomAmount = Random.Range(100, 500);
        for (int i = 0; i < randomAmount; i++)
        {
            CreateNewCustomer();
        }
        while (true)
        {
            float randomStep = Random.Range((timeManager.realTime_ToHour_Seconds / 8), timeManager.realTime_ToHour_Seconds / 2);
            int randomSFAmount = Random.Range(1, 5);
            for (int i = 0; i < randomSFAmount; i++)
            { 
                if (Random.value > .75f)
                {
                    Customer c = CreateNewCustomer();
                    Customer_StoreFront cSF = Instantiate(customerSF.GetComponent<Customer_StoreFront>(), emptyObjectStorage);
                    cSF.CopyFrom(c);
                    cSF.Randomize();
                    storeFront_Customers.Add(cSF);
                }
                else
                {
                    Customer c = allCustomers[Random.Range(0, allCustomers.Count - 1)];
                    Customer_StoreFront cSF = Instantiate(customerSF.GetComponent<Customer_StoreFront>(), emptyObjectStorage);
                    cSF.CopyFrom(c);
                    cSF.Randomize();
                    storeFront_Customers.Add(cSF);
                }
            }


            if(storeFront_Customers.Count > 10 /*TODO: Change this soon*/)
            {
                while(storeFront_Customers.Count > 10)
                {
                    storeFront_Customers.RemoveAt(Random.Range(0, storeFront_Customers.Count - 1));
                }
            }
            if (!spawnerRunning)
                StartCoroutine(CustomerSpawner());
            yield return new WaitForSeconds(randomStep);
        }    

    }
    Customer CreateNewCustomer()
    {
        Customer c = Instantiate(customer.GetComponent<Customer>(), emptyObjectStorage);
        c.Randomize();
        allCustomers.Add(c);
        return c;
    }
    IEnumerator CustomerController(CustomerInStore customer)
    {
        if(!customer.hasTicket())
        {
            AddToLine(customer);
            customer.inLine = true;
        }
        yield return new WaitWhile(() => customer.inLine);
        LevelManager.GridNode node = lManager.waitingAreaPositions[0];
        Vector3 pos = node.position;
        customer.agent.SetDestination(pos);
        yield return new WaitUntil(() => customer.ticketCalled);
        node = lManager.pickupPositon[0];
        pos = node.position;
        customer.agent.SetDestination(pos);
        yield return new WaitUntil(() => customer.hasFood);
        if(customer.hasCorrectFood() > -5)
        {
            customer.currentMood = Enums.Mood.Happy;
            customer.opinionOfRestaurant += 10;
        }
        else
        {
            customer.currentMood = Enums.Mood.Upset;
            customer.currentMood += -10;
        }
        customer.agent.SetDestination(doorPosition);
        yield return new WaitForSecondsRealtime(3);
        manager.LeaveRestaurant(customer);
        currentCustomers.Remove(customer);
        Destroy(customer.gameObject);
        yield return null;
    }
    IEnumerator LineController()
    {
        while (customersInLine.Count > 0)
        {
            CustomerInStore current = customersInLine[0];

            //While the current customer does not have a ticket
            while (!current.hasTicket())
            {
                //If the customer is not at the register
                if (!current.atRegister)
                {
                    //If they're not moving
                    if (!current.moving)
                    {
                        //Move the customer to the register
                        current.GetComponent<NavMeshAgent>().SetDestination(registerPosition);
                    }

                }
                //If they are at the register, but have yet to currently display their order
                else if (!current.displayingOrder)
                {
                    //Tells the customer to give their order
                    current.GiveOrder();
                }
                yield return null;
            }
            customersInLine.Remove(current);
            current.inLine = false;
            yield return 0;
        }
    }

    void AddToLine(CustomerInStore cu)
    {
        if(customersInLine.Count < 1)
        {
            customersInLine.Add(cu);
            StartCoroutine(LineController());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        manager = transform.parent.GetComponent<GameManager>();
        lManager = transform.parent.GetComponentInChildren<LevelManager>();
        timeManager = transform.parent.GetComponentInChildren<TimeManager>();
        LevelManager.GridNode temp = lManager.customerRegisterPosition[0];

        registerPosition = temp.position;
        temp = lManager.doorPosition[0];
        doorPosition = temp.position;

        //spaceInLine_Node = lManager.customerRegisterPosition.GetGroundNode();
        StartCoroutine(StoreFrontController());

        //StartCoroutine(CustomerSpawner());

        //StartCoroutine(LineController());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCustomer()
    {
        GameObject customerObject = Instantiate(blankCustomer);
        CustomerInStore customer = customerObject.GetComponent<CustomerInStore>();
        customer.Randomize();
        currentCustomers.Add(customer);
        StartCoroutine(CustomerController(customer));
    }
    public void SpawnCustomer(Customer_StoreFront cSF)
    {
        GameObject customerObject = Instantiate(blankCustomer);
        CustomerInStore customer = new CustomerInStore();
        customer.CopyFrom(cSF);
        customer.Randomize();
        customerObject.GetComponent<CustomerInStore>().CopyFrom(customer);
        currentCustomers.Add(customerObject.GetComponent<CustomerInStore>());
        StartCoroutine(CustomerController(customerObject.GetComponent<CustomerInStore>()));
    }
}
