using UnityEngine;
using System.Collections.Generic;

public class Player : Actor {

    float attackRadius = 3.5f;
    float attackAngle = 160;
    float attackX = 1.5f;
    float attackY = 5;
    public CombatUtility.AttackRangeType attackRangeType = CombatUtility.AttackRangeType.Cone;

    public List<Mob> interativeMobs = new List<Mob>();

    public override void Init(int _id) {
        type = SceneObjType.Player;
        base.Init(_id);
    }

    public override void Attack() {
        base.Attack();

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
        CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
        List<Mob> targets = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        foreach (Mob m in targets) {
            if (!m.IsDie) {
                m.Hurt();
            }
        }
    }

    public override void Hurt() {
        base.Hurt();
    }

    public override void Dead() {
        base.Dead();
    }

    void OnDrawGizmos() {

        Gizmos.color = Color.blue;

        interativeMobs.Clear();
        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;


        if (attackRangeType == CombatUtility.AttackRangeType.Cone) {
            
            Gizmos.DrawLine(_pos, _pos + _dir * attackRadius);

            Quaternion q1 = Quaternion.Euler(0, attackAngle / 2, 0);
            Vector3 _left = q1 * _dir;
            Gizmos.DrawLine(_pos, _pos + _left * attackRadius);

            Quaternion q2 = Quaternion.Euler(0, -attackAngle / 2, 0);
            Vector3 _right = q2 * _dir;
            Gizmos.DrawLine(_pos, _pos + _right * attackRadius);

            GizmosHelper.DrawConeArc(Quaternion.identity, _pos, _dir, attackRadius, attackAngle);

            CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
            interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Circle) {
            
            GizmosHelper.DrawCircle(Quaternion.identity, _pos, attackRadius);

            CombatUtility.CombatParam_AttackRange param = CombatUtility.GetCircleParam(_pos, attackRadius);
            interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Rectangle) {

            Vector3 _left = _pos + Vector3.Cross(transform.up, _dir) * attackX;
            Vector3 _right = _pos + Vector3.Cross(_dir, transform.up) * attackX;

            Gizmos.DrawLine(_pos, _pos + _dir * attackY);
            Gizmos.DrawLine(_left, _left + _dir * attackY);
            Gizmos.DrawLine(_right, _right + _dir * attackY);
            
            Gizmos.DrawLine(_left, _right);
            Gizmos.DrawLine(_left + _dir * attackY, _right + _dir * attackY);

            CombatUtility.CombatParam_AttackRange param = CombatUtility.GetRectangleParam(_pos, _dir, attackX, attackY);
            interativeMobs = CombatUtility.GetInteractiveObjects<Mob>(SceneMng.instance, ref param);
        }

        Gizmos.color = Color.red;
        foreach (Mob m in interativeMobs) {
            Gizmos.DrawWireSphere(m.transform.position + Vector3.up * 2.2f, 0.25f);
        }
    }
}
