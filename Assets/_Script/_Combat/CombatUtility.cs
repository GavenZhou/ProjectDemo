using UnityEngine;
using System.Collections.Generic;
using System;

public static class CombatUtility {

    ///////////////////////////////////////////////////////////////////////////////
    // 战斗区域计算
    ///////////////////////////////////////////////////////////////////////////////

    public enum AttackRangeType {
        None,
        Circle,
        Cone,
        Rectangle,
    }

    public struct CombatParam_AttackRange {

        public AttackRangeType type;
        public Vector3 pos;
        public Vector3 dir;

        public float radius;
        public float radians;
        public float x;
        public float y;

        public Func<Vector3, bool> prep;
    }

    public static CombatParam_AttackRange GetCircleParam(Vector3 _pos, float _radius) {
        CombatParam_AttackRange param = new CombatParam_AttackRange();
        param.type = AttackRangeType.Circle;
        param.pos = _pos;
        param.radius = _radius;
        param.prep = (vec) => {
            vec.y = 0;
            return (vec - param.pos).sqrMagnitude <= _radius * _radius;
        };
        return param;
    }

    public static CombatParam_AttackRange GetConeParam(Vector3 _pos, Vector3 _dir, float _radians, float _radius) {
        CombatParam_AttackRange param = new CombatParam_AttackRange();
        param.type = AttackRangeType.Cone;
        param.pos = _pos;
        param.radius = _radius;
        param.dir = Vector3.Normalize(new Vector3(_dir.x, 0, _dir.z));
        param.radians = _radians;
        param.prep = (vec) => {
            if ((vec - param.pos).sqrMagnitude <= _radius * _radius) {
                Vector3 v = vec - _pos;
                v = Vector3.Normalize(new Vector3(v.x, 0, v.z));
                float dot = Mathf.Clamp01(Vector3.Dot(v, param.dir));
                float r = Mathf.Acos(dot);
                return r >= 0 && r <= param.radians / 2;
            }
            return false;
        };
        return param;
    }

    public static CombatParam_AttackRange GetRectangleParam(Vector3 _pos, Vector3 _dir, float _x, float _y) {
        CombatParam_AttackRange param = new CombatParam_AttackRange();
        param.type = AttackRangeType.Rectangle;
        param.pos = _pos;
        param.dir = Vector3.Normalize(new Vector3(_dir.x, 0, _dir.z));
        param.x = _x;
        param.y = _y;
        param.prep = (vec) => {

            Vector3 _left = _pos + Vector3.Cross(Vector3.up, _dir) * _x;
            Vector3 _right = _pos + Vector3.Cross(_dir, Vector3.up) * _x;
            Vector3 _leftFront = _left + _dir * _y;
            Vector3 _rightFront = _right + _dir * _y;

            Vector2[] rect = new Vector2[4] {
                new Vector2(_left.x, _left.z),
                new Vector2(_leftFront.x, _leftFront.z),
                new Vector2(_rightFront.x, _rightFront.z),
                new Vector2(_right.x, _right.z)
            };
            return Misc.InsidePolygon(ref rect, 4, new Vector2(vec.x, vec.z));
        };
        return param;
    }

    public static List<T> GetInteractiveObjects<T>(SceneMng _scene, ref CombatParam_AttackRange _param) where T : SceneObj {

        List<T> objs = new List<T>();
        if (_scene != null && _param.prep != null) {
            List<T> sceneObjs = _scene.GetSceneObjs<T>();
            foreach (T obj in sceneObjs) {
                if (_param.prep(obj.transform.position)) {
                    objs.Add(obj);
                }
            }
        }
        return objs;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // 怪物生成
    ///////////////////////////////////////////////////////////////////////////////

    static int mobID = 0;
    public static int GenNextMobID() {
        return ++mobID;
    }



    ///////////////////////////////////////////////////////////////////////////////
    // 掉落计算
    ///////////////////////////////////////////////////////////////////////////////


    
}

