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
using NUnit.Framework;

namespace MFramework.Infrastructure.ServiceLocator.Test
{
    [TestFixture]
    public abstract partial class ServiceLocatorTests
    {
        [Test, Ignore]
        public void ShouldResolvePartiallyClosedGenerics()
        {
            var genericType = typeof (IHandler<>).MakeGenericType(typeof (GenericType<>));
            locator
                .Register(Given.OpenType(genericType)
                            .Then(typeof (PartiallyClosedGenericHandler<>)));

            var instance = locator.GetInstance<IHandler<GenericType<string>>>();

            Assert.IsInstanceOf<PartiallyClosedGenericHandler<string>>(instance);
        }
    }

    public class PartiallyClosedGenericHandler<T> : IHandler<GenericType<T>>
    {
        
    }

    public interface IHandler<T> {}
    public class GenericType<T> {}
}