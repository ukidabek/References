using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	public abstract class ReferenceHostInjector<ReferenceHostType, Type, InjectionType> : MonoBehaviour
		where ReferenceHostType : ReferenceHost<InjectionType>
		where Type : Object
	{
        [SerializeField] private Object[] m_injectionObjects = null;
		[SerializeField] private ReferenceHostType m_reference = null;
		
        private List<KeyValuePair<Object, MemberInfo>> m_fieldInfo = new List<KeyValuePair<Object, MemberInfo>>();
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		private void Awake()
		{
			var type = typeof(InjectionType);
			foreach (var injectObject in m_injectionObjects)
            {
                var objectType = injectObject.GetType();
				var fields = new List<MemberInfo>()
					.Concat(objectType.GetFields(bindingFlags))
					.Concat(objectType.GetMembers(bindingFlags));
				var fieldInfo = fields
                    .Where(field => ValidateType(field,type))
                    .FirstOrDefault(field => field.GetCustomAttribute<InjectAttribute>() != null);

				if (fieldInfo == null) continue;

				m_fieldInfo.Add(new KeyValuePair<Object, MemberInfo>(injectObject, fieldInfo));
            }

			m_reference.OnReferenceChanged += OnReferenceChanged;
			if (m_reference.Instance == null) return;
			OnReferenceChanged();
		}

		private bool ValidateType(MemberInfo memberInfo, System.Type type)
		{
			switch (memberInfo)
			{
				case PropertyInfo propertyInfo:
					return propertyInfo.PropertyType == type;
				case FieldInfo fieldInfo:
					return fieldInfo.FieldType == type;
				default: return false;
			}
		}

		private void OnReferenceChanged()
		{
			var instance = m_reference.Instance;
			foreach (var keyValuePair in m_fieldInfo)
			{
				var memberInfo = keyValuePair.Value;
				var injectionObject = keyValuePair.Key;
				switch (memberInfo)
				{
					case PropertyInfo propertyInfo:
						propertyInfo.SetValue(injectionObject, instance);
						break;
					case FieldInfo fieldInfo:
						fieldInfo.SetValue(injectionObject, instance);
						break;
				}
				if (injectionObject is IInitializable initializable)
					initializable.Initialize();
			}
		}

		private void OnDisable() => m_reference.OnReferenceChanged -= OnReferenceChanged;
	}

	public abstract class ReferenceHostInjector<ReferenceHostType, Type> : ReferenceHostInjector<ReferenceHostType, Type, Type>
		where ReferenceHostType : ReferenceHost<Type>
		where Type : Object
	{
	}
}