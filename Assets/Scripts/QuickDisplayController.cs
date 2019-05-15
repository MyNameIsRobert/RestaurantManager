using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuickDisplayController : MonoBehaviour
{
   public TextMeshProUGUI currentMoney, currentTime;
    public FinanceManager fmanager;
    public GameManager manager;
    public TimeManager tmanager;

    private void Start() {
        manager = FindObjectOfType<GameManager>();
        fmanager = manager.GetComponentInChildren<FinanceManager>();        
    }

   private void Update() 
   {
        if(currentMoney && fmanager)
        {
            currentMoney.text = fmanager.currentMoney + "$";
        }
        if(currentTime && tmanager)
        {
            int hour = TimeManager.currentHour + tmanager.dayStartHour; 
            hour = hour > 12? hour - 12: hour;
            string time = hour + ":";
            int minute = (int)((tmanager.currentTime / tmanager.realTime_ToHour_Seconds) * 60);
            time = minute < 10? time + "0"+minute: time + minute;
            time = (TimeManager.currentHour + tmanager.dayStartHour > 12)? time + " pm": time + " am";
            time += " " + (TimeManager.currentDay + 1) + "/" + (TimeManager.currentMonth+1)+"/"+(TimeManager.currentYear+1);
            currentTime.text = time;
        }    
   }
}
