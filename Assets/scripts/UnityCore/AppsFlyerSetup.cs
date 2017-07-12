#if APPS_FLYER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
using Core.Utils;

namespace Core
{
	namespace Analytics
	{
		public class AppsFlyerSetup : MonoSingleton<AppsFlyerSetup>
		{
			protected override void Init()
			{
				#if UNITY_IOS
				Core.Debugging.Debugger.Log("WaffleAppsFlyer init", (int)SharedSystems.Systems.APPS_FLYER);

				AppsFlyer.setAppsFlyerKey(Utils.EmbededCoreConfig.APPSFLYER_DEV_KEY);
				AppsFlyer.setAppID(Utils.EmbededCoreConfig.APPSFLYER_APP_ID);

				// For detailed logging
				AppsFlyer.setIsDebug(true); 

				// For getting the conversion data will be triggered on AppsFlyerTrackerCallbacks.cs file
				AppsFlyer.getConversionData(); 

				// For testing validate in app purchase (test against Apple's sandbox environment
				AppsFlyer.setIsSandbox(true);         

				AppsFlyer.trackAppLaunch();

				#elif UNITY_ANDROID
				AppsFlyer.init (Utils.EmbededCoreConfig.APPSFLYER_DEV_KEY); 
				AppsFlyer.setAppID (Utils.EmbededCoreConfig.ANDROID_PACKAGE_NAME); 

				AppsFlyer.setIsDebug (true);
				//AppsFlyer.createValidateInAppListener ("AppsFlyerTrackerCallbacks", "onInAppBillingSuccess", "onInAppBillingFailure");
				#pragma warning disable 618
				AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks","didReceiveConversionData", "didReceiveConversionDataWithError");
				#pragma warning restore 618
				#endif
			}

			public void ProductPurchased(string price, string currencyCode, string productCode)
			{
				Dictionary<string, string> eventValues = new Dictionary<string, string> ();

				eventValues.Add("af_price", price.ToString());
				eventValues.Add("af_currency", currencyCode);
				eventValues.Add("af_content_id", productCode);
				eventValues.Add("af_revenue", price.ToString());

				AppsFlyer.trackRichEvent("af_purchase", eventValues);

			}
		}
	}

}
#endif
