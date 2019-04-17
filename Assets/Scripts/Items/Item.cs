using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public string itemName;
    public string itemDescription;
    public int itemValue;
    public Sprite itemIcon;
    public int itemSize = 1;
    public ItemType itemType;
    public GameObject realWorldObject;
    public enum ItemType
    {
        None = -1,
        Food,
        Ingredient,
        Item,
        Recipe,
        Equipement
    }
    public Item()
    {

    }
    public Item(Item o)
    {
        itemName = o.itemName;
        itemDescription = o.itemDescription;
        itemValue = o.itemValue;
        itemIcon = o.itemIcon;
        itemSize = o.itemSize;
        itemType = o.itemType;
        realWorldObject = o.realWorldObject;
        if (realWorldObject)
            realWorldObject = Instantiate(realWorldObject);
        else
            Debug.LogError(realWorldObject + " is null");
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
