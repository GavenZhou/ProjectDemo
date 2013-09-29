using UnityEngine;
using System.Collections.Generic;

public class Mob : Actor {
	
    float attackRadius = 3.5f;
    float attackAngle = 60;

	EnemyMainLogic enemyMainLogic;

    public override void Init(int _id) {

        // 
        base.Init(_id); 
        
        type = SceneObjType.Player;
        Hp = MaxHp = 100;
        enemyMainLogic = (EnemyMainLogic)this.transform.GetComponent<EnemyMainLogic>();
    }

    public override void Attack() {
        
        //
        base.Attack();

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
        CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
        List<Actor> targets = CombatUtility.GetInteractiveObjects<Actor>(SceneMng.instance, ref param);
        foreach (Actor actor in targets) {
            if (!actor.IsDie) {
                actor.Hurt(this);
            }
        }
    }

    public override bool Hurt(SceneObj _object) {
        Hp -= 10;
        bool isdead = base.Hurt(_object);
		Debug.Log("mState="+enemyMainLogic.mState);
		if(isdead)
			enemyMainLogic.mState = EnemyMainLogic.EnemyState.Die;
		else
			enemyMainLogic.ChangeAnimationByState(EnemyMainLogic.EnemyState.BeHit,true);
		return isdead;
    }

    public override void Dead(SceneObj _object) {
        base.Dead(_object);
    }
}
