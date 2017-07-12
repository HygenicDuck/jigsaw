using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesIcon : MonoBehaviour {

	[SerializeField] 
	GameObject m_heart;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetHeart(bool active)
	{
		m_heart.SetActive(active);
	}

	public void LoseHeart()
	{
		// todo: lose with a flourish (e.g. flashing effect)
		m_heart.SetActive(false);
	}
}
