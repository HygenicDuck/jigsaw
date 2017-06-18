using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FrontEnd : MonoBehaviour {

	[SerializeField] 
	public GameStateClass gameState;
    [SerializeField]
    public GameObject m_levelButtonPrefab;
    [SerializeField]
    public Transform m_levelButtonsPanel;


    // Use this for initialization
    void Start ()
    {
        for (int i = 1; i <= 5; i++)
        {
            GameObject levelButton = Instantiate(m_levelButtonPrefab);
            LevelButton lb = levelButton.GetComponent<LevelButton>();
            lb.SetText("Level " + i);
            lb.SetLevelNumber(i);
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
		gameState.SetLevelNumber(levNumber);
		SceneManager.LoadScene("mainGame");
	}
}
