using UnityEngine;
using System.Collections;

public static class Misc {

    public static bool InsidePolygon(ref Vector2[] _polygon, int _N, Vector2 _p)
    {
        bool inside = false;
        int count1 = 0;
        int count2 = 0; 

        for (int i = 0, j = _N - 1; i < _N; j = i++)  {

            double value = (_p.x - _polygon[j].x) * (_polygon[i].y - _polygon[j].y) - (_p.y - _polygon[j].y) * (_polygon[i].x - _polygon[j].x);
            if (value > 0)
                ++count1;
            else if (value < 0)
                ++count2;
        }

        if (0 == count1 || 0 == count2) {
            inside = true;
        }
        return inside;
    }
}
