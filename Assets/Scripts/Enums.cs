using System;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum Mood
        {
            Enraged,
            Angry,
            Frustrated,
            Upset,
            Neutral,
            Satistfied,
            Good,            
            Happy,            
        }

    public enum CookingStyle
    {
        OneSide_Grill,
        TwoSides_Grill,
        Boil,
        Bake,
        Fry
    }

    public enum RestaurantTags
    {
        American,
        Italian,
        Mexican,
        Texmex,
        FastFood,
        Fine_Dining,
        Casual
    }

    public enum EmployeeInterests
    {
        Cooking,
        Prepping,
        Cleaning,
        Washing,
        Serving,
        RunningRegister,
        Lifting,
        Moving
    }
    public enum StationType
    {
        Register,
        Grill,
        Combine,
        FoodPickup,
        Container,
        None
    }
    public enum Weight
    {
        VeryLight,
        Light,
        ModeratelyLight,
        ModeratelyHeavy,
        Heavy,
        VeryHeavy,
        TooHeavy,
    }


}

