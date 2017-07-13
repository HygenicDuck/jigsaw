using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class completedGame : MonoBehaviour {

	[SerializeField] 
	GameObject m_continueButton;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (ActivateContinueButtonAfterAWhile());
		GameStateClass.Instance.FadeInMusic();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator ActivateContinueButtonAfterAWhile()
	{
		yield return new WaitForSeconds(5.0f);
		m_continueButton.SetActive (true);
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene("frontEnd");
	}

}
