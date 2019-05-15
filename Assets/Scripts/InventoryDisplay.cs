using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryDisplay : MonoBehaviour
{
    public PlayerController controller;
    int currentIndex = 0;
    public Image curMin2, curMin1, cur, curPlus1, curPlus2;
    public bool doneAnimating;
    bool cRunning;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        Transform current = transform;
        if (!controller)
        {
            controller = transform.GetComponent<PlayerController>();
            while (!controller && current.parent)
            {
                current = current.parent;
                controller = transform.GetComponent<PlayerController>();
            }
        }
        anim = GetComponent<Animator>();
    }
    IEnumerator AnimateChange(bool up)
    {
        cRunning = true;
        if(up)
        {
            anim.SetTrigger("down");
        }
        else
        {
            anim.SetTrigger("up");
        }
        float time = Time.time + .5f;
        yield return new WaitUntil(() => (doneAnimating || Time.time == time));
        if(up)
        {
            currentIndex = (currentIndex + 1 == controller.hand.transform.childCount) ? 0 : currentIndex + 1;
        }
        else
        {
            currentIndex = (currentIndex == 0 ) ? controller.hand.transform.childCount - 1 : currentIndex - 1;
        }
        cRunning = false;
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Scrollwheel") > 0)
        {
            //Scroll up
            if(!cRunning)
            {
                StartCoroutine(AnimateChange(true));
            }
        }
        else if(Input.GetAxis("Scrollwheel") < 0)
        {
            //Scroll down
            if (!cRunning)
            {
                StartCoroutine(AnimateChange(false));
            }
        }
        if(controller.hand.transform.childCount < 5)
        {
            curPlus2.sprite = null;
            curPlus2.color = Color.clear;
            if(controller.hand.transform.childCount < 4)
            {
                curMin2.sprite = null;
                curMin2.color = Color.clear;
                if (controller.hand.transform.childCount < 3)
                {
                    curPlus1.sprite = null;
                    curPlus1.color = Color.clear;
                    if (controller.hand.transform.childCount < 2)
                    {
                        curMin1.sprite = null;
                        curMin1.color = Color.clear;
                        if (controller.hand.transform.childCount < 1)
                        {
                            cur.sprite = null;
                            cur.color = Color.clear;
                        }
                        else
                        {
                            if (controller.hand.transform.GetChild(currentIndex))
                            {
                                cur.sprite = controller.hand.transform.GetChild(currentIndex).GetComponent<Item>().itemIcon;
                                cur.color = Color.white;
                            }
                        }
                    }
                    else
                    {
                        if (controller.hand.transform.GetChild(currentIndex))
                        {
                            cur.sprite = controller.hand.transform.GetChild(currentIndex).GetComponent<Item>().itemIcon;
                            cur.color = Color.white;
                        }
                        int temp = (currentIndex - 1 < 0) ? controller.hand.transform.childCount - 1 : currentIndex - 1;

                        if (controller.hand.transform.GetChild(temp))
                        {
                            curMin1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                            curMin1.color = Color.white;
                        }
                    }
                }
                else
                {
                    if (controller.hand.transform.GetChild(currentIndex))
                    {
                        cur.sprite = controller.hand.transform.GetChild(currentIndex).GetComponent<Item>().itemIcon;
                        cur.color = Color.white;
                    }
                    int temp = (currentIndex - 1 < 0) ? controller.hand.transform.childCount - 1 : currentIndex - 1;

                    if (controller.hand.transform.GetChild(temp))
                    {
                        curMin1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                        curMin1.color = Color.white;
                    }
                    temp = (currentIndex + 1 > controller.hand.transform.childCount - 1) ? 0 : currentIndex + 1;
                    if (controller.hand.transform.GetChild(temp))
                    {
                        curPlus1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                        curPlus1.color = Color.white;
                    }
                }
            }
            else
            {
                if (controller.hand.transform.GetChild(currentIndex))
                {
                    cur.sprite = controller.hand.transform.GetChild(currentIndex).GetComponent<Item>().itemIcon;
                    cur.color = Color.white;
                }
                int temp = (currentIndex - 1 < 0) ? controller.hand.transform.childCount - 1 : currentIndex - 1;

                if (controller.hand.transform.GetChild(temp))
                {
                    curMin1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                    curMin1.color = Color.white;
                }
                temp = (currentIndex - 2 < 0) ? controller.hand.transform.childCount - (2 - currentIndex) : currentIndex - 2;
                if (controller.hand.transform.GetChild(temp))
                {
                    curMin2.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                    curMin2.color = Color.white;
                }
                temp = (currentIndex + 1 > controller.hand.transform.childCount - 1) ? 0 : currentIndex + 1;
                if (controller.hand.transform.GetChild(temp))
                {
                    curPlus1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                    curPlus1.color = Color.white;
                }
            }
        }
        else
        {
            if (controller.hand.transform.GetChild(currentIndex))
            {
                cur.sprite = controller.hand.transform.GetChild(currentIndex).GetComponent<Item>().itemIcon;
                cur.color = Color.white;
            }
            int temp = (currentIndex - 1 < 0) ? controller.hand.transform.childCount - 1 : currentIndex - 1;
            if (controller.hand.transform.GetChild(temp))
            {
                curMin1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                curMin1.color = Color.white;
            }
            temp = (currentIndex - 2 < 0) ? controller.hand.transform.childCount - (2 - currentIndex) : currentIndex - 2;
            if (controller.hand.transform.GetChild(temp))
            {
                curMin2.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                curMin2.color = Color.white;
            }
            temp = (currentIndex + 1 > controller.hand.transform.childCount - 1) ? 0 : currentIndex + 1;
            if (controller.hand.transform.GetChild(temp))
            {
                curPlus1.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                curPlus1.color = Color.white;
            }
            temp = (currentIndex + 2 > controller.hand.transform.childCount - 1) ? 0 + ((controller.hand.transform.childCount) - currentIndex): currentIndex + 2;
            if (controller.hand.transform.GetChild(temp))
            {
                curPlus2.sprite = controller.hand.transform.GetChild(temp).GetComponent<Item>().itemIcon;
                curPlus2.color = Color.white;
            }
        }

        controller.currentInventoryIndex = currentIndex;

    }
}
