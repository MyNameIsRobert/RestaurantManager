using System;
using System.Collections.Generic;
using UnityEngine;


public class FinanceManager : MonoBehaviour
{
    public int currentMoney = 0;
    int foodAverageEarnings = 0;
    public List<int> averageRevenueFromFood = new List<int>(), ticketProfit = new List<int>();
    public List<ItemCost> itemCosts = new List<ItemCost>();
    public struct ItemCost
    {
        public Item item;
        public int cost;

        public ItemCost(Item i, int c)
        {
            item = i;
            cost = c;
        }
    }
    public void AddPurchase(int total, int numOfFoodItems)
    {
        currentMoney += total;
        averageRevenueFromFood.Add(total / numOfFoodItems);
    }
    public void AddCost(int amount)
    {
        currentMoney -= amount;
    }
    public void AddCost(int amount, Item item)
    {
        currentMoney -= amount;
        itemCosts.Add(new ItemCost(item, amount));
    }
    public void AddTicketProfit(int amount)
    {
        ticketProfit.Add(amount);
    }

    public int getFoodAverageEarnings()
    {
        if(averageRevenueFromFood.Count < 1)
            return 0;
        foodAverageEarnings = 0;
        foreach(int i in averageRevenueFromFood)
        {
            foodAverageEarnings += i;
        }
        foodAverageEarnings /= averageRevenueFromFood.Count;
        return foodAverageEarnings;
    }

    public int getAverageItemCost()
    {
        if(itemCosts.Count < 1)
            return 0;
        int total = 0;
        foreach(ItemCost i in itemCosts)
        {
            total += i.cost;
        }
        return total / itemCosts.Count;
    }

    public int getAverageTicketProfit()
    {
        if(ticketProfit.Count < 1)
            return 0;
        int total = 0;
        foreach(int i in ticketProfit)
        {
            total += i;
        }
        return total / ticketProfit.Count;
    }
}

