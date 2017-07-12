using UnityEngine;
using System.Collections;

namespace Core
{
	namespace Utils
	{
		public class BaseServer
		{
			private static int s_instanceId;
			private string m_requestGroupId;

			protected BaseServer(RequestBaseBehaviour owner)
			{
				if (owner != null)
				{
					owner.RegisterServer(this);
					m_requestGroupId = ToString() + (s_instanceId++);
				}
			}

			protected string GetRequestGroup()
			{
				return m_requestGroupId;
			}

			public void CancelRequests()
			{
				#if KANGA
				Kanga.Server.Instance.CancelGroupRequest(m_requestGroupId);
				#endif
			}
		}
	}
}

