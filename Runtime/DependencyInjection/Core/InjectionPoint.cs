using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	[Serializable]
	public class InjectionPoint : ISerializationCallbackReceiver
	{
		[SerializeField] private Object m_injectDestination = null;
		[SerializeField] private string m_fieldName = string.Empty;
		[SerializeField] private string m_injectionID = string.Empty;
		public string InjectionID => m_injectionID;
		private string m_id = string.Empty;
		public string ID => m_id;
		public string FieldTypeName => m_fieldInfo.FieldType.Name;

		protected IDynamicInjector m_injector = null;
		private FieldInfo m_fieldInfo = null;
		private const BindingFlags Binding_Flags = BindingFlags.NonPublic | BindingFlags.Instance;

		public InjectionPoint(Object injectDestination, FieldInfo injectionPoint)
		{
			m_injectDestination = injectDestination;
			m_fieldName = injectionPoint.Name;
			var injectAttribute = injectionPoint.GetCustomAttribute<InjectAttribute>();
			m_injectionID = injectAttribute.ID;
		}

		public void Initialize()
		{
			if (m_injectDestination == null)
				return;

			m_fieldInfo = m_injectDestination
				.GetType()
				.GetField(m_fieldName, Binding_Flags);
			m_id = $"{m_injectionID}_{FieldTypeName}";
		}

		public void Inject(IDictionary<string, Object> dictionary)
		{
			if(m_injector != null)
			{
				m_injector.Inject -= Inject;
				m_injector = null;
			}

			if (dictionary.TryGetValue(m_id, out var instance))
			{
				if (instance is IDynamicInjector injector)
				{
					m_injector = injector;
					m_injector.Inject += Inject;
					Inject(m_injector.ObjectToInject);
				}
				else
					Inject(instance);
			}
		}

		private void Inject(Object instance) => m_fieldInfo.SetValue(m_injectDestination, instance);

		public void Clear() => m_fieldInfo.SetValue(m_injectDestination, null);

		public void OnBeforeSerialize() { }

		public void OnAfterDeserialize() => Initialize();

		public static implicit operator InjectDefinition(InjectionPoint point) => new InjectDefinition(point.ID, point.FieldTypeName, true);
	}
}
