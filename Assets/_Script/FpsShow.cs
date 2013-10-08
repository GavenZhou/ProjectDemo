using UnityEngine;

using System.Collections;

 

public class FpsShow : MonoBehaviour {

    float timeA; 

    public int fps;

    public int lastFPS;

    public GUIStyle textStyle;

    // Use this for initialization

    void Start () {
		Application.targetFrameRate = 60;
        timeA = Time.timeSinceLevelLoad;

        DontDestroyOnLoad (this);

    }

    

    // Update is called once per frame

    void Update () {

        //Debug.Log(Time.timeSinceLevelLoad+" "+timeA);

        if(Time.timeSinceLevelLoad  - timeA <= 1)

        {

            fps++;

        }

        else

        {

            lastFPS = fps + 1;

            timeA = Time.timeSinceLevelLoad;

            fps = 0;

        }

    }

    void OnGUI()

    {
		this.transform.GetComponent<TextMesh>().text = lastFPS.ToString();
    //    GUI.Label();
		
		if(GUI.Button(new Rect( Screen.width*0.2f,Screen.height*0.1f,Screen.width*0.2f,Screen.width*0.1f),"100"))
		{
			Application.targetFrameRate = 100;
		}		
		if(GUI.Button(new Rect( Screen.width*0.2f,Screen.height*0.4f,Screen.width*0.2f,Screen.width*0.1f),"200"))
		{
			Application.targetFrameRate = 200;
		}		
		if(GUI.Button(new Rect( Screen.width*0.2f,Screen.height*0.6f,Screen.width*0.2f,Screen.width*0.1f),"30"))
		{
			Application.targetFrameRate = 30;
		}
    }

}