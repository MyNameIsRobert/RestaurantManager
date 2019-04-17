using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grill : Station {

    public GameObject stoveTop;
    UIManager uiManager;
    public List<Ingredient[]> extraIngredientStorages = new List<Ingredient[]>();

    public int grillTemperature = 400;

    //[HideInInspector]
    public bool chefConnected = false;
    public delegate void OnSideCooked();
    public List<OnSideCooked> sidesCooked = new List<OnSideCooked>();
    [System.Serializable]
    public class ExtraIngredientSlot
    {
        public GameObject realWorldObject;
        public int numOfSpace = 30;
        public List<Ingredient> heldIngredients;

        ExtraIngredientSlot()
        {
            numOfSpace = 30;
            heldIngredients = new List<Ingredient>();
        }

        public bool AddIngredient(Ingredient ingredient)
        {
            if(ingredient.itemSize + CalculateSpace() > numOfSpace)
            {
                return false;
            }
            else
            {
                heldIngredients.Add(ingredient);
                return true;
            }
        }
        public int CalculateSpace()
        {
            int size = 0;
            foreach(Ingredient i in heldIngredients)
            {
                size += i.itemSize;
            }
            return size;
        }
    }
    public ExtraIngredientSlot[] extraSlots;

    //Where on the grill ingredients can be placed.
    [Tooltip("NOTE: This is how you change how many ingredients can be cooked at once. It's always the same amount as the number of ingredient slots")]
    public GameObject[] ingredientSlots;
    public Ingredient[] ingredients;
    public bool[] firstSide;
    public Text[] timerText;
    Coroutine[] timerRoutines;
    StoveUIController uiController;

    IEnumerator StartTimer(int index)
    {
        Text text = timerText[index];
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;

            text.text = "" + (int)timer;
            yield return null;
        }
    }
    void Empty()
    {

    }
    private void Start()
    {
        foreach(Ingredient i in ingredients)
        {
            sidesCooked.Add(new OnSideCooked(Empty));
        }
        int count = 0;
        timerText = new Text[ingredientSlots.Length];
        timerRoutines = new Coroutine[ingredientSlots.Length];
        foreach(GameObject g in ingredientSlots)
        {
            if(g.GetComponentInChildren<Text>())
            {
                timerText[count] = g.GetComponentInChildren<Text>();
            }
            count++;
        }
        uiController = GetComponentInChildren<StoveUIController>();
        uiManager = FindObjectOfType<UIManager>();
    }
    // Update is called once per frame
    void Update ()
    {
        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            Ingredient currentIngredient;
            currentIngredient = ingredients[i];
            if (currentIngredient)
            {

                float heatToAdd = 0;
                //This effectively raises the temperature of the food at 1 degree per 100 degrees per second
                heatToAdd = (grillTemperature / 100) * Time.deltaTime;
                if (firstSide[i])
                {
                    currentIngredient.SideOne_AddHeat(heatToAdd);
                }
                else
                {
                    currentIngredient.SideTwo_AddHeat(heatToAdd);
                }
                //While the ingredient is on the grill, it calculates the taste for the ingredient,
                //once the ingredient leaves the grill, the taste stops calculating, so pulling off the
                //ingredient at the proper time is a good way to maximise taste
                currentIngredient.CalculateTaste();
            }
            else
            {
                timerText[i].gameObject.SetActive(false);
            }
        }
	}
    private void Awake()
    {
    }

    public void AddIngredient(Ingredient ingredient, int index)
    {
        if (ingredients[index] != null)
        {
            Debug.Log("Ingredient already cooking!");

            return;
        }
        //slot is the real world position
        GameObject slot = ingredientSlots[index];
        ingredient.realWorldObject.transform.SetParent(slot.transform);
        ingredient.realWorldObject.transform.localPosition = new Vector3(0, .2f, 0);
        ingredients[index] = ingredient;
        firstSide[index] = true;
        timerText[index].gameObject.SetActive(true);
        timerRoutines[index] = StartCoroutine(StartTimer(index));
        uiController.AddIngredient(ingredient, index);
        
    }
    public bool AddIngredient(Ingredient ingredient, Action onCooked)
    {
        int index = -1, count = 0;
        foreach(Ingredient i in ingredients)
        {
            if(i == null)
            {
                index = count;
                break;
            }
            count++;
        }
        if (index == -1)
            return false;
        GameObject slot = ingredientSlots[index];
        ingredient.realWorldObject.transform.SetParent(slot.transform);
        ingredient.realWorldObject.transform.localPosition = new Vector3(0, .2f, 0);
        ingredients[index] = ingredient;
        firstSide[index] = true;
        timerText[index].gameObject.SetActive(true);
        timerRoutines[index] = StartCoroutine(StartTimer(index));
        uiController.AddIngredient(ingredient, index);
        if(onCooked != null)
            sidesCooked[index] = new OnSideCooked(onCooked);

        return true;
    }
    //Removes the ingredient at index from the grill and returns the removed ingredient
    public Ingredient RemoveIngredient(int index)
    {
        Ingredient finishedIngredient = ingredients[index];
        //Removes the realworld object from the parenting of the grill
        finishedIngredient.realWorldObject.transform.SetParent(null);
        //Removes the ingredient from the slot
        ingredients[index] = null;
        StopCoroutine(timerRoutines[index]);
        timerText[index].gameObject.SetActive(false);
        return finishedIngredient;
    }
    public void FlipIngredient(int indexOfIngredient)
    {
        firstSide[indexOfIngredient] = !firstSide[indexOfIngredient];
        StopCoroutine(timerRoutines[indexOfIngredient]);
        timerRoutines[indexOfIngredient] = StartCoroutine(StartTimer(indexOfIngredient));
        //TODO: Play animation
    }
    public void StepToGrill()
    {
        chefConnected = true;
    }
    public void LeaveGrill()
    {
        chefConnected = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Something hit");
        if(other.CompareTag("Player"))
        {
            uiManager.GrillUI(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {

        Debug.Log("Something hit");
        if (other.CompareTag("Player"))
        {
            uiManager.GrillUI(false);
        }
    }
}
