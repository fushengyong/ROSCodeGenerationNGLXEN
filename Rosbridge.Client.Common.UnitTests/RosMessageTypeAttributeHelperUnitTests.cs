namespace Rosbridge.Client.Common.UnitTests
{
    using NUnit.Framework;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [TestFixture]
    public class RosMessageTypeAttributeHelperUnitTests
    {
        private RosMessageTypeAttributeHelper _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new RosMessageTypeAttributeHelper();
        }

        [Test]
        public void Test()
        {
            //arrange
            //TypeBuilder tb = GetTypeBuilder();
            //CustomAttributeBuilder c = new CustomAttributeBuilder();

            //tb.GetCustomAttributesData().Add
            //act
            //assert
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }
    }
}
