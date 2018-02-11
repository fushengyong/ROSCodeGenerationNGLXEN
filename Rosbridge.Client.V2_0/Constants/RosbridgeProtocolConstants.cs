namespace Rosbridge.Client.V2_0.Constants
{
    /// <summary>
    /// Rosbridge protocol constants for Rosbridge 2.0
    /// </summary>
    public static class RosbridgeProtocolConstants
    {
        public static class MessageTransformation
        {
            public const string FRAGMENT = "fragment";
            public const string PNG = "png";
        }

        public static class StatusMessage
        {
            public const string SET_STATUS_LEVEL = "set_level";
            public const string STATUS = "status";
        }

        public static class Authentication
        {
            public const string AUTHENTICATE = "auth";
        }

        public static class RosMessages
        {
            public const string ADVERTISE = "advertise";
            public const string UNADVERTISE = "unadvertise";
            public const string PUBLISH = "publish";
            public const string SUBSCRIBE = "subscribe";
            public const string UNSUBSCRIBE = "unsubscribe";
            public const string CALL_SERVICE = "call_service";
            public const string SERVICE_RESPONSE = "service_response";
        }
    }
}
