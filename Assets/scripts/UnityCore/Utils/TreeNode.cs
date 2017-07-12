using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	namespace Utils
	{
		class TreeNode : IEnumerable<TreeNode>
		{
			private Dictionary<string, TreeNode> m_children = new Dictionary<string, TreeNode> ();

			private string m_ID;
			public TreeNode Parent { get; private set; }

			public TreeNode(string id)
			{
				m_ID = id;
			}

			public TreeNode GetChild(string id)
			{
				return m_children[id];
			}

			public string GetID()
			{
				return m_ID;
			}

			public bool ContainsChild(string id)
			{
				return m_children.ContainsKey(id);
			}

			public bool HasAnyChildren()
			{
				return m_children.Count > 0;
			}

			public void Add(TreeNode item)
			{
				if (item.Parent != null)
				{
					item.Parent.m_children.Remove(item.m_ID);
				}

				item.Parent = this;
				m_children.Add(item.m_ID, item);
			}

			public IEnumerator<TreeNode> GetEnumerator()
			{
				return m_children.Values.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Count
			{
				get { return m_children.Count; }
			}
		}
	}
}