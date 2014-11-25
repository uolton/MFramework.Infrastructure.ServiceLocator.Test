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

using MFramework.Infrastructure.ServiceLocator.RegistrationSyntax;
using MFramework.Infrastructure.ServiceLocator.Test.TestClasses;
using NUnit.Framework;

namespace MFramework.Infrastructure.ServiceLocator.Test
{
    [TestFixture]
    public abstract partial class ServiceLocatorTests
    {
        [Test]
        public void ShouldBeAbleToBindAnInterfaceToATypeBasedOnMultipleConditionalRules()
        {
            locator
                .Register(Given<ITestInterface>
                            .When(context =>
                                      {
                                          context.When<TestEnum>(testenum => testenum == TestEnum.Case2);
                                          context.When<TestEnum>(testenum => testenum == TestEnum.Case1);
                                      })
                            .Then<TestCase2>());
            locator.AddContext(TestEnum.Case2);
            locator.AddContext(TestEnum.Case1);

            Assert.IsInstanceOf<TestCase2>(locator.GetInstance<ITestInterface>());
        }

        [Test]
        public void ShouldSelectDefaultWhenSomeContextDoesNotApply()
        {
            locator
                .Register(Given<ITestInterface>
                            .When(context =>
                            {
                                context.When<TestEnum>(testenum => testenum == TestEnum.Case2);
                                context.When<TestEnum>(testenum => testenum == TestEnum.Case1);
                            })
                            .Then<TestCase2>())
                .Register(Given<ITestInterface>.Then<TestCase1>());

            locator.AddContext(TestEnum.Case2);

            Assert.IsInstanceOf<TestCase1>(locator.GetInstance<ITestInterface>());
        }
    }
}