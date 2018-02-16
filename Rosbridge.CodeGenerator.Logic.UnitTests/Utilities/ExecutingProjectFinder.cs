namespace Rosbridge.CodeGenerator.Logic.UnitTests.Utilities
{
    using System;
    using System.IO;
    using System.Reflection;

    public class ExecutingProjectFinder
    {
        public static DirectoryInfo GetExecutingProjectDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            DirectoryInfo binDirectory = new DirectoryInfo(Path.GetDirectoryName(path));
            SetToBinDirectory(ref binDirectory);
            return binDirectory?.Parent;
        }

        private static void SetToBinDirectory(ref DirectoryInfo directory)
        {
            while (directory != null && directory.Name != "bin")
            {
                directory = directory.Parent;
            }
        }
    }
}
