namespace Rosbridge.CodeGeneration.Logic.Helpers
{
    using EnvDTE;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using Rosbridge.CodeGeneration.Logic.BaseClasses;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.Exceptions;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Code generation controller
    /// </summary>
    public class CodeGenerator
    {
        private const string ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH = @"CodeGenerators\RosMessage.tt";
        private const string CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH = @"CodeGenerators\TimeData.tt";

        private ITextTemplatingEngineHost _textTemplatingEngineHost;
        private ITextTemplating _textTemplating;
        private ITextTemplatingSessionHost _textTemplatingSessionHost;
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

            if (string.IsNullOrWhiteSpace(rosMessagesProjectName))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessagesProjectName));
            }

            if (string.IsNullOrWhiteSpace(rosMessageTypeAttributeName))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessageTypeAttributeName));
            }

            if (string.IsNullOrWhiteSpace(rosMessageTypeAttributeNamespace))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessageTypeAttributeNamespace));
            }

            _textTemplatingEngineHost = host;
            _textTemplating = textTemplating;
            _textTemplatingSessionHost = textTemplatingSessionHost;
            _solutionManager = solutionManager;
            _rosMessageTypeAttributeName = rosMessageTypeAttributeName;
            _rosMessageTypeAttributeNamespace = rosMessageTypeAttributeNamespace;

            _defaultNamespace = rosMessagesProjectName;
            _rosMessageCodeGenerationTemplatePath = _textTemplatingEngineHost.ResolvePath(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH);
            _rosMessageCodeGenerationTemplateContent = File.ReadAllText(_rosMessageCodeGenerationTemplatePath);
            _customTimeDataTemplatePath = _textTemplatingEngineHost.ResolvePath(CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH);
            _solutionManager.Initialize();
        }

        public void GenerateRosMessages(ISet<MsgFile> messageSet, string standardNamespace)
        {
            if (null == messageSet)
            {
                throw new ArgumentNullException(nameof(messageSet));
            }

            if (string.IsNullOrWhiteSpace(standardNamespace))
            {
                throw new NoStandardNamespaceException();
            }

            IEnumerable<IGrouping<string, MsgFile>> messageGroupList = messageSet.GroupBy(msg => msg.Type.Namespace);

            GenerateStandardNamespaceMessages(messageGroupList, standardNamespace);

            foreach (IGrouping<string, MsgFile> messageGroup in messageGroupList.Where(group => group.Key != standardNamespace))
            {
                string @namespace = messageGroup.Key;

                ProjectItem groupDirectoryProjectItem = GenerateMessagesByNamespace(@namespace, messageGroup, standardNamespace);
            }
        }

        private void GenerateStandardNamespaceMessages(IEnumerable<IGrouping<string, MsgFile>> messageGroupList, string standardNamespace)
        {
            IGrouping<string, MsgFile> standardNamespaceGroup = messageGroupList.SingleOrDefault(group => group.Key == standardNamespace);

            if (null == standardNamespaceGroup)
            {
                throw new NoStandardNamespaceException();
            }

            ProjectItem standardNamespaceDirectoryProjectItem = GenerateMessagesByNamespace(standardNamespace, standardNamespaceGroup, standardNamespace);

            GenerateCustomTimePrimitiveType(standardNamespaceDirectoryProjectItem, standardNamespace);
        }

        private void GenerateCustomTimePrimitiveType(ProjectItem standardNamespaceDirectoryProjectItem, string standardNamespace)
        {
            ITextTemplatingSession session = _textTemplatingSessionHost.CreateSession();
            session[TemplateParameterConstants.TimeData.NAMESPACE] = $"{_defaultNamespace}.{standardNamespace}";
            session[TemplateParameterConstants.TimeData.TYPE] = RosConstants.MessageTypes.CUSTOM_TIME_PRIMITIVE_TYPE;

            TransformTemplateToFile(session, standardNamespaceDirectoryProjectItem, _customTimeDataTemplatePath, File.ReadAllText(_customTimeDataTemplatePath), RosConstants.MessageTypes.CUSTOM_TIME_PRIMITIVE_TYPE);
        }

        private ProjectItem GenerateMessagesByNamespace(string @namespace, IGrouping<string, MsgFile> messageGroup, string standardNamespace)
        {
            ProjectItem groupDirectoryProjectItem = _solutionManager.AddNewDirectoryToProject(@namespace);

            GenerateMessages(groupDirectoryProjectItem, messageGroup, standardNamespace);

            return groupDirectoryProjectItem;
        }

        private void GenerateMessages(ProjectItem directoryProjectItem, IEnumerable<MsgFile> messageList, string standardNamespace)
        {
            foreach (MsgFile message in messageList)
            {
                ITextTemplatingSession session = _textTemplatingSessionHost.CreateSession();

                session[TemplateParameterConstants.RosMessage.ROS_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE] = _rosMessageTypeAttributeNamespace;
                session[TemplateParameterConstants.RosMessage.ROS_MESSAGE_TYPE_ATTRIBUTE_NAME] = _rosMessageTypeAttributeName;
                session[TemplateParameterConstants.RosMessage.NAMESPACE_PREFIX] = _defaultNamespace;

                ISet<string> dependencySet = new HashSet<string>() { standardNamespace };
                dependencySet.UnionWith(message.DependencySet.Select(type => type.Namespace));
                session[TemplateParameterConstants.RosMessage.DEPENDENCY_LIST] = dependencySet;
                session[TemplateParameterConstants.RosMessage.NAMESPACE] = message.Type.Namespace;
                session[TemplateParameterConstants.RosMessage.TYPE] = message.Type.Type;
                session[TemplateParameterConstants.RosMessage.CONSTANT_FIELD_LIST] = message.ConstantFieldSet.Select(field => Tuple.Create(field.Type.Type, field.FieldName, field.FieldValue)).ToList();
                session[TemplateParameterConstants.RosMessage.ARRAY_FIELD_LIST] = message.ArrayFieldSet.Select(field => Tuple.Create(field.Type.Type, field.FieldName, field.ArrayElementCount)).ToList();
                session[TemplateParameterConstants.RosMessage.FIELD_LIST] = message.FieldSet.ToDictionary(k => k.FieldName, v => v.Type.Type);

                TransformTemplateToFile(session, directoryProjectItem, _rosMessageCodeGenerationTemplatePath, _rosMessageCodeGenerationTemplateContent, message.Type.Type);
            };
        }

        private void TransformTemplateToFile(ITextTemplatingSession session, ProjectItem groupDirectoryProjectItem, string templatePath, string templateContent, string typeName)
        {
            string directoryPath = _solutionManager.GetProjectItemFullPath(groupDirectoryProjectItem);

            _textTemplatingSessionHost.Session = session;

            string transformedTemplate = _textTemplating.ProcessTemplate(templatePath, templateContent);

            string newFilePath = WriteToFile(directoryPath, typeName, transformedTemplate);

            _solutionManager.AddFileToDirectoryProjectItem(groupDirectoryProjectItem, newFilePath);
        }

        private string WriteToFile(string directoryPath, string typeName, string fileContent)
        {
            string newFilePath = Path.Combine(directoryPath, $"{typeName}.cs");

            File.WriteAllText(newFilePath, fileContent);

            return newFilePath;
        }
    }
}
