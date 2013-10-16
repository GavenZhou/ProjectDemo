using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerMainLogic : MonoBehaviour {
	
	
	public enum TouchState
	{
		None,
		AFingerOneTap,
		AFingerDoubleTap,
		AFingerHold,
		AFingerHoldStop,
		AFingerSlash,
		TwoFingersTap,
		TwoFingersDoubleTap,
		TwoFingersSlash,
	}
	
	public TouchState mTouchState = TouchState.None;
	
	private RaycastHit[] hit;
	
	private PlayerAnimationControl aniControlScript;
	
	private PlayerMoveBase moveBaseScript;
	
    private Player player;
	
	private Transform directionIcon;
	
    public GameObject[] attackSfx;
    
    public GameObject pickupSfx;
	
	public GameObject skill1Sfx;

	public Transform mTarget;
	
	private Transform specialAttackIcon;
	
	
	// Use this for initialization
	void Start () {
		aniControlScript = (PlayerAnimationControl)this.transform.FindChild("character").GetComponent<PlayerAnimationControl>();
		
		moveBaseScript = (PlayerMoveBase)this.GetComponent<PlayerMoveBase>();
		
		directionIcon = this.transform.FindChild("icon");
		specialAttackIcon = this.transform.FindChild("SpecialAttackIcon");
		
        player = gameObject.AddComponent<Player>();
        player.Init(1000);
        SceneMng.instance.AddMainPlayer(player);
	}
	
	// Update is called once per frame
	void Update () {

        if(mTarget == null) {
            mTarget = player.GetNearestAttackTarget();
        }	

		if(aniControlScript.attack1Finish == true)
		{
//			Debug.Log("aniControlScript.attack1Finish");
			aniControlScript.attack1Finish = false;
			
			//check whether need to start the attack2
			mTarget = player.GetNearestAttackTarget();
			if(mTarget != null)
			{
				aniControlScript.attack1AnimationFinish = 2;
				aniControlScript.attack2AnimationFinish = 0;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack2,true);
				return;
			}
			else
			{
				mTarget = null;
				PlayerDataClass.AttackStart = false;
			}
		}
		
		if(aniControlScript.attack2Finish == true)
		{
//			Debug.Log("aniControlScript.attack2Finish");
			aniControlScript.attack2Finish = false;
			
			//check whether need to start the attack3
			mTarget = player.GetNearestAttackTarget();
			if(mTarget != null)
			{
				aniControlScript.attack2AnimationFinish = 2;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,true);
				return;
			}
			else
			{
				mTarget = null;
				PlayerDataClass.AttackStart = false;
			}
		}
		
		if(aniControlScript.attack3Finish == true)
		{
			aniControlScript.attack3Finish = false;
			PlayerDataClass.AttackStart = false;
			
			// add cd time here
			//todo
			mTarget = player.GetNearestAttackTarget();
		}
		
		
		if(mTarget != null 
			&& PlayerDataClass.AttackStart == false
			&& aniControlScript.isSkillPlaying == false)
		{
			// attack start
			PlayerDataClass.AttackStart = true;
			
			//turn to the target
			TargetTheEnemy(mTarget.position);
			
//			Debug.Log("here");
			// attack!
			aniControlScript.attack1AnimationFinish = 0;
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack1,true);
			return;
		}
		
        //if(colliderBaseScript.mCatchTarget != null)
        //{
        //    colliderBaseScript.turnOnCatchRay = false;
        //    ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Catch,true);
        //}
		
		CheckMoveState();
		
	}
	
	void CheckMoveState()
	{
		if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run)
		{
			if(PlayerDataClass.isSpecialAttack == false)
			{
				if(PlayerDataClass.CheckSpecialAttack())
					specialAttackIcon.renderer.enabled = true;
			}
		}
		else
		{
			specialAttackIcon.renderer.enabled = false;
			PlayerDataClass.ResetSpecialAttackTime();
		}
		
		
		bool immedilate = false;
		switch(moveBaseScript.curMovementState)
		{
		case PlayerMoveBase.PlayerMovementState.RunOver:
		case PlayerMoveBase.PlayerMovementState.TrotOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Walk,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.WalkOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.BeHitOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack1Over:
//			Debug.Log(aniControlScript.attack1AnimationFinish);
			if(aniControlScript.attack1AnimationFinish > 0)
			{
				if(aniControlScript.attack1AnimationFinish == 1)
					immedilate = false;
				else
					immedilate = true;
				
				aniControlScript.attack1AnimationFinish = 0;
				
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,immedilate);
				mTarget = null;
			}
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack2Over:
//			Debug.Log(aniControlScript.attack2AnimationFinish);
			if(aniControlScript.attack2AnimationFinish > 0)
			{
				if(aniControlScript.attack2AnimationFinish == 1)
					immedilate = false;
				else
					immedilate = true;
				
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,immedilate);
				mTarget = null;
			}
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack3Over:
//			Debug.Log(aniControlScript.attack3AnimationFinish);
			if(aniControlScript.attack3AnimationFinish > 0)
			{
				aniControlScript.attack3AnimationFinish = 0;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
				mTarget = null;
			}
			break;	
		
		case PlayerMoveBase.PlayerMovementState.JumpOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.RushOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Catch,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.CatchOver:
	//		ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
	//		colliderBaseScript.mTarget = null;
			break;
			
		}
	}
	
	
	public void ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand cmd, bool immedilate)
	{
//		Debug.Log("ChangeAnimationByActionCmd =====  "+cmd);
		PlayerDataClass.PlayerNextActionSet(cmd);
		aniControlScript.UpdateCmdFromControl(immedilate);
	}
	
	// only call by other script
	public void GetInputState()
	{
		Vector3 targetPos ;
		switch(mTouchState)
		{
		case TouchState.AFingerOneTap:
			
			mTarget = null;
			PlayerDataClass.AttackStart = false;
			mTouchState = TouchState.None;
			
			targetPos = RayColliderByTapPos(InputStateClass.touchPointPos);
			
			if(targetPos != Vector3.zero)
			{
				// turn to the direction first
			//	SetPlayerToRun(targetPos-this.transform.position);
				
				if(directionIcon.renderer.enabled == true)
				{
					directionIcon.renderer.enabled = false;
					Time.timeScale = 1;
					return;
				}
				
				if(aniControlScript.isSkillPlaying)
				{
					return;
				}
				
				// check the attack area
                mTarget = player.GetNearestAttackTarget();
				
				// if there is no enemy in the area, then player start to run
				if(mTarget == null)
				{
					if(moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Jump
						&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack1
						&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack2
						&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack3)
					{
						float dis = Vector3.Distance(targetPos,transform.position);
						if(dis < 1)
						{				
							ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
						}
						else
						{
							moveBaseScript.targetPos = targetPos;
							if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run)
								ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,false);
							else
								ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,true);
						}
					}
				}
			}
			break;
			
		case TouchState.AFingerDoubleTap:
			mTouchState = TouchState.None;
			if(moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Jump)
			{
				targetPos = RayColliderByTapPos(InputStateClass.touchPointPos);
				SetPlayerToRun(targetPos-this.transform.position);	
				if(aniControlScript.isSkillPlaying == false)
				{
					ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Jump,true);
				}				
			}
			break;
			
		case TouchState.AFingerHold:
			if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Attack3)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,false);
			else
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,true);
				
        //    mTouchState = TouchState.None;
			break;	
		
		case TouchState.AFingerHoldStop:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
            mTouchState = TouchState.None;
			break;
			
		case TouchState.AFingerSlash:
//            colliderBaseScript.turnOnCatchRay = true;
//            colliderBaseScript.mCatchTarget = null;
            mTouchState = TouchState.None;
            //targetPos = RayColliderByTapPos(InputStateClass.oldSlashPos);
            //SetPlayerToRun(targetPos-this.transform.position);
            //if(aniControlScript.isSkillPlaying == false)
            //{
            //    ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Rush,true);
            //}			
			break;
			
		case TouchState.TwoFingersDoubleTap:
			mTouchState = TouchState.None;
			break;
			
		case TouchState.TwoFingersSlash:
			mTouchState = TouchState.None;
			break;
			
		case TouchState.TwoFingersTap:
			mTouchState = TouchState.None;
			break;
			
		default:
			break;
		}
	}
	
	Vector3 RayColliderByTapPos(Vector3 pos)
	{
		// NGUI area
		if(pos.x > Screen.width *0.8f)
			return Vector3.zero;
		
		Ray ray1 = Camera.mainCamera.ScreenPointToRay(pos);
		
		hit = Physics.RaycastAll(ray1,100,1<<1);
		foreach(RaycastHit hit1 in hit)
		{
			if(hit1.transform.tag == "ground")
				return hit1.point;
		}
		return Vector3.zero;
	}
	
	
	void SetPlayerToRun(Vector3 pos)
	{
		pos.y = transform.position.y;
		
		Vector3 dir = Vector3.Cross(transform.forward,pos.normalized);
		float dot = Vector3.Dot(transform.forward,pos.normalized);
		float angle = (float)Mathf.Acos(dot)*Mathf.Rad2Deg;
//		Debug.Log("pos="+pos+"  dir="+dir+"  dot="+dot+"  angle"+angle);
		
		if(float.IsNaN(angle))
		{
			angle = 0;
		}
		if(dir.y  < 0)
			angle = -1*angle;
		
		transform.RotateAround(transform.position,Vector3.up,angle);
	}
	
	void NGUI_Skill1()
	{			
		Time.timeScale = 0.2f;
		ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Skill1,true);
		directionIcon.renderer.enabled = true;
		aniControlScript.isSkillPlaying = true;
        skill1Sfx.GetComponent<particleControl>().turnOn = true;
	}
	
	
	
	
	public void OnStartSkill(int skillId)
	{
		Time.timeScale = 1;
		directionIcon.renderer.enabled = false;
		ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,false);
	}

	public void OnSkillShootPoint(int skillId)
	{
        player.PlaySkill(skillId);
	}

	void TargetTheEnemy(Vector3 enemy)
	{
		enemy.y = transform.position.y;
		transform.LookAt(enemy);
	}

    public void OnStartToAttack(int attackId)
    {
        attackSfx[attackId - 1].GetComponent<particleControl>().turnOn = true;
        player.Attack();
    }

    public void OnAttackFinish(int attackId)
    {

    }

    public void OnAttackAnimationFinish(int attackId)
    {

    }

    // 物品被拾取，播放拾取特效
    public void OnPickup(SceneObj _object)
    {
        pickupSfx.GetComponent<particleControl>().turnOn = true;
    }

}
