namespace T4Template.Utilities.TemplateProcess.Interfaces
{
    using Microsoft.VisualStudio.TextTemplating;
    using System.IO;

    public interface ITemplateProcessor
    {
        string ProcessTemplateWithSession(FileInfo template, ITextTemplatingSession session);
    }
}
