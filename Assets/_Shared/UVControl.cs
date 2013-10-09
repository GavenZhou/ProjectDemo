using UnityEngine;
using System.Collections;

public class UVControl : MonoBehaviour {
	
	public bool loop;
	
	public int rowNum ;
	
	public int lineNum ;
	
	 
	
	public float iconwidth ;
	
	public float iconheight ;
	
	public int tileNum=6;
	
	public float texWidth ;
	
	public float texHeight ;
	
	int achievementIndex=0;
	
	float uWidth = 0;
	
	float vHeight = 0;

	// Use this for initialization
	void Start () {
	    uWidth=iconwidth/texWidth;
		vHeight=iconheight/texHeight;
	    InvokeRepeating("AnimationTexture",0,0.1f);
	}

 

	void AnimationTexture()
	{   
		if(achievementIndex>tileNum)
		{
			if(loop)
				achievementIndex=0;
			else
				return;
		}

		int rowIndex=achievementIndex/rowNum;
		
		int lineIndex=achievementIndex%lineNum;
		
		float uNums=lineIndex*uWidth;
		
		float vNums=1-rowIndex*vHeight;
		
		Vector2 size=new Vector2(uWidth,vHeight);
		
		renderer.material.SetTextureOffset("_MainTex",new Vector2(uNums,vNums));
		
		renderer.material.SetTextureScale("_MainTex",size);
		
		
		achievementIndex++;

	}
	
}
