using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCarouselController : MonoBehaviour {

	[SerializeField] 
	ScrollRect m_scrollRect = null;
	[SerializeField] 
	Movement m_contentMover = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveToLevel(int levelNum, int lastPlayedLevel)
	{
		StartCoroutine(MoveToLevelCoroutine(levelNum, lastPlayedLevel));
	}

	IEnumerator MoveToLevelCoroutine(int levelNum, int lastPlayedLevel)
	{
		const float SCROLLTIMEMULTIPLIER = 0.0015f;

		m_scrollRect.enabled = false;
		float iconDistance = IconDistance ();

		float startpos = 0f;

		if (lastPlayedLevel >= 2) 
		{
			startpos = iconDistance * (lastPlayedLevel - 1);
		}
		m_contentMover.SetPosition (new Vector2 (-startpos, 0f));

		if (levelNum >= 2) 
		{
			float distance = IconDistance() * (levelNum-1);
			float scrolltime = SCROLLTIMEMULTIPLIER * (distance - startpos);
			m_contentMover.MoveTo (new Vector2 (-distance, 0f), scrolltime);

			yield return new WaitForSeconds (scrolltime);
		}

		m_scrollRect.enabled = true;
	}

	float IconDistance()
	{
		// calc the distance between level icons
		RectTransform content = m_contentMover.gameObject.transform as RectTransform;
		Transform child = content.GetChild (0);
		RectTransform childRect = child.gameObject.GetComponent<RectTransform> ();
		float width = childRect.rect.width;
		Debug.Log ("IconDistance width = " + width);

		HorizontalLayoutGroup hor = content.gameObject.GetComponent<HorizontalLayoutGroup> ();
		float spacing = hor.spacing;
		Debug.Log ("IconDistance spacing = " + spacing);

		return width + spacing;
	}
}
