namespace RosbridgeMessagesCodeGenerationLogic.Interfaces
{
    using RosbridgeMessagesCodeGenerationLogic.BaseClasses;
    using System.Collections.Generic;

    public interface IYAMLParser
    {
        ISet<MessageField> YAMLStringToMessageFieldSet(string yamlString);
    }
}
