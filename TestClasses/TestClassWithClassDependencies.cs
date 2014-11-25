namespace MFramework.Infrastructure.ServiceLocator.Test.TestClasses
{
    public class TestClassWithClassDependencies
    {
        public TestClassWithInterfaceDependencies TestClassDependices { get; set; }

        public TestClassWithClassDependencies(TestClassWithInterfaceDependencies testClassDependices)
        {
            TestClassDependices = testClassDependices;
        }
    }
}
