using UnityEngine;
using UnityEngine.UI;

public class SlotType : MonoBehaviour
{
    public DiceSlotEnum slotType;

    void Start()
    {

        // If attack, turn on Sword image, If defend , turn on shield image
        for (int i = 0; i < transform.childCount; i++)        
        {
            // Ignore none-Image child
            GameObject img = transform.GetChild(i).gameObject;
            if (!img.GetComponent<Image>()) 
                continue;

            if(slotType == DiceSlotEnum.ATTACK)
            {
                if (img.name == "SwordImage")
                    img.SetActive(true);
                else
                    img.SetActive(false);
            }
            else if(slotType == DiceSlotEnum.DEFEND)
            {
                if (img.name == "ShieldImage")
                    img.SetActive(true);
                else
                    img.SetActive(false);
            }
        }
    }
}
