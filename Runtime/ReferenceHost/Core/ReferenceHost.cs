using System;
using UnityEngine;

namespace Utilities.ReferenceHost
{
    public abstract class ReferenceHost<T> : ScriptableObject
	{
		[field: SerializeField] public T Instance { get; private set; }

		public event Action OnReferenceChanged = null;

		internal void SetReference(T reference)
		{
			if(ReferenceEquals(Instance, reference)) return;
			Instance = reference;
			OnReferenceChanged?.Invoke();
		}

		private void OnDisable()
		{
			Instance = default;
		}
	}
}