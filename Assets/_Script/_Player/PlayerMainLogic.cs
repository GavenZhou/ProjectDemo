using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerMainLogic : MonoBehaviour {
	
	
	public enum TouchState
	{
		None,
		AFingerOneTap,
		AFingerDoubleTap,
		AFingerSlash,
		TwoFingersTap,
		TwoFingersDoubleTap,
		TwoFingersSlash,
	}
	
	public TouchState mTouchState = TouchState.None;
	
	private RaycastHit[] hit;
	
	private PlayerAnimationControl aniControlScript;
	
	private PlayerMoveBase moveBaseScript;
	
	private PlayerColliderBase colliderBaseScript;

    private Player player;

    public GameObject[] sfx;
	
	
	// Use this for initialization
	void Start () {
		aniControlScript = (PlayerAnimationControl)this.transform.FindChild("character").GetComponent<PlayerAnimationControl>();
		
		moveBaseScript = (PlayerMoveBase)this.GetComponent<PlayerMoveBase>();
		
		colliderBaseScript = (PlayerColliderBase)this.GetComponent<PlayerColliderBase>();
        player = gameObject.AddComponent<Player>();
        player.Init(1000);
        SceneMng.instance.AddSceneObj(player);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(aniControlScript.attack1Finish == true)
		{
//			Debug.Log("aniControlScript.attack1Finish");
			aniControlScript.attack1Finish = false;
			
			//check whether need to start the attack2
			colliderBaseScript.RayToAttackArea();
			if(colliderBaseScript.mTarget != null)
			{
				aniControlScript.attack1AnimationFinish = 2;
				aniControlScript.attack2AnimationFinish = 0;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack2,true);
				return;
			}
			else
			{
				colliderBaseScript.mTarget = null;
				PlayerDataClass.AttackStart = false;
			}
		}
		
		if(aniControlScript.attack2Finish == true)
		{
//			Debug.Log("aniControlScript.attack2Finish");
			aniControlScript.attack2Finish = false;
			
			//check whether need to start the attack3
			colliderBaseScript.RayToAttackArea();
			if(colliderBaseScript.mTarget != null)
			{
				aniControlScript.attack2AnimationFinish = 2;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,true);
				return;
			}
			else
			{
				colliderBaseScript.mTarget = null;
				PlayerDataClass.AttackStart = false;
			}
		}
		
		if(aniControlScript.attack3Finish == true)
		{
			aniControlScript.attack3Finish = false;
			PlayerDataClass.AttackStart = false;
			
			// add cd time here
			//todo
			colliderBaseScript.RayToAttackArea();
		}
		
		
		if(colliderBaseScript.mTarget != null 
			&& PlayerDataClass.AttackStart == false)
		{
			// attack start
			PlayerDataClass.AttackStart = true;
			
			//turn to the target
			TargetTheEnemy(colliderBaseScript.mTarget.position);
			
//			Debug.Log("here");
			// attack!
			aniControlScript.attack1AnimationFinish = 0;
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack1,true);
			return;
		}
		
		if(colliderBaseScript.mCatchTarget != null)
		{
			colliderBaseScript.turnOnCatchRay = false;
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Catch,true);
		}
		
		CheckMoveState();
		
	}
	
	
	void CheckMoveState()
	{
		bool immedilate = false;
		switch(moveBaseScript.curMovementState)
		{
		case PlayerMoveBase.PlayerMovementState.RunOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Walk,true);
			colliderBaseScript.mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.WalkOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			colliderBaseScript.mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.BeHitOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			colliderBaseScript.mTarget = null;
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
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,immedilate);
				colliderBaseScript.mTarget = null;
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
				
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,immedilate);
				colliderBaseScript.mTarget = null;
			}
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack3Over:
//			Debug.Log(aniControlScript.attack3AnimationFinish);
			if(aniControlScript.attack3AnimationFinish > 0)
			{
				aniControlScript.attack3AnimationFinish = 0;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,true);
				colliderBaseScript.mTarget = null;
			}
			break;	
		
		case PlayerMoveBase.PlayerMovementState.JumpOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			colliderBaseScript.mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.RushOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Catch,true);
			colliderBaseScript.mTarget = null;
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
			
			byte skillId = IsTouchSkillBtn(InputStateClass.touchPointPos);
			
			if(skillId != 0)
			{
				Time.timeScale = 0.5f;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Plyaer_SkillIdel,true);
				mTouchState = TouchState.None;
				return;
			}
			
			targetPos = RayColliderByTapPos(InputStateClass.touchPointPos);
			
			if(targetPos != Vector3.zero)
			{
				// turn to the direction first
				SetPlayerToRun(targetPos-this.transform.position);
				
				// check the attack area
				colliderBaseScript.RayToAttackArea();
				
//				Debug.Log(PlayerDataClass.playerAniCmdNext);
				
				// if there is no enemy in the area, then player start to run
				if(colliderBaseScript.mTarget == null)
				{
					if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run
						||moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Walk)
					{
						float dis = Vector3.Distance(targetPos,transform.position);
						if(dis < 4)
						{				
							ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
						}
					}
					else
					{
						ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,true);
					}
					colliderBaseScript.mTarget = null;
					PlayerDataClass.AttackStart = false;
				}
			}
			mTouchState = TouchState.None;
			break;
			
		case TouchState.AFingerDoubleTap:
			targetPos = RayColliderByTapPos(InputStateClass.touchPointPos);
			SetPlayerToRun(targetPos-this.transform.position);
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Jump,true);
			mTouchState = TouchState.None;
			break;
			
		case TouchState.AFingerSlash:
			targetPos = RayColliderByTapPos(InputStateClass.oldSlashPos);
			SetPlayerToRun(targetPos-this.transform.position);
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Rush,true);
			colliderBaseScript.turnOnCatchRay = true;
			colliderBaseScript.mCatchTarget = null;
			mTouchState = TouchState.None;
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
	
	
	byte IsTouchSkillBtn(Vector3 pos)
	{
		Ray ray1 = Camera.mainCamera.ScreenPointToRay(pos);
		hit = Physics.RaycastAll(ray1,100);
		foreach(RaycastHit hit1 in hit)
		{
			if(hit1.transform.tag == "skill1")
				return 1;
			else if(hit1.transform.tag == "skill2")
				return 2;
		}
		return 0;
	}
	
	
	Vector3 RayColliderByTapPos(Vector3 pos)
	{
		Debug.Log(pos.x);
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
	
	
	
	void TargetTheEnemy(Vector3 enemy)
	{
		enemy.y = transform.position.y;
		transform.LookAt(enemy);
	}

    public void OnStartToAttack(int attackId)
    {
        sfx[attackId - 1].GetComponent<particleControl>().turnOn = true;
        player.Attack();
    }

    public void OnAttackFinish(int attackId)
    {

    }

    public void OnAttackAnimationFinish(int attackId)
    {

    }
}
