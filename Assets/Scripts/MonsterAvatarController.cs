using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAvatarController : MonoBehaviour
{
    Animator[] monsters;
    MonsterInfoBehavior[] monsterInfos;
    void OnEnable()
    {
        monsters = GetComponentsInChildren<Animator>(true);
        monsterInfos = GetComponentsInChildren<MonsterInfoBehavior>(true);
    }

    public void SelectAvatar(MonsterEnum monsterEnum)
    {
        foreach (var m in monsterInfos)
        {
            if(m.monsterEnum == monsterEnum)
                m.gameObject.SetActive(true);
            else
                m.gameObject.SetActive(false);
        }
    }
}
