using UnityEngine;

public class GizmosHelper
{
    public static void DrawCircle(Quaternion _rot, Vector3 _center, float _radius) {

        DrawConeArc(_rot, _center, Vector3.right, _radius, 360.0f);
    }

    public static void DrawConeArc(Quaternion _rot, Vector3 _center, Vector3 _forward, float _radius, float _angle) {

        _angle = Mathf.Clamp(_angle, 0, 360);
        float segments = 32.0f;
        float step = _angle * Mathf.Deg2Rad / segments;
        float deta = 0f;
        float theta = 0f;
        Vector3 up = Vector3.Cross(Vector3.right, _forward);
        if (up.y < 0) {
            theta = Mathf.Acos(Vector3.Dot(Vector3.right, _forward));
        }
        else {
            theta = -1 * Mathf.Acos(Vector3.Dot(Vector3.right, _forward));
        }

        Vector3 last1 = _center + _rot * (_radius * new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta)));
        Vector3 last2 = last1;
        deta += step;

        for (int i = 1; i <= segments / 2; ++i) {
            Vector3 cur = _center + _rot * (_radius * new Vector3(Mathf.Cos(theta + deta), 0.0f, Mathf.Sin(theta + deta)));
            Gizmos.DrawLine(last1, cur);
            last1 = cur;

            cur = _center + _rot * (_radius * new Vector3(Mathf.Cos(theta - deta), 0.0f, Mathf.Sin(theta - deta)));
            Gizmos.DrawLine(last2, cur);
            last2 = cur;

            deta += step;
        }
    }
}
