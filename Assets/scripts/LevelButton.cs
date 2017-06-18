using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour {

    [SerializeField]
    public Text m_buttonText;

    [SerializeField]
    public Image m_buttonGraphic;

    int m_levelNumber;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetImage(Sprite sprite)
    {
        m_buttonGraphic.sprite = sprite;
    }

    public void SetText(string text)
    {
        m_buttonText.text = text;
    }

    public void SetLevelNumber(int number)
    {
        m_levelNumber = number;
    }

    public void StartLevel()
    {
		if (GameStateClass.Instance.m_unlockedLevels [m_levelNumber]) {
			GameStateClass.Instance.SetLevelNumber (m_levelNumber);
			SceneManager.LoadScene ("mainGame");
		}
    }
}
