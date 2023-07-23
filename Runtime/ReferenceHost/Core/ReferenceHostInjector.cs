using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	public abstract class ReferenceHostInjector<ReferenceHostType, Type, InjectionType> : MonoBehaviour
		where ReferenceHostType : ReferenceHost<Type>
		where Type : Object
	{
        [SerializeField] private Object[] m_injectionObjects = null;
		[SerializeField] private ReferenceHostType m_reference = null;

        private List<KeyValuePair<Object, FieldInfo>> m_fieldInfo = new List<KeyValuePair<Object, FieldInfo>>();
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		[Obsolete("Use IInitializable interface to initialize object on injection")]
		public UnityEvent OnReferenceChangedEvent = new UnityEvent();

		private void Awake()
		{
			var type = typeof(InjectionType);
			foreach (var injectObject in m_injectionObjects)
            {
                var objectType = injectObject.GetType();
                var fields = objectType.GetFields(bindingFlags);
                var fieldInfo = fields
                    .Where(field => field.FieldType == type)
                    .FirstOrDefault(field => field.GetCustomAttribute<InjectAttribute>() != null);

				if (fieldInfo == null) continue;

				m_fieldInfo.Add(new KeyValuePair<Object, FieldInfo>(injectObject, fieldInfo));
            }

			m_reference.OnReferenceChanged += OnReferenceChanged;
			if (m_reference.Instance == null) return;
			OnReferenceChanged();
		}

		private void OnReferenceChanged()
		{
			var instance = m_reference.Instance;
			foreach (var keyValuePair in m_fieldInfo)
			{
				var fieldInfo = keyValuePair.Value;
				var injectionObject = keyValuePair.Key;
				fieldInfo.SetValue(injectionObject, instance);
				if (injectionObject is IInitializable initializable)
					initializable.Initialize();
			}
			OnReferenceChangedEvent.Invoke();
		}

		private void OnDisable() => m_reference.OnReferenceChanged -= OnReferenceChanged;
	}

	public abstract class ReferenceHostInjector<ReferenceHostType, Type> : ReferenceHostInjector<ReferenceHostType, Type, Type>
		where ReferenceHostType : ReferenceHost<Type>
		where Type : Object
	{
	}
}