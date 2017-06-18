using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MessagingManager {

	[SerializeField] 
	int m_startValue;

	float m_startCountdownTime;
	bool m_counting = false;
	Text m_textComponent;

	// Use this for initialization
	void Start () {
		m_textComponent = gameObject.GetComponent<Text>();
	}

	void OnEnable()
	{
		StartCountdown();
	}

	void StartCountdown()
	{
		m_startCountdownTime = Time.timeSinceLevelLoad;
		m_counting = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_counting)
		{
			float currentCountdown = (float)m_startValue - Time.timeSinceLevelLoad + m_startCountdownTime;
			int currentCountdownInt = (int)currentCountdown;
			m_textComponent.text = currentCountdownInt.ToString();
			if (currentCountdownInt == 0)
			{
				m_counting = false;
				TriggerEvent("COUNTDOWN_COMPLETE");
			}
		}
	}
}
