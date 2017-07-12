using System.Collections;
using System.Collections.Generic;

#if FACEBOOK
using Facebook.Unity;
using Core.Debugging;

namespace Core
{
	namespace Analytics
	{
		public class FacebookProvider : Provider
		{
			private HashSet<string> m_whiteList;

			private const string PLAYS = "plays";

			public FacebookProvider(HashSet<string> whiteList)
			{
				m_providerName = "Facebook";

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
				FB.LogAppEvent(eventName);
			}

			public override void LogEvent(string eventName, Hashtable parameters)
			{
				if (m_whiteList.Contains(eventName))
				{
					Debugger.Log("FB logging: " + eventName, (int)SharedSystems.Systems.ANALYTICS);

					if (parameters != null)
					{
						Dictionary<string, object> dict = new Dictionary<string, object> ();

						foreach (DictionaryEntry pair in parameters)
						{
							dict.Add(pair.Key.ToString(), pair.Value);
						}

						FB.LogAppEvent(eventName, parameters: dict);
					}
					else
					{
						FB.LogAppEvent(eventName);
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
		public class FacebookProvider : Provider
		{
			public FacebookProvider(HashSet<string> whiteList)
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