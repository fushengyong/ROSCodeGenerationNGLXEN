namespace RosbridgeMessagesCodeGenerationLogic.Interfaces
{
    using RosbridgeMessagesCodeGenerationLogic.BaseClasses;

    public interface IYAMLParser
    {
        void SetMsgFileFieldsFromYAMLString(string yamlString, MsgFile msgFile);
    }
}
