using System;
using Object = UnityEngine.Object;

namespace Utilities.ReferenceHost
{
	public interface IDynamicInjector
	{
		Type Type { get; }
		Object ObjectToInject { get; }
		event Action<Object> Inject;
	}
}