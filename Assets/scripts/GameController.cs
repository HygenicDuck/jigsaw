using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
	const int NUM_FACES_ON_A_CUBE = 6;
	int m_requiredNumber = 0;
	int m_score = 0;
    int m_faceFoundCount = 0;
	CubeState m_cubeState = null;
	GameStateClass m_gameStateClass = null;
    Vector3 m_initialCubePosition;

    [SerializeField] 
	Text m_UIRequiredNumber;
	[SerializeField] 
	Text m_UIScore;
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
	Material[] m_faceMaterials;
	[SerializeField] 
	Material[] m_UIfaceMaterials;
    [SerializeField]
    Material m_memorizeMaterial;

    //	[SerializeField] 
    //	RawImage m_UIRequiredFaceImage;
    [SerializeField] 
	BackgroundPlane m_backgroundPlane;
	[SerializeField] 
	GameObject m_ImReadyButton;
    [SerializeField]
    Fader m_faderPlane;
    [SerializeField]
    CubeOrientation m_cubeOrientation;

    [SerializeField]
    FaceStates m_agTempFaceStates;

    int[] m_cubeMaterialMappings = new int[NUM_FACES_ON_A_CUBE];
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
	//CubeState.Faces[] m_sequence2 = { CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT, CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT, CubeState.RotationActions.UP, CubeState.RotationActions.RIGHT };


	enum GameState
	{
		MEMORIZE,
		ROTATE_CUBE_INTO_POSITION,
		SHOW_REQUIRED_FACE,
		FIND
	}

	GameState m_currentGameState;

	void Start()
	{
		//GameObject gOb = GameObject.Find("persistentObject");
		//if (gOb != null)
		//{
		m_gameStateClass = GameStateClass.Instance;
		//}

		m_cubeState = m_cube.GetComponent<CubeState>();
        m_initialCubePosition = m_cubeMover.gameObject.transform.localPosition;
        ResetCubeOrientation();
		SetUpFacesOfCube();
		NewGame();
	}





	void StartMoveSequence()
	{
		// set up everything for the start of a new sequence of 6 moves
		m_sequenceType = Random.Range(0,2);
        //m_sequenceStartPosition = 2;
        //m_flipSequenceHorizontally = false;
        //m_flipSequenceVertically = false;

        m_sequenceStartPosition = Random.Range(0,6);
		m_sequencePosition = m_sequenceStartPosition;
		m_flipSequenceHorizontally = Random.value < 0.5f;
		m_flipSequenceVertically = Random.value < 0.5f;
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

        Debug.Log("GetNextMoveInSequence = " + yflipped);
		return yflipped;
	}



	void ResetCubeOrientation()
	{
		m_cubeState.Init();
		m_cube.GetComponentInChildren<Rotator>().ResetRotation();
	}

	void NewGame()
	{
		m_score = 0;
		ShowScore();
		m_ImReadyButton.SetActive(false);

		ResetCubeOrientation();
  //      StartMoveSequence();
  //      m_requiredNumber = GetNextRequiredFace(GetNextMoveInSequence());
  //      //PickRandomRequiredFace();
		//DisplayRequiredFace();

		//m_cube.GetComponent<Cube>().PlayShowSidesAnimation();
		SwitchToState(GameState.MEMORIZE);
	}

    int GetNextRequiredFace(CubeState.RotationActions move)
    {
        m_agTempFaceStates = m_cubeOrientation.GetFaceStates();
        int returnValue = 0;
        switch (move)
        {
            case CubeState.RotationActions.UP:
                returnValue = m_agTempFaceStates.GetFaceState(CubeState.Faces.BOTTOM).m_state;
                break;
            case CubeState.RotationActions.DOWN:
                returnValue = m_agTempFaceStates.GetFaceState(CubeState.Faces.TOP).m_state;
                break;
            case CubeState.RotationActions.LEFT:
                returnValue = m_agTempFaceStates.GetFaceState(CubeState.Faces.RIGHT).m_state;
                break;
            case CubeState.RotationActions.RIGHT:
                returnValue = m_agTempFaceStates.GetFaceState(CubeState.Faces.LEFT).m_state;
                break;
        }

        Debug.Log("GetNextRequiredFace = " + returnValue);

        return returnValue;
    }


 //   void PickRandomRequiredFace()
	//{
 //       int[] adjacentNumbers = new int[4];
 //       m_agTempFaceStates = m_cubeOrientation.GetFaceStates();
 //       adjacentNumbers[0] = m_agTempFaceStates.GetFaceState(CubeState.Faces.TOP).m_state;
	//	adjacentNumbers[1] = m_agTempFaceStates.GetFaceState(CubeState.Faces.BOTTOM).m_state;
	//	adjacentNumbers[2] = m_agTempFaceStates.GetFaceState(CubeState.Faces.LEFT).m_state;
	//	adjacentNumbers[3] = m_agTempFaceStates.GetFaceState(CubeState.Faces.RIGHT).m_state;
	//	m_requiredNumber = adjacentNumbers[Random.Range(0,4)];
	//}

    void DisplayRequiredFace()
    {
        Debug.Log("m_requiredNumber " + m_requiredNumber + ", " + m_cubeMaterialMappings[m_requiredNumber]);
        m_backgroundPlane.SetAllMaterials(m_faceMaterials[m_cubeMaterialMappings[m_requiredNumber]]);
    }

    void ChangeRequiredFace()
    {
        StartCoroutine(ChangeRequiredFaceCoroutine());
    }

    IEnumerator ChangeRequiredFaceCoroutine()
    {
        m_faderPlane.FadeTo(new Color(1f, 1f, 1f, 1f), 0.15f);
        yield return new WaitForSeconds(0.25f);
        DisplayRequiredFace();
        //m_cubeMover.SetPosition(m_initialCubePosition);
        CubePullsBackOutOfBackground();
        m_faderPlane.FadeTo(new Color(1f, 1f, 1f, 0f), 0.15f);
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator LevelCompleteCoroutine()
    {
        m_levelCompleteText.SetActive(true);
        yield return new WaitForSeconds(1.6f);
        m_levelCompleteText.SetActive(false);

        if (m_score == 0)
        {
            // perfect score - the next level is unlocked
            m_perfectText.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            m_perfectText.SetActive(false);
            m_levelUnlockedText.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            m_levelUnlockedText.SetActive(false);
        }

        if (m_gameStateClass != null)
        {
            m_gameStateClass.CompletedLevel(m_score);
        }
        SceneManager.LoadScene("frontEnd");
    }

    IEnumerator CorrectFaceSequence()
    {
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
        m_agTempFaceStates = m_cubeOrientation.GetFaceStates();
        if (!m_cubeState.CheckState(m_agTempFaceStates))
        {
            Debug.LogError("ERROR!!!! difference in facestates");
        }

        //Debug.Log("front = "+m_cubeState.GetFaceState(CubeState.Faces.FRONT).m_state+", m_requiredNumber = "+m_requiredNumber);
        //if (m_cubeState.GetFaceState(CubeState.Faces.FRONT).m_state == m_requiredNumber)
        Debug.Log("front = "+ m_agTempFaceStates.GetFaceState(CubeState.Faces.FRONT).m_state+", m_requiredNumber = "+m_requiredNumber);
        if (m_agTempFaceStates.GetFaceState(CubeState.Faces.FRONT).m_state == m_requiredNumber)
        {
            // correct
            //IncrementScore();
            m_requiredNumber = GetNextRequiredFace(GetNextMoveInSequence());
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
		m_cube.GetComponentInChildren<Rotator>().BlockRotationForTimePeriod(1.0f);
		m_cube.GetComponent<Cube>().PlayShakeHeadAnimation();
		yield return new WaitForSeconds(0.5f);
		IncrementScore();
		yield return new WaitForSeconds(0.5f);
		m_cube.GetComponent<Cube>().ResetAnimation();

		//yield return new WaitForSeconds(2.0f);
		//NewGame();
	}

	void IncrementScore()
	{
		m_score++;
		ShowScore();
	}

	void ShowScore()
	{
		m_UIScore.text = m_score.ToString();
	}



	void SwitchToState(GameState state)
	{
		Rotator rotator;
		rotator = m_cube.GetComponentInChildren<Rotator>();

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
			StartCoroutine(ShowImReadyButtonAfterDelay());
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
            StartMoveSequence();
            m_requiredNumber = GetNextRequiredFace(GetNextMoveInSequence());
            //PickRandomRequiredFace();
            DisplayRequiredFace();

            // enable controls
            m_freeRotation.enabled = false;
			m_touchSwipeDetector.enabled = true;
			rotator.StartListening("ROTATION_COMPLETE", CheckIfCorrect);
			break;
		}

		m_currentGameState = state;
	}

	IEnumerator ShowImReadyButtonAfterDelay()
	{
		yield return new WaitForSeconds(3.5f);
		m_ImReadyButton.SetActive(true);
	}

	public void MemorizeCountdownComplete()
	{
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
		yield return new WaitForSeconds(1.5f);
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

		m_cubeMaterialMappings[0] = (levelNum * NUM_FACES_ON_A_CUBE) + 0;
        m_cubeMaterialMappings[1] = (levelNum * NUM_FACES_ON_A_CUBE) + 1;
		m_cubeMaterialMappings[2] = (levelNum * NUM_FACES_ON_A_CUBE) + 2;
		m_cubeMaterialMappings[3] = (levelNum * NUM_FACES_ON_A_CUBE) + 4;
		m_cubeMaterialMappings[4] = (levelNum * NUM_FACES_ON_A_CUBE) + 3;
		m_cubeMaterialMappings[5] = (levelNum * NUM_FACES_ON_A_CUBE) + 5;
	}

	void ApplyMaterialsToCube()
	{
		for(int i = 0; i<6; i++)
		{
			Cube cube = m_cube.GetComponent<Cube>();
			cube.SetFaceMaterial(i, m_faceMaterials[m_cubeMaterialMappings[i]]);
		}
	}

}
