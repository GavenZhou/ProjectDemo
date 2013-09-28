using UnityEngine;
using System.Collections.Generic;

public class Player : Actor {

    float attackRadius = 5;
    float attackAngle = 90;
    float attackX = 1.5f;
    float attackY = 5;
    CombatUtility.AttackRangeType attackRangeType = CombatUtility.AttackRangeType.Rectangle;

    public List<Mob> interativeMobs = new List<Mob>();

    public override void Init(int _id) {
        type = SceneObjType.Player;
        base.Init(_id);
    }

    public void Attack() {
        Debug.Log("Attack");

        //Vector3 _pos = transform.position;
        //Vector3 _dir = transform.forward;
        //CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, 160 * Mathf.Deg2Rad, 10);
        //interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
    }

    void OnDrawGizmos() {

        Gizmos.color = Color.blue;

        interativeMobs.Clear();
        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
        //CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, 160 * Mathf.Deg2Rad, 5);
        //interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);


        if (attackRangeType == CombatUtility.AttackRangeType.Cone) {
            Gizmos.DrawLine(_pos + Vector3.up, _pos + _dir * attackRadius + Vector3.up);
            Quaternion q1 = Quaternion.Euler(0, attackAngle / 2, 0);
            Vector3 _left = q1* _dir;
            Gizmos.DrawLine(_pos + Vector3.up, _pos + _left * attackRadius + Vector3.up);
            Gizmos.DrawLine(_pos + _left * attackRadius + Vector3.up, _pos + _dir * attackRadius + Vector3.up);
            Quaternion q2 = Quaternion.Euler(0, -attackAngle / 2, 0);
            Vector3 _right = q2* _dir;
            Gizmos.DrawLine(_pos + Vector3.up, _pos + _right * attackRadius + Vector3.up);
            Gizmos.DrawLine(_pos + _right * attackRadius + Vector3.up, _pos + _dir * attackRadius + Vector3.up);

            //CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, 160 * Mathf.Deg2Rad, 5);
            //interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Circle) {
            CombatUtility.CombatParam_AttackRange param = CombatUtility.GetCircleParam(_pos, attackRadius);
            interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Rectangle) {
            Vector3 _left = _pos + Vector3.Cross(transform.up, _dir) * attackX;
            Vector3 _right = _pos + Vector3.Cross(_dir, transform.up) * attackX;

            Gizmos.DrawLine(_pos + Vector3.up, _pos + _dir * attackY + Vector3.up);
            Gizmos.DrawLine(_left + Vector3.up, _left + _dir * attackY + Vector3.up);
            Gizmos.DrawLine(_right + Vector3.up, _right + _dir * attackY + Vector3.up);
            
            Gizmos.DrawLine(_left + Vector3.up, _right + Vector3.up);
            Gizmos.DrawLine(_left + _dir * attackY + Vector3.up, _right + _dir * attackY + Vector3.up);

            CombatUtility.CombatParam_AttackRange param = CombatUtility.GetRectangleParam(_pos, _dir, attackX, attackY);
            interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        }

        Gizmos.color = Color.red;
        foreach (Mob m in interativeMobs) {
            Gizmos.DrawWireSphere(m.transform.position + Vector3.up * 2.2f, 0.25f);
        }
    }
}
