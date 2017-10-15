namespace RosbridgeClientWPF
{
    using RosbridgeClientCommon.Attributes;

    [RosMessageType("geometry_msgs/Twist")]
    public class Twist
    {
        public Vector3 linear { get; set; }
        public Vector3 angular { get; set; }
    }

    public class Vector3
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }
}
