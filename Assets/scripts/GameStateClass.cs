using UnityEngine;
using System.Collections;
using System.IO;

public class GameStateClass : MonoBehaviour 
{

	public static int NUMBER_OF_LEVELS = 15;
	public bool[] m_unlockedLevels = new bool[NUMBER_OF_LEVELS];
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
    }

    // Use this for initialization
    void Start () 
	{
	}

	public void Initialise()
	{
		m_instance = this;
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
		return 2;	//m_levelNumber;
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


	//AGTEMP - untested code below. There must be a better way
	// (e.g. make a simple class to contain just 1 int, which is the highest
	// level unlocked. Serialize that to json.)

	void SaveProgressToFile()
	{
		string json = JsonUtility.ToJson (this);
		string path = Application.persistentDataPath + "/savefile.json";
		StreamWriter writer = new StreamWriter (path, false);
		writer.WriteLine (json);
		writer.Close ();
	}

	void LoadProgressFromFile()
	{
		string path = Application.persistentDataPath + "/savefile.json";
		StreamReader reader = new StreamReader (path);
		string json = reader.ReadToEnd ();
		GameStateClass gs = JsonUtility.FromJson<GameStateClass> (json);
		m_unlockedLevels = gs.m_unlockedLevels;
		reader.Close ();
	}

}
