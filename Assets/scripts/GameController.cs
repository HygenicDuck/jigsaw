using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	const int NUM_FACES_ON_A_CUBE = 6;
	const bool CHOOSE_ANOTHER_FACE_AFTER_EACH_WRONG_ANSWER = false;

	FaceState m_requiredFace;
	//int m_score = 0;
    int m_faceFoundCount = 0;
	CubeState m_cubeState = null;
	GameStateClass m_gameStateClass = null;
    Vector3 m_initialCubePosition;

    [SerializeField] 
	Text m_UIRequiredNumber;
    [SerializeField]
    GameObject m_cube;
    [SerializeField]
    Mover m_cubeMover;
    [SerializeField] 
	TouchSwipeDetector m_touchSwipeDetector;
	[SerializeField] 
	FreeRotation m_freeRotation;

	[SerializeField] 
	GameObject m_memorizeText;
    [SerializeField]
    GameObject m_levelCompleteText;
    [SerializeField]
    GameObject m_perfectText;
	[SerializeField]
	GameObject m_levelUnlockedText;
	[SerializeField]
	GameObject m_gameOverText;
	[SerializeField]
	Text[] m_levelNumberTexts;
	[SerializeField]
	RawImage[] m_UIThingsThatChangeColour;
	[SerializeField]
	MeshRenderer m_backPlaneMeshRenderer;
	[SerializeField]
	GameObject m_gameOverButtons;
	[SerializeField]
	HUDController m_hudController;

	[SerializeField] 
	Material m_prototypeFaceMaterial;

	[SerializeField] 
	Texture[] m_faceTextures;
    [SerializeField]
    Material m_memorizeMaterial;
	[SerializeField]
	bool[] m_levelRequiresCorrectCubeOrientation;
	[SerializeField]
	int[] m_levelNumberOfLives;
	[SerializeField]
	Color[] m_levelHUDColours;

    [SerializeField] 
	BackgroundPlane m_backgroundPlane;
	[SerializeField] 
	GameObject m_ImReadyButton;
    [SerializeField]
    Fader m_faderPlane;
    [SerializeField]
    CubeOrientation m_cubeOrientation;
	[SerializeField]
	GameObject m_memorizePlane;

	[SerializeField]
	AudioSource m_audioSource;
	[SerializeField]
	AudioClip m_audioImReadyAppears;
	[SerializeField]
	AudioClip m_audioImReadyPressed;
	[SerializeField]
	AudioClip m_audioGotFaceCorrect;
	[SerializeField]
	AudioClip m_audioGotFaceWrong;
	[SerializeField]
	AudioClip m_audioCubeComplete;
	[SerializeField]
	AudioClip m_audioPerfect;

//    [SerializeField]
//    FaceStates m_agTempFaceStates;

    int[] m_cubeMaterialMappings = new int[NUM_FACES_ON_A_CUBE];
	List<Material> m_faceMaterialsList = new List<Material> ();
	CubeState.RotationActions[,] m_sequence = new CubeState.RotationActions[2,6]
		{ 
				{ CubeState.RotationActions.RIGHT, CubeState.RotationActions.RIGHT, CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT, CubeState.RotationActions.RIGHT, CubeState.RotationActions.UP },
				{ CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT, CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT, CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT } 
		};
	bool m_flipSequenceHorizontally = false;
	bool m_flipSequenceVertically = false;
	int m_sequencePosition = 0;
	int m_sequenceType = 0;
	int m_sequenceStartPosition = 0;
	int m_livesLeft, m_initialNumberOfLives;

	public enum GameState
	{
		MEMORIZE,
		ROTATE_CUBE_INTO_POSITION,
		SHOW_REQUIRED_FACE,
		FIND,
		ROTATE_BACK_TO_PREVIOUS
	}

	GameState m_currentGameState;

	public GameState State
	{
		get
		{
			return m_currentGameState;
		}
	}

	static GameController m_instance = null;

	public static GameController Instance
	{
		get
		{
			return m_instance;
		}
	}

	public GameController()
	{
		m_instance = this;
	}

	void Start()
	{
		m_gameStateClass = GameStateClass.Instance;
		m_cubeState = m_cube.GetComponent<CubeState>();
        m_initialCubePosition = m_cubeMover.gameObject.transform.localPosition;
        ResetCubeOrientation();
		GenerateMaterialsFromTextures ();
		SetUpFacesOfCube();
		NewGame();
	}



	void GenerateMaterialsFromTextures()
	{
		foreach (Texture t in m_faceTextures) {
			Material m = new Material (m_prototypeFaceMaterial);
			m.SetTexture ("_MainTex", t);
			m_faceMaterialsList.Add (m);
		}
	}

	void StartMoveSequence()
	{
		if ((m_gameStateClass != null) && (m_gameStateClass.GetLevelNumber() == 0))	// && !m_gameStateClass.HasTutorialBeenShown (TutorialManager.MessageID.SWIPE_TO_ROTATE)) 
		{
			// predetermined sequence for the first ever play, so that the tutorial message matches with what your supposed to do.
			m_sequenceType = 0;
			m_sequenceStartPosition = 0;
			m_sequencePosition = m_sequenceStartPosition;
			m_flipSequenceHorizontally = false;
			m_flipSequenceVertically = false;
		} 
		else 
		{	
			// set up everything for the start of a new sequence of 6 moves
			m_sequenceType = Random.Range (0, 2);
			m_sequenceStartPosition = Random.Range (0, 6);
			m_sequencePosition = m_sequenceStartPosition;
			m_flipSequenceHorizontally = Random.value < 0.5f;
			m_flipSequenceVertically = Random.value < 0.5f;
		}
	}

	CubeState.RotationActions GetNextMoveInSequence()
	{
		CubeState.RotationActions unflipped = m_sequence[m_sequenceType,m_sequencePosition];
        CubeState.RotationActions xflipped;
        if (m_flipSequenceHorizontally)
            xflipped = CubeState.m_flipHorizontally[(int)unflipped];
        else
            xflipped = unflipped;

        CubeState.RotationActions yflipped;
        if (m_flipSequenceVertically)
            yflipped = CubeState.m_flipVertically[(int)xflipped];
        else
            yflipped = xflipped;

		if (++m_sequencePosition >= 6)
		{
			m_sequencePosition = 0;
		}

		return yflipped;
	}



	void ResetCubeOrientation()
	{
		m_cubeState.Init();
		m_cube.GetComponentInChildren<Rotator>().ResetRotation();
	}

	void NewGame()
	{
		m_gameStateClass.FadeOutMusic();

		//m_score = 0;
		int levelNum = 0;
		if (m_gameStateClass != null)
		{
			levelNum = m_gameStateClass.GetLevelNumber();
		}
		m_initialNumberOfLives = m_levelNumberOfLives [levelNum];
		m_livesLeft = m_initialNumberOfLives;
		//ShowLivesLeft();
		m_ImReadyButton.SetActive(false);

		ResetCubeOrientation();

		m_hudController.SetupLives(m_initialNumberOfLives);
		m_hudController.SetupProgression(NUM_FACES_ON_A_CUBE);
		if (levelNum < m_levelHUDColours.Length)
		{
			m_hudController.SetColour(m_levelHUDColours[levelNum]);
			SetColourOfUIThingsThatChangeColour(m_levelHUDColours[levelNum]);
		}
		SetLevelNumberTexts(levelNum+1);

		m_memorizePlane.SetActive (true);

  //      StartMoveSequence();
  //      m_requiredNumber = GetNextRequiredFace(GetNextMoveInSequence());
  //      //PickRandomRequiredFace();
		//DisplayRequiredFace();

		//m_cube.GetComponent<Cube>().PlayShowSidesAnimation();
		SwitchToState(GameState.MEMORIZE);
	}

	void SetLevelNumberTexts(int number)
	{
		foreach(Text tex in m_levelNumberTexts)
		{
			tex.text = "Level "+number;
		}
	}

	void SetColourOfUIThingsThatChangeColour(Color col)
	{
		foreach(RawImage raw in m_UIThingsThatChangeColour)
		{
			raw.color = col;
		}

		Material material = m_backPlaneMeshRenderer.material;
		material.color = col;
	}

	FaceState GetNextRequiredFace(CubeState.RotationActions move)
    {
		FaceStates faceStates = m_cubeOrientation.GetFaceStates();
		FaceState returnValue = null;
        switch (move)
        {
            case CubeState.RotationActions.UP:
				returnValue = faceStates.GetFaceState(CubeState.Faces.BOTTOM);
                break;
            case CubeState.RotationActions.DOWN:
				returnValue = faceStates.GetFaceState(CubeState.Faces.TOP);
                break;
            case CubeState.RotationActions.LEFT:
				returnValue = faceStates.GetFaceState(CubeState.Faces.RIGHT);
                break;
            case CubeState.RotationActions.RIGHT:
				returnValue = faceStates.GetFaceState(CubeState.Faces.LEFT);
                break;
        }

        Debug.Log("GetNextRequiredFace = " + returnValue);

        return returnValue;
    }


    void DisplayRequiredFace()
    {
		m_backgroundPlane.SetAllMaterials(m_faceMaterialsList[m_cubeMaterialMappings[m_requiredFace.m_state]]);
		float rot = 0f;
		switch (m_requiredFace.m_orientation) {
		case FaceState.Orientations.UP:
			rot = 0f;
			break;
		case FaceState.Orientations.DOWN:
			rot = 180f;
			break;
		case FaceState.Orientations.LEFT:
			rot = 90f;
			break;
		case FaceState.Orientations.RIGHT:
			rot = -90f;
			break;
		}
		if (m_requiredFace.m_state == 3)
			rot -= 90f;
		if (m_requiredFace.m_state == 1)
			rot += 90f;
		if (m_requiredFace.m_state == 5)
			rot += 180f;

		m_backgroundPlane.transform.rotation = Quaternion.Euler(0f,0f,rot);
    }

    void ChangeRequiredFace()
    {
        StartCoroutine(ChangeRequiredFaceCoroutine());
    }

    IEnumerator ChangeRequiredFaceCoroutine()
    {
		m_faderPlane.gameObject.SetActive (true);
        m_faderPlane.FadeTo(new Color(1f, 1f, 1f, 1f), 0.10f);
        yield return new WaitForSeconds(0.15f);
        DisplayRequiredFace();
        //m_cubeMover.SetPosition(m_initialCubePosition);
        CubePullsBackOutOfBackground();
        m_faderPlane.FadeTo(new Color(1f, 1f, 1f, 0f), 0.10f);
        yield return new WaitForSeconds(0.2f);
		m_faderPlane.gameObject.SetActive (false);
    }

	IEnumerator ShowFirstFaceFaceCoroutine()
	{
		m_faderPlane.gameObject.SetActive (true);
		m_faderPlane.FadeTo(new Color(1f, 1f, 1f, 1f), 0.10f);
		yield return new WaitForSeconds(0.15f);
		DisplayRequiredFace();
		m_memorizePlane.SetActive (false);
		m_faderPlane.FadeTo(new Color(1f, 1f, 1f, 0f), 0.10f);
		yield return new WaitForSeconds(0.2f);
		m_faderPlane.gameObject.SetActive (false);
	}

	void LevelCompleteAnalytics()
	{
		int level = GameStateClass.Instance.GetLevelNumber () + 1;
		//Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_COMPLETED, "level", level);
		switch (level) 
		{
		case 1:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_1_COMPLETED);
			break;
		case 2:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_2_COMPLETED);
			break;
		case 3:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_3_COMPLETED);
			break;
		case 4:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_4_COMPLETED);
			break;
		case 5:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_5_COMPLETED);
			break;
		case 10:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_10_COMPLETED);
			break;
		case 15:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_15_COMPLETED);
			break;
		case 16:
			Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.LEVEL_16_COMPLETED);
			break;
		}
	}

    IEnumerator LevelCompleteCoroutine()
    {
		m_audioSource.PlayOneShot(m_audioCubeComplete, 1f);

		LevelCompleteAnalytics ();

        m_levelCompleteText.SetActive(true);
        yield return new WaitForSeconds(1.6f);
        m_levelCompleteText.SetActive(false);

		if (m_livesLeft == m_initialNumberOfLives) 
		{
			// perfect score - the next level is unlocked
			m_audioSource.PlayOneShot(m_audioPerfect, 1f);
			m_perfectText.SetActive (true);
			yield return new WaitForSeconds (2.0f);
			m_perfectText.SetActive (false);
		}

//        m_levelUnlockedText.SetActive(true);
//        yield return new WaitForSeconds(1.0f);
//        m_levelUnlockedText.SetActive(false);

        if (m_gameStateClass != null)
        {
			m_gameStateClass.CompletedLevel(m_initialNumberOfLives - m_livesLeft);
        }

		if (m_gameStateClass.GetLevelNumber () == GameStateClass.NUMBER_OF_LEVELS - 1) 
		{
			SceneManager.LoadScene ("completedGame");
		} 
		else 
		{
			GameStateClass.Instance.FadeInMusic();
			SceneManager.LoadScene ("frontEnd");
		}
    }

    IEnumerator CorrectFaceSequence()
    {
		m_touchSwipeDetector.enabled = false;
		m_audioSource.PlayOneShot(m_audioGotFaceCorrect, 1f);
        CubeSinksIntoBackground();
        yield return new WaitForSeconds(1.5f);
        m_faceFoundCount++;
		if (m_faceFoundCount == NUM_FACES_ON_A_CUBE)
        {
            yield return LevelCompleteCoroutine();
        }
        else
        {
            yield return ChangeRequiredFaceCoroutine();
        }

		m_touchSwipeDetector.enabled = true;
    }

    void CubeSinksIntoBackground()
    {
        Vector3 pos = m_initialCubePosition;
        pos.z = 5.47f;
        m_cubeMover.MoveTo(pos, 0.75f);
    }

    void CubePullsBackOutOfBackground()
    {
        Vector3 pos = m_initialCubePosition;
        m_cubeMover.MoveTo(pos, 0.25f);
    }





    void CheckIfCorrect()
	{
		TutorialManager.Instance.ClearAllTutorialMessages ();

		FaceStates faceStates = m_cubeOrientation.GetFaceStates();
		if (!m_cubeState.CheckState(faceStates))
        {
            Debug.LogError("ERROR!!!! difference in facestates");
        }

		int levelNum = 0;
		if (m_gameStateClass != null)
		{
			levelNum = m_gameStateClass.GetLevelNumber();
		}

		bool correctOrientation = (faceStates.GetFaceState(CubeState.Faces.FRONT).m_orientation == m_requiredFace.m_orientation);
		bool levelRequiresCorrectCubeOrientation = true;
		if (levelNum < m_levelRequiresCorrectCubeOrientation.Length) 
		{
			levelRequiresCorrectCubeOrientation = m_levelRequiresCorrectCubeOrientation [levelNum];
		}

		if ((faceStates.GetFaceState(CubeState.Faces.FRONT).m_state == m_requiredFace.m_state) &&
			(correctOrientation || !levelRequiresCorrectCubeOrientation))
        {
            // correct
            //IncrementScore();
			m_hudController.AdvanceProgression(m_faceFoundCount);

			m_requiredFace = GetNextRequiredFace(GetNextMoveInSequence());
            //PickRandomRequiredFace();
            StartCoroutine(CorrectFaceSequence());
        }
		else
		{
			// wrong
			StartCoroutine(GotItWrongCoRoutine());
		}
	}

	IEnumerator GotItWrongCoRoutine()
	{
		m_audioSource.PlayOneShot(m_audioGotFaceWrong, 1f);

		m_cube.GetComponentInChildren<Rotator>().BlockRotationForTimePeriod(1.0f);
		m_cube.GetComponent<Cube>().PlayShakeHeadAnimation();
		yield return new WaitForSeconds(0.5f);
		DecrementLivesLeft();
		yield return new WaitForSeconds(0.5f);
		m_cube.GetComponent<Cube>().ResetAnimation();

		if (m_livesLeft == 0) 
		{
			m_touchSwipeDetector.enabled = false;
			m_hudController.ScrollOut();
			m_gameOverText.SetActive(true);
			GameStateClass.Instance.RecordLastPlayedLevel ();
			yield return new WaitForSeconds(1.5f);
			m_gameOverButtons.SetActive (true);
		} 
		else 
		{
			if (CHOOSE_ANOTHER_FACE_AFTER_EACH_WRONG_ANSWER) {
				// you've chosen the wrong face. Now we choose another face to find that is adjacent to this one.
				m_requiredFace = GetNextRequiredFace (GetNextMoveInSequence ());
				yield return ChangeRequiredFaceCoroutine ();
			} else {
				// go back to the face we were looking at before you got it wrong
				SwitchToState (GameState.ROTATE_BACK_TO_PREVIOUS);
				//yield return new WaitForSeconds(0.5f);
			}
		}
	}

	void FinishedRotatingBackToPrevious()
	{
		SwitchToState(GameState.FIND);
	}

	void DecrementLivesLeft()
	{
		m_livesLeft--;
		ShowLivesLeft();
	}

	void ShowLivesLeft()
	{
		m_hudController.LoseLife(m_livesLeft);
	}



	void SwitchToState(GameState state)
	{
		Rotator rotator;
		rotator = m_cube.GetComponentInChildren<Rotator>();

		m_currentGameState = state;

		switch(state)
		{
		case GameState.MEMORIZE:
            m_backgroundPlane.SetAllMaterials(m_memorizeMaterial);
            m_cube.GetComponent<Cube>().ResetAnimation();
			m_freeRotation.enabled = true;
			m_touchSwipeDetector.enabled = false;
			m_memorizeText.SetActive(true);
			Countdown countdown = m_memorizeText.gameObject.GetComponentInChildren<Countdown>();
			m_ImReadyButton.SetActive(false);
			//countdown.StartListening("COUNTDOWN_COMPLETE", MemorizeCountdownComplete);
			break;
		case GameState.ROTATE_CUBE_INTO_POSITION:
			// rotate cube to initial position
			m_freeRotation.enabled = false;
			m_touchSwipeDetector.enabled = false;
			m_memorizeText.SetActive(false);
			RotateCubeToStartPosition();
			break;
		case GameState.FIND:
            // enable controls
            m_freeRotation.enabled = false;
			m_touchSwipeDetector.enabled = true;
			rotator.StopListening("ROTATION_COMPLETE", FinishedRotatingBackToPrevious);
			rotator.StartListening("ROTATION_COMPLETE", CheckIfCorrect);
			break;
		case GameState.ROTATE_BACK_TO_PREVIOUS:
			// rotate cube to initial position
			m_freeRotation.enabled = false;
			m_touchSwipeDetector.enabled = false;
			m_memorizeText.SetActive(false);
			rotator.StopListening("ROTATION_COMPLETE", CheckIfCorrect);
			rotator.StartListening("ROTATION_COMPLETE", FinishedRotatingBackToPrevious);
			rotator.ReversePreviousRotation();
			break;
		}

	}

	public void StartRevealOfImReadyButton()
	{
		StartCoroutine(ShowImReadyButtonAfterDelay());
	}

	IEnumerator ShowImReadyButtonAfterDelay()
	{
		yield return new WaitForSeconds(2.0f);
		m_ImReadyButton.SetActive(true);
		m_audioSource.PlayOneShot(m_audioImReadyAppears, 1f);
	}

	public void MemorizeCountdownComplete()
	{
		// user has just pressed the 'I'm ready' button
		m_audioSource.PlayOneShot(m_audioImReadyPressed, 1f);
		m_ImReadyButton.SetActive(false);
		SwitchToState(GameState.ROTATE_CUBE_INTO_POSITION);
    }

	void RotateCubeToStartPosition()
	{
		Rotator rotator = m_cube.GetComponentInChildren<Rotator>();
		rotator.StopListening("ROTATION_COMPLETE", CheckIfCorrect);

        rotator.RotateTo(m_cubeOrientation.Closest90DegreePosition(), 1f);

        //rotator.RotateTo(Quaternion.identity, 1f);
		StartCoroutine(WaitForRotateCubeToStartPosition());
	}

	IEnumerator WaitForRotateCubeToStartPosition()
	{
		yield return new WaitForSeconds(0.75f);
		m_hudController.ScrollIn();
		yield return new WaitForSeconds(0.75f);

		StartMoveSequence();
		m_requiredFace = GetNextRequiredFace(GetNextMoveInSequence());
		//PickRandomRequiredFace();
		yield return ShowFirstFaceFaceCoroutine();

		if ((m_gameStateClass != null) && (m_gameStateClass.GetLevelNumber () == 0)) // && !m_gameStateClass.HasTutorialBeenShown (TutorialManager.MessageID.SWIPE_TO_ROTATE)) 
		{
			TutorialManager.Instance.ShowTutorialMessage (TutorialManager.MessageID.SWIPE_TO_ROTATE);
		}

		SwitchToState(GameState.FIND);
	}

	void SetUpFacesOfCube()
	{
		ChooseFaceMaterialMappings();
		ApplyMaterialsToCube();
	}

	void ChooseFaceMaterialMappings()
	{
		int levelNum = 0;
		if (m_gameStateClass != null)
		{
			levelNum = m_gameStateClass.GetLevelNumber();
		}

		m_cubeMaterialMappings[0] = (levelNum * NUM_FACES_ON_A_CUBE) + 1;	// + 0;
		m_cubeMaterialMappings[1] = (levelNum * NUM_FACES_ON_A_CUBE) + 2;	// + 1;
		m_cubeMaterialMappings[2] = (levelNum * NUM_FACES_ON_A_CUBE) + 5;	// + 2;
		m_cubeMaterialMappings[3] = (levelNum * NUM_FACES_ON_A_CUBE) + 3;	// + 4;
		m_cubeMaterialMappings[4] = (levelNum * NUM_FACES_ON_A_CUBE) + 0;	// + 3;
		m_cubeMaterialMappings[5] = (levelNum * NUM_FACES_ON_A_CUBE) + 4;	// + 5;
	}

	void ApplyMaterialsToCube()
	{
		for(int i = 0; i<6; i++)
		{
			Cube cube = m_cube.GetComponent<Cube>();
			cube.SetFaceMaterial(i, m_faceMaterialsList[m_cubeMaterialMappings[i]]);
		}
	}

	public void LevelRetry()
	{
		SceneManager.LoadScene("mainGame");
	}

	public void BackToMainMenu()
	{
		GameStateClass.Instance.FadeInMusic();

		SceneManager.LoadScene("frontEnd");
	}

}
