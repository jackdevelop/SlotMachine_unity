using UnityEngine;
using System.Collections;

public class HighlightLine : MonoBehaviour {

    public Vector3[] path;
    public LineRenderer[] lineRenderers;
    public int total = 5;

	void Start () {
        lineRenderers = GetComponentsInChildren<LineRenderer>();
        foreach (LineRenderer r in lineRenderers)
        {
            r.SetWidth(0.02f, 0.02f);
            r.SetVertexCount(2);
            //r.SetColors(Color.cyan, Color.cyan);
        }
    }

    public void DrawLines()
    {
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            LineRenderer r = lineRenderers[i];
            r.enabled = true;
            r.SetPosition(0, path[i]);
            r.SetPosition(1, path[i+1]);
        }
    }

    public void SetVisible(bool on)
    {
        foreach (LineRenderer r in lineRenderers)
        {
            r.enabled = on;
        }
    }
	
	void Update () {
	
	}
}
