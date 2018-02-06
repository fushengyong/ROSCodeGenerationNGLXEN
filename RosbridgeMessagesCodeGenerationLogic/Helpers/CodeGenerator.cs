namespace RosbridgeMessagesCodeGenerationLogic.Helpers
{
    using EnvDTE;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using RosbridgeMessagesCodeGenerationLogic.BaseClasses;
    using RosbridgeMessagesCodeGenerationLogic.Interfaces;
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

            _host = host;
            _textTemplating = textTemplating;
            _textTemplatingHost = textTemplatingSessionHost;
            _solutionManager = solutionManager;
            _rosMessageTypeAttributeName = rosMessageTypeAttributeName;
            _rosMessageTypeAttributeNamespace = rosMessageTypeAttributeNamespace;

            _defaultNamespace = rosMessagesProjectName;
            _rosMessageCodeGenerationTemplatePath = _host.ResolvePath(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH);
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
                    TextTemplatingSession session = new TextTemplatingSession();
                    session["Namespace"] = string.Format("{0}.{1}", _defaultNamespace, standardNamespace);
                    session["Type"] = MsgFile.CUSTOM_TIME_PRIMITIVE_TYPE;

                    TransformTemplateToFile(_customTimeDataTemplatePath, session, groupDirectoryProjectItem, MsgFile.CUSTOM_TIME_PRIMITIVE_TYPE);
                }
            }
        }

        private ProjectItem GenerateMsgByNamespace(IGrouping<string, MsgFile> msgGroup, string standardNamespace)
        {
            string directoryName = msgGroup.Key;

            ProjectItem groupDirectoryProjectItem = _solutionManager.AddDirectoryToProject(directoryName);

            foreach (MsgFile message in msgGroup)
            {
                TextTemplatingSession session = new TextTemplatingSession();

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
                session["ConstantFieldList"] = message.FieldSet.Where(field => field.IsConst && !field.IsArray).Select(field => Tuple.Create(field.Type.TypeName, field.FieldName, field.FieldValue)).ToList();
                session["ArrayFieldList"] = message.FieldSet.Where(field => field.IsArray && !field.IsConst).Select(field => Tuple.Create(field.Type.TypeName, field.FieldName, field.ArrayElementCount)).ToList();
                session["FieldList"] = message.FieldSet.Where(field => !field.IsConst && !field.IsArray).ToDictionary(k => k.FieldName, v => v.Type.TypeName);

                TransformTemplateToFile(_rosMessageCodeGenerationTemplatePath, session, groupDirectoryProjectItem, message.Type.TypeName);
            }

            return groupDirectoryProjectItem;
        }

        private void TransformTemplateToFile(string templatePath, TextTemplatingSession session, ProjectItem groupDirectoryProjectItem, string fileName)
        {
            string directoryPath = _solutionManager.GetProjectItemFullPath(groupDirectoryProjectItem);

            _textTemplatingHost.Session = session;

            string transformedTemplate = _textTemplating.ProcessTemplate(templatePath, File.ReadAllText(templatePath));

            FileInfo newFile = WriteToFile(directoryPath, fileName, transformedTemplate);

            _solutionManager.AddFileToProjectItem(groupDirectoryProjectItem, newFile);
        }

        private FileInfo WriteToFile(string directoryPath, string fileName, string fileContent)
        {
            FileInfo newFile = new FileInfo(Path.Combine(directoryPath, string.Format("{0}.cs", fileName)));

            File.WriteAllText(newFile.FullName, fileContent);

            newFile.Refresh();

            return newFile;
        }
    }
}
