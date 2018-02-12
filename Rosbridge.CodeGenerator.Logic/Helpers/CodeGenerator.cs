namespace Rosbridge.CodeGenerator.Logic.Helpers
{
    using EnvDTE;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using Rosbridge.CodeGenerator.Logic.BaseClasses;
    using Rosbridge.CodeGenerator.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CodeGenerator
    {
        private const string ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH = @"CodeGenerators\RosMessage.tt";
        private const string CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH = @"CodeGenerators\TimeData.tt";

        private ITextTemplatingEngineHost _host;
        private ITextTemplating _textTemplating;
        private ITextTemplatingSessionHost _textTemplatingHost;
        private ISolutionManager _solutionManager;
        private string _defaultNamespace;
        private string _rosMessageTypeAttributeName;
        private string _rosMessageTypeAttributeNamespace;
        private string _rosMessageCodeGenerationTemplatePath;
        private string _customTimeDataTemplatePath;
        private string _rosMessageCodeGenerationTemplateContent;

        public CodeGenerator(ITextTemplatingEngineHost host, ITextTemplatingSessionHost textTemplatingSessionHost, ITextTemplating textTemplating, ISolutionManager solutionManager, string rosMessagesProjectName, string rosMessageTypeAttributeName, string rosMessageTypeAttributeNamespace)
        {
            if (null == host)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (null == textTemplatingSessionHost)
            {
                throw new ArgumentNullException(nameof(textTemplatingSessionHost));
            }

            if (null == textTemplating)
            {
                throw new ArgumentNullException(nameof(textTemplating));
            }

            if (null == solutionManager)
            {
                throw new ArgumentNullException(nameof(solutionManager));
            }

            if (null == rosMessagesProjectName)
            {
                throw new ArgumentNullException(nameof(rosMessagesProjectName));
            }

            if (string.Empty == rosMessagesProjectName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessagesProjectName));
            }

            if (null == rosMessageTypeAttributeName)
            {
                throw new ArgumentNullException(nameof(rosMessageTypeAttributeName));
            }

            if (string.Empty == rosMessageTypeAttributeName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessageTypeAttributeName));
            }

            if (null == rosMessageTypeAttributeNamespace)
            {
                throw new ArgumentNullException(nameof(rosMessageTypeAttributeNamespace));
            }

            if (string.Empty == rosMessageTypeAttributeNamespace)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessageTypeAttributeNamespace));
            }

            _host = host;
            _textTemplating = textTemplating;
            _textTemplatingHost = textTemplatingSessionHost;
            _solutionManager = solutionManager;
            _rosMessageTypeAttributeName = rosMessageTypeAttributeName;
            _rosMessageTypeAttributeNamespace = rosMessageTypeAttributeNamespace;

            _defaultNamespace = rosMessagesProjectName;
            _rosMessageCodeGenerationTemplatePath = _host.ResolvePath(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH);
            _rosMessageCodeGenerationTemplateContent = File.ReadAllText(_rosMessageCodeGenerationTemplatePath);
            _customTimeDataTemplatePath = _host.ResolvePath(CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH);
            _solutionManager.Initialize();
        }

        public void GenerateRosMessages(ISet<MsgFile> messageSet)
        {
            string standardNamespace = messageSet.SingleOrDefault(msg => msg.Type.TypeName == MsgFile.HEADER_TYPE).Type.NamespaceName;
            foreach (IGrouping<string, MsgFile> msgGroup in messageSet.GroupBy(msg => msg.Type.NamespaceName))
            {
                ProjectItem groupDirectoryProjectItem = GenerateMsgByNamespace(msgGroup, standardNamespace);
                if (msgGroup.Key == standardNamespace)
                {
                    ITextTemplatingSession session = new TextTemplatingSession();
                    session["Namespace"] = $"{_defaultNamespace}{standardNamespace}";
                    session["Type"] = MsgFile.CUSTOM_TIME_PRIMITIVE_TYPE;

                    TransformTemplateToFile(session, groupDirectoryProjectItem, _customTimeDataTemplatePath, File.ReadAllText(_customTimeDataTemplatePath), MsgFile.CUSTOM_TIME_PRIMITIVE_TYPE);
                }
            }
        }

        private ProjectItem GenerateMsgByNamespace(IGrouping<string, MsgFile> msgGroup, string standardNamespace)
        {
            string directoryName = msgGroup.Key;

            ProjectItem groupDirectoryProjectItem = _solutionManager.AddDirectoryToProject(directoryName);

            foreach (MsgFile message in msgGroup)
            {
                ITextTemplatingSession session = new TextTemplatingSession();

                session["MessageTypeAttributeName"] = _rosMessageTypeAttributeName;
                session["MessageTypeAttributeNamespace"] = _rosMessageTypeAttributeNamespace;
                session["NamespacePrefix"] = _defaultNamespace;
                ISet<string> standardNamespaceSet = new HashSet<string>();
                if (message.Type.NamespaceName != standardNamespace)
                {
                    standardNamespaceSet.Add(standardNamespace);
                }
                standardNamespaceSet.UnionWith(message.DependencySet.Select(dep => dep.NamespaceName).ToList());
                session["DependencyList"] = standardNamespaceSet.ToList();
                session["MessageNamespace"] = message.Type.NamespaceName;
                session["MessageType"] = message.Type.TypeName;
                session["ConstantFieldList"] = message.ConstantFieldSet.Select(field => Tuple.Create(field.Type.TypeName, field.FieldName, field.FieldValue)).ToList();
                session["ArrayFieldList"] = message.ArrayFieldSet.Select(field => Tuple.Create(field.Type.TypeName, field.FieldName, field.ArrayElementCount)).ToList();
                session["FieldList"] = message.FieldSet.ToDictionary(k => k.FieldName, v => v.Type.TypeName);

                TransformTemplateToFile(session, groupDirectoryProjectItem, _rosMessageCodeGenerationTemplatePath, _rosMessageCodeGenerationTemplateContent, message.Type.TypeName);
            }

            return groupDirectoryProjectItem;
        }

        private void TransformTemplateToFile(ITextTemplatingSession session, ProjectItem groupDirectoryProjectItem, string templatePath, string templateContent, string typeName)
        {
            string directoryPath = _solutionManager.GetProjectItemFullPath(groupDirectoryProjectItem);

            _textTemplatingHost.Session = session;

            string transformedTemplate = _textTemplating.ProcessTemplate(templatePath, templateContent);

            string newFilePath = WriteToFile(directoryPath, typeName, transformedTemplate);

            _solutionManager.AddFileToProjectItem(groupDirectoryProjectItem, newFilePath);
        }

        private string WriteToFile(string directoryPath, string typeName, string fileContent)
        {
            string newFilePath = Path.Combine(directoryPath, $"{typeName}.cs");

            File.WriteAllText(newFilePath, fileContent);

            return newFilePath;
        }
    }
}
