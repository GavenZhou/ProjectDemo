using System.Collections.Generic;
using UnityEngine;

public static class CombatUtility {

#region 
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
        public System.Func<Vector3, bool> prep;
    }

    public static CombatParam_AttackRange GetCircleParam(Vector3 _pos, float _radius) {

        //
        CombatParam_AttackRange param = new CombatParam_AttackRange();
        
        param.type = AttackRangeType.Circle;
        param.prep = (vec) => {
            vec.y = 0;
            return (vec - _pos).sqrMagnitude <= _radius * _radius;
        };
        return param;
    }   

    public static CombatParam_AttackRange GetConeParam(Vector3 _pos, Vector3 _dir, float _radians, float _radius) {

        _dir = Vector3.Normalize(new Vector3(_dir.x, 0, _dir.z));
        
        //
        CombatParam_AttackRange param = new CombatParam_AttackRange();

        param.type = AttackRangeType.Cone;
        param.prep = (vec) => {
            if ((vec - _pos).sqrMagnitude <= _radius * _radius) {
                Vector3 v = vec - _pos;
                v = Vector3.Normalize(new Vector3(v.x, 0, v.z));
                float dot = Mathf.Clamp01(Vector3.Dot(v, _dir));
                float r = Mathf.Acos(dot);
                return r >= 0 && r <= _radians / 2;
            }
            return false;
        };
        return param;
    }

    public static CombatParam_AttackRange GetRectangleParam(Vector3 _pos, Vector3 _dir, float _x, float _y) {

        _dir = Vector3.Normalize(new Vector3(_dir.x, 0, _dir.z));
        
        //        
        CombatParam_AttackRange param = new CombatParam_AttackRange();

        param.type = AttackRangeType.Rectangle;
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
#endregion


#region 
    ///////////////////////////////////////////////////////////////////////////////
    // 怪物生成
    ///////////////////////////////////////////////////////////////////////////////

    static int mobID = 0;
    static int maxMobTemplate = 2;
    static Vector3[] mobBornLoctions
        = {
            new Vector3(-3.60f, 0, 6.94f),
            new Vector3(-0.86f, 0, -8.60f),
            new Vector3(7.79f, 0, -4.27f),
            new Vector3(6.24f, 0, 5.57f),
            new Vector3(-7.72f, 0, -2.00f),
          };

    public static int GenNextMobID() {
        return ++mobID;
    }

    // 根据xxx算法在出生点随机生成mob, (待修改)
    public static Mob MobGenerator() {

        List<Vector3> locs = null;
        if (GetSuitableMobLocation(out locs)) {
            int loction = Random.Range(0, locs.Count - 1);
            return Spawner.instance.SpawnMob(Random.Range(0, maxMobTemplate), locs[loction]);
        }
        return null;
    }

    // 根据xxx算法检测mob生成时机
    public static bool MobGeneratorDetector() {
        if (mobID < TestScene.instance.totalMobCount) {
            if (SceneMng.instance.GetSceneObjs<Mob>().Count < TestScene.instance.maxMobInScene) {
                MobGenerator();
                return true;
            }
        }
        return false;
    }

    static float minDistance = 2.0f;
    static bool GetSuitableMobLocation(out List<Vector3> _locs) {

        _locs = new List<Vector3>();
        List<Mob> mobs = SceneMng.instance.GetSceneObjsWithPred<Mob>(m => !m.IsDied);

        foreach (Vector3 vec in mobBornLoctions) {
            bool suitable = true;
            foreach (Mob m in mobs) {
                float magnitude = new Vector2(m.transform.position.x - vec.x, m.transform.position.z - vec.z).sqrMagnitude;
                if (magnitude <= minDistance * minDistance) {
                    suitable = false;
                    break;
                }
            }
            if (suitable) _locs.Add(vec);
        }
        return _locs.Count != 0;
    }
#endregion


#region 
    ///////////////////////////////////////////////////////////////////////////////
    // 掉落计算
    ///////////////////////////////////////////////////////////////////////////////

    static int dropID = 0;
    static float dropProbability = 0.8f;
    public static int GenNextDropID() {
        return ++dropID;
    }
   
    // 根据xxx算法在计算物品掉落, (待修改)
    public static DropItem DropGenerator(Vector3 _pos) {

        if (dropProbability >= Random.Range(0.0f, 1.0f)) {
            return Spawner.instance.SpawnDrop(GetDropLocation(_pos));
        }
        return null;
    }

    static Vector3 GetDropLocation(Vector3 _center) {

        float delta = Random.Range(0, 7) * Mathf.PI / 4;
        Vector3 vec = _center + new Vector3(Mathf.Cos(delta), 0, Mathf.Sin(delta))
                        + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        vec.y = 0.2f;
        return vec ;
    }
#endregion
}

