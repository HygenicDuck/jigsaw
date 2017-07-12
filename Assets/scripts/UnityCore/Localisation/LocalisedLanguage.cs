using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

//	TypeDefs
using LangDict = System.Collections.Generic.Dictionary<string, object>;

namespace Core
{
	namespace Localisation
	{
		public class LocalisedLanguage
		{
			private LangDict m_strings;
			private int m_version;
			private string m_id;
			private Dictionary<string, string[]> m_arrays = new Dictionary<string, string[]> ();

			public int Version
			{
				get
				{
					return m_version;
				}
			}
			public LangDict Strings
			{
				get
				{
					return m_strings;
				}
			}
			public Dictionary<string, string[]> Arrays
			{
				get
				{
					return m_arrays;
				}
			}
			public string ID
			{
				get
				{
					return m_id;
				}
			}

			public string GetString(string key)
			{
				string ret = null;

				if (key != null)
				{
					LangDict dict = m_strings;
					string[] keys = key.Split('.');
					int keysLength = keys.Length;

					for (int i = 0; i < keysLength - 1; i++)
					{
						string subKey = keys[i];
						if (dict.ContainsKey(subKey))
						{
							dict = (LangDict)dict[subKey];
						}
						else
						{
							return null;
						}
					}

					string endNodeKey = keys[keysLength - 1];

					if (dict.ContainsKey(endNodeKey) && dict[endNodeKey] is string)
					{
						ret = (string)dict[endNodeKey];	
					}
				}

				return ret;
			}

			public void ParseJsonData(Hashtable expandData)
			{
				Hashtable stringsData = expandData["text"] as Hashtable;
				m_version = (int)expandData["version"];
				m_id = (string)expandData["locale"];

				m_strings = new LangDict ();

				ParseRecursive(stringsData, m_strings);
			}

			private void ParseRecursive(Hashtable expandData, LangDict dict, string fullKey = "")
			{
				foreach (DictionaryEntry child in expandData)
				{
					string key = (string)child.Key;

					object value = child.Value;

					if (value.GetType() == typeof(string))
					{
						string val = (string)value;						
						dict.Add(key, val);
					}
					else if (value.GetType() == typeof(ArrayList))
					{
						ArrayList array = value as ArrayList;

						string[] entries = new string[array.Count];
						for (int i = 0; i < array.Count; ++i)
						{
							entries[i] = (string)array[i];
						}

						m_arrays.Add(fullKey + "." + key, entries);
					}
					else
					{
						LangDict subDict = new LangDict ();
						dict.Add(key, subDict);

						if (fullKey != "")
						{
							fullKey += ".";
						}

						ParseRecursive((Hashtable)value, subDict, fullKey + key);
					}
				}
			}
		}
	}
}