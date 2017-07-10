using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FrontEnd : MonoBehaviour {

    [SerializeField]
    public GameObject m_levelButtonPrefab;
    [SerializeField]
    public Transform m_levelButtonsPanel;
	[SerializeField]
	public Sprite m_levelLockedPadlock;
	[SerializeField]
	public Sprite[] m_levelIcons;
	[SerializeField]
	public GameObject m_persistantObjectPrefab;


    // Use this for initialization
    void Start ()
    {
		Debug.Log ("FrontEnd Start "+GameStateClass.Instance);

		if (GameStateClass.Instance == null) 
		{
			Debug.Log ("Instantiate "+GameStateClass.Instance);
			GameObject g = Instantiate (m_persistantObjectPrefab, transform.parent);
			GameStateClass gameStateClass = g.GetComponent<GameStateClass> ();
			gameStateClass.Initialise ();
		}

		for (int i = 0; i < GameStateClass.NUMBER_OF_LEVELS; i++)
        {
            GameObject levelButton = Instantiate(m_levelButtonPrefab);
            LevelButton lb = levelButton.GetComponent<LevelButton>();
			lb.SetText("Level " + (i+1));
            lb.SetLevelNumber(i);
			Debug.Log ("m_unlockedLevels "+i+"  = "+GameStateClass.Instance.m_unlockedLevels[i]);
			if (GameStateClass.Instance.m_unlockedLevels[i]) {
				lb.SetImage (m_levelIcons[i]);
			} else {
				lb.SetImage (m_levelLockedPadlock);
			}
            levelButton.transform.SetParent(m_levelButtonsPanel);
            levelButton.transform.localScale = new Vector3(1f, 1f, 1f);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

	public void StartLevel(int levNumber)
	{
		GameStateClass.Instance.SetLevelNumber(levNumber);
		SceneManager.LoadScene("mainGame");
	}
}
