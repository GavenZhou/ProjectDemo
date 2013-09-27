using UnityEngine;
using System.Collections.Generic;
using System;

public static class CombatUtility {

    ///////////////////////////////////////////////////////////////////////////////
    // 战斗区域计算
    ///////////////////////////////////////////////////////////////////////////////

    public enum CombatRangeType {
        None,
        Circle,
        Cone,
        Rectangle,
    }

    public struct CombatRangeParam {
        
        public CombatRangeType type;
        public Vector3 pos;
        public Vector3 dir;

        public float radius;
        public float radians;
        public float x;
        public float y;

        public Func<Vector3, bool> prep;

        private CombatRangeParam(CombatRangeType _type, Vector3 _pos)
        {
            type = _type;
            pos = _pos;
            dir = new Vector3();
            radius = radians = x = y = 0;
            prep = null;
        }

        public static CombatRangeParam CircleParam(Vector3 _pos, float _radius) {
            CombatRangeParam param = new CombatRangeParam(CombatRangeType.Circle, _pos);
            param.radius = _radius;
            param.prep = (vec) => {
                vec.y = 0;
                return (_pos - param.pos).sqrMagnitude <= _radius * _radius;
            };
            return param;
        }

        public static CombatRangeParam ConeParam(Vector3 _pos, Vector3 _dir, float _radians, float _radius) {
            CombatRangeParam param = new CombatRangeParam(CombatRangeType.Cone, _pos);
            param.radius = _radius;
            param.dir = Vector3.Normalize(new Vector3(_dir.x, 0, _dir.z));
            param.radians = _radians;
            param.prep = (vec) => {
                if ((_pos - param.pos).sqrMagnitude <= _radius * _radius) {
                    Vector3 v = vec - _pos;
                    v = Vector3.Normalize(new Vector3(v.x, 0, v.z));
                    float dot = Vector3.Dot(v, param.dir);
                    float ang = Mathf.Acos(dot) * Mathf.Rad2Deg;
                    return true;
                }
                return false;
            };
            return param;
        }

        public static CombatRangeParam RectangleParam(Vector3 _pos, Vector3 _dir, float _x, float _y) {
            CombatRangeParam param = new CombatRangeParam(CombatRangeType.Rectangle, _pos);
            param.dir = Vector3.Normalize(new Vector3(_dir.x, 0, _dir.z));
            param.x = _x;
            param.y = _y;
            param.prep = (vec) => {
                vec.y = 0;
                float xMin = Mathf.Min(param.pos.x, param.pos.x + param.dir.x * _x);
                float xMax = Mathf.Max(param.pos.x, param.pos.x + param.dir.x * _x);
                float zMin = Mathf.Max(param.pos.z, param.pos.z + param.dir.z * _y);
                float zMax = Mathf.Max(param.pos.z, param.pos.z + param.dir.z * _y);
                return (_pos.x > xMin && _pos.x < xMax) && (_pos.z > zMin && _pos.z < zMax);
            };
            return param;
        }
    }

    public static List<T> GetInteractiveObjects<T>(SceneMng _scene, ref CombatRangeParam _param) where T : SceneObj {

        List<T> objs = new List<T>();
        if (_scene != null && _param.prep != null) {
            objs = _scene.GetSceneObjs<T>();
            foreach (T obj in objs) {
                if (_param.prep(obj.transform.position)) {
                    objs.Add(obj);
                }
            }
        }
        return objs;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // 掉落计算
    ///////////////////////////////////////////////////////////////////////////////


    
}

