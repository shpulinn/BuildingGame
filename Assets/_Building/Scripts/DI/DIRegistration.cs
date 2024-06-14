using System;

namespace DI
{
    public class DIRegistration
    {
        // delegate, gets container and returns an object
        public Func<DIContainer, object> Factory { get; set; }
        public bool IsSingleton { get; set; }
        public object Instance { get; set; }
    }
}