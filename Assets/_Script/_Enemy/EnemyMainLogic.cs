using UnityEngine;
using System.Collections;

public class EnemyMainLogic : MonoBehaviour {
	
	
	public int mPatrolAreaDistance; // patrol area
	
	public int mHoverAreaDistance; //attack area, hover when attack cd
	
	public int mAttackCdTime;
	
	public string modelName;
	
	public enum EnemyState
	{
		None,
		Idel,
		Patrol,
		Hover,
		Run,
		Attack,
		BeHit,
		Die,
		BeCatch,
	};
	 
	public EnemyState mState = EnemyState.Patrol;
	
	private Transform player;
	
	
	private EnemyAnimationControl enemyAnimationScript;
	
	
	private float mAttackTime;
    private Mob mob;
	
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("player").transform;
		enemyAnimationScript = (EnemyAnimationControl)this.transform.FindChild(modelName).GetComponent<EnemyAnimationControl>();
        mob = gameObject.AddComponent<Mob>();
        mob.Init(SceneObjType.Mob, CombatUtility.GenNextMobID());
        SceneMng.instance.AddSceneObj(mob);
	}
	
	// Update is called once per frame
	void Update () {
		
		switch(mState)
		{
		case EnemyState.Patrol:
			if(DistanceFromPlayer() < mPatrolAreaDistance)
			{
				mState = EnemyState.Run;
				enemyAnimationScript.UpdateEnemyStateForAnimation(true);
			}
			break;
		
		case EnemyState.Run:
			if(DistanceFromPlayer() < mHoverAreaDistance)
			{
				mState = EnemyState.Hover;
				enemyAnimationScript.UpdateEnemyStateForAnimation(true);
			}
			break;
			
		case EnemyState.Hover:
			float dis = DistanceFromPlayer(); 
			if(dis > mPatrolAreaDistance)
			{
				mState = EnemyState.Patrol;
				enemyAnimationScript.UpdateEnemyStateForAnimation(true);
				return;
			}
			if(dis > mHoverAreaDistance)
			{
				mState = EnemyState.Run;
				enemyAnimationScript.UpdateEnemyStateForAnimation(true);
				return;
			}
			if(IsReadyToAttack())
			{
				mState = EnemyState.Attack;
				enemyAnimationScript.UpdateEnemyStateForAnimation(true);
				return;
			}
			break;
			
			
		case EnemyState.Attack:
			float dis1 = DistanceFromPlayer(); 
			if(dis1 > mPatrolAreaDistance)
			{
				mState = EnemyState.Patrol;
				enemyAnimationScript.UpdateEnemyStateForAnimation(false);
				return;
			}
			if(dis1 > mHoverAreaDistance)
			{
				mState = EnemyState.Run;
				enemyAnimationScript.UpdateEnemyStateForAnimation(false);
				return;
			}
			if(IsReadyToAttack())
			{
				mState = EnemyState.Attack;
				enemyAnimationScript.UpdateEnemyStateForAnimation(false);
				return;
			}
			else
			{
				mState = EnemyState.Hover;
				enemyAnimationScript.UpdateEnemyStateForAnimation(false);
			}
			break;
		}
	}
	
	bool IsReadyToAttack()
	{
		if(Time.time - mAttackTime > Random.Range(mAttackCdTime-1,mAttackCdTime+1))
		{
			mAttackTime = Time.time;
			return true;
		}
		else
			return false;
	}
	
	
	float DistanceFromPlayer()
	{
		Vector3 pos = player.position;
		pos.y = this.transform.position.y;
		float dis = Vector3.Distance(this.transform.position, pos);
		return dis;
	}
}
