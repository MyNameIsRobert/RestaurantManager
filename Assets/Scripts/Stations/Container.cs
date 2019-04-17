using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Station {


    public enum ContainerType
    {
        Freezer,
        Fridge,
        RoomTemp,
        Warm
    }
    public ContainerType containerType;
    public GameObject objectHolster;
    public List<GameObject> itemsStored = new List<GameObject>();
    public int storageSize = 200;
    public bool spawnInfnitely = false;
    public Item itemToSpawn;
    public OnGrab onGrab;
    public delegate void OnGrab();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (spawnInfnitely)
        {
            if (CalculateSpace() < storageSize)
            {
                Item item = new Item(itemToSpawn);
                item.realWorldObject.transform.SetParent(objectHolster.transform);
                item.realWorldObject.transform.localPosition = Vector3.zero;
                itemsStored.Add(item.realWorldObject);
            } 
        }
	}
    public bool AddItem(GameObject item)
    {
        Item added = item.GetComponent<Item>();
        if(added.itemSize + CalculateSpace() > storageSize)
        {
            return false;
        }
        else
        {
            itemsStored.Add(item);
            item.transform.SetParent(objectHolster.transform);
            item.transform.localPosition = Vector3.zero;
            return true;
        }
    }

    public int CalculateSpace()
    {
        int size = 0;
        foreach (GameObject i in itemsStored)
        {
            size += i.GetComponent<Item>().itemSize;
        }
        return size;
    }
    public bool IsAllTheSame()
    {
        bool temp = false;
        for(int i = 0; i < itemsStored.Count -1; i++)
        {
            Item itemStored = itemsStored[i].GetComponent<Item>();
            if(itemStored.itemType == itemsStored[i+1].GetComponent<Item>().itemType)
            {
                temp = true;
            }
            else
            {
                temp = false;
                break;
            }
        }
        return temp;
    }

    public GameObject GrabItem()
    {
        GameObject returnItem = itemsStored[itemsStored.Count - 1];
        itemsStored.RemoveAt(itemsStored.Count - 1);       
        FindObjectOfType<FinanceManager>().AddCost(returnItem.GetComponent<Item>().itemValue);

        return returnItem;
    }
    public GameObject GrabItem(Action callBack)
    {
        GameObject returnItem = itemsStored[itemsStored.Count - 1];
        itemsStored.RemoveAt(itemsStored.Count - 1);
        FindObjectOfType<FinanceManager>().AddCost(returnItem.GetComponent<Item>().itemValue);

        if(callBack != null)
        {
            onGrab = new OnGrab(callBack);
        }
        onGrab();
        return returnItem;
    }
}
