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
		Dead,
	};
	 
	public EnemyState mState = EnemyState.Patrol;
	
	private Transform player;
	
	private EnemyAnimationControl enemyAnimationScript;
	private EnemyMoveBase enemyMoveBaseScript;
    private Mob mob;
	
	private float mAttackTime;

	
	// Use this for initialization
	void Awake () {
		player = GameObject.FindGameObjectWithTag("player").transform;
		enemyAnimationScript = GetComponentInChildren<EnemyAnimationControl>() as EnemyAnimationControl;
		enemyMoveBaseScript = gameObject.GetComponent<EnemyMoveBase>() as EnemyMoveBase;
		mob = gameObject.GetComponent<Mob>();
	}

    public void Init() {
        mState = EnemyState.Patrol;
        transform.collider.enabled = true;
        transform.rigidbody.isKinematic = false;
        enemyAnimationScript.enabled = true;
    }

	// Update is called once per frame
	void Update () {
		
		if(mState == EnemyState.Dead)
		{
			enemyAnimationScript.enabled = false;
			this.transform.collider.enabled = false;
			this.transform.rigidbody.isKinematic = true;
			//enemyMoveBaseScript.enabled = false;
			//Destroy(this.gameObject,2);
			return;
		}
		
		if(mState == EnemyState.Die)
		{
			Debug.Log("be dead");
			ChangeAnimationByState(EnemyState.Die,true);
			mState = EnemyState.Dead;
			return;
		}
		switch(mState)
		{
		case EnemyState.Patrol:
			if(DistanceFromPlayer() < mPatrolAreaDistance)
			{
				ChangeAnimationByState(EnemyState.Run,true);
			}
			break;
		
		case EnemyState.Run:
			if(DistanceFromPlayer() < mHoverAreaDistance)
			{
				ChangeAnimationByState(EnemyState.Hover,true);
			}
			break;
			
		case EnemyState.Hover:
			float dis = DistanceFromPlayer(); 
			if(dis > mPatrolAreaDistance)
			{
				ChangeAnimationByState(EnemyState.Patrol,true);
				return;
			}
			if(dis > mHoverAreaDistance)
			{
				ChangeAnimationByState(EnemyState.Run,true);
				return;
			}
			if(IsReadyToAttack())
			{
				Attack(true);
				return;
			}
			break;
			
			
		case EnemyState.Attack:
			float dis1 = DistanceFromPlayer(); 
			if(dis1 > mPatrolAreaDistance)
			{
				ChangeAnimationByState(EnemyState.Patrol,false);
				return;
			}
			if(dis1 > mHoverAreaDistance)
			{
				ChangeAnimationByState(EnemyState.Run,false);
				return;
			}
			if(IsReadyToAttack())
			{
				Attack(true);
				return;
			}
			else
			{
				ChangeAnimationByState(EnemyState.Hover,false);
			}
			break;
		}
		CheckEnemyMovementState();
	}
	
	
	void CheckEnemyMovementState()
	{
		switch(enemyMoveBaseScript.curMovementState)
		{
		case EnemyMoveBase.EnemyMovementState.BeHitOver:
			mState = EnemyState.Hover;
			break;
		}
	}
	
	public void ChangeAnimationByState(EnemyState state, bool immedilate)
	{
		if(state == EnemyState.BeHit)
		{
			mAttackTime = Time.time;
			CancelInvoke("MobAttack");
		}
		mState = state;
		enemyAnimationScript.UpdateEnemyStateForAnimation(immedilate);
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
	
	
	void Attack(bool immedilate)
	{
		//Debug.Log("Monster attack now!!");
		ChangeAnimationByState(EnemyState.Attack,immedilate);
		Invoke("MobAttack",0.8f);	
	}
	
	
	void MobAttack()
	{
		CancelInvoke("MobAttack");
		mob.Attack();
	}
	
}
