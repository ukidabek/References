using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.ReferenceHost
{
	[CustomEditor(typeof(InjectionManager))]
	public class InjectionManagerEditor : Editor
	{
		private InjectionManager m_manager;
		private FieldInfo m_injectionDefinitionDictionaryFieldInfo = null;

		private void OnEnable()
		{
			m_manager = target as InjectionManager;
			var type = m_manager.GetType();
			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			m_injectionDefinitionDictionaryFieldInfo = type.GetField("m_injectionDefinitionDictionary", bindingFlags);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Generate Injection Dictionary"))
				GenerateInjectionDictionary(m_manager.transform);
		}

		public void GenerateInjectionDictionary(Transform transform)
		{
			var rootGameObject = transform.root.gameObject;
			var injectCollections = rootGameObject.GetComponentsInChildren<InjectionPointCollection>();
			var dictionary = m_injectionDefinitionDictionaryFieldInfo.GetValue(target) as InjectionDictionary;
			var baseInjectionDefinitionsList = dictionary
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

			var injectDefinitions = dictionary.InjectDefinitions;
			injectDefinitions.Clear();
			injectDefinitions.AddRange(baseInjectionDefinitionsList);

			m_injectionDefinitionDictionaryFieldInfo.SetValue(target, dictionary);
		}
	}
}