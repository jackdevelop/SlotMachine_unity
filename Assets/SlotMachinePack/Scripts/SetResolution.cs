using UnityEngine;
using System.Collections;

public class SetResolution : MonoBehaviour {
    public int width = 480;
    public int height = 800;

	void Start () {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        //Application.targetFrameRate = 60;
        if (Screen.width != width || Screen.height != height)
            Screen.SetResolution(width, height, false);
#endif
        Destroy(this);
    }
	
}
