using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

using LocLangDict = System.Collections.Generic.Dictionary<string, LocalizedLanguage>;
using Core.Utils;

public class LocalizationManager
{
	public const string FALLBACK_TEXT = "-KEY NOT FOUND-";
	public const string DEFAULT_LANGUAGE_ID = "en";
	private const string BASEFILENAME = "Localisation_";
	private const string FILEEXTENSION = ".txt";
	private const string FILEFOLDER = "Configs/Localisation";

	private string m_currentLanguageID = DEFAULT_LANGUAGE_ID;
	private LocLangDict m_languages = new LocLangDict ();
	private LocalizedLanguage m_currentLanguage;
	
	public static bool ContainsKey(string key)
	{
		return Instance.m_currentLanguage.Strings.ContainsKey(key);
	}

	public static string GetString(string key)
	{
		if (Instance.m_currentLanguage == null)
		{
			return FALLBACK_TEXT;
		}

		string ret = Instance.m_currentLanguage.GetString(key); 

		if (ret != null)
		{
			return ret;
		}
		else
		{
			// Couldn't find key in the user's language use the fallback text instead
			return FALLBACK_TEXT;
		}
	}
	public static int GetArrayCount(string key)
	{
		Dictionary<string, string[]> arrays = Instance.m_currentLanguage.Arrays;
			
		if (arrays.ContainsKey(key))
		{
			return arrays.Count;
		}
		else
		{
			return 0;
		}
	}
	public static string[] GetArray(string key)
	{
		Dictionary<string, string[]> arrays = Instance.m_currentLanguage.Arrays;
		
		if (arrays.ContainsKey(key))
		{
			return arrays[key];
		}
		else
		{
			return new string[]{ };
		}
	}
	public static string GetArrayEntry(string key, int index)
	{
		Dictionary<string, string[]> arrays = Instance.m_currentLanguage.Arrays;

		if (arrays.ContainsKey(key))
		{
			return arrays[key][index];
		}
		else
		{
			return FALLBACK_TEXT;
		}
	}
	public static string GetArrayRandomEntry(string key)
	{
		int arrayCount = GetArrayCount(key);
		if (arrayCount > 0)
		{
			return GetArrayEntry(key, UnityEngine.Random.Range(0, arrayCount));
		}
		else
		{
			return FALLBACK_TEXT;
		}
	}

	private void InitializeInstance()
	{
		m_currentLanguageID = DEFAULT_LANGUAGE_ID;
	}

	public void SaveLanguageStringToFile(string language, string languageID)
	{
		string fileName = BASEFILENAME + languageID + FILEEXTENSION;
		string templateFileName = Path.Combine(FILEFOLDER, Path.GetFileNameWithoutExtension(fileName));
		System.IO.File.WriteAllText(templateFileName, language);
	}

	private bool ReadLanguageFile(string languageId)
	{
		string fileName = BASEFILENAME + languageId + FILEEXTENSION;
		string templateFileName = Path.Combine(FILEFOLDER, Path.GetFileNameWithoutExtension(fileName));
		TextAsset asset = ((TextAsset)Resources.Load(templateFileName));

		if (asset != null)
		{
			string fileJSON = asset.text;
			LocalizedLanguage newLanguage = new LocalizedLanguage ();

			try
			{
				Hashtable data = JSON.JsonDecode(fileJSON) as Hashtable;
				newLanguage.ParseJsonData(data);
			}
			catch (Exception je)
			{
				UnityEngine.Debug.Log("The text file is not in JSON format, " + je.ToString());
			}

			if (m_languages.ContainsKey(newLanguage.ID))
			{
				if (newLanguage.Version > m_languages[newLanguage.ID].Version)
				{
					m_languages.Remove(newLanguage.ID);
					m_languages.Add(newLanguage.ID, newLanguage);
					return true;
				}
			}
			else
			{
				m_languages.Add(newLanguage.ID, newLanguage);
				return true;
			}
		}

		return false;
	}
	private bool DoLanguageExist(string languageID)
	{
		return m_languages.ContainsKey(languageID);
	}

	public void SetLanguage(string languageID)
	{
		m_currentLanguageID = null;

		if (ReadLanguageFile(languageID) || DoLanguageExist(languageID))
		{
			m_currentLanguageID = languageID;
		}
		else
		{
			int indexOf_ = languageID.IndexOf('_');
			if (indexOf_ > 0)
			{
				string fallbackLangID = languageID.Substring(0, indexOf_);
				if (ReadLanguageFile(fallbackLangID) || DoLanguageExist(fallbackLangID))
				{
					m_currentLanguageID = fallbackLangID;
				}
			}
		}
		if (string.IsNullOrEmpty(m_currentLanguageID))
		{
			m_currentLanguageID = DEFAULT_LANGUAGE_ID;

			if (!DoLanguageExist(DEFAULT_LANGUAGE_ID))
			{
				ReadLanguageFile(DEFAULT_LANGUAGE_ID);
			}
		}

		if (m_languages.ContainsKey(m_currentLanguageID))
		{
			m_currentLanguage = m_languages[m_currentLanguageID];
		}

	}

	public static void Initialize()
	{
		Instance.InitializeInstance();
	}
	public static void SetCurrentLanguage(string languageID)
	{
		Instance.SetLanguage(languageID);
	}
	public static int GetCurrentVersion(/*string languageID*/)
	{
		return Instance.m_currentLanguage.Version;
	}
	public static void SaveLanguageToFile(string language, string languageID)
	{
		Instance.SaveLanguageStringToFile(language, languageID);
	}


		
	#region Singleton

	static LocalizationManager instance;

	private static LocalizationManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new LocalizationManager ();
				Initialize();
			}
            
			return instance;
		}
	}

	#endregion
}

