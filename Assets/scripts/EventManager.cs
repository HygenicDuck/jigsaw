using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MessagingManager : MonoBehaviour {

	private Dictionary <string, UnityEvent> m_eventDictionary;
	static MessagingManager m_instance = null;


	public static MessagingManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public MessagingManager()
	{
		m_instance = this;
		Init();
	}


	void Init ()
	{
		if (m_eventDictionary == null)
		{
			m_eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	public void StartListening (string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (m_instance.m_eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);	// just in case you've already added this listener. Don't want it twice.
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			m_instance.m_eventDictionary.Add (eventName, thisEvent);
		}
	}

	public void StopListening (string eventName, UnityAction listener)
	{
		if (m_eventDictionary == null) return;
		UnityEvent thisEvent = null;
		if (m_instance.m_eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public static void TriggerEvent (string eventName)
	{
		UnityEvent thisEvent = null;
		if (m_instance.m_eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke ();
		}
	}

}
