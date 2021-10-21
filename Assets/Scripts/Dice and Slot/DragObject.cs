using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    Vector3 mOffset;
    float mZcoord;

    [SerializeField] Camera viewCamera;
    [SerializeField] float zAxisWhenDrag = -100f;
    [SerializeField] float zAxisWhenNotDrag = 0;
    [SerializeField] float zAxisSpeedWhenDrag = -10f; // Per sec

    GearDiceBehavior diceBehavior;
    public HolderSlot slotParent { get; set; }

    void Start()
    {
        slotParent = null;
        diceBehavior = GetComponent<GearDiceBehavior>();
    }

    private void Update()
    {
        
    }

    void OnMouseDown()
    {
        //mZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mZcoord = viewCamera.WorldToScreenPoint(gameObject.transform.position).z;
        
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;

        // Apply float effect
        //if (transform.localPosition.z > zAxisWhenDrag)
        //    transform.localPosition += new Vector3(0, 0, zAxisSpeedWhenDrag*Time.deltaTime);
        //else
        //    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zAxisWhenDrag);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zAxisWhenDrag);
    }

    void OnMouseUp()
    {
        StartCoroutine(HandleRelease());
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZcoord;
        return viewCamera.ScreenToWorldPoint(mousePoint);
    }

    IEnumerator HandleRelease()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zAxisWhenNotDrag);

        yield return null;

        // CHeck if it is colliding with any slot
        HolderSlot[] slots = FindObjectsOfType<HolderSlot>();

        List<HolderSlot> intersectedSlots = new List<HolderSlot>();
        foreach (var slot in slots)
            if (slot.CurrentCollidingForeignObject == this || slot.OccupiedObject == this.gameObject)
                intersectedSlots.Add(slot);

        if(intersectedSlots.Count == 1)
        {
            Debug.Log("intersect " + intersectedSlots[0].name);
        }

        // If > 1 slot, check which one is nearest
        if (intersectedSlots.Count > 1)
        {
            HolderSlot nearestSlot = FindNearestSlots(this.gameObject, intersectedSlots.ToArray());
            TrySnapToSlot(nearestSlot);

            //Debug.Log(this.gameObject + " exchange");
        }
        else if(intersectedSlots.Count == 1) // If 1 slot, snap to that
        {
            TrySnapToSlot(intersectedSlots[0]);
            //Debug.Log(this.gameObject + " is snapping to " + intersectedSlots[0].gameObject);
        }
        else // Else, simply put on ground
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zAxisWhenNotDrag);

        // Clear
        foreach (var slot in slots)
            slot.ClearCurrentCollidingObject();
    }

    void TrySnapToSlot(HolderSlot slot)
    {
        // Check 1: is the slot compatible ?
        DiceSlotEnum slotType = slot.GetComponent<SlotType>().slotType;
        if (!diceBehavior.RepresentedDice.CompatibleWith(slotType))       
        {
            // Snap it back if not
            GetComponent<JumpToEmptySlot>().JumpToRandomEmptySlot();
            Debug.Log("Dice not compatible to slot " + slotType);
            return;
        }


        // CHeck 2: if Occupied ? If so, do Swap.
        if (slot.IsOccupied)
        {
            // Is occupied by this own object, reseat its position
            if (slot.OccupiedObject == this.gameObject)
            {
                // Get the UI Position to align
                Vector3 snapPos = slot.GetUIPosition();
                this.transform.position = snapPos;

                // OVerride the Z and use this one
                this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zAxisWhenNotDrag);
            }
            else // Do Swap
            {
                GameObject targetToSwap = slot.OccupiedObject;
                HolderSlot slotToGiveTarget = this.transform.parent.GetComponent<HolderSlot>();
                if (slotToGiveTarget)
                    slotToGiveTarget.ReceiveObject(targetToSwap);
                else
                {
                    slot.EjectObject();
                    targetToSwap.GetComponent<RollDiceViewTransform>().AnimateJumpTo(targetToSwap.transform.position + new Vector3(-1, 0, 0));
                }

                //slot.EjectObject();
                slot.ReceiveObject(this.gameObject);
            }

        }
        else // Snap object into simple slot
        {
            // Get the UI Position to align
            Vector3 snapPos = slot.GetUIPosition();
            this.transform.position = snapPos;

            // OVerride the Z and use this one
            this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zAxisWhenNotDrag);

            // Set parent slot
            this.slotParent = slot;
            slotParent.ReceiveObject(this.gameObject);
        }

    }


    HolderSlot FindNearestSlots(GameObject main, HolderSlot[] holderSlots)
    {
        float lowestDist = 100000;
        HolderSlot lowestGO = null;
        foreach (HolderSlot slot in holderSlots)
        {
            //float dist = Vector2.Distance(main.transform.position, slot.transform.position);
            float dist = Vector3.Distance(main.transform.position, slot.transform.position);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                lowestGO = slot;
            }
        }

        return lowestGO;
    }

}
