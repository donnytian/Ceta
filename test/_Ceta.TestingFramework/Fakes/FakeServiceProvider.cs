using System;

namespace _Ceta.TestingFramework.Fakes
{
    public class FakeServiceProvider : IServiceProvider
    {
        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
            {
                return this;
            }
            return null;
        }
    }
}
