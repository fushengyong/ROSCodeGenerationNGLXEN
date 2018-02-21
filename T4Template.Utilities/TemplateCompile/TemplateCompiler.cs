namespace T4Template.Utilities.TemplateCompile
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using T4Template.Utilities.Interfaces;

    /// <summary>
    /// Compile a T4 template with Roslyn
    /// </summary>
    public class TemplateCompiler : ITemplateCompiler
    {

        /// <summary>
        /// Compile the given Syntax Tree with the given parameters. Load the compiled assembly to the current AppDomain
        /// </summary>
        /// <param name="syntaxTreeToCompile"></param>
        /// <param name="compilationOptions"></param>
        /// <param name="references"></param>
        /// <param name="assemblyName"></param>
        /// <returns>Compiled and loaded Assembly</returns>
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

        /// <summary>
        /// Parse the template output string into a Syntax Tree
        /// </summary>
        /// <param name="templateOutput"></param>
        /// <returns>Parsed template output as a Syntax Tree</returns>
        public SyntaxTree ParseTemplateOutput(string templateOutput)
        {
            return CSharpSyntaxTree.ParseText(templateOutput);
        }
    }
}
