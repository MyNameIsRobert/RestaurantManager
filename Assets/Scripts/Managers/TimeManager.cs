using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    //Defining the game's TimeScale
    public int monthsPerYear = 4;
    public int daysPerMonth = 4;
    //Hours start at 0, and go up to the number (i.e. 12 = 0-)
    public int hoursPerDay = 12;
    public int dayStartHour = 9;
    //In case I want the night to be playable
    public int hoursPerNight = 3;
    //How long an hour is in seconds realtime
    //A full day will take 12 * this number
    public float realTime_ToHour_Seconds = 90; //90 is default. A full day takes 18 minutes. A month takes 72 minutes, and a year takes 288 
    
    public static int currentHour = 0, currentDay = 0, currentMonth = 0, currentYear = 0;

    public float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime < realTime_ToHour_Seconds)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            //Increment timeStep
            currentTime = 0;
            if(currentHour < hoursPerDay -1)
            {
                currentHour++;
                Debug.Log("Increasing hours to " + currentHour);
            }
            else
            {
                currentHour = 0;
                if(currentDay < daysPerMonth - 1)
                {
                    currentDay++;
                }
                else 
                {
                    currentDay = 0;
                    if(currentMonth < monthsPerYear - 1)
                    {
                        currentMonth++;
                    }
                    else
                    {
                        currentMonth = 0;
                        currentYear++; 
                    }    
                }
            }
        }
    }
}
