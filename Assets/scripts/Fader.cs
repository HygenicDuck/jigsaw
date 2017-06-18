// Conversion of standard DragRigidbody.js to DragRigidbody2D.cs
// Ian Grant v001
using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour
{
	float m_timer;
	float m_moveDuration;
	Color m_startColor;
	Color m_destColor;
	bool m_moving;
    [SerializeField]
    Material m_material;


	void Awake()
	{
		m_moving = false;
		m_timer = 0f;
       // m_material = GetComponent<Material>();
    }

    // Update
    void Update ()
	{
		if (m_moving)
		{
			m_timer += Time.deltaTime;
			if (m_timer >= m_moveDuration)
			{
				m_timer = m_moveDuration;
				m_moving = false;
			}

			float p = m_timer / m_moveDuration;
            Color col = new Color(
                lerp(m_startColor.r, m_destColor.r, p),
                lerp(m_startColor.g, m_destColor.g, p),
                lerp(m_startColor.b, m_destColor.b, p),
                lerp(m_startColor.a, m_destColor.a, p)
                );
            m_material.color = col;
		}
	}

    float lerp(float start, float dest, float p)
    {
        return start + (dest - start) * p;
    }

	public void FadeTo(Color color, float duration)
	{
		m_timer = 0f;
		m_moveDuration = duration;
		m_moving = true;
        //m_material = GetComponent<Material>();
        m_startColor = m_material.color;
        m_destColor = color;
	}

}

