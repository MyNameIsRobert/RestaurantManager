using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class OverviewUI : MonoBehaviour
{
    public TextMeshProUGUI currentMoney, averageFoodRevenue, averageTicketProfit, averageItemCost;
    public FinanceManager financeManager; 

    // Start is called before the first frame update
    void Start()
    {
        financeManager = FindObjectOfType<FinanceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(financeManager)
        {
            if(currentMoney)
            {
                currentMoney.text = financeManager.currentMoney + "$";
            }
            if(averageFoodRevenue)
            {
                averageFoodRevenue.text = financeManager.getFoodAverageEarnings() + "$";
            }
            if(averageTicketProfit)
            {
                averageTicketProfit.text = financeManager.getAverageTicketProfit() + "$";
            }
            if(averageItemCost)
            {
                averageItemCost.text = financeManager.getAverageItemCost() + "$";
            }
        }
    }
}
