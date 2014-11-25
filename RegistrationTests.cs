using System;
using System.Collections.Generic;
using MFramework.Infrastructure.ServiceLocator.InternalStorage;
using MFramework.Infrastructure.ServiceLocator.Registrations;
using MFramework.Infrastructure.ServiceLocator.Registrations.Conventions;
using MFramework.Infrastructure.ServiceLocator.Registrations.Meta;
using MFramework.Infrastructure.ServiceLocator.Registrations.Named;
using MFramework.Infrastructure.ServiceLocator.Registrations.Stores;
using MFramework.Infrastructure.ServiceLocator.RegistrationSyntax;
using MFramework.Infrastructure.ServiceLocator.RegistrationTemplates;
using MFramework.Infrastructure.ServiceLocator.Resolution.Pipeline;
using MFramework.Infrastructure.ServiceLocator.Test.AOP;
using NUnit.Framework;

namespace MFramework.Infrastructure.ServiceLocator.Test
{
    [TestFixture]
    public abstract partial class ServiceLocatorTests
    {
        [Test]
        public virtual void ShouldProxyAllTypes()
        {
            locator.Register(Using.Convention<SampleProxyAttributeConvention>());
            locator.Register(Using.Convention<ProxyConvention>());
            locator.Register(Given<TestType2>.Then<TestType2>());

            var testType2 = locator.GetInstance<TestType2>();
            Assert.AreEqual("lolarg1", testType2.Test("arg1", "arg2"));
            Assert.AreEqual(3, Counter.Count);

            Counter.Count = 0;
        }
    }

    public class SampleProxyAttributeConvention : IConvention
    {
        public Action<IServiceLocator> Build()
        {
            return serviceLocator => serviceLocator.Register(new List<IRegistration>
                       {
                           Given<SampleEncapsulatingAttribute>.Then<SampleEncapsulatingAttribute>(),
                           Given<SamplePreProcessingAttribute>.Then<SamplePreProcessingAttribute>(),
                           Given<SamplePostProcessingAttribute>.Then<SamplePostProcessingAttribute>()
                       });
        }
    }

    public class ProxyConvention : IConvention
    {
        public Action<IServiceLocator> Build()
        {
            return serviceLocator => serviceLocator.Register(new List<IRegistration>
                       {
                           Given<Common.Proxy.SiegeProxy>.Then<Common.Proxy.SiegeProxy>(),
                           Given<IMetaRegistration>.Then<ProxyRegistration>()
                       });
        }
    }

    public class ProxyRegistration : IMetaRegistration
    {
        private readonly Common.Proxy.SiegeProxy proxy;
        private IRegistration registration;

        public ProxyRegistration(Common.Proxy.SiegeProxy proxy)
        {
            this.proxy = proxy;
        }

        public void MapsTo(object implementationType)
        {
            this.registration.MapsTo(implementationType);
        }

        public object GetMappedTo()
        {
            return registration.GetMappedTo();
        }

        public Type GetMappedToType()
        {
            return (Type)registration.GetMappedTo();
        }

        public IRegistrationStore GetRegistrationStore()
        {
            return registration.GetRegistrationStore();
        }

        public IRegistrationTemplate GetRegistrationTemplate()
        {
            return registration.GetRegistrationTemplate();
        }

        public Type GetMappedFromType()
        {
            return registration.GetMappedFromType();
        }

        public object ResolveWith(IInstanceResolver resolver, IServiceLocatorStore context, PostResolutionPipeline pipeline)
        {
            return registration.ResolveWith(resolver, context, pipeline);
        }

        public bool IsValid(IInstanceResolver locator, IServiceLocatorStore context)
        {
            return registration.IsValid(locator, context);
        }

        public void ChainTo(IRegistration registration)
        {
            this.registration = registration;
            this.registration.MapsTo(proxy.WithServiceLocator().Create(registration.GetMappedToType()));
        }

        public bool IsValid(IRegistration registration)
        {
            return true;
        }

        public string Key
        {
            get
            {
                if(registration is INamedRegistration) return ((INamedRegistration) registration).Key;

                return null;
            }
        }

        public bool Equals(IRegistration other)
        {
            return this.registration.Equals(other);
        }
    }
}