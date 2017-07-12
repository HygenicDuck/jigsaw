using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Text.RegularExpressions;


public class CoreBuildProject : MonoBehaviour 
{	
	private const string buildsDirectoryName = "Builds";
	private const string PROJECT_PREPROCESSOR_DEFINITIONS = "Tools/Configs/ProjectDefinitions.txt";

	static public void BuilsIOSTest()
	{
		int enabledScenesCount = 0;

		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			enabledScenesCount += scene.enabled ? 1 : 0;
		}

		string[] scenePaths = new string[enabledScenesCount];
		int sceneIndex = 0;

		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled) {
				continue;
			}

			scenePaths[sceneIndex++] = scene.path;
		}
			
		string error = UnityEditor.BuildPipeline.BuildPlayer(scenePaths, Path.Combine(Directory.GetParent(Application.dataPath).FullName, CoreBuildProject.buildsDirectoryName), BuildTarget.iOS, BuildOptions.None);

		if (!string.IsNullOrEmpty(error)) {
			throw new UnityException("Build failure: " + error);
		}
	}

	static public void AddProjectDefinitions()
	{
		if (File.Exists(PROJECT_PREPROCESSOR_DEFINITIONS))
		{
			BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

			string definitions = File.ReadAllText (PROJECT_PREPROCESSOR_DEFINITIONS);

			Hashtable json = Core.Utils.JSON.JsonDecode(definitions) as Hashtable;

			foreach (string key in json.Keys)
			{
				if (key == "iOS") {
					targetGroup = BuildTargetGroup.iOS;
				} else if (key == "Android") {
					targetGroup = BuildTargetGroup.Android;
				} else if (key == "Amazon") {
					targetGroup = BuildTargetGroup.Android;
				}
				string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

				Hashtable targetGroupSettings = (Hashtable)json [key];

				foreach (string groupKey in targetGroupSettings.Keys)
				{
					if ((int)targetGroupSettings[groupKey] == 1 && !symbols.Contains(groupKey))
					{
						symbols = groupKey + ";" + symbols;
					}
					else if ((int)targetGroupSettings[groupKey] == 0 && symbols.Contains(groupKey))
					{
						symbols = symbols.Replace(groupKey, "");
					}
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);
			}
		}
	}

	static void DoNothing()
	{
		// Dummy call just to make unity reload the project
	}

}
