using UnityEngine;

namespace Utilities.ReferenceHost
{
    public abstract class CastReferenceHostSetter<ReferenceHostType, Type, ObjectType> : ReferenceHostSetter<ReferenceHostType, Type>
        where ReferenceHostType : ReferenceHost<Type>
        where Type : class
    {
        [SerializeField] private ObjectType m_reference = default;
        protected override Type Reference => m_reference as Type;
    }
}