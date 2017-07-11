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
		LoadProgressFromFile();
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
		if (score == 0) 
		{
			Debug.Log ("CompletedLevel 2 "+score+", "+m_levelNumber);

			if (m_levelNumber + 1 < NUMBER_OF_LEVELS) 
			{
				Debug.Log ("CompletedLevel 3 "+score+", "+m_levelNumber);
				m_unlockedLevels [m_levelNumber + 1] = true;
				SaveProgressToFile();
			}
		}
    }


	//AGTEMP - untested code below. There must be a better way
	// (e.g. make a simple class to contain just 1 int, which is the highest
	// level unlocked. Serialize that to json.)

	public class SaveClass
	{
		public int levelReached;
	}

	void SaveProgressToFile()
	{
		Debug.Log("SaveProgressToFile 1");
		SaveClass saveClass = new SaveClass();
		for(int i=0; i<NUMBER_OF_LEVELS; i++)
		{
			if (m_unlockedLevels[i])
			{
				saveClass.levelReached = i;
			}
		}
		Debug.Log("SaveProgressToFile 2");

		string json = JsonUtility.ToJson (saveClass);
		string path = Application.persistentDataPath + "/savefile.json";
		StreamWriter writer = new StreamWriter (path, false);
		Debug.Log("SaveProgressToFile 3");

		writer.WriteLine (json);
		writer.Close ();
		Debug.Log("SaveProgressToFile 4");

	}

	void LoadProgressFromFile()
	{
		string path = Application.persistentDataPath + "/savefile.json";
		if (System.IO.File.Exists(path))
		{
			StreamReader reader = new StreamReader (path);
			string json = reader.ReadToEnd ();
			reader.Close ();
			if (json != null)
			{
				SaveClass saveClass = JsonUtility.FromJson<SaveClass> (json);
				for(int i=0; i<NUMBER_OF_LEVELS; i++)
				{
					m_unlockedLevels[i] = (i <= saveClass.levelReached);
				}
			}
		}
	}

}
