using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

	[SerializeField] 
	Animator m_animator;

	[SerializeField] 
	MeshRenderer[] m_faceRenderers;


	void Start()
	{
	}

	public void ResetAnimation()
	{
		Debug.Log("reset animation");
		m_animator.Play("static");
	}

	public void PlayShakeHeadAnimation()
	{
		Debug.Log("shake head 2");
		m_animator.Play("shake head 2");
	}

	public void PlayShowSidesAnimation()
	{
		Debug.Log("Show side faces");
		//m_animator.Play("Show side faces");
		m_animator.Play("cube intro");
	}

	public void SetFaceMaterial(int faceNum, Material material)
	{
		m_faceRenderers[faceNum].material = material;
	}


//	void Start()
//	{
//		Mesh mesh = GetComponent<MeshFilter>().mesh;
//		Vector3[] vertices = mesh.vertices;
//		Vector2[] uvs = new Vector2[] { 
//			new Vector2(2,0), 
//			new Vector2(3,0), 
//			new Vector2(2,1), 
//			new Vector2(3,1), 
//
//			new Vector2(1,0), 
//			new Vector2(2,0), 
//			new Vector2(1,1), 
//			new Vector2(2,1), 
//
//			new Vector2(0,1), 
//			new Vector2(1,1), 
//			new Vector2(0,2), 
//			new Vector2(1,2), 
//
//			new Vector2(2,2), 
//			new Vector2(3,2), 
//			new Vector2(2,1), 
//			new Vector2(3,1), 
//
//			new Vector2(1,1), 
//			new Vector2(2,1), 
//			new Vector2(1,2), 
//			new Vector2(2,2), 
//
//			new Vector2(0,1), 
//			new Vector2(1,1), 
//			new Vector2(0,0), 
//			new Vector2(1,0)
//		};
//
//		for (int i = 0; i < uvs.Length; i++)
//		{
//			uvs[i].x /= 4;
//			uvs[i].y /= 2;
////			uvs[i] = new Vector2(vertices[i].x*6, vertices[i].z*6);
////			Debug.Log("cube vertex "+i+" xyz = "+vertices[i].x+", "+vertices[i].y+", "+vertices[i].z);
//		}
//
//		Debug.Log("vertices.Length "+vertices.Length);
//		Debug.Log("uvs.Length "+uvs.Length);
//
//		mesh.uv = uvs;
//	}
}
