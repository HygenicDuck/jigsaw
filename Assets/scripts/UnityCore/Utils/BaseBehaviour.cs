using UnityEngine;

namespace Core
{
	namespace Utils
	{
		/// <summary>
		/// Base behaviour.
		/// All classes which inherit from MonoBehaviour should inherit from this class instead
		/// </summary>

		public class BaseBehaviour : MonoBehaviour
		{
			protected virtual void OnDestroy()
			{
			}

			protected virtual void Awake()
			{
			}

			protected virtual void Start()
			{
			}

			protected virtual void OnEnable()
			{
			}

			protected virtual void OnDisable()
			{
			}

			protected virtual void Update()
			{
			}
		}
	}
}