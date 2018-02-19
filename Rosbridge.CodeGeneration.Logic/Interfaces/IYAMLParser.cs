namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    using Rosbridge.CodeGeneration.Logic.BaseClasses;

    public interface IYAMLParser
    {
        void SetMsgFileFieldsFromYAMLString(string yamlString, MsgFile msgFile);
    }
}
