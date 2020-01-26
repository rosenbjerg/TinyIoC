using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TinyInversionOfControl
{
    public class TinyIoC
    {
        private readonly ConcurrentDictionary<Type, DependencyConstructor[]> _registered = new ConcurrentDictionary<Type, DependencyConstructor[]>();
        private readonly ConcurrentDictionary<Type, DependencyConstructor> _preferred = new ConcurrentDictionary<Type, DependencyConstructor>();

        public void Register<TProvider>(bool singleton = false) => InternalRegister<TProvider, TProvider>();
        public void Register<TProvider>(TProvider instance) => InternalRegister<TProvider, TProvider>();

        public void Register<TInterface, TProvider>() where TProvider : TInterface => InternalRegister<TInterface, TProvider>();

        private void InternalRegister<TInterface, TProvider>()
            where TProvider : TInterface
        {
            var dependencyConstructors = GetDependencyConstructors<TProvider>();
            if (!dependencyConstructors.Any()) throw new Exception("No public constructors found");
            _registered[typeof(TInterface)] = dependencyConstructors;
        }
        
        private static DependencyConstructor[] GetDependencyConstructors<TProvider>()
        {
            return typeof(TProvider).GetConstructors().Select(ctor => new DependencyConstructor
            {
                Constructor = ctor.Invoke,
                ArgumentTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray()
            }).OrderBy(dependency => dependency.ArgumentTypes.Length).ToArray();
        }

        private object Resolve(Type type, ResolveContext resolveContext)
        {
            if (!_registered.TryGetValue(type, out var dependency)) throw new Exception($"{type.Name} is not registered");
            else if (resolveContext.Resolved.TryGetValue(type, out var value)) return value;
            else if (!resolveContext.ResolvedTypes.Contains(type)) resolveContext.ResolvedTypes.Add(type);
            else 
            {
                var circle = string.Join(" -> ", resolveContext.ResolvedTypes.SkipWhile(e => e != type).Select(rt => rt.Name));
                throw new Exception($"Circular dependency detected {circle} -> {type.Name}");
            }

            var preferredCtor = _preferred[type];
            if (preferredCtor == null) throw new Exception("No suitable constructor found");

            var arguments = preferredCtor.ArgumentTypes.Select(argType => Resolve(argType, resolveContext)).ToArray();
            var instance = preferredCtor.Constructor(arguments);
            resolveContext.Resolved[type] = instance;
            return instance;
        }
        public TProvider Resolve<TProvider>()
        {
            return (TProvider) Resolve(typeof(TProvider), new ResolveContext());
        }

        public void Build()
        {
            foreach (var (key, dependency) in _registered)
            {
                _preferred[key] = dependency.FirstOrDefault(ctor => ctor.ArgumentTypes.All(_registered.ContainsKey));
            }
        }

        private class Dependency
        {
            public bool Singleton;
            public DependencyConstructor[] Constructors;

            public DependencyConstructor PreferredConstructor;
            
            public Dependency(DependencyConstructor[] constructors)
            {
                Constructors = constructors;
            }
        }
        private class DependencyConstructor
        {
            public Func<object[], object> Constructor;
            public Type[] ArgumentTypes;
        }

        private class ResolveContext
        {
            public readonly HashSet<Type> ResolvedTypes = new HashSet<Type>();
            public readonly Dictionary<Type, object> Resolved = new Dictionary<Type, object>();
        }
    }
}