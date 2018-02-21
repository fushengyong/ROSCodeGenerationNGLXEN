namespace T4Template.Utilities.Interfaces
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Collections.Generic;
    using System.Reflection;

    public interface ITemplateCompiler
    {
        /// <summary>
        /// Parse the given template output into a syntax tree.
        /// </summary>
        /// <param name="templateOutput"></param>
        /// <returns></returns>
        SyntaxTree ParseTemplateOutput(string templateOutput);
        /// <summary>
        /// Compile the given syntax tree.
        /// </summary>
        /// <param name="syntaxTreeToCompile"></param>
        /// <param name="compilationOptions"></param>
        /// <param name="references"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        Assembly CompileSyntaxTree(SyntaxTree syntaxTreeToCompile, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> references, string assemblyName);
    }
}
