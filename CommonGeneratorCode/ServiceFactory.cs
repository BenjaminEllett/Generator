//
// MIT License
//
// Copyright(c) 2019-2024 Benjamin Ellett
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CommonGeneratorCode
{
    public class ServiceFactory
    {
        private Dictionary<Type, ServiceMetadata> registeredServiceDictionary;

        public ServiceFactory()
        {
            this.registeredServiceDictionary = new Dictionary<Type, ServiceMetadata>();
        }

        public void RegisterSingletonService<TServiceInterfaceType, TServiceType>()
            where TServiceInterfaceType : class
            where TServiceType : class, TServiceInterfaceType
        {
            Type serviceInterfaceType = typeof(TServiceInterfaceType);
            Type serviceType = typeof(TServiceType);

            if (!serviceInterfaceType.IsInterface)
            {
                throw new ArgumentException(
                    message: $"{nameof(TServiceInterfaceType)} must contain an interface type.  Here is the type it contained: {serviceInterfaceType.FullName}",
                    paramName: nameof(TServiceInterfaceType));
            }

            if (!serviceType.IsClass)
            {
                throw new ArgumentException(
                    message: $"{nameof(TServiceType)} must contain a class.  Here is the type it contained: {serviceType.FullName}",
                    paramName: nameof(TServiceType));
            }

            ConstructorInfo[] constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    "Services can only have one public constructor.",
                    paramName: nameof(TServiceType));
            }

            if (ServiceRegistered(serviceInterfaceType))
            {
                throw new ArgumentException(
                    $"Cannot register a service because a service of type {serviceInterfaceType.FullName} has already been registered.",
                    paramName: nameof(TServiceInterfaceType));
            }

            this.registeredServiceDictionary[serviceInterfaceType] = new ServiceMetadata(typeof(TServiceType), isSingleton: true);
        }

        public TServiceInterfaceType GetService<TServiceInterfaceType>()
            where TServiceInterfaceType : class
        {
            return (TServiceInterfaceType)this.GetService(typeof(TServiceInterfaceType));
        }

        private object GetService(Type serviceInterfaceType)
        {
            if (!ServiceRegistered(serviceInterfaceType))
            {
                throw new ArgumentException($"Cannot get the service because a service of type {serviceInterfaceType.FullName} has not been registered.");
            }

            ServiceMetadata serviceMetadata = this.registeredServiceDictionary[serviceInterfaceType];

            if (serviceMetadata.IsSingletonService && (serviceMetadata.SingletonServiceInstance != null))
            {
                return serviceMetadata.SingletonServiceInstance;
            }

            ConstructorInfo[] constructors = serviceMetadata.ServiceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            Debug.Assert(
                constructors.Length == 1,
                "Services are only allowed one public constructor because there is no easy way to determine which constructor to use if a " +
                "service has multiple constructors.  This requirement is checked in RegisterSingletonService().");

            ConstructorInfo serviceConstructor = constructors[0];
            ParameterInfo[] serviceConstructorParametersMetadata = serviceConstructor.GetParameters();
            object[] constructorParameters = new object[serviceConstructorParametersMetadata.Length];

            // Get the constructor's parameter values
            for (int parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
            {
                ParameterInfo currentParameterMetadata = serviceConstructorParametersMetadata[parameterIndex];

                // Services are only allowed to have parameters which are other services.
                constructorParameters[parameterIndex] = this.GetService(currentParameterMetadata.ParameterType);
            }

            object newServiceInstance = serviceConstructor.Invoke(constructorParameters);

            if (serviceMetadata.IsSingletonService)
            {
                serviceMetadata.SingletonServiceInstance = newServiceInstance;
            }

            return newServiceInstance;
        }

        private bool ServiceRegistered(Type serviceInterfaceType) => this.registeredServiceDictionary.ContainsKey(serviceInterfaceType);
    }
}
