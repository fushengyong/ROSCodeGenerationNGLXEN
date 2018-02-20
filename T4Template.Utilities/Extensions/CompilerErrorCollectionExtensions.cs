namespace T4Template.Utilities.Extensions
{
    using System;
    using System.CodeDom.Compiler;

    public static class CompilerErrorCollectionExtensions
    {
        /// <summary>
        /// Indicates that the collection has an element that satisfies the predicate
        /// </summary>
        /// <param name="errorCollection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Any(this CompilerErrorCollection errorCollection, Func<CompilerError, bool> predicate)
        {
            foreach (CompilerError error in errorCollection)
            {
                if (predicate(error))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
