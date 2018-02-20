namespace T4Template.Utilities.TemplateCompile
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using T4Template.Utilities.Interfaces;

    public class TemplateCompiler : ITemplateCompiler
    {
        public Assembly CompileSyntaxTree(SyntaxTree syntaxTreeToCompile, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> references, string assemblyName)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(string.Format("{0}.dll", assemblyName), new SyntaxTree[] { syntaxTreeToCompile }, null, compilationOptions);
            compilation = compilation.WithReferences(references);
            using (MemoryStream stream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(stream);
                if (result.Success)
                {
                    Assembly loadedAssembly = Assembly.Load(stream.GetBuffer());
                    return loadedAssembly;
                }

                return null;
            }
        }

        public SyntaxTree ParseTemplateOutput(string templateOutput)
        {
            return CSharpSyntaxTree.ParseText(templateOutput);
        }
    }
}
