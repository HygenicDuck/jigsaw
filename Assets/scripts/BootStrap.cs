using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;



// Analytic event example:
// Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_SELECTED);
// Create new event types in AnalyticsEvents.cs



public class BootStrap : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake()
	{
		//PlayerPrefs.DeleteAll ();
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Application.targetFrameRate = 60;
		}

		FB.Init(FBInitCallback);
	}

	private void FBInitCallback()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
	}

	public void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			if (FB.IsInitialized)
			{
				FB.ActivateApp();
			}
		}
	}


}
