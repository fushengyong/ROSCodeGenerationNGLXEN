namespace Rosbridge.CodeGeneration.Logic.UnitTests.Utilities
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class RoslynUtilities
    {
        public static Assembly CompileAndLoadSyntaxTree(SyntaxTree syntaxTree, CSharpCompilationOptions defaultCompilationOptions, IEnumerable<MetadataReference> defaultReferences, [CallerMemberName] string assemblyName = "TestAssembly")
        {
            CSharpCompilation compilation = CSharpCompilation.Create($"{assemblyName}.dll", new SyntaxTree[] { syntaxTree }, null, defaultCompilationOptions);
            compilation = compilation.WithReferences(defaultReferences);
            using (var stream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(stream);
                if (result.Success)
                {
                    var assembly = Assembly.Load(stream.GetBuffer());
                    return assembly;
                }
                return null;
            }
        }
    }
}
