namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    using Rosbridge.CodeGeneration.Logic.BaseClasses;
    using System.Collections.Generic;

    public interface IMsgFile : IRosFile
    {
        /// <summary>
        /// Message dependeny collection
        /// </summary>
        ISet<RosType> DependencySet { get; }
        /// <summary>
        /// Message field collection
        /// </summary>
        ISet<MessageField> FieldSet { get; }
        /// <summary>
        /// Message constant field collection
        /// </summary>
        ISet<MessageField> ConstantFieldSet { get; }
        /// <summary>
        /// Message array field collection
        /// </summary>
        ISet<MessageField> ArrayFieldSet { get; }
    }
}