using UnityEngine;
using UnityEditor;
using System.IO;


public class KwaleeCoreMenuItems
{
//	[MenuItem("Kwalee/Check Version", false, 1)]
//	static public void CheckVersion()
//	{
//		ExternalScripts.RunBashScript("GetBuildNumberAndWait");
//	}

	[MenuItem("Kwalee/Set Project Definitions", false, 1)]
	static public void SetPreprocessors()
	{
		CoreBuildProject.AddProjectDefinitions();
	}

	[MenuItem("Assets/Create/Kwalee C# Script")]
	private static void NewScriptOption()
	{
		var selectedObjects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
		if (selectedObjects.Length > 0)
		{
			string path = AssetDatabase.GetAssetPath(selectedObjects[0]);
			Debug.Log("script path " + path);

			string name = "NewBehaviourScript";
			Debug.Log("Creating Classfile: " + path);

			string copyPath = path + "/" + name + ".cs";
			int version = 1;
			while (File.Exists(copyPath))
			{
				name = "NewBehaviourScript" + version;
				copyPath = path + "/" + name + ".cs";
				version++;
			}

			if (File.Exists(copyPath) == false)
			{ 
				// do not overwrite
				using (StreamWriter outfile = new StreamWriter (copyPath))
				{
					outfile.WriteLine("using UnityEngine;");
					outfile.WriteLine("using System.Collections;");
					outfile.WriteLine("using Core;");
					outfile.WriteLine("");
					outfile.WriteLine("public class " + name + " : WaffleBaseBehaviour {");
					outfile.WriteLine(" ");
					outfile.WriteLine(" ");
					outfile.WriteLine(" // Use this for initialization");
					outfile.WriteLine(" void Start () {");
					outfile.WriteLine(" ");
					outfile.WriteLine(" }");
					outfile.WriteLine(" ");         
					outfile.WriteLine(" ");
					outfile.WriteLine(" // Update is called once per frame");
					outfile.WriteLine(" void Update () {");
					outfile.WriteLine(" ");
					outfile.WriteLine(" }");
					outfile.WriteLine("}");
				}//File written
			}
			AssetDatabase.Refresh();
			Object obj = AssetDatabase.LoadAssetAtPath(copyPath, typeof(Object));
			Selection.activeObject = obj;
		}
	}

	[MenuItem("Assets/Add Prefab to Canvas %#g")]
	private static void AddToCanvas()
	{
		if (Selection.activeObject != null)
		{
			bool isPrefab = PrefabUtility.GetPrefabParent(Selection.activeObject) == null && PrefabUtility.GetPrefabObject(Selection.activeObject) != null;
			if (isPrefab)
			{
				GameObject itemPrefab = PrefabUtility.InstantiatePrefab(Selection.activeObject as GameObject) as GameObject;
				GameObject canvas = GameObject.Find("Canvas");
				if( itemPrefab != null && canvas != null )
				{
					itemPrefab.transform.SetParent(canvas.transform, false);
				}
			}
		}
	}

	[MenuItem("Kwalee/GameObject/Create UI Text")]
	private static void NewTextOption()
	{
		GameObject itemPrefab = Resources.Load("UIText") as GameObject;
		if (itemPrefab != null)
		{
			GameObject obj = itemPrefab != null ? Object.Instantiate(itemPrefab, Vector3.zero, Quaternion.identity) as GameObject : null;
			if (obj != null)
			{
				if (Selection.activeGameObject != null)
				{
					obj.transform.SetParent(Selection.activeGameObject.transform, false);
				}

				Selection.activeGameObject = obj;
				obj.name = "UIText";
			}
		}
	}

//	[MenuItem("Kwalee/Generate Texts From Gooogle Sheet", false, 1)]
//	static public void BuildLocaleConfigs()
//	{
//		ExternalScripts.RunPythonScript("Localisation/BuildLocaleConfigs.py",
//			() => {
//				AssetDatabase.Refresh();
//			});
//	}
		

//	[MenuItem("Kwalee/Update version number", false, 1)]
//	static public void UpdateVersionNumber()
//	{
//		BuildProject.UpdateVersionNumber();
//		Debug.Log("Update version number - done");
//	}
}

 