using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TextureManager))]
public class TextureInspector : Editor 
{
	private TextureManager creator;
	
	private void OnEnable () 
	{
		creator = target as TextureManager;
		Undo.undoRedoPerformed += RefreshCreator;
	}
	
	private void OnDisable () 
	{
		Undo.undoRedoPerformed -= RefreshCreator;
	}
	
	private void RefreshCreator () 
	{
		if (Application.isPlaying) 
		{
			creator.ResizeTexture();
		}
	}

	public override void OnInspectorGUI () 
	{
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
			
		if (EditorGUI.EndChangeCheck() && Application.isPlaying) 
		{
			RefreshCreator ();
		}
	}
}

