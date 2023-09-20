using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utilities.ReferenceHost
{
	public static class InjectionHelpers
	{
		private const BindingFlags Binding_Flags = BindingFlags.NonPublic | BindingFlags.Instance;

		public static void GatherInjectionPoints(this IList<InjectionPoint> injectionPoints, GameObject gameObject)
		{
			injectionPoints.Clear();
			var components = gameObject.GetComponentsInChildren<Component>();

			foreach (var component in components)
			{
				var componentType = component.GetType();
				var fields = componentType
					.GetFields(Binding_Flags)
					.Where(fieldInfo => fieldInfo.GetCustomAttribute<InjectAttribute>() != null);

				foreach (var field in fields)
					injectionPoints.Add(new InjectionPoint(component, field));
			}
		}

		public static void ClearInjectionPoints(this IEnumerable<InjectionPoint> injectionPoints)
		{
			foreach (var point in injectionPoints)
				point.Clear();
		}
	}
}