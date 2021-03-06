﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyIoC
{
    public class Container
    {
        public Dictionary<Type, Type> ContainerTypes { get; set; }

        public Container()
        {
            ContainerTypes = new Dictionary<Type, Type>();
        }

        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                var import = type.GetCustomAttribute<ImportConstructorAttribute>();
                if (import != null)
                {
                    AddType(type);
                }
                var importProperties = type.GetProperties().Where(p => p.GetCustomAttributes<ImportAttribute>() != null);
                if (importProperties != null)
                {
                    foreach (var importProp in importProperties)
                    {
                        AddType(importProp.PropertyType);
                    }
                }

                var export = type.GetCustomAttribute<ExportAttribute>();
                if (export != null)
                {
                    if (export.Contract != null)
                        AddType(type, export.Contract);
                    else
                        AddType(type);
                }

            }
        }

        public void AddType(Type type)
        {
            if (!ContainerTypes.ContainsKey(type))
                ContainerTypes.Add(type, type);
        }

        public void AddType(Type type, Type baseType)
        {
            if (ContainerTypes.ContainsKey(baseType))
                ContainerTypes[baseType] = type;
            else
                ContainerTypes.Add(baseType, type);
        }

        public object CreateInstance(Type type)
        {
            if (!ContainerTypes.ContainsKey(type))
            {
                throw new ResolvedTypeNotFoundException("There is no appropriate item in container to resolve this type. Add resolver for this type to container. ");
            }

            var resolvedType = ContainerTypes[type];               
            var constructor = resolvedType.GetConstructors().FirstOrDefault();
            var propsResolvers = constructor.GetParameters().Select(p => Activator.CreateInstance(ContainerTypes[p.ParameterType])).ToArray();
            return Activator.CreateInstance(resolvedType, propsResolvers);
        }

        public T CreateInstance<T>()
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] props = typeof(T).GetProperties();

            foreach (var property in props)
            {
                property.SetValue(instance, Activator.CreateInstance(ContainerTypes[property.PropertyType]));
            }
            return instance;
        }
    }

    public class ResolvedTypeNotFoundException : ApplicationException
    {
        public ResolvedTypeNotFoundException(string message)
       : base(message)
        {
        }
    }
}
