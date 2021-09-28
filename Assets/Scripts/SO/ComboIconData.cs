using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboIcon", menuName = "ScriptableObjects/ComboIcon", order = 1)]
public class ComboIconData : ScriptableObject
{
    public Sprite doubleIcon;
    public Sprite tripleIcon;
    public Sprite fourAKindIcon;
    public Sprite allSameIcon;
    public Sprite sumEqualIcon;
    public Sprite straightIcon;
    public int test = 2;
}
