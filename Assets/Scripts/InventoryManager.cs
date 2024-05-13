using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject itemCursor;

    [SerializeField] GameObject slotHolder;
    [SerializeField] ItemClass itemToAdd;
    [SerializeField] ItemClass itemToRemove;

    [SerializeField] SlotClass[] startingItems;
    private SlotClass[] items;

    private GameObject[] slots;

    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;
    bool isMovingItem;

    private void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new SlotClass[slots.Length];
        
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new SlotClass();
            
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];

        }

        //set all the slots
        for (int i = 0; i < slotHolder.transform.childCount; i++)
            { slots[i] = slotHolder.transform.GetChild(i).gameObject; }

        RefreshUI();

        Add(itemToAdd, 1);
        Remove(itemToRemove);
    }

    private void Update()
    {
        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        /*if (isMovingItem)
        {
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemImage;
        }*/

        if (Input.GetMouseButton(0)) //left click activared
        {
            //find the slot that was clicked on
            if (isMovingItem)
            {
                //want to end item move
                EndItemMove();

            }
            else
            {
                BeginItemMove();
            }
           
        }
        else if (Input.GetMouseButton(1)) //right click activated
        {
            //find the slot that was clicked on
            if (isMovingItem)
            {
                //want to end item move
                //EndItemMove();

            }
            else
            {
                BeginItemMove_Half();
            }
        }
    }

    #region Inventory Utils
    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length;i++) 
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemImage;

                if (items[i].GetItem().isStackable)
                {
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = items[i].GetQuantity() + "";
                }
                else
                {
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }

            }
            catch 
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
           
        }
    }

    public bool Add(ItemClass item, int quantity)
    {
        //items.Add(item);

        //check if inventory contains item

        SlotClass slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            slot.AddQuantity(1);
        }
        else
        {
            for (int i =0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null) //this is an empty slot 
                {
                    items[i].AddItem(item, quantity);
                    break;
                } 
            }


           /* if (items.Count < slots.Length)
            {
                items.Add(new SlotClass(item, 1));
            }
            else
            {
                return false;
            }*/
        }

        RefreshUI();
        return true;
    }

    public bool Remove(ItemClass item)
    {
        //items.Remove(item);

        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
            {
                temp.SubtractQuantity(1);
            }
            else
            {
                //compare items in each slot
                int slotToRemoveIndex = 0;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == item)
                    {
                        //items.Remove(slot);
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                items[slotToRemoveIndex].Clear();
            }
        }
        else
        {
            return false;
        }
       
        RefreshUI();
        return true;
    }

    public SlotClass Contains(ItemClass item)
    {
        for (int i = 0; i < items.Length; i ++)
        {
            if (items[i].GetItem() == item)
            {
                return items[i];
            }
        }

        /*foreach (SlotClass slot in items)
        {
            if(slot.GetItem() == item) 
                return slot;
        }*/

        return null;
    }
    #endregion Inventory Utils

    #region Moving stuff
   private bool BeginItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
        {
            return false; //no item to pick up
        }

        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }

    private bool BeginItemMove_Half()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
        {
            return false; //no item to pick up
        }

        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }

    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
        {
            Add(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
        }
        else
        {
            if (originalSlot.GetItem() != null)
            {
                if (originalSlot.GetItem() == movingSlot.GetItem()) //the same item (they should stack)
                {
                    if (originalSlot.GetItem().isStackable)
                    {
                        originalSlot.AddQuantity(movingSlot.GetQuantity());
                        movingSlot.Clear();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    tempSlot = new SlotClass(originalSlot); //a = b
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());  //b = c
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity()); //c = a / a = c
                    RefreshUI();
                    return true;
                }
            }
            else //place item as usual
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
            }
        }

        RefreshUI();
        return true;    

    }

    private SlotClass GetClosestSlot()
    {

        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
            {
                return items[i];
            }
        }

        return null; 
    }

    #endregion Moving Stuff
}
