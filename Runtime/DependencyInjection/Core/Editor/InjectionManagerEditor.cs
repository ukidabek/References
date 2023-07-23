using UnityEditor;
using UnityEngine;

namespace Utilities.ReferenceHost
{
    [CustomEditor(typeof(InjectionManager))]
	public class InjectionManagerEditor : Editor
    {
		private InjectionManager m_manager;

		private void OnEnable()
		{
			m_manager = target as InjectionManager;	
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Generate Injection Dictionary"))
				m_manager.GenerateInjectionDictionary();
		}
	}
}