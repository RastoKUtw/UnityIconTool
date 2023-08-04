using UnityEngine;
using UnityEditor;
using System.IO;

public class ThumbnailWindow : EditorWindow{
	private static int thumbnailWidth = 512;
	private static int thumbnailHeight = 512;

	string fileName = "FileName";
	string defalutPath = "Assets/Thumbnails";
	string m_path = "";

	[MenuItem("Window/Thumbnail Generator")]
	public static void ShowWindow(){
		ThumbnailWindow window = EditorWindow.GetWindow<ThumbnailWindow>();
		window.titleContent = new GUIContent("Thumbnail Generator");
		window.Show();
	}

	private void OnGUI(){
		GUILayout.Label("Scene Screen Shot", EditorStyles.boldLabel);

		if (GUILayout.Button("SceneShot")){
			GenerateThumbnail();
		}

		GUILayout.Space(20);

		fileName = EditorGUILayout.TextField("FileName", fileName);

		if (GUILayout.Button("Select Save Path")){
			SelectPath();
		}

		GUILayout.TextArea(m_path);
	}

	private void GenerateThumbnail()
	{
		if (m_path == ""){
			if (!AssetDatabase.IsValidFolder(defalutPath)){AssetDatabase.CreateFolder("Assets", "Thumbnails");
			}
			m_path = "Assets/Thumbnails";
		}
		string screenshotName = fileName + ".png";
		string screenshotFullPath = Path.Combine(m_path, screenshotName);
		
		RenderTexture renderTexture = new RenderTexture(thumbnailWidth, thumbnailHeight, 24, RenderTextureFormat.ARGB32);
		Camera camera = SceneView.GetAllSceneCameras()[0];
		camera.targetTexture = renderTexture;
		camera.backgroundColor = new Color(0, 0, 0, 0); 
		camera.Render();

		RenderTexture.active = renderTexture;
		Texture2D thumbnail = new Texture2D(thumbnailWidth, thumbnailHeight, TextureFormat.ARGB32, false);
		thumbnail.ReadPixels(new Rect(0, 0, thumbnailWidth, thumbnailHeight), 0, 0);
		thumbnail.Apply();

		RenderTexture.active = null;
		camera.targetTexture = null;
		camera.backgroundColor = Color.white; 

		byte[] bytes = thumbnail.EncodeToPNG();
		File.WriteAllBytes(screenshotFullPath, bytes);

		AssetDatabase.Refresh();
		//Debug.Log("Thumbnail generated at: " + screenshotPath);
	}
	void SelectPath(){
		m_path = EditorUtility.SaveFolderPanel("Save textures to folder", "", "");
	}
}
