using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : Item {
    
    //0 Is neutral, < 0 means it negatively affects taste, > 0 means it positively effects taste
    public float taste = 0;

    public int preferredCookTime = 10;
    public int preferredCookTemp = 162;
    public int burningTemp = 180;
    public float burnSeconds = 5;
    public Enums.CookingStyle cookingStyle;

    public float totalCurrentTemp = 30, sideOneTemp = 30, sideTwoTemp = 30;
    public bool isBurnt = false;
    float burnTempReachTime = -1;

    private bool hasBeenCooked = false;
    Material attachedMaterial;
    // Use this for initialization

    public Material rawMaterial, cookedMaterial, burntMaterial;

    public static bool  operator==(Ingredient a, Ingredient b)
    {
        if (!a && !b)
            return true;
        if (!a && b || a && !b)
            return false;
        if (a.itemName == b.itemName)
            return true;
        return false;
    }
    public static bool  operator!=(Ingredient a, Ingredient b)
    {
        if (!a && !b)
            return false;
        if (!a && b || a && !b)
            return true;

        if (a.itemName != b.itemName)
            return true;
        return false;
    }

    public enum CookedLevel
    {
        Raw_Cold,
        Raw_Warm,
        Cooking_Warm,
        Cooking_Hot,
        Cooked_Cold,
        Cooked_Warm,
        Cooked_Perfect,
        Cooked_Burnt_Warm,
        Cooked_Burnt_Cold,
        Raw_Burnt,
        Frozen
    }
    public Ingredient(Ingredient o)
    {
        itemName = o.itemName;
        itemDescription = o.itemDescription;
        itemValue = o.itemValue;
        itemIcon = o.itemIcon;
        itemSize = o.itemSize;
        itemType = o.itemType;
        realWorldObject = o.realWorldObject;
        preferredCookTime = o.preferredCookTime;
        preferredCookTemp = o.preferredCookTemp;
        burningTemp = o.burningTemp;
        burnSeconds = o.burnSeconds;
        cookingStyle = o.cookingStyle;
        totalCurrentTemp = o.totalCurrentTemp;
        sideOneTemp = o.sideOneTemp;
        sideTwoTemp = o.sideTwoTemp;
        isBurnt = o.isBurnt;
        burnTempReachTime = o.burnTempReachTime;
        hasBeenCooked = o.hasBeenCooked;
        if (realWorldObject)
            realWorldObject = Instantiate(realWorldObject);
        else
            Debug.LogError(realWorldObject + " is null");
    }
    public Ingredient IngredientCopy(Ingredient o)
    {
        itemName = o.itemName;
        itemDescription = o.itemDescription;
        itemValue = o.itemValue;
        itemIcon = o.itemIcon;
        itemSize = o.itemSize;
        itemType = o.itemType;
        realWorldObject = o.realWorldObject;
        preferredCookTime = o.preferredCookTime;
        preferredCookTemp = o.preferredCookTemp;
        burningTemp = o.burningTemp;
        burnSeconds = o.burnSeconds;
        cookingStyle = o.cookingStyle;
        totalCurrentTemp = o.totalCurrentTemp;
        sideOneTemp = o.sideOneTemp;
        sideTwoTemp = o.sideTwoTemp;
        isBurnt = o.isBurnt;
        burnTempReachTime = o.burnTempReachTime;
        hasBeenCooked = o.hasBeenCooked;
        if (realWorldObject)
            realWorldObject = Instantiate(realWorldObject);
        else
            Debug.LogError(realWorldObject + " is null");
        return this;
    }
    IEnumerator UpdateMaterial()
    {
        float updateTime = 1;
        while(true)
        {
            yield return new WaitForSecondsRealtime(updateTime);
            attachedMaterial = GetComponent<Renderer>().material;
            if (attachedMaterial && burntMaterial && cookedMaterial && rawMaterial)
            {
                if (isBurnt)
                {
                    GetComponent<Renderer>().material = burntMaterial;
                }
                else if (hasBeenCooked)
                {
                    GetComponent<Renderer>().material = cookedMaterial;
                }
                else
                {
                    GetComponent<Renderer>().material = rawMaterial;
                }
            }
        }
        

    }

	void Start () {
        //If realWorldObject is null, then it is set to the gameObject this script is attached to
        realWorldObject = realWorldObject ? realWorldObject : gameObject;
        attachedMaterial = GetComponent<Renderer>().material;
        StartCoroutine("UpdateMaterial");
	}
	
    
	// Update is called once per frame
	void Update ()
    {
        
	}
    public void CalculateTotalTemp()
    {
        totalCurrentTemp = (sideOneTemp + sideTwoTemp) / 2;
        hasBeenCooked = totalCurrentTemp >= preferredCookTemp;
    }

    public int getTemp()
    {
        CalculateTotalTemp();
        return (int)totalCurrentTemp;
    }
    public int getSide1Temp()
    {
        return (int)sideOneTemp;
    }
    public int getSide2Temp()
    {
        return (int)sideTwoTemp;
    }

    //SIDEONE FUNCTIONS
    public void SideOne_AddHeat(float amount)
    {
        sideOneTemp += amount;
        CalculateBurnt();
    }
    public void SideOne_RemoveHeat(float amount)
    {
        sideOneTemp -= amount;
    }

    //SIDETWO FUNCTIONS
    public void SideTwo_AddHeat(float amount)
    {
        sideTwoTemp += amount;
        CalculateBurnt();
    }
    public void SideTwo_RemoveHeat(float amount)
    {
        sideTwoTemp -= amount;
    }
    public void CalculateBurnt()
    {
        if(getTemp() > burningTemp)
        {
            if(burnTempReachTime == -1)
            {
                burnTempReachTime = Time.time;
            }
            else if(Time.time - burnTempReachTime > burnSeconds)
            {
                isBurnt = true;
            }

        }
        if(sideOneTemp > burningTemp)
        {
            //...
        }
        if(sideTwoTemp > burningTemp)
        {
            //...
        }
        if (sideOneTemp < totalCurrentTemp)
        {
           // SideOne_AddHeat((totalCurrentTemp / 100) * Time.deltaTime);
        }
        else if (sideTwoTemp < totalCurrentTemp)
        {
            //SideTwo_AddHeat((totalCurrentTemp / 100) * Time.deltaTime);
        }
    }
    public virtual float CalculateTaste()
    {
        taste = 0;
        if(getTemp() >= preferredCookTemp && !isBurnt)
        {
            taste += 5;
        }
        //if(isSeasoned) ...
        if(sideOneTemp >= preferredCookTemp && sideOneTemp < burningTemp)
        {
            taste += 1;
        }
        if(sideTwoTemp >= preferredCookTemp && sideTwoTemp < burningTemp)
        {
            taste += 1;
        }
        return taste;
    }
    public virtual void DeteriorateTaste(float amount)
    {
        taste -= amount;
    }
    public virtual CookedLevel GetCookedLevel()
    {
        CookedLevel temp = CookedLevel.Raw_Cold;
        CalculateTotalTemp();
        if(totalCurrentTemp + 5 > preferredCookTemp)
        {
            if(hasBeenCooked)
            {
                if(!isBurnt)
                {
                    temp = CookedLevel.Cooked_Perfect;
                }
                else
                {
                    temp = CookedLevel.Cooked_Burnt_Warm;
                }
            }
            else
            {
                temp = CookedLevel.Cooking_Hot;
            }
        }
        else if (totalCurrentTemp > 70)
        {
            if(hasBeenCooked)
            {
                if(!isBurnt)
                {
                    temp = CookedLevel.Cooked_Warm;
                }
                else
                {
                    temp = CookedLevel.Cooked_Burnt_Warm;
                }
            }
            else
            {
                temp = CookedLevel.Cooking_Warm;
            }
        }
        else if (totalCurrentTemp > 32)
        {
            if(hasBeenCooked)
            {
                if(!isBurnt)
                {
                    temp = CookedLevel.Cooked_Cold;
                }
                else
                {
                    temp = CookedLevel.Cooked_Burnt_Cold;
                }
            }
            else
            {
                temp = CookedLevel.Raw_Cold;
            }
        }
        else
        {
            temp = CookedLevel.Frozen;
        }

        return temp;
    }

}

