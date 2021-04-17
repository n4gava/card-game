using UnityEditor;

[CustomEditor(typeof(CurveText))]
public class CurvedTextEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}
