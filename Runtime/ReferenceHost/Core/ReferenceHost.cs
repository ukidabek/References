using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	public abstract class ReferenceHost<T> : ScriptableObject where T : Object
	{
		public T Instance {get; private set;}

        public event Action OnReferenceChanged = null;

		internal void SetReference(T reference)
		{
			if(Instance == reference) return;
			Instance = reference;
			OnReferenceChanged?.Invoke();
		}

		private void OnDisable()
		{
			Instance = default;
		}
	}
}