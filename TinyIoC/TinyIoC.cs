using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TinyInversionOfControl
{
    public class TinyIoCContainer
    {
        private readonly Dictionary<Type, DependencyConstructor> _preferred;

        internal TinyIoCContainer(Dictionary<Type, DependencyConstructor> preferred)
        {
            _preferred = preferred;
        }
        private object Resolve(Type type, ResolveContext resolveContext)
        {
            if (!_preferred.TryGetValue(type, out var dependency)) return ThrowNotRegisteredException();
            else if (resolveContext.Resolved.TryGetValue(type, out var value)) return value;
            else if (!resolveContext.ResolvedTypes.Contains(type)) resolveContext.ResolvedTypes.Add(type);
            else ThrowCircularDependencyException();

            var arguments = dependency.ArgumentTypes.Select(argType => Resolve(argType, resolveContext)).ToArray();
            var instance = dependency.Constructor(arguments);
            resolveContext.Resolved[type] = instance;
            return instance;

            object ThrowNotRegisteredException() => throw new Exception($"{type.Name} is not registered");
            
            void ThrowCircularDependencyException()
            {
                var circle = string.Join(" -> ", resolveContext.ResolvedTypes.SkipWhile(e => e != type).Select(rt => rt.Name));
                throw new Exception($"Circular dependency detected {circle} -> {type.Name}");
            }
        }
        public TProvider Resolve<TProvider>()
        {
            return (TProvider) Resolve(typeof(TProvider), new ResolveContext());
        }
    }
    public class TinyIoC
    {
        private readonly Dictionary<Type, DependencyConstructor[]> _registered = new Dictionary<Type, DependencyConstructor[]>();

        public void Register<TProvider>(bool singleton = false) => InternalRegister<TProvider, TProvider>();
        public void Register<TProvider>(TProvider instance) => InternalRegister<TProvider, TProvider>();

        public void Register<TInterface, TProvider>() where TProvider : TInterface => InternalRegister<TInterface, TProvider>();

        private void InternalRegister<TInterface, TProvider>()
            where TProvider : TInterface
        {
            var dependencyConstructors = GetDependencyConstructors<TProvider>();
            if (!dependencyConstructors.Any()) ThrowNoPublicConstructors();
            _registered[typeof(TInterface)] = dependencyConstructors;
            
            void ThrowNoPublicConstructors() => throw new Exception("No public constructors found");
        }
        
        private static DependencyConstructor[] GetDependencyConstructors<TProvider>()
        {
            return typeof(TProvider).GetConstructors().Select(ctor => new DependencyConstructor
            {
                Constructor = ctor.Invoke,
                ArgumentTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray()
            }).OrderBy(dependency => dependency.ArgumentTypes.Length).ToArray();
        }


        public TinyIoCContainer Build()
        {
            var preferred = new Dictionary<Type, DependencyConstructor>(); 
            foreach (var (key, dependency) in _registered)
            {
                var ctor = dependency.FirstOrDefault(ctor => ctor.ArgumentTypes.All(_registered.ContainsKey));
                preferred[key] = ctor ?? throw new Exception("No suitable constructor found");
            }
            return new TinyIoCContainer(preferred);
        }
        

    }
    internal class ResolveContext
    {
        public readonly HashSet<Type> ResolvedTypes = new HashSet<Type>();
        public readonly Dictionary<Type, object> Resolved = new Dictionary<Type, object>();
    }
    internal class DependencyConstructor
    {
        public Func<object[], object> Constructor;
        public Type[] ArgumentTypes;
    }
}