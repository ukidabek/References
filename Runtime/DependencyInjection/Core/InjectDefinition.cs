using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	[Serializable]
	public class InjectDefinition
	{
		[SerializeField] private string m_referenceID = string.Empty;
		public string ReferenceID => m_referenceID;
		public string ID
		{
			get
			{
				if (m_objectToInject is IDynamicInjector injector)
					return $"{m_referenceID}_{injector.Type.Name}";
				else
					return $"{m_referenceID}_{m_objectToInject.GetType().Name}";
			}
		}
		
		[SerializeField] private string m_typeName = string.Empty;

		[SerializeField] private Object m_objectToInject = null;
		public Object ObjectToInject => m_objectToInject;

		[SerializeField] private bool m_lock = false;
		public bool Lock => m_lock;

		public InjectDefinition() { }

		public InjectDefinition(string id, Object objectToInject, bool manual)
		{
			m_referenceID = id.Split('_').First();
			m_objectToInject = objectToInject;
			m_typeName = objectToInject.GetType().Name;
			m_lock = manual;
		}

		internal InjectDefinition(string id, string typeName, bool manual)
		{
			m_referenceID = id.Split('_').First();
			m_typeName = typeName;
			m_lock = manual;
		}

		public bool IsEqual(InjectionPoint right) => ReferenceID == right.InjectionID && m_typeName == right.FieldTypeName;
	}
}