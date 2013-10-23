using UnityEngine;
using System.Collections.Generic;

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

    public static List<Vector3> GetConeArcPoints(Vector3 _center, Vector3 _forward, float _radius, float _totoalDegree, int _segments = 12) {

        // 
        _totoalDegree = Mathf.Clamp(_totoalDegree, 0, 360);
        
        Quaternion _rot = Quaternion.identity;
        float step = _totoalDegree * Mathf.Deg2Rad / _segments;
        float deta = 0f;
        float theta = 0f;
        Vector3 up = Vector3.Cross(Vector3.right, _forward);
        if (up.y < 0) {
            theta = Mathf.Acos(Vector3.Dot(Vector3.right, _forward));
        }
        else {
            theta = -1 * Mathf.Acos(Vector3.Dot(Vector3.right, _forward));
        }

        List<Vector3> points = new List<Vector3>();
        Vector3 last1 = _center + _rot * (_radius * new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta)));
        Vector3 last2 = last1;
        points.Add(last1);
        deta += step;

        for (int i = 1; i <= _segments; ++i) {
            last1 = _center + _rot * (_radius * new Vector3(Mathf.Cos(theta + deta), 0.0f, Mathf.Sin(theta + deta)));
            points.Add(last1);
            deta += step;
        }
        return points;
    }
}
