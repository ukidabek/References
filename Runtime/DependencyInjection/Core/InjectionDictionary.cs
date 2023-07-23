using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	[Serializable]
	public class InjectionDictionary : Dictionary<string, Object>, ISerializationCallbackReceiver
	{
		[SerializeField] private List<InjectDefinition> m_injectDefinitions = new List<InjectDefinition>();
		public List<InjectDefinition> InjectDefinitions => m_injectDefinitions;

		public void OnAfterDeserialize()
		{
			this.Clear();
			foreach (var item in m_injectDefinitions)
			{
				if (item.ObjectToInject == null) continue;
				Add(item.ID, item.ObjectToInject);
			}
		}

		public void OnBeforeSerialize()
		{
			//	List is modified form editor, no before serialization logic required.
		}
	}
}