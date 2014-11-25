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

using System.Linq;
using MFramework.Infrastructure.ServiceLocator.Exceptions;
using MFramework.Infrastructure.ServiceLocator.RegistrationSyntax;
using MFramework.Infrastructure.ServiceLocator.Test.TestClasses;
using NUnit.Framework;

namespace MFramework.Infrastructure.ServiceLocator.Test
{
    [TestFixture]
    public abstract partial class ServiceLocatorTests
    {
        [Test]
        public void ShouldBeAbleToBindAnInterfaceToATypeBasedOnRule()
        {
            locator
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>());
            locator.AddContext(CreateContext(TestEnum.Case2));

            Assert.IsInstanceOf<TestCase2>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldBeAbleToBindAnInterfaceToAnImplementationBasedOnRule()
        {
            locator
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then(new TestCase2()));
            locator.AddContext(CreateContext(TestEnum.Case2));

            Assert.IsInstanceOf<TestCase2>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldUseRuleWhenSatisfied()
        {
            locator
                .Register(Given<ITestInterface>.Then<TestCase1>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>());
            locator.AddContext(CreateContext(TestEnum.Case2));

            Assert.IsInstanceOf<TestCase2>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldUseCorrectRuleGivenMultipleRules()
        {
            locator
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case1)
                              .Then<TestCase1>());
            locator.AddContext(CreateContext(TestEnum.Case1));

            Assert.IsInstanceOf<TestCase1>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldChangeImplementationWhenContextIsAdded()
        {
            locator
                .Register(Given<ITestInterface>.Then<TestCase1>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case1)
                              .Then<TestCase1>());

            Assert.IsInstanceOf<TestCase1>(locator.GetInstance<ITestInterface>());

            locator.AddContext(CreateContext(TestEnum.Case2));

            Assert.IsInstanceOf<TestCase2>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldUseCorrectRuleGivenMultipleRulesAndDefault()
        {
            locator.Register(Given<ITestInterface>.Then<TestCase1>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case1)
                              .Then<TestCase1>());
            locator.AddContext(CreateContext(TestEnum.Case1));

            Assert.IsInstanceOf<TestCase1>(locator.GetInstance<ITestInterface>());
        }

        [Test, ExpectedException(typeof (RegistrationNotFoundException))]
        public void ShouldThrowExceptionWhenTypeNoDefaultSpecifiedAndNoRulesMatch()
        {
            locator.Register(Given<ITestInterface>
                                 .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                                 .Then<TestCase2>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case1)
                              .Then<TestCase1>());

            Assert.IsInstanceOf<TestCase1>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldNotUseRuleWhenNotSatisfied()
        {
            locator.Register(Given<ITestInterface>.Then<TestCase1>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>());
            locator.AddContext(CreateContext(TestEnum.Case3));

            Assert.IsInstanceOf<TestCase1>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldResolveAllFromServiceLocatorRegardlessOfContext()
        {
            locator.Register(Given<ITestInterface>.Then<TestCase1>())
                   .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>());

            var instances = locator.GetAllInstances<ITestInterface>();

            foreach (ITestInterface item in instances)
            {
                Assert.IsInstanceOf<ITestInterface>(item);
            }

            Assert.AreEqual(2, instances.ToList().Count);

            Assert.AreEqual(1, instances.Where(i => i is TestCase1).Count());
            Assert.AreEqual(1, instances.Where(i => i is TestCase2).Count());
        }

        [Test]
        public void ShouldResolveAllFromServiceLocatorRegardlessOfContextNonGeneric()
        {
            locator.Register(Given<ITestInterface>.Then<TestCase1>())
                .Register(Given<ITestInterface>
                              .When<TestClasses.TestContext>(context => context.TestCases == TestEnum.Case2)
                              .Then<TestCase2>());

            var instances = locator.GetAllInstances<ITestInterface>();

            foreach (ITestInterface item in instances)
            {
                Assert.IsInstanceOf<ITestInterface>(item);
            }
        }

        [Test]
        public void ShouldResolveUsingICondition()
        {
            locator
                .Register(Given<TestCase4>.Then<TestCase4>())
                .Register(Given<IConstructorArgument>.Then<ConstructorArgument>())
                .Register(Given<ITestInterface>
                              .When<TestEvaluation>()
                              .Then<TestCase2>());
            locator.AddContext(CreateContext(TestEnum.Case2));

            Assert.IsInstanceOf<TestCase2>(locator.GetInstance<ITestInterface>());
        }
    }
}