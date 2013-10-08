using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerInputBase : MonoBehaviour {
	

	
	private PlayerMainLogic baseControlScript;
	
	
	
	private bool mOneClick = false;
	private bool mTwiceClick = false;
	private float mPressTime = 0;
	
	private Vector3 oldPos;
	
	// Use this for initialization
	void Start () {
		
		baseControlScript = (PlayerMainLogic)this.GetComponent<PlayerMainLogic>();
		
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(baseControlScript.mTouchState != PlayerMainLogic.TouchState.None)
		{
			//send the result to the center control
			baseControlScript.GetInputState();
			
			InputStateClass.ClearTouchSlashPosArray();
			InputStateClass.oldSlashPos = Vector3.zero;
			return;
		}
		
		// one tap 
		if(Time.time - mPressTime > 0.2f && mOneClick == true)
		{
			baseControlScript.mTouchState = PlayerMainLogic.TouchState.AFingerOneTap;
			mOneClick = false;
			InputStateClass.touchPointPos = oldPos;
//			Debug.Log(baseControlScript.mTouchState);
			return;
		}
		
#if UNITY_EDITOR		
	//	if(Input.GetMouseButton(0))
#else
		if(Input.touchCount == 1)
#endif
		{
#if UNITY_EDITOR			
			if(Input.GetMouseButtonDown(0))
#else
			if(Input.GetTouch(0).phase == TouchPhase.Began)
#endif
			{
				if(mOneClick == false) // no tap before
				{
					// click once
					mPressTime = Time.time;
					mOneClick = true;
				}
				else // tap once before
				{
					// the second tap here
					mOneClick = false;
					baseControlScript.mTouchState = PlayerMainLogic.TouchState.AFingerDoubleTap;
					Debug.Log(baseControlScript.mTouchState);
					InputStateClass.touchPointPos = Input.mousePosition;
					InputStateClass.oldSlashPos = Input.mousePosition;
					InputStateClass.AddPointToSlashPosArray(Input.mousePosition);
					return;
				}
				oldPos = Input.mousePosition;
			}
			
#if UNITY_EDITOR				
			if(Input.GetMouseButtonUp(0))
#else
			if(Input.GetTouch(0).phase == TouchPhase.Ended)
#endif			
			{
				float dis = Vector3.Distance(oldPos,Input.mousePosition);
				
				if(dis > Screen.width/20)
				{
					baseControlScript.mTouchState = PlayerMainLogic.TouchState.AFingerSlash;
					Debug.Log(baseControlScript.mTouchState);
					mOneClick = false;
					
					InputStateClass.oldSlashPos = Input.mousePosition;
					InputStateClass.AddPointToSlashPosArray(Input.mousePosition);
					return;
				}
			}
			
#if UNITY_EDITOR			
			if(Input.GetMouseButton(0))
#else
			if(Input.GetTouch(0).phase == TouchPhase.Moved)
#endif	
			{
				if(Vector3.Distance(InputStateClass.oldSlashPos, Input.mousePosition)  >= InputStateClass.DisPointToPoint)
				{
					InputStateClass.oldSlashPos = Input.mousePosition;
					InputStateClass.AddPointToSlashPosArray(Input.mousePosition);
				}
			}
		}
	}
}
