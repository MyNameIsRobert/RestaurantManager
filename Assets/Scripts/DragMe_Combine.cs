using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragMe_Combine : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;
    public GameObject draggingIcon;
    public RectTransform draggingPlane;
    public int itemIndex = -1, ticketIndex = -1;
    public bool isRecipe = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;
        var uiController = FindInParents<CombineUI>(gameObject);

        if (ticketIndex != -1)
        {
            uiController.PickUpItem(ticketIndex, itemIndex);
        }
        else if(!isRecipe)
        {
            uiController.PickUpItem(itemIndex);
        }
        else
        {
            uiController.PickUpRecipe(itemIndex);
        }
    
        draggingIcon = new GameObject("Icon");

        draggingIcon.transform.SetParent(canvas.transform.GetChild(0), false);
        draggingIcon.transform.localScale = new Vector3(.25f, 1, 1);
        draggingIcon.transform.SetAsLastSibling();

        var image = draggingIcon.AddComponent<Image>();
        image.sprite = GetComponent<Image>().sprite;
        image.SetNativeSize();

        if (dragOnSurfaces)
            draggingPlane = transform as RectTransform;
        else
            draggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
            
    }
    public void OnDrag(PointerEventData data)
    {
        if (draggingIcon != null)
            SetDraggedPosition(data);
        var uiController = FindInParents<PickupUIController>(gameObject);
        //uiController.draggedItem_Index = itemIndex;
        //uiController.lastTicketIndex = ticketIndex;
        var pickup = FindInParents<FoodPickup>(gameObject);
    }

    void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            draggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = draggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (draggingIcon != null)
        {
            var uiController = FindInParents<CombineUI>(gameObject);
            uiController.PutDownItem(draggingIcon);
            Destroy(draggingIcon);
        }
        
    }

    public void SetIndexes(int item, int ticket)
    {
        itemIndex = item;
        ticketIndex = ticket;
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
