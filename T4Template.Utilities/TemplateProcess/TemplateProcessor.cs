﻿namespace T4Template.Utilities.TemplateProcess
{
    using Microsoft.VisualStudio.TextTemplating;
    using System;
    using System.IO;
    using T4Template.Utilities.Interfaces;

    /// <summary>
    /// T4 template processor
    /// </summary>
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly ICustomTextTemplatingEngineHost _textTemplatingEngineHost;

        public TemplateProcessor(ICustomTextTemplatingEngineHost textTemplatingEngineHost)
        {
            _textTemplatingEngineHost = textTemplatingEngineHost;
        }

        /// <summary>
        /// Process the given template with the given session
        /// </summary>
        /// <param name="template"></param>
        /// <param name="session"></param>
        /// <returns></returns>
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
