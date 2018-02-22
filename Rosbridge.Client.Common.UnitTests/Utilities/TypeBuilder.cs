namespace Rosbridge.Client.Common.UnitTests.Utilities
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    public class TypeBuilder
    {
        private AppDomain _appDomain;
        private AssemblyName _assemblyName;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private System.Reflection.Emit.TypeBuilder _typeBuilder;

        public TypeBuilder(AppDomain appDomainToUse, string assemblyName, AssemblyBuilderAccess assemblyBuilderAccess, string moduleName) : this(assemblyName, assemblyBuilderAccess, moduleName)
        {
            _appDomain = appDomainToUse;
        }

        public TypeBuilder(string assemblyName, AssemblyBuilderAccess assemblyBuilderAccess, string moduleName)
        {
            if (null == _appDomain)
            {
                _appDomain = Thread.GetDomain();
            }
            _assemblyName = new AssemblyName(assemblyName);
            _assemblyBuilder = _appDomain.DefineDynamicAssembly(_assemblyName, assemblyBuilderAccess);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(moduleName);
        }

        public TypeBuilder Inicialize(string typeName, TypeAttributes typeAttributes, Type[] implementedInterfaces)
        {
            _typeBuilder = _moduleBuilder.DefineType(typeName, typeAttributes, null, implementedInterfaces);
            return this;
        }

        public TypeBuilder SetParent(Type parent)
        {
            _typeBuilder.SetParent(parent);
            return this;
        }

        public TypeBuilder AddCustomAttribute(Type attributeType, Type[] attributeConstructorParameters, object[] attributeConstuctorArgs)
        {
            if (null == attributeType)
            {
                throw new ArgumentNullException(nameof(attributeType));
            }

            ConstructorInfo ctorInfo = attributeType.GetConstructor(attributeConstructorParameters);

            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(ctorInfo, attributeConstuctorArgs);

            _typeBuilder.SetCustomAttribute(customAttributeBuilder);

            return this;
        }

        public Type CreateType()
        {
            return _typeBuilder.CreateType();
        }
    }
}
