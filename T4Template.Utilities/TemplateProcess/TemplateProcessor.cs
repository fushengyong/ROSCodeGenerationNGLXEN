namespace T4Template.Utilities.TemplateProcess
{
    using Microsoft.VisualStudio.TextTemplating;
    using System;
    using System.IO;
    using T4Template.Utilities.Interfaces;

    public class TemplateProcessor : ITemplateProcessor
    {
        private ICustomTextTemplatingEngineHost _textTemplatingEngineHost;

        public TemplateProcessor(ICustomTextTemplatingEngineHost textTemplatingEngineHost)
        {
            _textTemplatingEngineHost = textTemplatingEngineHost;
        }

        public string ProcessTemplateWithSession(FileInfo template, ITextTemplatingSession session)
        {
            if (null == template)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (null == session)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (!template.Exists)
            {
                throw new FileNotFoundException(template.FullName);
            }

            _textTemplatingEngineHost.TemplateFile = template.FullName;
            _textTemplatingEngineHost.Session = session;

            string templateContent = File.ReadAllText(template.FullName);

            Engine engine = new Engine();

            string templateOutput = engine.ProcessTemplate(templateContent, _textTemplatingEngineHost);

            return templateOutput;
        }
    }
}
