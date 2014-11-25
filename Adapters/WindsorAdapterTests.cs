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

using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using MFramework.Infrastructure.ServiceLocator.Exceptions;
using MFramework.Infrastructure.ServiceLocator.RegistrationSyntax;
using MFramework.Infrastructure.ServiceLocator.Test.TestClasses;
using NUnit.Framework;

namespace MFramework.Infrastructure.ServiceLocator.Test.Adapters
{
    [TestFixture]
    [Category("Windsor")]
    public class WindsorAdapterTests : ServiceLocatorTests
    {
        private IKernel kernel;

        protected override void ResolveWithoutSiege<T>()
        {
            kernel.Resolve<T>();
        }

        public override void SetUp()
        {
            kernel = new DefaultKernel();
            base.SetUp();
        }

        protected override IServiceLocatorAdapter GetAdapter()
        {
            return new WindsorAdapter.WindsorAdapter(kernel);
        }

        protected override void RegisterWithoutSiege<TFrom, TTo>()
        {
            kernel.Register(Component.For<TFrom>().ImplementedBy<TTo>());
        }

        [Test, Ignore("Bug in Windsor lol")]
        public void ShouldDisposeFromContainers()
        {
            var disposableKernel = new DefaultKernel();
            using (var disposableLocater = new ThreadedServiceLocator(new WindsorAdapter.WindsorAdapter(disposableKernel)))
            {
                disposableLocater.Register(Given<ITestInterface>.Then<TestCase1>());
                Assert.IsTrue(disposableLocater.GetInstance<ITestInterface>() is TestCase1);
            }

            Assert.IsFalse(disposableKernel.HasComponent(typeof(ITestInterface)));
        }

        [ExpectedException(typeof(RegistrationNotFoundException))]
        public override void ShouldNotBeAbleToBindAnInterfaceToATypeWithANameWhenWrongNameProvided()
        {
            base.ShouldNotBeAbleToBindAnInterfaceToATypeWithANameWhenWrongNameProvided();
        }

        [ExpectedException(typeof(RegistrationNotFoundException))]
        public override void ShouldNotBeAbleToBindAnInterfaceToATypeWithANameWhenNoNameProvided()
        {
            base.ShouldNotBeAbleToBindAnInterfaceToATypeWithANameWhenNoNameProvided();
        }

        [Ignore("Castle Dynamic Proxy doesn't play nice!")]
        public override void ShouldProxyAllTypes()
        {
            base.ShouldProxyAllTypes();
        }
    }
}