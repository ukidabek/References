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

#if UNITY_EDITOR
		public void GenerateInjectionDictionary()
		{
			var rootGameObject = transform.root.gameObject;
			var injectCollections = rootGameObject.GetComponentsInChildren<InjectionPointCollection>();
			var baseInjectionDefinitionsList = m_injectionDefinitionDictionary
				.InjectDefinitions
				.Where(injector => injector.Lock)
				.ToList();

			foreach (var injectCollection in injectCollections)
			{
				var injectionPoints = injectCollection.InjectionPoints;
				foreach (var injectionPoint in injectionPoints)
				{
					var definition = baseInjectionDefinitionsList.FirstOrDefault(def => def.IsEqual(injectionPoint));
					if (definition != null) continue;
					baseInjectionDefinitionsList.Add(injectionPoint);
				}
			}

			var InjectDefinitions = m_injectionDefinitionDictionary.InjectDefinitions;
			InjectDefinitions.Clear();
			InjectDefinitions.AddRange(baseInjectionDefinitionsList);
		}
#endif
	}
}