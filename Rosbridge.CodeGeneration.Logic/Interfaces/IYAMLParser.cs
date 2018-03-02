namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    /// <summary>
    /// YAML string parser
    /// </summary>
    public interface IYAMLParser
    {
        /// <summary>
        /// Parse YAML string into MsgFile fields
        /// </summary>
        /// <param name="yamlString"></param>
        /// <param name="msgFile"></param>
        void SetMsgFileFieldsFromYAMLString(string yamlString, IMsgFile msgFile);
    }
}
