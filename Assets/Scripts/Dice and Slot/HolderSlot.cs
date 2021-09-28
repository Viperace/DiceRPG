using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HolderSlot : MonoBehaviour
{
    DragObject _currentCollidingForeignObject;
    RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    private void OnTriggerStay(Collider other)
    {
        // First, exclude own child
        if (other.transform.parent == this.transform)
            return;

        // Then, check if tihs is dragobj
        DragObject dragObject = other.GetComponent<DragObject>();
        if (dragObject) // Ignore already occupied object
        {
            _currentCollidingForeignObject = dragObject;
        }
        else
            _currentCollidingForeignObject = null;
    }

    public DragObject CurrentCollidingForeignObject { get { return _currentCollidingForeignObject; } }

    public Vector3 GetUIPosition()
    {
        //Vector3 screenToWorldPosition = Camera.main.ScreenToWorldPoint(rectTransform.transform.position);
        //Vector3 screenToWorldPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, this.transform.position);
        Vector3 screenToWorldPosition = rectTransform.position;
        return screenToWorldPosition;
    }

    public GameObject OccupiedObject
    {
        get 
        {
            RollDiceViewTransform child = transform.GetComponentInChildren<RollDiceViewTransform>();
            if (child)
                return child.gameObject;
            else
                return null;
        }
    }

    public void ReceiveObject(GameObject go)
    {
        // Eject Old object
        EjectObject();

        // Receive new 
        go.transform.SetParent(this.transform);
        go.transform.localPosition = new Vector3(0, 0, go.transform.localPosition.z);
    }

    public void EjectObject()
    {
        RollDiceViewTransform v = transform.GetComponentInChildren<RollDiceViewTransform>();
        if (v)
            v.transform.SetParent(this.transform.parent);
    }

    public bool IsOccupied
    {
        get 
        {
            if (transform.GetComponentInChildren<RollDiceViewTransform>())
                return true;
            else
                return false;
        }
    }

    public void ClearCurrentCollidingObject()
    {
        _currentCollidingForeignObject = null;
    }
}
