using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [SerializeField]
    GameObject[] m_tutorialRoots;

    public enum MessageID
    {
        SWIPE_TO_ROTATE = 0,
        SWIPE_TO_SPIN
    }


	// Use this for initialization
	void Start ()
    {
        ClearAllTutorialMessages();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowTutorialMessage(MessageID tutorialID)
    {
        for(int i=0; i<m_tutorialRoots.Length; i++)
        {
            m_tutorialRoots[i].SetActive(i == (int)tutorialID);
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
