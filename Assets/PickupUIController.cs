using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickupUIController : MonoBehaviour
{
    FoodPickup pickup;
    public RectTransform[] ticketContainers = new RectTransform[3];
    public GameObject playerInventoryHolder;
    public GameObject itemButton;
    public PlayerController player;

    public Transform draggedItem_Transform;
    public Item draggedItem_Item;
    public int lastTicketIndex = -1;
    public int draggedItem_Index = -1;
    [SerializeField]
    Canvas canvas;

    private void Update()
    {
        if(draggedItem_Transform)
        {
            Vector2 mousePos = player.cam.ScreenToWorldPoint(Input.mousePosition);
            //Vector3 pos;
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform,
            //                mousePos, chef.cam, out pos);
            draggedItem_Transform.position = mousePos;
        }
    }

    private void Start()
    {
        pickup = transform.parent.GetComponent<FoodPickup>();
        canvas = GetComponent<Canvas>();
    }

    public void AddTicket(int playerIndex, int ticketIndex)
    {
        Ticket ticket = player.RemoveItemFromHand(playerIndex).GetComponent<Ticket>();
        GameObject go = pickup.AddTicket(ticket, ticketIndex);
        if (go)
        {
            go.transform.SetParent(player.hand.transform);
            go.transform.localPosition = Vector3.zero;
        }
        StartCoroutine(DelayedUpdate(.08f, ticketIndex));
        
    }
    public void RemoveTicket(int ticketIndex)
    {
        Ticket ticket = pickup.currentTickets[ticketIndex];
        ticket.realWorldObject.transform.SetParent(player.hand.transform);
        ticket.realWorldObject.transform.localPosition = Vector3.zero;
        pickup.currentTickets[ticketIndex] = null;
        StartCoroutine(DelayedUpdate(.08f, ticketIndex));

    }
    public void AddFood(int playerIndex, int ticketIndex)
    {
        Food food = player.RemoveItemFromHand(playerIndex).GetComponent<Food>();
        pickup.AddFood(food, ticketIndex);
        StartCoroutine(DelayedUpdate(.08f, ticketIndex));

    }

    public void CraftTicket(int ticketIndex)
    {
        pickup.CombineTicket(ticketIndex);
    }
    IEnumerator DelayedUpdate(float sec, int ticketIndex)
    {
        yield return new WaitForSeconds(sec);
        UpdatePlayerInventory();
        if (ticketIndex != -1)
            UpdateTicketFoods(ticketIndex);
    }
    public void UpdatePlayerInventory()
    {
        ClearTransformChildren(playerInventoryHolder.transform);
        int i = 0, buttonNum = 0;
        foreach (Transform child in player.hand.transform)
        {

            Debug.Log("Spawning button");
            Item item = child.GetComponent<Item>();
            if (item is Ticket || item is Food)
            {
                GameObject go = Instantiate(itemButton, playerInventoryHolder.transform);
                DragMe drag = go.GetComponent<DragMe>();
                if(drag)
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
    public void UpdateTicketFoods(int ticketIndex)
    {
        ClearTransformChildren(ticketContainers[ticketIndex].GetChild(0).transform);
        if (pickup.foodToBeAdded[ticketIndex] == null)
            pickup.foodToBeAdded[ticketIndex] = new Dictionary<int, Food>();
        for(int i = 0; i < pickup.foodToBeAdded[ticketIndex].Count; i++)
        {
            GameObject go = Instantiate(itemButton, ticketContainers[ticketIndex].GetChild(0).transform);
            Item item = pickup.foodToBeAdded[ticketIndex][i];
            DragMe drag = go.GetComponent<DragMe>();
            
            if(drag)
            {
                drag.SetIndexes(i, ticketIndex);
            }
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
            go.GetComponent<Image>().sprite = item.itemIcon;
        }

        if (pickup.currentTickets[ticketIndex])
        {
            TextMeshProUGUI text = ticketContainers[ticketIndex].GetChild(1).GetComponent<TextMeshProUGUI>();
            text.text = pickup.currentTickets[ticketIndex].itemName;
            Button b = ticketContainers[ticketIndex].GetChild(1).GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            int i = ticketIndex;
            b.onClick.AddListener(() => CraftTicket(i));
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
    public void PickUpItem(int ticketIndex, int foodIndex)
    {
        draggedItem_Item = pickup.foodToBeAdded[ticketIndex][foodIndex] as Item;
        draggedItem_Index = foodIndex;
        lastTicketIndex = ticketIndex;
    }
    public void PutDownItem(GameObject iconObject)
    {
        RectTransform data = iconObject.GetComponent<RectTransform>();
        Debug.Log("Put down item!");
        if (WithinArea(data.anchoredPosition, ticketContainers[0]))
        {

            Debug.Log("Ticket 1");
            if (draggedItem_Item is Food)
                AddFood(draggedItem_Index, 0);
            else if (draggedItem_Item is Ticket && lastTicketIndex == -1)
                AddTicket(draggedItem_Index, 0);
            else if (lastTicketIndex > -1)
                CraftTicket(lastTicketIndex);
        }
        else if (WithinArea(data.anchoredPosition, ticketContainers[1]))
        {
            Debug.Log("Ticket 2");

            if (draggedItem_Item is Food)
                AddFood(draggedItem_Index, 1);
            else if (draggedItem_Item is Ticket && lastTicketIndex == -1)
                AddTicket(draggedItem_Index, 1);
            else if (lastTicketIndex > -1)
                CraftTicket(lastTicketIndex);
        }
        else if (WithinArea(data.anchoredPosition, ticketContainers[2]))
        {
            Debug.Log("Ticket 3");

            if (draggedItem_Item is Food)
                AddFood(draggedItem_Index, 2);
            else if (draggedItem_Item is Ticket && lastTicketIndex == -1)
                AddTicket(draggedItem_Index, 2);
            else if (lastTicketIndex > -1)
                CraftTicket(lastTicketIndex);
        }
        else if (WithinArea(data.anchoredPosition, playerInventoryHolder.transform as RectTransform))
        {

            Debug.Log("Player inventory");
            if(lastTicketIndex != -1)
            {
                if (draggedItem_Item is Food)
                {
                    GameObject go = pickup.RemoveFood(lastTicketIndex, draggedItem_Index);
                    go.transform.SetParent(player.hand.transform);
                    go.transform.position = Vector3.zero;
                    UpdateTicketFoods(lastTicketIndex);
                    UpdatePlayerInventory(); 
                }
                else
                {
                    RemoveTicket(lastTicketIndex);
                }
            }            
        }
        else
        {

            Debug.Log("Not anywhere near!");
            if(lastTicketIndex != -1)
            {
                UpdateTicketFoods(lastTicketIndex);
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
        if(butt.x <= xMax && butt.x >= xMin)
            Debug.Log("X is Good for" + area.name);
        if (butt.y <= yMax && butt.y >= yMin)
            Debug.Log("Y is Good for" + area.name);
        Debug.Log(butt + " is not within " + xMax + "-" + xMin + ", " + yMax + "-" + yMin);
        return false;
    }
    void ClearTransformChildren(Transform t)
    {
        foreach(Transform child in t)
        {
            Debug.Log("Destroying " + child);
            Destroy(child.gameObject);
        }
    }
}
