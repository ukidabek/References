using System.Collections.Generic;
using UnityEngine;

namespace Utilities.ReferenceHost
{
	[AddComponentMenu("Dependency Injection/InjectionPointCollection")]
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
	}
}