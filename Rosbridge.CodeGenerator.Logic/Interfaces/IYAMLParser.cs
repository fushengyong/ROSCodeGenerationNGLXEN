namespace Rosbridge.CodeGenerator.Logic.Interfaces
{
    using Rosbridge.CodeGenerator.Logic.BaseClasses;

    public interface IYAMLParser
    {
        void SetMsgFileFieldsFromYAMLString(string yamlString, MsgFile msgFile);
    }
}
