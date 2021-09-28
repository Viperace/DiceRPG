using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JumpToEmptySlot : MonoBehaviour
{
    GameObject slotParent;
    List<RectTransform> slots;
    void Start()
    {
        slots = new List<RectTransform>();
        slotParent = GameObject.Find("EmptySlotsPoints");
        for (int i = 0; i < slotParent.transform.childCount; i++)
            slots.Add(slotParent.transform.GetChild(i).GetComponent<RectTransform>());
    }

    public void JumpToRandomEmptySlot()
    {
        foreach (var slot in slots)
            if(slot.childCount == 0)
            {
                this.transform.SetParent(slot);
                this.transform.DOLocalJump(new Vector3(0, 0, this.transform.localPosition.z), 5, 2, 0.1f);
                //this.GetComponent<RectTransform>().DOLocalJump(new Vector3(0, 0, this.transform.localPosition.z), 10, 2, 0.1f);
                //this.transform.DOJump(slot.transform.position, 10, 2, 0.5f);
            }
    }
}
