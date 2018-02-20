namespace T4Template.Utilities.Interfaces
{
    using Microsoft.VisualStudio.TextTemplating;
    using System.IO;

    public interface ITemplateProcessor
    {
        /// <summary>
        /// Process the template using the given session.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="session"></param>
        /// <returns>Template output</returns>
        string ProcessTemplateWithSession(FileInfo template, ITextTemplatingSession session);
    }
}
