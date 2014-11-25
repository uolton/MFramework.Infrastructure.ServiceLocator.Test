/*   Copyright 2009 - 2010 Marcus Bratton

     Licensed under the Apache License, Version 2.0 (the "License");
     you may not use this file except in compliance with the License.
     You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

     Unless required by applicable law or agreed to in writing, software
     distributed under the License is distributed on an "AS IS" BASIS,
     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     See the License for the specific language governing permissions and
     limitations under the License.
*/

using MFramework.Infrastructure.ServiceLocator.InternalStorage;
using MFramework.Infrastructure.ServiceLocator.RegistrationSyntax;
using MFramework.Infrastructure.ServiceLocator.Test.ContextualTests.Classes;
using NUnit.Framework;

namespace MFramework.Infrastructure.ServiceLocator.Test.ContextualTests
{
	[TestFixture]
	public abstract class BaseContextTests
	{
        protected IServiceLocator locator;
		protected abstract IServiceLocatorAdapter GetAdapter();

		[SetUp]
		public virtual void SetUp()
		{
			locator = new ThreadedServiceLocator(GetAdapter());
		}

		[TearDown]
		public void TearDown()
		{
			locator.Dispose();
		}

		[Test]
		public void ShouldChooseTestService2()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<ITestRepository>.Then<TestRepository1>())
				.Register(Given<IBaseService>
                        .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                        .Then<TestService1>())
				.Register(Given<IBaseService>
                        .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                        .Then<TestService2>());

			locator.AddContext(new TestCondition(TestTypes.Test2));
			var controller = locator.GetInstance<ITestController>();

            Assert.IsInstanceOf<TestService2>(controller.Service);
		}

		[Test]
		public void ShouldChooseTestService1()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<ITestRepository>.Then<TestRepository1>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                            .Then<TestService2>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                            .Then<TestService1>());
			
            locator.AddContext(new TestCondition(TestTypes.Test1));
			var controller = locator.GetInstance<ITestController>();

            Assert.IsInstanceOf<TestService1>(controller.Service);
            Assert.IsInstanceOf<TestRepository1>(controller.Service.Repository);
		}

		[Test]
		public void ShouldChooseService1AndRepository1()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                            .Then<TestService1>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                            .Then<TestService2>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionA)
                            .Then<TestRepository1>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionB)
                            .Then<TestRepository2>());

			locator.AddContext(new TestCondition(TestTypes.Test1));
			locator.AddContext(new RepositoryCondition(Conditions.ConditionA));

			var controller = locator.GetInstance<ITestController>();

            Assert.IsInstanceOf<TestService1>(controller.Service);
            Assert.IsInstanceOf<TestRepository1>(controller.Service.Repository);
		}

		[Test]
		public void ShouldChooseService2AndRepository2()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<IBaseService>
                        .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                        .Then<TestService1>())
				.Register(Given<IBaseService>
                        .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                        .Then<TestService2>())
				.Register(Given<ITestRepository>
                        .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionA)
                        .Then<TestRepository1>())
				.Register(Given<ITestRepository>
                        .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionB)
                        .Then<TestRepository2>());

			locator.AddContext(new TestCondition(TestTypes.Test2));
			locator.AddContext(new RepositoryCondition(Conditions.ConditionB));

			var controller = locator.GetInstance<ITestController>();

            Assert.IsInstanceOf<TestService2>(controller.Service);
            Assert.IsInstanceOf<TestRepository2>(controller.Service.Repository);
		}

		[Test]
		public void ShouldChooseDefaultTestServiceAndRepository2()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                            .Then<TestService1>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                            .Then<TestService2>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionA)
                            .Then<TestRepository1>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionB)
                            .Then<TestRepository2>())
				.Register(Given<IBaseService>.Then<DefaultTestService>());

			locator.AddContext(new TestCondition(TestTypes.Test3));
			locator.AddContext(new RepositoryCondition(Conditions.ConditionB));

			var controller = locator.GetInstance<ITestController>();

            Assert.IsInstanceOf<DefaultTestService>(controller.Service);
            Assert.IsInstanceOf<TestRepository2>(controller.Service.Repository);
		}

		[Test]
		public void ShouldChooseDefaultsWhenNoContextApplies()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<IBaseService>.Then<DefaultTestService>())
				.Register(Given<ITestRepository>.Then<DefaultTestRepository>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                            .Then<TestService1>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                            .Then<TestService2>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionA)
                            .Then<TestRepository1>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionB)
                            .Then<TestRepository2>());

			locator.AddContext(new TestCondition(TestTypes.Test3));
			locator.AddContext(new RepositoryCondition(Conditions.ConditionC));

			var controller = locator.GetInstance<ITestController>();

            Assert.IsInstanceOf<DefaultTestService>(controller.Service);
            Assert.IsInstanceOf<DefaultTestRepository>(controller.Service.Repository);
		}

		[Test]
		public void ShouldChangeSelectionAsContextIsApplied()
		{
            Assert.AreEqual(0, locator.Store.Get<IExecutionStore>().RequestedTypes.Count);

			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<IBaseService>.Then<DefaultTestService>())
				.Register(Given<ITestRepository>.Then<DefaultTestRepository>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                            .Then<TestService1>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                            .Then<TestService2>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionA)
                            .Then<TestRepository1>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionB)
                            .Then<TestRepository2>());

			var controller = locator.GetInstance<ITestController>();
			
            Assert.IsInstanceOf<DefaultTestService>(controller.Service);
            Assert.IsInstanceOf<DefaultTestRepository>(controller.Service.Repository);

			locator.AddContext(new TestCondition(TestTypes.Test1));

			controller = locator.GetInstance<ITestController>();
            Assert.IsInstanceOf<TestService1>(controller.Service);
            Assert.IsInstanceOf<DefaultTestRepository>(controller.Service.Repository);

			locator.AddContext(new RepositoryCondition(Conditions.ConditionB));

			controller = locator.GetInstance<ITestController>();
            Assert.IsInstanceOf<TestService1>(controller.Service);
            Assert.IsInstanceOf<TestRepository2>(controller.Service.Repository);
		}

		[Test]
		public void ShouldChooseDefaultsWhenNoContextProvided()
		{
			locator
                .Register(Given<ITestController>.Then<TestController>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test1)
                            .Then<TestService1>())
				.Register(Given<IBaseService>
                            .When<ITestCondition>(context => context.TestType == TestTypes.Test2)
                            .Then<TestService2>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionA)
                            .Then<TestRepository1>())
				.Register(Given<ITestRepository>
                            .When<IRepositoryCondition>(context => context.Condition == Conditions.ConditionB)
                            .Then<TestRepository2>())
				.Register(Given<IBaseService>.Then<DefaultTestService>())
				.Register(Given<ITestRepository>.Then<DefaultTestRepository>());


			var controller = locator.GetInstance<ITestController>();
            Assert.IsInstanceOf<DefaultTestService>(controller.Service);
            Assert.IsInstanceOf<DefaultTestRepository>(controller.Service.Repository);
		}
	}
}