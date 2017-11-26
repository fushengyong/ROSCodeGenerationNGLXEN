namespace RosbridgeMessagesCodeGenerationLogic.Helpers
{
    using RosbridgeMessagesCodeGenerationLogic.BaseClasses;
    using RosbridgeMessagesCodeGenerationLogic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class YAMLParser : IYAMLParser
    {
        private const string NamespaceRegexGroupName = "namespace";
        private const string TypeRegexGroupName = "type";
        private const string IsArrayRegexGroupName = "isArray";
        private const string ElementCountRegexGroupName = "elementCount";
        private const string VariableNameRegexGroupName = "name";
        private const string ConstantValueRegexGroupName = "value";

        private const string YAMLParserRegexString = @"^\s*(?:(?<namespace>\w+)\/)?(?<type>\w+)\b(?<isArray>\[(?<elementCount>\d*)\])?\s+(?<name>\w+)(?:\s*=\s*(?<value>\w+))?";
        private static Regex YAMLParserRegex = new Regex(YAMLParserRegexString, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private readonly IDictionary<string, string> _primitiveTypeDictionary;

        public YAMLParser(IDictionary<string, string> primitiveTypeDictionary)
        {
            if (null == primitiveTypeDictionary)
            {
                throw new ArgumentNullException(nameof(primitiveTypeDictionary));
            }

            _primitiveTypeDictionary = primitiveTypeDictionary;
        }

        ISet<MessageField> IYAMLParser.YAMLStringToMessageFieldSet(string yamlString)
        {
            if (null == yamlString)
            {
                throw new ArgumentNullException(nameof(yamlString));
            }

            HashSet<MessageField> result = new HashSet<MessageField>();
            foreach (Match currentMatch in YAMLParserRegex.Matches(yamlString))
            {
                if (currentMatch.Success)
                {
                    string namespaceName = currentMatch.Groups[NamespaceRegexGroupName].Value;
                    string type = currentMatch.Groups[TypeRegexGroupName].Value;
                    bool isArray = currentMatch.Groups[IsArrayRegexGroupName].Success;
                    int elementCount = 0;
                    bool hasCount = int.TryParse(currentMatch.Groups[ElementCountRegexGroupName].Value, out elementCount);
                    string name = currentMatch.Groups[VariableNameRegexGroupName].Value;
                    string memberValue = currentMatch.Groups[ConstantValueRegexGroupName].Value;

                    MessageField newField = new MessageField(name, _primitiveTypeDictionary.ContainsKey(type) ? _primitiveTypeDictionary[type] : type, namespaceName, isArray, elementCount, memberValue);
                    result.Add(newField);
                }
            }
            return result;
        }
    }
}
