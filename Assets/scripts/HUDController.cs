using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour {

	[SerializeField] 
	GameObject m_lifePrefab;

	[SerializeField] 
	GameObject m_progressionPrefab;

	[SerializeField] 
	Transform m_progressionParent;

	[SerializeField] 
	Transform m_livesParent;

	LivesIcon[] m_livesIcons;
	ProgressionIcon[] m_progressionIcons;

	Color m_uiColor = Color.red;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetupLives(int numLives)
	{
		ClearChildren(m_livesParent);

		m_livesIcons = new LivesIcon[numLives];

		for(int i=0; i<numLives; i++)
		{
			GameObject g = Instantiate(m_lifePrefab, m_livesParent);
			m_livesIcons[i] = g.GetComponent<LivesIcon>();
			m_livesIcons[i].SetHeart(true);
		}
	}

	void ClearChildren(Transform parent)
	{
		for ( int i=parent.childCount-1; i>=0; --i )
		{
			GameObject child = parent.GetChild(i).gameObject;
			Destroy( child );
		}
	}

	public void SetupProgression(int numFaces)
	{
		ClearChildren(m_progressionParent);

		m_progressionIcons = new ProgressionIcon[numFaces];

		for(int i=0; i<numFaces; i++)
		{
			GameObject g = Instantiate(m_progressionPrefab, m_progressionParent);
			m_progressionIcons[i] = g.GetComponent<ProgressionIcon>();
			m_progressionIcons[i].SetTick(false);
			m_progressionIcons[i].SetColour(m_uiColor);
		}
	}

	public void LoseLife(int lifeNumber)
	{
		m_livesIcons[lifeNumber].LoseHeart();
	}

	public void AdvanceProgression(int faceNumber)
	{
		Debug.Log("AdvanceProgression "+faceNumber);
		m_progressionIcons[faceNumber].RevealTick();
	}

	public void SetColour(Color color)
	{
		foreach(ProgressionIcon p in m_progressionIcons)
		{
			p.SetColour(color);
		}

		m_uiColor = color;
	}

	public void ScrollIn()
	{
		Movement mov = GetComponent<Movement>();
		mov.MoveBy(new Vector2(0,-405f),0.2f);
	}

	public void ScrollOut()
	{
		Movement mov = GetComponent<Movement>();
		mov.MoveBy(new Vector2(0,405f),0.2f);
	}
}
