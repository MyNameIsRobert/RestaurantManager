using UnityEngine;

public class Customer_StoreFront : Customer 
{
    public int amountOfMoney = 25;
    public Enums.Mood currentMood;
    public Enums.RestaurantTags[] wantedTags;
    

    public override void Randomize()
    {
        amountOfMoney = Random.Range(20, 45);

    }
    public override void CopyFrom(Person p)
    {
        base.CopyFrom(p);
        if(p is Customer_StoreFront)
        {
            Customer_StoreFront cSF = p as Customer_StoreFront;
            amountOfMoney = cSF.amountOfMoney;
            wantedTags = cSF.wantedTags;
        }
    }
}