namespace Rosbridge.CodeGeneration.Logic.Helpers
{
    using EnvDTE;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Find app.config in the given template file's project
    /// </summary>
    public static class ConfigFinder
    {
        public static string GetConfigFilePath(IServiceProvider serviceProvider, string templateFile)
        {
            if (null == serviceProvider)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (null == templateFile)
            {
                throw new ArgumentNullException(nameof(templateFile));
            }

            if (string.Empty == templateFile)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(templateFile));
            }

            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));
            Project project = dte.Solution.FindProjectItem(templateFile).ContainingProject;

            string configFilePath = null;
            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                if (Regex.IsMatch(projectItem.Name, "(app|web).config", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    configFilePath = projectItem.get_FileNames(0);
                }
            }

            if (null == configFilePath)
            {
                throw new FileNotFoundException("Configuration file cannot be found!");
            }

            return configFilePath;
        }
    }
}
