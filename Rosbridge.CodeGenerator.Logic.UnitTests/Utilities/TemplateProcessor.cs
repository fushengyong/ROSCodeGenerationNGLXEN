namespace Rosbridge.CodeGenerator.Logic.UnitTests
{
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using System;
    using System.IO;

    public class TemplateProcessor
    {
        private ITextTemplatingEngineHost _textTemplatingEngineHost;
        private ITextTemplating _textTemplating;
        private ITextTemplatingSessionHost _textTemplatingSessionHost;

        public TemplateProcessor(ITextTemplatingEngineHost textTemplatingEngineHost, ITextTemplating textTemplating, ITextTemplatingSessionHost textTemplatingSessionHost)
        {
            if (null == textTemplatingEngineHost)
            {
                throw new ArgumentNullException(nameof(textTemplatingEngineHost));
            }

            if (null == textTemplating)
            {
                throw new ArgumentNullException(nameof(textTemplating));
            }

            if (null == textTemplatingSessionHost)
            {
                throw new ArgumentNullException(nameof(textTemplatingSessionHost));
            }

            _textTemplatingEngineHost = textTemplatingEngineHost;
            _textTemplating = textTemplating;
            _textTemplatingSessionHost = textTemplatingSessionHost;
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

            _textTemplatingSessionHost.Session = session;

            string templateContent = File.ReadAllText(template.FullName);

            Engine engine = new Engine();

            string templateOutput = engine.ProcessTemplate(templateContent, _textTemplatingEngineHost);

            return templateOutput;
        }
    }
}
