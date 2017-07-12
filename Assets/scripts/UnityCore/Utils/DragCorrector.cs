using UnityEngine;
using UnityEngine.EventSystems;

public class DragCorrector : MonoBehaviour
{
	[SerializeField] public int m_baseTH = 10;
	[SerializeField] public int m_basePPI = 210;

	private int m_dragTH = 0;

	void Start()
	{
		m_dragTH = (m_baseTH * (int)UnityEngine.Screen.dpi) / m_basePPI;
		UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = m_dragTH;
	}
}