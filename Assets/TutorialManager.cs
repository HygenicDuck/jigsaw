using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [SerializeField]
    GameObject[] m_tutorialRoots;

	static TutorialManager m_instance = null;

	public enum MessageID
	{
		SWIPE_TO_ROTATE = 0,
		SWIPE_TO_SPIN,
		LAST
	}

	public static TutorialManager Instance
	{
		get
		{
			return m_instance;
		}
	}




	// Use this for initialization
	void Start ()
    {
		m_instance = this;
        ClearAllTutorialMessages();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowTutorialMessage(MessageID tutorialID)
    {
		if (!GameStateClass.Instance.HasTutorialBeenShown (tutorialID)) 
		{
			for (int i = 0; i < m_tutorialRoots.Length; i++) 
			{
				m_tutorialRoots [i].SetActive (i == (int)tutorialID);
			}

			GameStateClass.Instance.ShownTutorial (tutorialID);
		}
    }

    public void ClearAllTutorialMessages()
    {
        for (int i = 0; i < m_tutorialRoots.Length; i++)
        {
            m_tutorialRoots[i].SetActive(false);
        }
    }
}
