namespace T4Template.Utilities.Interfaces
{
    using Microsoft.VisualStudio.TextTemplating;
    using System.CodeDom.Compiler;

    public interface ICustomTextTemplatingEngineHost : ITextTemplatingEngineHost, ITextTemplatingSessionHost
    {
        /// <summary>
        /// The T4 template file path
        /// </summary>
        new string TemplateFile { get; set; }
        /// <summary>
        /// Get the template processing errors
        /// </summary>
        CompilerErrorCollection Errors { get; }
    }
}
