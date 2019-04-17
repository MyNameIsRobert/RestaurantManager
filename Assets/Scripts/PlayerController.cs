using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public Camera cam;
    public GameObject hand;
    CharacterController controller;
    GameManager manager;
    public float moveSpeed;
    public float lookSpeed;

    public float minYRotation = -.5f, maxYRotation = .5f;
    bool reachedMax = false, reachedMin = false;

    bool lockCursor = true;

    public enum ControlMode
    {
        FreeWalk,
        StoveTop,
        HandWashing,
        DishWashing,
        Register,
        WorldUI
    }
    public ControlMode controlMode = ControlMode.FreeWalk;

    Grill grill = null;
    RunRegister register = null;
    public List<GameObject> itemsInHand = null;
    public int itemHoldAmount = 20;
    Transform locationOfWorldCanvas = null;
    public int currentInventoryIndex = 0;
    CanvasGroup playerHUD;
    // Use this for initialization
    void Start()
    {
        cam = transform.GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        manager = FindObjectOfType<GameManager>();
        playerHUD = GetComponentInChildren<CanvasGroup>();

    }

    private void Update()
    {
        
        switch(controlMode)
        {
            case ControlMode.FreeWalk:
                FreeControlMode();
                break;
            case ControlMode.StoveTop:
                StoveTopMode();
                break;
            case ControlMode.Register:
                RegisterMode();
                break;
            case ControlMode.WorldUI:
                UsingWorldUIMode();
                break;
        }

        //Controlling lockCursor
        if(Input.GetAxis("Menu") != 0)
        {
            lockCursor = !lockCursor;
        }
        //Cursor lock
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator DashTowardsPoint(Vector3 point, float speed)
    {
        while (transform.position != point)
        {
            var heading = point - transform.position;
            controller.SimpleMove(heading * speed);
            yield return null;
        }
        yield return null;
    }
    public void FreeControlMode()
    {
        SimpleMove();
        SimpleLook();
        if (playerHUD)
        {
            playerHUD.alpha = 1;
            playerHUD.interactable = true;
            playerHUD.blocksRaycasts = true;
        }
        #region Raycasting forward
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Container"))
            {
                
                Container container = hit.collider.GetComponent<Container>();
                //If Left Click
                if (Input.GetButtonDown("Use"))
                {
                    //Debug.Log("Clicked on container");
                    AddItemToHand(container.GrabItem());
                }
                //If Right Click
                else if (Input.GetButtonDown("Alt Use"))
                {
                    container.AddItem(hand.transform.GetChild(currentInventoryIndex).gameObject);
                }
            }
            if (hit.collider.CompareTag("Combining"))
            {
                
            }
            if (hit.collider.CompareTag("Pickup"))
            {
                FoodPickup pickup = hit.collider.GetComponent<FoodPickup>();
                if(Input.GetButtonDown("Use"))
                {
                    pickup.AddChef(this);
                }
                
            }
        }
        #endregion
    }

    public void UsingWorldUIMode()
    {
        if(Input.GetButton("Recipe"))
        {
            lockCursor = true;
            SimpleLook();
        }
        else
        {
            lockCursor = false;
            if(locationOfWorldCanvas)
            {
                //transform.LookAt(locationOfWorldCanvas);
            }
        }
        if(playerHUD)
        {
            playerHUD.alpha = 0;
            playerHUD.interactable = false;
            playerHUD.blocksRaycasts = false;
        }
        if(Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            controlMode = ControlMode.FreeWalk;
            lockCursor = true;
            locationOfWorldCanvas = null;
        }
    }

    void SimpleMove()
    {
        //Moving character based on Player inputs using CharacterController's SimpleMove
        if (Input.GetAxis("Vertical") != 0)
        {
            controller.SimpleMove(transform.forward * Input.GetAxis("Vertical") * moveSpeed);
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            controller.SimpleMove(transform.right * Input.GetAxis("Horizontal") * moveSpeed);
        }

    }

    void SimpleLook()
    {

        //Rotating player around y-axis
        Vector3 rotationAmount = new Vector3(0, Input.GetAxis("Look X") * lookSpeed, 0);
        transform.Rotate(rotationAmount);

        //Rotating camera around x-axis, being clamped by a min and max rotation **** 
        //****************
        float lookYInput = Input.GetAxis("Look Y");
        float xRotation = 0;

        xRotation = cam.transform.localRotation.x;

        reachedMax = xRotation >= maxYRotation;
        reachedMin = xRotation <= minYRotation;
        if (lookYInput == 0)
        {

        }
        else if (lookYInput < 0)
        {
            if (!reachedMin)
            {
                cam.transform.Rotate(lookYInput * lookSpeed, 0, 0);
            }
        }
        else if (lookYInput > 0)
        {
            if (!reachedMax)
            {
                cam.transform.Rotate(lookYInput * lookSpeed, 0, 0);
            }
        }
        //*************
    }

    public void StoveTopMode()
    {

        SimpleMove();
        SimpleLook();

        #region Raycasting forward
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("StoveTop_IngredientSlot"))
            {
                //Debug.Log("Looking at ingredient slot on stove");

                int index = -1;
                for (int i = 0; i < grill.ingredientSlots.Length; i++)
                {
                    if (grill.ingredientSlots[i] == hit.collider.gameObject)
                    {
                        index = i;
                        break;
                    }
                }
                if(index == -1)
                {
                    //Debug.Log("index is -1");            
                }
                //If Left Click
                if (Input.GetButtonDown("Use"))
                {

                    if (!grill.ingredients[index])
                    {
                        if (itemsInHand != null && itemsInHand.Count >= 1)
                        {
                            Item item = itemsInHand[currentInventoryIndex].GetComponent<Item>();
                            Ingredient held = (Ingredient)item;

                            grill.AddIngredient(held, index);
                            itemsInHand.RemoveAt(currentInventoryIndex);
                            if(currentInventoryIndex > itemsInHand.Count -1)
                            {
                                currentInventoryIndex = itemsInHand.Count - 1;
                            }

                        }
                    }
                    else
                    {
                        AddItemToHand(grill.RemoveIngredient(index).realWorldObject);
                    }
                }
                //If Right Click
                else if (Input.GetButtonDown("Alt Use"))
                {
                    grill.FlipIngredient(index);
                }
            }
        } 
        #endregion

    }

    public void RegisterMode()
    {
        
        if(Input.GetButtonDown("Alt Use"))
        {
            lockCursor = !lockCursor;
        }
        if (lockCursor)
        {
            #region Camera Movement
            SimpleMove();
            SimpleLook();
            #endregion
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Equipement_ChefStand"))
        {
            Transform currentTransform = other.transform;
            grill = currentTransform.GetComponent<Grill>();
            while(currentTransform.parent != null && !grill)
            {
                currentTransform = currentTransform.parent;
                grill = currentTransform.GetComponent<Grill>();
            }
            if(grill)
            {
                grill.StepToGrill();
                controlMode = ControlMode.StoveTop;
                //transform.position = other.transform.position;
            }
        }
        else if(other.CompareTag("Register"))
        {
            Transform currentTransform = other.transform;
            register = currentTransform.GetComponent<RunRegister>();
            if(!register)
            {
                while(currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if(currentTransform.GetComponent<RunRegister>())
                    {
                        register = currentTransform.GetComponent<RunRegister>();
                        break;
                    }
                }
            }
            if(register)
            {
                register.player = this;
                controlMode = ControlMode.WorldUI;
                locationOfWorldCanvas = register.uIController.transform;
                RegisterUIController uIController = register.uIController;
                uIController.SetUIStage(RegisterUIController.UIStage.NoTicketYet);
            }
        }
        else if(other.CompareTag("Pickup"))
        {
            Transform currentTransform = other.transform;
            FoodPickup pickup = currentTransform.GetComponent<FoodPickup>();
            if (!pickup)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<FoodPickup>())
                    {
                        pickup = currentTransform.GetComponent<FoodPickup>();
                        break;
                    }
                }
            }
            if (pickup)
            {
                pickup.AddChef(this);
                controlMode = ControlMode.WorldUI;
            }
        }
        else if(other.CompareTag("Combining"))
        {
            Transform currentTransform = other.transform;
            Combine combiner = currentTransform.GetComponent<Combine>();
            if(!combiner)
            {
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    if (currentTransform.GetComponent<Combine>())
                    {
                        combiner = currentTransform.GetComponent<Combine>();
                        break;
                    }
                }
            }
            if(combiner)
            {
                combiner.AddChef(this);
                controlMode = ControlMode.WorldUI;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Register"))
        {
            if (register)
            {
                RegisterUIController uIController = register.uIController;
                uIController.SetUIStage(RegisterUIController.UIStage.Off);
                register.StepAway();
            }
            controlMode = ControlMode.FreeWalk;

        }
        if (other.CompareTag("Equipement_ChefStand"))
        {
            if (grill)
            {
                grill.LeaveGrill();
                //grill = null;
            }
            controlMode = ControlMode.FreeWalk;
        }
    }

    public bool AddItemToHand(GameObject item)
    {
        if (item.GetComponent<Item>().itemSize+ CalculateSpace() > itemHoldAmount)
        {
            return false;
        }
        else
        {
            itemsInHand.Add(item);
            item.transform.SetParent(hand.transform);
            item.transform.localPosition = Vector3.zero;
            return true;
        }
    }
    public GameObject RemoveItemFromHand(int itemIndex)
    {
        GameObject go = hand.transform.GetChild(itemIndex).gameObject;
        itemsInHand.RemoveAt(itemIndex);
        go.transform.SetParent(null);
        return go;
    }
    public int CalculateSpace()
    {
        int size = 0;
        foreach (GameObject i in itemsInHand)
        {
            if (i && i.GetComponent<Item>())
                size += i.GetComponent<Item>().itemSize;
            else
                Debug.Log(i + " is not an Item or an object");

        }
        return size;
    }
}





