using UnityEngine;
using System.Collections;

public class BackgroundPlane : MonoBehaviour {

	[SerializeField] private GameObject m_planeCell; 
	[SerializeField] private int m_gridSizeX; 
	[SerializeField] private int m_gridSizeY; 

	// Use this for initialization
	void Start () 
	{
		int minX = -m_gridSizeX/2;
		int minY = -m_gridSizeY/2;
		// create the grid of background planes
		for(int x=0; x<m_gridSizeX; x++)
		{
			for(int y=0; y<m_gridSizeY; y++)
			{
				Vector3 pos = new Vector3((x+minX)*10f,(y+minY)*10f,0);
				GameObject planeCell = Instantiate(m_planeCell) as GameObject;
				planeCell.transform.parent = gameObject.transform;
				planeCell.transform.localPosition = pos;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetAllMaterials(Material material)
	{
		foreach(Transform child in transform)
		{
			MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
			Material[] mats = mr.materials;
			mats[0] = material;
			mr.materials = mats;
		}
	}
}
