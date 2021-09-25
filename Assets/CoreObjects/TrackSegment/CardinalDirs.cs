using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardinalDirs
{
    public static Vector2 UP => new Vector2(0, 1);
    public static Vector2 DOWN => new Vector2(0, -1);
    public static Vector2 LEFT => new Vector2(-1, 0);
    public static Vector2 RIGHT => new Vector2(1, 0);
    public static Vector2 UP_LEFT => (UP + LEFT).normalized;
    public static Vector2 UP_RIGHT => (UP + RIGHT).normalized;
    public static Vector2 DOWN_LEFT => (DOWN + LEFT).normalized;
    public static Vector2 DOWN_RIGHT => (DOWN + RIGHT).normalized;
    private static Vector2[] m_cardinalDirs;
    public static Vector2[] Dirs()
    {
        if (m_cardinalDirs == null)
        {
            m_cardinalDirs = new Vector2[8];
            m_cardinalDirs[0] = UP;
            m_cardinalDirs[1] = DOWN;
            m_cardinalDirs[2] = LEFT;
            m_cardinalDirs[3] = RIGHT;
            m_cardinalDirs[4] = UP_LEFT;
            m_cardinalDirs[5] = UP_RIGHT;
            m_cardinalDirs[6] = DOWN_LEFT;
            m_cardinalDirs[7] = DOWN_RIGHT;
        }

        return m_cardinalDirs;
    }

    public static List<Vector2> SortedBySimilarity(Vector2 dir)
    {
        List<Vector2> sortedList = new List<Vector2>();

        foreach (Vector2 vec in Dirs())
        {
            sortedList.Add(vec);
        }

        sortedList.Sort(delegate (Vector2 a, Vector2 b)
            {
                return Vector2.Distance(dir, a)
                    .CompareTo(Vector2.Distance(dir, b));
            });

        return sortedList;
    }
}
