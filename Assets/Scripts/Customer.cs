using UnityEngine;

public class Customer : Person 
{
    //Higher is better. Caps at -100 and 100
    public int opinionOfRestaurant = 0;

    public virtual void Randomize()
    {
        opinionOfRestaurant = Random.Range(-10, 10);
        firstNameIndex = NameGenerator.GetRandomFirstName();
        lastNameIndex = NameGenerator.GetRandomLastName();
    }
    public Customer()
    {
        opinionOfRestaurant = 0;
    }
    private void Awake()
    {
    }
    public override void CopyFrom(Person p)
    {
        base.CopyFrom(p);
        if(p is Customer)
        {
           Customer c = p as Customer;
           opinionOfRestaurant = c.opinionOfRestaurant; 
        }
    }
}