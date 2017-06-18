using UnityEngine;
using System.Collections;

public class GameStateClass {

	public static int NUMBER_OF_LEVELS = 5;
	public bool[] m_unlockedLevels = new bool[NUMBER_OF_LEVELS];
	int m_levelNumber;
    static GameStateClass m_instance = null;


    public static GameStateClass Instance
    {
        get
        {
			if (m_instance == null) {
				m_instance = new GameStateClass();
			}
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

	public void Initialise()
	{
		m_unlockedLevels[0] = true;
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
		Debug.Log ("CompletedLevel "+score+", "+m_levelNumber);
        // TODO - unlock the next level if the score was 6 (i.e. did it perfectly)
		if (score == 0) {
			Debug.Log ("CompletedLevel 2 "+score+", "+m_levelNumber);

			if (m_levelNumber + 1 < NUMBER_OF_LEVELS) {
					Debug.Log ("CompletedLevel 3 "+score+", "+m_levelNumber);
					m_unlockedLevels [m_levelNumber + 1] = true;
			}
		}
    }

}
