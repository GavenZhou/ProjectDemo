using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
	
	RaycastHit hit = new RaycastHit();
	
	Vector3 targetPos = Vector3.zero;
	
	bool isRun = false;
	
	
	public float fMoveSpeed;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
#if UNITY_EDITOR		
		if(Input.GetMouseButtonDown(0))
#elif UNITY_IPHONE
		if(Input.touchCount == 1)
#endif			
		{
			Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit,100))
			{
				targetPos = this.transform.position;
				targetPos.x = hit.point.x;
				targetPos.z = hit.point.z;
				SetPlayerToRun(targetPos-this.transform.position);
				isRun = true;
			}
		}
	
		float dis = Vector3.Distance(targetPos,transform.position);
		if(isRun &&dis > 0.2f)
		{
			transform.Translate(Vector3.forward*Time.deltaTime*5);
		}
		if(dis <= 0.2f)
			isRun = false;
	}
	
	
	
	
	
	void SetPlayerToRun(Vector3 pos)
	{
		Vector3 dir = Vector3.Cross(transform.forward,pos.normalized);
		float dot = Vector3.Dot(transform.forward,pos.normalized);
		float angle = (float)Mathf.Acos(dot)*Mathf.Rad2Deg;
		
		if(float.IsNaN(angle))
		{
			angle = 0;
		}
		if(dir.y  < 0)
			angle = -1*angle;
		transform.RotateAround(this.transform.position,Vector3.up,angle);
	}
	
	
}
