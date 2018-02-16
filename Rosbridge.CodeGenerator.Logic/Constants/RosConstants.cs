namespace Rosbridge.CodeGenerator.Logic.Constants
{
    using System.Collections.Generic;

    public class RosConstants
    {
        public class FileExtensions
        {
            public const string MSG_FILE_EXTENSION = "msg";
            public const string SRV_FILE_EXTENSION = "srv";
        }

        public class MessageTypes
        {
            public const string HEADER_TYPE = "Header";
            public const string CUSTOM_TIME_PRIMITIVE_TYPE = "TimeData";
            public static readonly ISet<string> CustomTimePrimitiveTypeSet = new HashSet<string> {
                "Time",
                "Duration" };
            public static readonly Dictionary<string, string> PrimitiveTypeDictionary = new Dictionary<string, string> {
            {"float64", "double"},
            {"uint64", "ulong"},
            {"uint32", "uint"},
            {"uint16", "ushort"},
            {"uint8", "byte"},
            {"int64", "long"},
            {"int32", "int"},
            {"int16", "short"},
            {"int8", "sbyte"},
            {"byte", "byte"},
            {"bool", "bool"},
            {"char", "char"},
            {"string", "string"},
            {"float32", "Single"},
            {"time", "Time"},
            {"duration", "Duration"}};
        }
    }
}
