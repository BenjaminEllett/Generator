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

using TestUtil;
using CommonGeneratorCode;
using CommonGeneratorCodeUnitTests.TestServiceFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommonGeneratorCodeUnitTests
{
    [TestClass]
    public class ServiceFactoryUnitTests
    {
        private ServiceFactory? serviceFactory;

        // This method runs before each test
        [TestInitialize]
        public void TestInitialize()
        {
            this.serviceFactory = new ServiceFactory();

            // The order of service registrations should not matter.
            serviceFactory.RegisterSingletonService<ITestServiceWith_1_Dependency, TestServiceWith_1_Dependency>();
            serviceFactory.RegisterSingletonService<ITestServiceWith_2_Dependencies, TestServiceWith_2_Dependencies>();
            serviceFactory.RegisterSingletonService<ITestServiceWithNoDepenedencies, TestServiceWithNoDepenedencies>();
        }

        // This method runs after each test
        [TestCleanup]
        public void TestCleanup()
        {
            this.serviceFactory = null;
        }

        [TestMethod]
        public void ServiceFactoryShouldBeAbleToCreateAServiceWith_No_Dependencies()
        {
            this.ServiceFactoryShouldBeAbleToCreateAService<ITestServiceWithNoDepenedencies, TestServiceWithNoDepenedencies>();
        }

        [TestMethod]
        public void ServiceFactoryShouldBeAbleToCreateAServiceWith_1_Dependency()
        {
            this.ServiceFactoryShouldBeAbleToCreateAService<ITestServiceWith_1_Dependency, TestServiceWith_1_Dependency>();
            AssertTestServiceWith_1_DependencyIsCorrectlyConfigured(serviceFactory!.GetService<ITestServiceWith_1_Dependency>());
        }

        [TestMethod]
        public void ServiceFactoryShouldBeAbleToCreateAServiceWith_2_Dependencies()
        {
            this.ServiceFactoryShouldBeAbleToCreateAService<ITestServiceWith_2_Dependencies, TestServiceWith_2_Dependencies>();

            TestServiceWith_2_Dependencies testServiceImplementation =
                GetServiceImplementation<ITestServiceWith_2_Dependencies, TestServiceWith_2_Dependencies>();

            AssertDependencySet(testServiceImplementation.TestServiceWithNoDepenedencies);
            AssertDependencySet(testServiceImplementation.TestServiceWith_1_Dependency);
            AssertTestServiceWith_1_DependencyIsCorrectlyConfigured(testServiceImplementation.TestServiceWith_1_Dependency);
        }

        [TestMethod]
        public void ServiceFactoryShouldAlwaysReturnTheSameInstanceForSingletonServices()
        {
            ITestServiceWith_2_Dependencies singletonService = this.serviceFactory!.GetService<ITestServiceWith_2_Dependencies>();
            Assert.IsNotNull(singletonService, "The service should exist");

            // The choice of 10 is an arbitrary number.
            for (int numberOfTimesToTest = 0; numberOfTimesToTest < 10; numberOfTimesToTest++)
            {
                ITestServiceWith_2_Dependencies currentSingletonService = this.serviceFactory.GetService<ITestServiceWith_2_Dependencies>();
                Assert.IsTrue(
                    object.ReferenceEquals(singletonService, currentSingletonService), 
                    "GetService() should always return the same instance of a singleton service.");
            }    
        }

        [TestMethod]
        public void ServiceFactoryShouldThrowAnExceptionIfAnUnregisteredServiceIsRequested()
        {
            // No registered service implements the ITestDataSource interface.
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                () => this.serviceFactory!.GetService<ITestDataSource>(),
                $"Cannot get the service because a service of type {typeof(ITestDataSource).FullName} has not been registered.");
        }

        [TestMethod]
        public void RegisterSingletonServiceShouldThrowAnExceptionIfIts_TServiceInterfaceType_ParameterDoesNotContainAnInterface()
        {
            // Note that the first type parameter is a class, not an interface like it should be.
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                () => this.serviceFactory!.RegisterSingletonService<TestServiceWithNoDepenedencies, TestServiceWithNoDepenedencies>(),
                
                $"TServiceInterfaceType must contain an interface type.  Here is the type it contained: " +
                $"{typeof(TestServiceWithNoDepenedencies).FullName}" );
        }

        [TestMethod]
        public void RegisterSingletonServiceShouldThrowAnExceptionIfIts_TServiceType_ParameterDoesNotContainAClass()
        {
            // Note that the second type parameter is an interface, not a class like it should be.
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                () => this.serviceFactory!.RegisterSingletonService<ITestServiceWithNoDepenedencies, ITestServiceWithNoDepenedencies>(),
                
                $"TServiceType must contain a class.  Here is the type it contained: " +
                $"{typeof(ITestServiceWithNoDepenedencies).FullName}");
        }

        [TestMethod]
        public void RegisterSingletonServiceShouldThrowAnExceptionIfIts_TServiceType_HasMultipleConstructors()
        {
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                () => this.serviceFactory!.RegisterSingletonService<IInvalidServiceWithMultipleConstructors, InvalidServiceWithMultipleConstructors>(),
                "Services can only have one public constructor.");
        }

        [TestMethod]
        public void RegisterSingletonServiceShouldThrowAnExceptionIfAServiceHasAlreadyBeenRegistered()
        {
            TestHelper.TestActionWhichShouldThrowAnException<ArgumentException>(
                () => this.serviceFactory!.RegisterSingletonService<ITestServiceWithNoDepenedencies, TestServiceWithNoDepenedencies>(),
                $"Cannot register a service because a service of type {typeof(ITestServiceWithNoDepenedencies).FullName} has already been registered.");
        }



        private void ServiceFactoryShouldBeAbleToCreateAService<TServiceInterfaceType, TServiceType>()
            where TServiceInterfaceType : class
            where TServiceType : class
        {
            TServiceInterfaceType requestedService = this.serviceFactory!.GetService<TServiceInterfaceType>();
            Type requestedServiceType = ((object)requestedService).GetType();
            Assert.IsTrue(
                requestedServiceType == typeof(TServiceType),
                "The service factory should create the object specified in the call to RegisterSingletonService()");
        }

        private TServiceType GetServiceImplementation<TServiceInterfaceType, TServiceType>()
            where TServiceInterfaceType : class
            where TServiceType : class
        {
            TServiceInterfaceType requestedService = this.serviceFactory!.GetService<TServiceInterfaceType>();
            TServiceType? serviceImplementation = requestedService as TServiceType;
            Assert.IsNotNull(serviceImplementation, "The service does not have the expected implementation.");
            return serviceImplementation;
        }

        private static void AssertTestServiceWith_1_DependencyIsCorrectlyConfigured(ITestServiceWith_1_Dependency testServiceWith_1_Dependency)
        {
            TestServiceWith_1_Dependency requestedImplementation = (TestServiceWith_1_Dependency)testServiceWith_1_Dependency;
            AssertDependencySet(requestedImplementation.TestServiceWithNoDepenedencies);
        }

        private static void AssertDependencySet<TServiceInterfaceType>(TServiceInterfaceType service)
        {
            Assert.IsNotNull(service, "The service should have its dependencies set.");
        }
    }
}
