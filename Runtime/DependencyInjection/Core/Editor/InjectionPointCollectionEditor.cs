using UnityEditor;
using UnityEngine;

namespace Utilities.ReferenceHost
{
	[CustomEditor(typeof(InjectionPointCollection))]
	public class InjectionPointCollectionEditor : Editor
	{
		private InjectionPointCollection m_injectionPointCollection = null;

		private void OnEnable()
		{
			m_injectionPointCollection = target as InjectionPointCollection;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUI.BeginChangeCheck();
			if (GUILayout.Button("Get Injection Points"))
			{
				var gameObject = m_injectionPointCollection.gameObject;
				m_injectionPointCollection.InjectionPoints.GatherInjectionPoints(gameObject);
			}
			if (GUILayout.Button("Clear references"))
			{
				m_injectionPointCollection.InjectionPoints.ClearInjectionPoints();
			}

			if (EditorGUI.EndChangeCheck())
			{
				var gameObject = m_injectionPointCollection.gameObject;
				EditorUtility.SetDirty(gameObject);
			}
		}
	}
}