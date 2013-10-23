using UnityEngine;
using System.Collections.Generic;

public class Player : Actor {

    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    float attackRadius = 3.5f;
    float attackAngle = 160;
    float attackX = 0.5f;
    float attackY = 10f;

    PlayerMainLogic playerMainLogic;
    PlayerAnimationControl aniControlScript;

    public CombatUtility.AttackRangeType attackRangeType = CombatUtility.AttackRangeType.Cone;
	

    public bool IsSkillPlaying {
        get { return aniControlScript.isSkillPlaying; }
    }


    ///////////////////////////////////////////////////////////////////////////////
    // virtual
    ///////////////////////////////////////////////////////////////////////////////

    public override void Init(int _id) {

        //
        base.Init(_id);
        type = SceneObjType.Player;        
        Hp = MaxHp = 1000;

		playerMainLogic = transform.GetComponent<PlayerMainLogic>() as PlayerMainLogic;
        aniControlScript = transform.GetComponentInChildren(typeof(PlayerAnimationControl)) as PlayerAnimationControl;
    }

    public override void Attack() {
        
        //
        base.Attack();

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
        CombatUtility.CombatParam_AttackRange param;

        if (attackRangeType == CombatUtility.AttackRangeType.Cone) {
            param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Circle) {
            param = CombatUtility.GetCircleParam(_pos, attackRadius);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Rectangle) {
            param = CombatUtility.GetRectangleParam(_pos, _dir, attackX, attackY);
        }
        else {
            return;
        }

        List<Mob> targets = CombatUtility.GetInteractiveMobs(SceneMng.instance, ref param);
        foreach (Mob m in targets) {
            if (!m.IsDied) {
                m.Hurt(this, 1.0f);
            }
        }
    }

    public override bool Hurt(SceneObj _object, object _param) {
        Hp -= (int)(25 * (float)_param);
        bool isdead = base.Hurt(_object, _param);
		if(isdead)
			playerMainLogic.ChangeAnimationByActionCmd(GameBaseData.PlayerDataClass.PlayerActionCommand.Player_Die,true,true);
		else
		{
			playerMainLogic.mContinueAttack = false;
			playerMainLogic.ChangeAnimationByActionCmd(GameBaseData.PlayerDataClass.PlayerActionCommand.Player_BeHit,true,false);
		}
		return isdead;
    }

    public override void Dead(SceneObj _object) {
        base.Dead(_object);
    }

    public virtual void PlaySkill(int _skillID) {
        
        Vector3 _pos = transform.position;
        CombatUtility.CombatParam_AttackRange param;

        param = CombatUtility.GetCircleParam(_pos, attackRadius);

        List<Mob> targets = CombatUtility.GetInteractiveMobs(SceneMng.instance, ref param);
        foreach (Mob m in targets) {
            if (!m.IsDied) {
                m.Hurt(this, 1.5f);
            }
        }
    }

    public Transform GetNearestAttackTarget(Vector3 dir) {

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
        CombatUtility.CombatParam_AttackRange param
            = CombatUtility.GetConeParam(_pos, dir, attackAngle * Mathf.Deg2Rad, attackRadius);

        List<Mob> targets = CombatUtility.GetInteractiveMobs(SceneMng.instance, ref param);
        Mob mob = CombatUtility.GetNearestObject<Mob>(SceneMng.instance, transform, ref targets);
        return mob != null ? mob.transform : null;
    }


    ///////////////////////////////////////////////////////////////////////////////
    // Gizmos & Debug
    ///////////////////////////////////////////////////////////////////////////////

    void OnDrawGizmos() {

        Gizmos.color = Color.blue;

        List<Mob> interativeMobs = new List<Mob>();
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
            interativeMobs = CombatUtility.GetInteractiveMobs(SceneMng.instance, ref param);
        }
        else if (attackRangeType == CombatUtility.AttackRangeType.Circle) {
            
            GizmosHelper.DrawCircle(Quaternion.identity, _pos, attackRadius);

            CombatUtility.CombatParam_AttackRange param = CombatUtility.GetCircleParam(_pos, attackRadius);
            interativeMobs = CombatUtility.GetInteractiveMobs(SceneMng.instance, ref param);
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
            interativeMobs = CombatUtility.GetInteractiveMobs(SceneMng.instance, ref param);
        }

        Gizmos.color = Color.red;
        foreach (Mob m in interativeMobs) {
            Gizmos.DrawWireSphere(m.transform.position + Vector3.up * 2.2f, 0.25f);
        }
    }
}
