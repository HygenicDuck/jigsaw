using System.Collections;
using System.Collections.Generic;

#if APPS_FLYER
using Core.Debugging;

namespace Core
{
	namespace Analytics
	{
		public class AppsFlyerProvider : Provider
		{
			private HashSet<string> m_whiteList;

			public AppsFlyerProvider(HashSet<string> whiteList)
			{
				m_providerName = "AppsFlyer";

				m_whiteList = whiteList;
			}

			public override void StartSession()
			{
				//TODO
			}

			public override void FlushSession()
			{
				//TODO
			}

			public override void LogAdEvent(string eventName)
			{
				AppsFlyer.trackRichEvent(eventName, new Dictionary<string,string> ());
			}

			public override void LogEvent(string eventName, Hashtable parameters)
			{
				if (m_whiteList.Contains(eventName))
				{
					Debugger.Log("AppsFlyer logging: " + eventName, (int)SharedSystems.Systems.ANALYTICS);

					if (parameters != null)
					{
						Dictionary<string, string> dict = new Dictionary<string, string> ();

						foreach (DictionaryEntry pair in parameters)
						{
							dict.Add(pair.Key.ToString(), pair.Value.ToString());
						}

						AppsFlyer.trackRichEvent(eventName, dict);
					}
					else
					{
						AppsFlyer.trackRichEvent(eventName, new Dictionary<string,string> ());
					}
				}
			}

			public override void LogErrorEvent(string eventName, string errorDescription)
			{
				//TODO
			}

			public override void StartTimedEvent(string eventName, Hashtable parameters)
			{
				//TODO
			}

			public override void StopTimedEvent(string eventName)
			{
				//TODO
			}
		}
	}
}
#else

namespace Core
{
	namespace Analytics
	{
		public class AppsFlyerProvider : Provider
		{
			public AppsFlyerProvider(HashSet<string> whiteList)
			{
			}

			public override void StartSession()
			{
			}

			public override void FlushSession()
			{
			}

			public override void LogAdEvent(string eventName)
			{
			}

			public override void LogEvent(string eventName, Hashtable parameters)
			{
			}

			public override void LogErrorEvent(string eventName, string errorDescription)
			{
			}

			public override void StartTimedEvent(string eventName, Hashtable parameters)
			{
			}

			public override void StopTimedEvent(string eventName)
			{
			}
		}
	}
}

#endif