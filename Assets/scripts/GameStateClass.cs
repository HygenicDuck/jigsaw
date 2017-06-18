using UnityEngine;
using System.Collections;

public class GameStateClass : MonoBehaviour {

	int m_levelNumber;
    static GameStateClass m_instance = null;


    public static GameStateClass Instance
    {
        get
        {
            return m_instance;
        }
    }

    public GameStateClass()
    {
        m_instance = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetLevelNumber(int num)
	{
		m_levelNumber = num;
	}

	public int GetLevelNumber()
	{
		return m_levelNumber;
	}

    public void CompletedLevel(int score)
    {
        // TODO - unlock the next level if the score was 6 (i.e. did it perfectly)
    }

}
