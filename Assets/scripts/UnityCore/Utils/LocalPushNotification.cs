using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_ANDROID && LOCAL_PUSHES
using Assets.SimpleAndroidNotifications;
#endif

namespace Core
{
	public static class LocalPushNotification
	{
		const string CANCEL_KEY = "cancelkey";

		public static void SendLocalPushNotification(long inMilisecons, string title, string messageBody, string cancelKey)
		{
			#if UNITY_IOS
			UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification ();
			notification.fireDate = DateTime.Now.AddMilliseconds(inMilisecons);
			notification.alertBody = messageBody;

			IDictionary userInfo = new Dictionary<string,string>();
			userInfo[CANCEL_KEY] =  cancelKey;
			notification.userInfo = userInfo;

			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);
			#elif UNITY_ANDROID && LOCAL_PUSHES
			Debug.LogWarning("Set NOTIFICATION " + inMilisecons/1000);

			NotificationManager.Send(TimeSpan.FromSeconds(inMilisecons/1000), title, messageBody, new Color(0.0f, 0.0f, 0.0f));
			#else

			#endif

		}

		public static void CancelLocalNotification(string cancelKey)
		{
			#if UNITY_IOS
			List<UnityEngine.iOS.LocalNotification> notificationsToClear = new List<UnityEngine.iOS.LocalNotification> ();

			for (int i = 0; i < UnityEngine.iOS.NotificationServices.scheduledLocalNotifications.Length; ++i)
			{
				UnityEngine.iOS.LocalNotification notification = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications[i];

				string currentCancelKey = (string)notification.userInfo[CANCEL_KEY];

				if (currentCancelKey == cancelKey)
				{
					notificationsToClear.Add(notification);
				}
			}

			foreach (UnityEngine.iOS.LocalNotification n in notificationsToClear)
			{
				UnityEngine.iOS.NotificationServices.CancelLocalNotification(n);
			}
			#else


			#endif
		}

		public static void CancelAllLocalNotifications()
		{
			#if UNITY_IOS
			UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
			#elif UNITY_ANDROID && LOCAL_PUSHES
			NotificationManager.CancelAll();
			#else
			#endif
		}
	}
}
