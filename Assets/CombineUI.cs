using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CombineUI : MonoBehaviour {

    Combine combine;
    public RectTransform[] recipeContainers = new RectTransform[3];
    public GameObject playerInventoryHolder;
    public Transform allRecipeHolder;
    public GameObject itemButton;
    GameManager manager;
    public PlayerController player;
    
    public Transform draggedItem_Transform;

    public Item draggedItem_Item;
    public int lastTicketIndex = -1;
    public int draggedItem_Index = -1;
    [SerializeField]
    Canvas canvas;

    private void Update()
    {
    }

    private void Start()
    {
        combine = GetComponentInParent<Combine>();
        canvas = GetComponent<Canvas>();
        manager = FindObjectOfType<GameManager>();
    }

    public void AddRecipe(int itemIndex, int recipeIndex)
    {
        Recipe recipe = manager.knownRecipes[itemIndex];
        combine.recipes[recipeIndex] = recipe;
        UpdateCurrentRecipes(recipeIndex);
        if(lastTicketIndex != -1)
        {
            UpdateCurrentRecipes(lastTicketIndex);
        }
    }
    public void RemoveRecipe(int recipeIndex)
    {
        combine.recipes[recipeIndex] = null;

    }
    public void AddIngredient(int playerIndex, int recipeIndex)
    {
        if (lastTicketIndex == -1)
        {
            Ingredient food = player.RemoveItemFromHand(playerIndex).GetComponent<Ingredient>();
            combine.AddIngredient(food, recipeIndex);
            UpdateCurrentRecipes(recipeIndex);
            UpdatePlayerInventory();
        }
        else
        {
            Ingredient ingredient = combine.ingredients[lastTicketIndex][playerIndex];
            GameObject go = combine.RemoveIngredient(lastTicketIndex, playerIndex);
            combine.AddIngredient(go.GetComponent<Ingredient>(), recipeIndex);
        }

    }

    public void CraftFood(int ticketIndex)
    {
        combine.UseRecipe(ticketIndex);
    }
    IEnumerator DelayedUpdate(float sec, int ticketIndex)
    {
        yield return new WaitForSeconds(sec);
        UpdatePlayerInventory();
        if (ticketIndex != -1)
            UpdateCurrentRecipes(ticketIndex);
    }
    public void UpdatePlayerInventory()
    {
        ClearTransformChildren(playerInventoryHolder.transform);
        int i = 0, buttonNum = 0;
        foreach (Transform child in player.hand.transform)
        {

            Debug.Log("Spawning button");
            Item item = child.GetComponent<Item>();
            if (item is Ingredient)
            {
                GameObject go = Instantiate(itemButton, playerInventoryHolder.transform);
                DragMe_Combine drag = go.GetComponent<DragMe_Combine>();
                if (drag)
                {
                    drag.SetIndexes(i, -1);
                }
                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
                go.GetComponent<Image>().sprite = item.itemIcon;
                buttonNum++;
            }
            i++;
        }

    }
    public void UpdateCurrentRecipes(int ticketIndex)
    {
        ClearTransformChildren(recipeContainers[ticketIndex].GetChild(0).transform);
        if (combine.ingredients[ticketIndex] == null)
            combine.ingredients[ticketIndex] = new Dictionary<int, Ingredient>();
        for (int i = 0; i < combine.ingredients[ticketIndex].Count; i++)
        {
            Ingredient item;
            if (combine.ingredients[ticketIndex].TryGetValue(i, out item))
            {
                if (item)
                {
                    GameObject go = Instantiate(itemButton, recipeContainers[ticketIndex].GetChild(0).transform);
                    DragMe_Combine drag = go.GetComponent<DragMe_Combine>();
                    if (drag)
                    {
                        drag.SetIndexes(i, ticketIndex);
                    }
                    go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
                    go.GetComponent<Image>().sprite = item.itemIcon;
                }
            }
        }

        if (combine.recipes[ticketIndex])
        {
            TextMeshProUGUI text = recipeContainers[ticketIndex].GetChild(1).GetComponent<TextMeshProUGUI>();
            text.text = combine.recipes[ticketIndex].itemName;
            Button button = recipeContainers[ticketIndex].GetChild(1).GetComponent<Button>();
            int i = ticketIndex;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => CraftRecipe(ticketIndex));
        }

    }

    public void AddFoodToWarmer(int playerIndex)
    {

    }
    public void AddFoodToWarmer(int ticketIndex, int foodIndex)
    {

    }

    public void PickUpItem(int playerIndex)
    {
        Debug.Log("Picked up item");
        
        draggedItem_Item = player.hand.transform.GetChild(playerIndex).GetComponent<Item>();
        draggedItem_Index = playerIndex;
        lastTicketIndex = -1;
    }
    public void PickUpRecipe(int recipeIndex)
    {
        draggedItem_Item = manager.knownRecipes[recipeIndex] as Item;
        draggedItem_Index = recipeIndex;
        lastTicketIndex = -1;
    }
    public void PickUpItem(int ticketIndex, int foodIndex)
    {
        draggedItem_Item = combine.ingredients[ticketIndex][foodIndex] as Item;
        draggedItem_Index = foodIndex;
        lastTicketIndex = ticketIndex;
    }
    public void PutDownItem(GameObject iconObject)
    {
        RectTransform data = iconObject.GetComponent<RectTransform>();
        if (WithinArea(data.anchoredPosition, recipeContainers[0]))
        {
            if (draggedItem_Item is Ingredient)
                AddIngredient(draggedItem_Index, 0);
            else if (draggedItem_Item is Recipe && lastTicketIndex == -1)
                AddRecipe(draggedItem_Index, 0);
            else if (lastTicketIndex == 0)
                CraftFood(lastTicketIndex);
            else
                AddRecipe(draggedItem_Index, 0);
        }
        else if (WithinArea(data.anchoredPosition, recipeContainers[1]))
        {;

            if (draggedItem_Item is Ingredient)
                AddIngredient(draggedItem_Index, 1);
            else if (draggedItem_Item is Recipe && lastTicketIndex == -1)
                AddRecipe(draggedItem_Index, 1);
            else if (lastTicketIndex == 1)
                CraftFood(lastTicketIndex);
            else
                AddRecipe(draggedItem_Index, 1);
        }
        else if (WithinArea(data.anchoredPosition, recipeContainers[2]))
        {
            if (draggedItem_Item is Ingredient)
                AddIngredient(draggedItem_Index, 2);
            else if (draggedItem_Item is Recipe && lastTicketIndex == -1)
                AddRecipe(draggedItem_Index, 2);
            else if (lastTicketIndex == 2)
                CraftFood(lastTicketIndex);
            else
                AddRecipe(draggedItem_Index, 2);
        }
        else if (WithinArea(data.anchoredPosition, playerInventoryHolder.transform as RectTransform))
        {

            Debug.Log("Player inventory");
            if (lastTicketIndex != -1)
            {
                if (draggedItem_Item is Ingredient)
                {
                    GameObject go = combine.RemoveIngredient(lastTicketIndex, draggedItem_Index);
                    go.transform.SetParent(player.hand.transform);
                    go.transform.position = Vector3.zero;
                    UpdateCurrentRecipes(lastTicketIndex);
                    UpdatePlayerInventory();
                }
                else
                {
                    RemoveRecipe(lastTicketIndex);
                }
            }
        }
        else
        {
            if (lastTicketIndex != -1)
            {
                UpdateCurrentRecipes(lastTicketIndex);
            }
            else
            {
                UpdatePlayerInventory();
            }
        }

        draggedItem_Transform = null;
        draggedItem_Index = -1;
        lastTicketIndex = -1;
    }

    bool WithinArea(Vector2 butt, RectTransform area)
    {
        Vector2 center = area.localPosition;
        float xMax, xMin, yMax, yMin;
        xMax = center.x + area.sizeDelta.x;
        xMin = center.x - area.sizeDelta.x;
        yMax = center.y + area.sizeDelta.y;
        yMin = center.y - area.sizeDelta.y;
        if (butt.x <= xMax && butt.x >= xMin
              && butt.y <= yMax && butt.y >= yMin)
            return true;
        if (butt.x <= xMax && butt.x >= xMin)
            Debug.Log("X is Good for" + area.name);
        if (butt.y <= yMax && butt.y >= yMin)
            Debug.Log("Y is Good for" + area.name);
        Debug.Log(butt + " is not within " + xMax + "-" + xMin + ", " + yMax + "-" + yMin);
        return false;
    }
    void ClearTransformChildren(Transform t)
    {
        foreach (Transform child in t)
        {
            Debug.Log("Destroying " + child);
            Destroy(child.gameObject);
        }
    }
    public void CraftRecipe(int recipeIndex)
    {
        combine.UseRecipe(recipeIndex);
    }
    public void FillAllRecipes()
    {

        foreach (Transform child in allRecipeHolder)
        {
            Button button = child.GetComponent<Button>();
            if (button)
                button.onClick.RemoveAllListeners();
            Destroy(child.gameObject);
        }

        for (int i = 0; i < manager.knownRecipes.Count; i++)
        {
            Debug.Log("Added recipe");

            GameObject go = Instantiate(itemButton, allRecipeHolder);
            Item item = manager.knownRecipes[i] as Item;
            go.GetComponent<Image>().sprite = item.itemIcon;
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
            int index = i;
            DragMe_Combine drag = go.GetComponent<DragMe_Combine>();
            drag.SetIndexes(index, -1);
            drag.isRecipe = true;
            Button button = go.GetComponent<Button>();
        }
    }
}
