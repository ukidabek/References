using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	public abstract class DynamicInjector : MonoBehaviour, IDynamicInjector
	{
		[SerializeField] private string m_transformPath = string.Empty;
		public abstract Type Type { get; }
		[SerializeField] private Object m_objectToInject = null;
		public Object ObjectToInject => m_objectToInject;

		public event Action<Object> Inject;

		protected virtual void OnTransformChildrenChanged() => GetReferenceAndInject();

		private void GetReferenceAndInject()
		{
			if (string.IsNullOrEmpty(m_transformPath))
			{
				m_objectToInject = GetComponentInChildren(Type);
			}
			else
			{
				var selectedTransform = transform.Find(m_transformPath);
				if (selectedTransform == null)
				{
					Debug.LogError($"There is no transform at path {m_transformPath}.");
					return;
				}
				var gameObject = selectedTransform.gameObject;
				m_objectToInject = gameObject.GetComponent(Type);
			}

			if (m_objectToInject == null) return;
			Inject?.Invoke(m_objectToInject);
		}

		protected virtual void Awake() => GetReferenceAndInject();
	}

	public abstract class DynamicInjector<ComponentType> : DynamicInjector where ComponentType : Component
	{
		public override Type Type => typeof(ComponentType);
	}
}