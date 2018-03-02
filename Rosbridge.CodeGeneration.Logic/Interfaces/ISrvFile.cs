namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    public interface ISrvFile
    {
        /// <summary>
        /// Service request message
        /// </summary>
        IMsgFile Request { get; }
        /// <summary>
        /// Service response message
        /// </summary>
        IMsgFile Response { get; }
    }
}
