using UnityEngine;
using System.Collections;

public class Mob : Actor {
	
	EnemyMainLogic enemyMainLogic;

    public override void Init(int _id) {
		
		enemyMainLogic = (EnemyMainLogic)this.transform.GetComponent<EnemyMainLogic>();
        type = SceneObjType.Player;
        base.Init(_id);
    }

    public override void Attack() {
        base.Attack();
    }

    public override void Hurt() {
        base.Hurt();
		enemyMainLogic.ChangeAnimationByState(EnemyMainLogic.EnemyState.BeHit,true);
    }

    public override void Dead() {
        base.Dead();
    }

}
