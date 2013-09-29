using UnityEngine;
using System.Collections.Generic;

public class Mob : Actor {
	
	EnemyMainLogic enemyMainLogic;
    float attackRadius = 3.5f;
    float attackAngle = 60;


    public override void Init(int _id) {
		
		enemyMainLogic = (EnemyMainLogic)this.transform.GetComponent<EnemyMainLogic>();
        base.Init(_id);
        type = SceneObjType.Player;
        Hp = MaxHp = 100;
    }

    public override void Attack() {
        base.Attack();

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
		enemyMainLogic.ChangeAnimationByState(EnemyMainLogic.EnemyState.BeHit,true);
        CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
        List<Actor> targets = CombatUtility.GetInteractiveObjects<Actor>(SceneMng.instance, ref param);
        foreach (Actor actor in targets) {
            if (!actor.IsDie) {
                actor.Hurt(this);
            }
        }
    }

    public override void Hurt(SceneObj _object) {
        Hp -= 10;
        base.Hurt(_object);
    }

    public override void Dead(SceneObj _object) {
        base.Dead(_object);
    }
}
