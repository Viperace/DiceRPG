using UnityEngine;

public class _TestSelectMonster : MonoBehaviour
{
    void Start()
    {
        
    }

    public void RandomSelectMonster()
    {
        MonsterAvatarController m = FindObjectOfType<MonsterAvatarController>();
        int n = System.Enum.GetNames(typeof(MonsterEnum)).Length;
        int roll = Random.Range(0, n);
        m.SelectAvatar((MonsterEnum)roll);
    }
}
