using System;
using System.Collections.Generic;

namespace DI
{
    public class DIContainer
    {
        private readonly DIContainer _parentContainer;
        // using tuple for a key (string + type)
        private readonly Dictionary<(string, Type), DIRegistration> _registrations = new();
        // requests cache
        private readonly HashSet<(string, Type)> _resolutions = new();

        public DIContainer(DIContainer parentContainer = null)
        {
            _parentContainer = parentContainer;
        }
        
        // no tag Singleton registration
        public void RegisterSingleton<T>(Func<DIContainer, T> factory)
        {
            RegisterSingleton(null, factory);
        }
        
        // tag Singleton registration
        public void RegisterSingleton<T>(string tag, Func<DIContainer, T> factory)
        {
            var key = (tag, typeof(T));
            Register(key, factory, true);
        }
        
        // no tag Transient registration
        public void RegisterTransient<T>(Func<DIContainer, T> factory)
        {
            RegisterTransient(null, factory);
        }
        
        // tag Transient registration
        public void RegisterTransient<T>(string tag, Func<DIContainer, T> factory)
        {
            var key = (tag, typeof(T));
            Register(key, factory, false);
        }
        
        // no tag Instance registration
        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(null, instance);
        }
        
        // resolving
        public T Resolve<T>(string tag = null)
        {
            var key = (tag, typeof(T));
            
            // check request int resolutions for preventing requests looping
            if (_resolutions.Contains(key))
            {
                throw new Exception($"Cyclic dependency for tag {tag} and type {key.Item2.FullName} is detected");
            }
            
            //caching request
            _resolutions.Add(key);

            try
            {
                if (_registrations.TryGetValue(key, out var registration))
                {
                    if (registration.IsSingleton)
                    {
                        if (registration.Instance == null && registration.Factory != null)
                        {
                            // create instance
                            registration.Instance = registration.Factory(this);
                        }

                        return (T)registration.Instance;
                    }
                
                    // if is not a singleton
                    return (T)registration.Factory(this);
                }

                if (_parentContainer != null)
                {
                    return _parentContainer.Resolve<T>(tag);
                }
            }
            finally
            {
                _resolutions.Remove(key);
            }

            // if any error
            throw new Exception($"Couldn't find dependency for tag {tag} and type {key.Item2.FullName}");
        }
        
        // tag Instance registration
        public void RegisterInstance<T>(string tag, T instance)
        {
            var key = (tag, typeof(T));
            if (_registrations.ContainsKey(key))
            {
                throw new Exception($"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            _registrations[key] = new DIRegistration
            {
                Instance = instance,
                IsSingleton = true
            };
        }

        // can used both for Singleton and Transient
        private void Register<T>((string, Type) key, Func<DIContainer, T> factory, bool isSingleton)
        {
            if (_registrations.ContainsKey(key))
            {
                throw new Exception($"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            _registrations[key] = new DIRegistration
            {
                Factory = c => factory(c),
                IsSingleton = isSingleton
            };
        }
    }
}
