namespace Rosbridge.CodeGeneration.Logic.UnitTests.Extensions
{
    using System;
    using System.CodeDom.Compiler;

    public static class CompilerErrorCollectionExtensions
    {
        public static bool ContainsException<TException>(this CompilerErrorCollection errorCollection, string exceptionThrownFile = null) where TException : Exception
        {
            Type type = typeof(TException);

            foreach (CompilerError error in errorCollection)
            {
                if (error.ErrorText.Contains(type.FullName))
                {
                    if (exceptionThrownFile != null)
                    {
                        if (error.FileName.Equals(exceptionThrownFile))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
