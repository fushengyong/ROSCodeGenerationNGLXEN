namespace RosbridgeClientV2_0.Enums
{
    public enum StatusMessageLevel
    {
        none = 0,
        /// <summary>
        /// Whenever a user sends a message that is invalid or requests something that does not exist (ie. Sending an incorrect opcode or publishing to a topic that doesn't exist)
        /// </summary>
        error = 1,
        /// <summary>
        /// Error, plus, whenever a user does something that may succeed but the user has still done something incorrectly (ie. Providing a partially-complete published message)
        /// </summary>
        warning = 2,
        /// <summary>
        /// Warning, plus messages indicating success of various operations
        /// </summary>
        info = 3
    }
}
