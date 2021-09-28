using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyAlgorithm
{
    public JourneyAlgorithm()
    {

    }

    public static MonsterEnum RollEnemy_Old(JourneyLog journeyLog)
    {
        int n = journeyLog.numberOfEncounter;

        // Max, hardest possible encounter
        int maxLevel;
        if (n < 2)
            maxLevel = 0;
        else if (n < 25)
            maxLevel = Mathf.RoundToInt(n / 2f);
        else
            maxLevel = System.Enum.GetNames(typeof(MonsterEnum)).Length;

        // Min is max - 1
        int minLevel = maxLevel - 1;
        minLevel = Mathf.Clamp(minLevel, 0, minLevel);

        // Roll probability of lower monster
        MonsterEnum m;
        float roll = Random.value;
        if (roll > 0.05f) // Extremely easy enemy
            m = (MonsterEnum)(Mathf.Clamp(minLevel - 1, 0, minLevel - 1));
        else if (roll > 0.333f)  // Easier enemy
            m = (MonsterEnum)maxLevel;
        else // On par enemy
            m = (MonsterEnum)minLevel;

        return m;
    }

    public static MonsterEnum RollEnemy(JourneyLog journeyLog)
    {
        int n = journeyLog.numberOfEncounter;

        if (n <= 2)
            return MonsterEnum.Slime;
        else if (n < 6)
            return MonsterEnum.TurtleShell;
        else if (n < 10)
            return MonsterEnum.Bat;
        else if (n < 12)
            return MonsterEnum.MonsterPlant;
        else if (n < 15)
            return MonsterEnum.Spider;
        else if (n < 18)
            return MonsterEnum.Skeleton;
        else if (n < 25)
        {
            if (Random.value > 0.5f)
                return MonsterEnum.Orc;
            else
                return MonsterEnum.Golem;
        }
        else
            return MonsterEnum.Dragon;
    }

}
