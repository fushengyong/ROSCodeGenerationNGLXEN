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
        public const string TEMPLATE_PARAMETER_NAMESPACE = "Namespace";
        public const string TEMPLATE_PARAMETER_TYPE = "Type";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_MESSAGE_TYPE_ATTRIBUTE_NAME = "MessageTypeAttributeName";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE = "MessageTypeAttributeNamespace";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_NAMESPACE_PREFIX = "NamespacePrefix";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_DEPENDENCY_LIST = "DependencyList";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_CONSTANT_FIELD_LIST = "ConstantFieldList";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_ARRAY_FIELD_LIST = "ArrayFieldList";
        public const string ROS_MESSAGE_TEMPLATE_PARAMETER_FIELD_LIST = "FieldList";

        private const string ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH = @"CodeGenerators\RosMessage.tt";
        private const string CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH = @"CodeGenerators\TimeData.tt";

        private ITextTemplatingEngineHost _engineHost;
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

            _engineHost = host;
            _textTemplating = textTemplating;
            _textTemplatingSessionHost = textTemplatingSessionHost;
            _solutionManager = solutionManager;
            _rosMessageTypeAttributeName = rosMessageTypeAttributeName;
            _rosMessageTypeAttributeNamespace = rosMessageTypeAttributeNamespace;

            _defaultNamespace = rosMessagesProjectName;
            _rosMessageCodeGenerationTemplatePath = _engineHost.ResolvePath(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH);
            _rosMessageCodeGenerationTemplateContent = File.ReadAllText(_rosMessageCodeGenerationTemplatePath);
            _customTimeDataTemplatePath = _engineHost.ResolvePath(CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH);
            _solutionManager.Initialize();
        }

        public void GenerateRosMessages(ISet<MsgFile> messageSet)
        {
            string standardNamespace = messageSet.SingleOrDefault(msg => msg.Type.Type == MsgFile.HEADER_TYPE).Type.Namespace;
            foreach (IGrouping<string, MsgFile> msgGroup in messageSet.GroupBy(msg => msg.Type.Namespace))
            {
                ProjectItem groupDirectoryProjectItem = GenerateMsgByNamespace(msgGroup, standardNamespace);
                if (msgGroup.Key == standardNamespace)
                {
                    ITextTemplatingSession session = new TextTemplatingSession();
                    session[TEMPLATE_PARAMETER_NAMESPACE] = $"{_defaultNamespace}{standardNamespace}";
                    session[TEMPLATE_PARAMETER_TYPE] = MsgFile.CUSTOM_TIME_PRIMITIVE_TYPE;

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

                session[ROS_MESSAGE_TEMPLATE_PARAMETER_MESSAGE_TYPE_ATTRIBUTE_NAME] = _rosMessageTypeAttributeName;
                session[ROS_MESSAGE_TEMPLATE_PARAMETER_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE] = _rosMessageTypeAttributeNamespace;
                session[ROS_MESSAGE_TEMPLATE_PARAMETER_NAMESPACE_PREFIX] = _defaultNamespace;
                ISet<string> standardNamespaceSet = new HashSet<string>();
                if (message.Type.Namespace != standardNamespace)
                {
                    standardNamespaceSet.Add(standardNamespace);
                }
                standardNamespaceSet.UnionWith(message.DependencySet.Select(dep => dep.Namespace).ToList());
                session[ROS_MESSAGE_TEMPLATE_PARAMETER_DEPENDENCY_LIST] = standardNamespaceSet.ToList();
                session[TEMPLATE_PARAMETER_NAMESPACE] = message.Type.Namespace;
                session[TEMPLATE_PARAMETER_TYPE] = message.Type.Type;
                session[ROS_MESSAGE_TEMPLATE_PARAMETER_CONSTANT_FIELD_LIST] = message.ConstantFieldSet.Select(field => Tuple.Create(field.Type.Type, field.FieldName, field.FieldValue)).ToList();
                session[ROS_MESSAGE_TEMPLATE_PARAMETER_ARRAY_FIELD_LIST] = message.ArrayFieldSet.Select(field => Tuple.Create(field.Type.Type, field.FieldName, field.ArrayElementCount)).ToList();
                session[ROS_MESSAGE_TEMPLATE_PARAMETER_FIELD_LIST] = message.FieldSet.ToDictionary(k => k.FieldName, v => v.Type.Type);

                TransformTemplateToFile(session, groupDirectoryProjectItem, _rosMessageCodeGenerationTemplatePath, _rosMessageCodeGenerationTemplateContent, message.Type.Type);
            }

            return groupDirectoryProjectItem;
        }

        private void TransformTemplateToFile(ITextTemplatingSession session, ProjectItem groupDirectoryProjectItem, string templatePath, string templateContent, string typeName)
        {
            string directoryPath = _solutionManager.GetProjectItemFullPath(groupDirectoryProjectItem);

            _textTemplatingSessionHost.Session = session;

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
