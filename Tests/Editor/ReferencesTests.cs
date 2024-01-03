using NUnit.Framework;
using System.Reflection;
using UnityEngine;

namespace Utilities.ReferenceHost
{
    public interface ITest { }

    public class TestReference : MonoBehaviour, ITest { }

    public class TestReferenceHost : ReferenceHost<TestReference> { }

    public class TestInterfaceReferenceHost : ReferenceHost<ITest> { }

    public class TestReferenceHostSetter : SimpleReferenceHostSetter<TestReferenceHost, TestReference> { }

    public class TestInterfaceReferenceHostSetter : CastReferenceHostSetter<TestInterfaceReferenceHost, ITest, TestReference> { }

    public class TestReferenceHostInjector : ReferenceHostInjector<TestReferenceHost, TestReference, TestReference> { }

    public class TestInterfaceReferenceHostInjector : ReferenceHostInjector<TestInterfaceReferenceHost, TestReference, ITest> { }

    public class InjectionTarget : MonoBehaviour
    {
        [Inject] private TestReference m_testReference = null;
        public TestReference TestReference => m_testReference;

        [Inject] private ITest m_testInterfaceReference = null;
        public ITest TestInterfaceReference => m_testInterfaceReference;
    }

    public class ReferencesTests
    {
        private const BindingFlags m_bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        private const string m_hostFieldName = "m_host";
        private const string m_injectionObjectsFieldName = "m_injectionObjects";
        private const string m_referenceObjectsFieldName = "m_reference";
        private const string m_awakeMethodInfoName = "Awake";
        private const string m_onReferenceChangedMethodName = "OnReferenceChanged";

        private FieldInfo m_hostFieldInfo = null;
        private FieldInfo m_hostInterfaceFieldInfo = null;
        private FieldInfo m_hostInterfaceReferenceFieldInfo = null;

        private FieldInfo m_injectionObjectsFieldInfo = null;
        private FieldInfo m_referenceObjectsFieldInfo = null;
        private MethodInfo m_awakeMethodInfo = null;
        private MethodInfo m_onReferenceChangedMethodInfo = null;

        private FieldInfo m_injectionObjectsInterfaceFieldInfo = null;
        private FieldInfo m_referenceObjectsInterfaceFieldInfo = null;
        private MethodInfo m_awakeInterfaceMethodInfo = null;
        private MethodInfo m_onReferenceChangedInterfaceMethodInfo = null;

        private GameObject m_testGameObject = null;
        private TestReference m_testReference = null;
        private TestReferenceHost m_testReferenceHost = null;
        private TestInterfaceReferenceHost m_testInterfaceReferenceHost = null;

        public ReferencesTests()
        {
            var type = typeof(ReferenceHostSetter<TestReferenceHost, TestReference>);
            m_hostFieldInfo = type.GetField(m_hostFieldName, m_bindingFlags);
            type = typeof(ReferenceHostSetter<TestInterfaceReferenceHost, ITest>);
            m_hostInterfaceFieldInfo = type.GetField(m_hostFieldName, m_bindingFlags);
            type = typeof(CastReferenceHostSetter<TestInterfaceReferenceHost, ITest, TestReference>);
            m_hostInterfaceReferenceFieldInfo = type.GetField(m_referenceObjectsFieldName, m_bindingFlags);
            type = typeof(ReferenceHostInjector<TestReferenceHost, TestReference, TestReference>);
            m_injectionObjectsFieldInfo = type.GetField(m_injectionObjectsFieldName, m_bindingFlags);
            m_referenceObjectsFieldInfo = type.GetField(m_referenceObjectsFieldName, m_bindingFlags);
            m_awakeMethodInfo = type.GetMethod(m_awakeMethodInfoName, m_bindingFlags);
            m_onReferenceChangedMethodInfo = type.GetMethod(m_onReferenceChangedMethodName, m_bindingFlags);
            type = typeof(ReferenceHostInjector<TestInterfaceReferenceHost, TestReference, ITest>);
            m_injectionObjectsInterfaceFieldInfo = type.GetField(m_injectionObjectsFieldName, m_bindingFlags);
            m_referenceObjectsInterfaceFieldInfo = type.GetField(m_referenceObjectsFieldName, m_bindingFlags);
            m_awakeInterfaceMethodInfo = type.GetMethod(m_awakeMethodInfoName, m_bindingFlags);
            m_onReferenceChangedInterfaceMethodInfo = type.GetMethod(m_onReferenceChangedMethodName, m_bindingFlags);
        }

        [SetUp]
        public void SetUp()
        {
            m_testGameObject = new GameObject("TestObject");
            m_testReference = m_testGameObject.AddComponent<TestReference>();
            m_testReferenceHost = ScriptableObject.CreateInstance<TestReferenceHost>();
            m_testInterfaceReferenceHost = ScriptableObject.CreateInstance<TestInterfaceReferenceHost>();
        }

        [Test]
        public void Validate_If_TestReferenceHostSetter_Sets_Reference_Correctly()
        {
            var setter = m_testGameObject.AddComponent<TestReferenceHostSetter>();
            m_hostFieldInfo.SetValue(setter, m_testReferenceHost);

            var currentValue = m_hostFieldInfo.GetValue(setter);
            Assert.AreEqual(m_testReferenceHost, currentValue);
            setter.SetReference();
            Assert.AreEqual(m_testReference, m_testReferenceHost.Instance);
        }

        [Test]
        public void Validate_If_Reference_Change_Event_Is_Invoked()
        {
            var setter = m_testGameObject.AddComponent<TestReferenceHostSetter>();
            m_hostFieldInfo.SetValue(setter, m_testReferenceHost);

            var invoked = false;
            m_testReferenceHost.OnReferenceChanged += () => invoked = true;
            setter.SetReference();
            Assert.IsTrue(invoked);
        }

        [Test]
        public void Validate_If_Reference_Is_Injected_Correctly()
        {
            var setter = m_testGameObject.AddComponent<TestReferenceHostSetter>();
            m_hostFieldInfo.SetValue(setter, m_testReferenceHost);
            setter.SetReference();

            var injector = m_testGameObject.AddComponent<TestReferenceHostInjector>();
            var target = m_testGameObject.AddComponent<InjectionTarget>();
            m_injectionObjectsFieldInfo.SetValue(injector, new Object[] { target });
            m_referenceObjectsFieldInfo.SetValue(injector, m_testReferenceHost);
            m_awakeMethodInfo.Invoke(injector, null);
            m_onReferenceChangedMethodInfo.Invoke(injector, null);

            Assert.AreEqual(m_testReference, target.TestReference);
        }

        [Test]
        public void Validate_If_Reference_Is_Injected_Correctly_Using_Interface()
        {
            var setter = m_testGameObject.AddComponent<TestInterfaceReferenceHostSetter>();
            m_hostInterfaceFieldInfo.SetValue(setter, m_testInterfaceReferenceHost);
            m_hostInterfaceReferenceFieldInfo.SetValue(setter, m_testReference);
            setter.SetReference();

            var injector = m_testGameObject.AddComponent<TestInterfaceReferenceHostInjector>();
            var target = m_testGameObject.AddComponent<InjectionTarget>();
            m_injectionObjectsInterfaceFieldInfo.SetValue(injector, new Object[] { target });
            m_referenceObjectsInterfaceFieldInfo.SetValue(injector, m_testInterfaceReferenceHost);
            m_awakeInterfaceMethodInfo.Invoke(injector, null);
            m_onReferenceChangedInterfaceMethodInfo.Invoke(injector, null);

            Assert.AreEqual(m_testReference, target.TestInterfaceReference);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_testGameObject);
            Object.DestroyImmediate(m_testReferenceHost);
            Object.DestroyImmediate(m_testInterfaceReferenceHost);
        }
    }
}