using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.ReferenceHost
{
	[AddComponentMenu("Dependency Injection/InjectionManager")]
	public class InjectionManager : MonoBehaviour
	{
		[SerializeField] private InjectionDictionary m_injectionDefinitionDictionary = new InjectionDictionary();

		private List<InjectionPointCollection> m_staticInjectionPointCollections = new List<InjectionPointCollection>();

		public void Inject(InjectionPointCollection injectionPointCollection)
		{
			if(injectionPointCollection.IsStatic)
			{
				if (m_staticInjectionPointCollections.Contains(injectionPointCollection))
					return;
				m_staticInjectionPointCollections.Add(injectionPointCollection);
			}
			injectionPointCollection.Inject(m_injectionDefinitionDictionary);
		}
	}
}