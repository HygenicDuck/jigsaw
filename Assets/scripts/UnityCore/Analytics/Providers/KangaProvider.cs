using UnityEngine;
using System.Collections;

#if KANGA
using System.Collections.Generic;
using WaffleUtils;
using WaffleIron;
using WaffleIronObjects;
using Core.Debugging;
using Core.Utils;

namespace Core
{
	namespace Analytics
	{
		public class KangaProvider : Provider
		{
			private const string CACHE_KEY = "KangaAnalyticsProviderCache";
			private const int MAX_CACHE_ENTRY_COUNT = 30;

			private int m_maxCacheEntryCount = 0;
			private ArrayList m_eventCache = new ArrayList ();
			private Hashtable m_newInstallationEvent;
			private int m_order = 0;


			public KangaProvider()
			{
				m_providerName = "KangaAnalyticsProvider";
				m_maxCacheEntryCount = MAX_CACHE_ENTRY_COUNT;

				GameSettings gameSettings = WaffleUtils.GameSettingsManager.Instance.GetGameSettings();
				if (gameSettings != null)
				{
					m_maxCacheEntryCount = gameSettings.analyticsMaxCacheEntryCount > 0 ? gameSettings.analyticsMaxCacheEntryCount : MAX_CACHE_ENTRY_COUNT;
				}

				GameSettingsEvents.GameSettingsUpdated += HandleGameSettingsUpdated;
			}

			public override void StartSession()
			{
				FlushCache();
			}

			public override void FlushSession()
			{
				FlushCache();
			}

			public void HandleGameSettingsUpdated()
			{
				GameSettings gameSettings = WaffleUtils.GameSettingsManager.Instance.GetGameSettings();
				if (gameSettings != null)
				{
					m_maxCacheEntryCount = gameSettings.analyticsMaxCacheEntryCount > 0 ? gameSettings.analyticsMaxCacheEntryCount : MAX_CACHE_ENTRY_COUNT;
				}
			}

			public override void LogAdEvent(string eventName)
			{
			}

			public override void LogEvent(string eventName, Hashtable parameters)
			{
				CacheEvent(eventName, parameters);
			}

			public override void LogErrorEvent(string eventName, string errorDescription)
			{
				Debugger.Log("KangaProvider: LogErrorEvent not implemented", Debugger.Severity.WARNING, (int)SharedSystems.Systems.ANALYTICS);
			}

			public override void StartTimedEvent(string eventName, Hashtable parameters)
			{
				Debugger.Log("KangaProvider: StartTimedEventnot implemented", Debugger.Severity.WARNING, (int)SharedSystems.Systems.ANALYTICS);
			}

			public override void StopTimedEvent(string eventName)
			{
				Debugger.Log("KangaProvider: StopTimedEvent not implemented", Debugger.Severity.WARNING, (int)SharedSystems.Systems.ANALYTICS);
			}

			/**
		 * KANGA FUNCTIONS
		 **/

			private void FlushCache()
			{
				// no auth player then send the new installation events...
				if (!Server.Instance.HasAuthPlayer())
				{
					if (m_newInstallationEvent != null)
					{
						Debugger.Log("Sending new installation event...", (int)SharedSystems.Systems.ANALYTICS);

						WaffleRequests.KangaAnalytics.SendAnonymous(m_newInstallationEvent, null, null);

						m_newInstallationEvent = null;
					}

					return;
				}
				else if (m_newInstallationEvent != null)
				{
					m_eventCache.Add(m_newInstallationEvent);

					m_newInstallationEvent = null;
				}

				if (m_eventCache.Count > 0)
				{
					for (int i = 0; i < m_eventCache.Count; ++i)
					{
						Debugger.LogHashtableAsJson("SENDING EVENT", (Hashtable)m_eventCache[i], (int)SharedSystems.Systems.ANALYTICS);
					}

					WaffleRequests.KangaAnalytics.AnalyticsOptions options = new WaffleRequests.KangaAnalytics.AnalyticsOptions (new ArrayList (m_eventCache));

					WaffleRequests.KangaAnalytics.Send(options, null, null);

					m_eventCache.Clear();
				}
			}

			private Hashtable CreateEvent(string eventName, Hashtable parameters)
			{
				Debugger.LogHashtableAsJson("CREATE EVENT " + eventName + ", order " + m_order, (Hashtable)parameters, (int)SharedSystems.Systems.ANALYTICS);

				Hashtable aEvent = new Hashtable ();

				aEvent.Add("event_type", eventName);
				aEvent.Add("parameters", parameters);
				aEvent.Add("device_time_ms", Date.GetEpochTimeMills());
				aEvent.Add("order", m_order);

				++m_order;

				return aEvent;
			}
			
			private void CacheEvent(string eventName, Hashtable parameters)
			{
				Debugger.LogHashtableAsJson("CACHE EVENT " + eventName, (Hashtable)parameters, (int)SharedSystems.Systems.ANALYTICS);

				Hashtable kangaEvent = CreateEvent(eventName, parameters);

				if (eventName == Events.NEW_GAME_INSTALLATION_KEY)
				{
					m_newInstallationEvent = kangaEvent;
				}
				else
				{
					m_eventCache.Add(kangaEvent);
				}

				if (m_eventCache.Count >= m_maxCacheEntryCount)
				{
					FlushCache();
				}
			}
		}
	}
}
#else

namespace Core
{
	namespace Analytics
	{
		public class KangaProvider : Provider
		{
			public KangaProvider()
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