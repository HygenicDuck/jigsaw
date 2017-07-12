using UnityEngine;
using System.Collections;

#if APTELIGENT

namespace Core
{
	namespace Analytics
	{
		public class ApteligentProvider : Provider
		{

			public ApteligentProvider()
			{
				m_providerName = "ApteligentProvider";
			}

			public override void StartSession()
			{
				Crittercism.SetUsername(m_userId);
			}

			public override void FlushSession()
			{
				// Empty
			}

			public override void LogAdEvent(string eventName)
			{
			}

			public override void LogEvent(string eventName, Hashtable parameters)
			{
				Crittercism.LeaveBreadcrumb(eventName + Core.Utils.JSON.JsonEncode(parameters));
			}

			public override void LogErrorEvent(string eventName, string errorDescription)
			{
				Crittercism.LeaveBreadcrumb(eventName + " " + errorDescription);
			}

			public override void StartTimedEvent(string eventName, Hashtable parameters)
			{
				Crittercism.BeginUserflow(eventName);
			}

			public override void StopTimedEvent(string eventName)
			{
				Crittercism.EndUserflow(eventName);
			}
		}
	}
}

#else

namespace Core
{
	namespace Analytics
	{
		public class ApteligentProvider : Provider
		{
			public ApteligentProvider()
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