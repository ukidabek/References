using System.Collections.Generic;
using UnityEngine;

namespace Utilities.ReferenceHost
{
	public class InjectionPointCollection : MonoBehaviour
	{
		[SerializeField] private List<InjectionPoint> m_injectionPoints = new List<InjectionPoint>();
		public List<InjectionPoint> InjectionPoints => m_injectionPoints;

		[SerializeField] private bool m_isStatic = true;
		public bool IsStatic => m_isStatic;

		public void Inject(IDictionary<string, Object> m_injectionDefinitionDictionary)
		{
			foreach (InjectionPoint point in m_injectionPoints)
				point.Inject(m_injectionDefinitionDictionary);
		}

		[ContextMenu("Get Injection Points")]
		public void GetInjectionPoints() => m_injectionPoints.GatherInjectionPoints(gameObject);

		[ContextMenu("Clear references")]
		public void ClearReferences()
		{
			foreach (InjectionPoint point in m_injectionPoints)
				point.Clear();
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(transform.root.gameObject);
#endif
		}

		private void Reset() => GetInjectionPoints();
	}
}