//
// MIT License
//
// Copyright(c) 2019-2021 Benjamin Ellett
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

namespace CommonGeneratorCode
{
    public class ServiceMetadata
    {
        private object singletonServiceInstance;

        public ServiceMetadata(Type serviceType, bool isSingleton)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            this.ServiceType = serviceType;
            this.IsSingletonService = isSingleton;
            this.SingletonServiceInstance = null;
        }

        public Type ServiceType { get; init; }

        public bool IsSingletonService { get; init; }

        public object SingletonServiceInstance
        {
            get
            {
                if (!this.IsSingletonService)
                {
                    throw new InvalidOperationException($"{nameof(this.SingletonServiceInstance)} should only be called if a service is a singleton service.");
                }

                return this.singletonServiceInstance;
            }

            set
            {
                if ((value != null) && (value.GetType() != this.ServiceType))
                {
                    throw new InvalidOperationException($"Only values of type {this.ServiceType.FullName} can be passed to {nameof(this.SingletonServiceInstance)}.");
                }

                this.singletonServiceInstance = value;
            }
        }
    }
}
