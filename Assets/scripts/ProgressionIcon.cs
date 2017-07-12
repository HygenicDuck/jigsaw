using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionIcon : MonoBehaviour {

	[SerializeField] 
	GameObject m_tick;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetTick(bool active)
	{
		m_tick.SetActive(active);
	}

	public void RevealTick()
	{
		// todo: reveal with a flourish (e.g. scaling effect)
		m_tick.SetActive(true);
	}

	public void SetColour(Color color)
	{
		RawImage raw = gameObject.GetComponent<RawImage>();
		raw.color = color;
	}
}
