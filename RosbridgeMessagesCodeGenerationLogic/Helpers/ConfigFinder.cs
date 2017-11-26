namespace RosbridgeMessagesCodeGenerationLogic.Helpers
{
    using EnvDTE;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class ConfigFinder
    {
        public static System.Configuration.Configuration GetConfigFile(IServiceProvider serviceProvider, string templateFile)
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
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (Regex.IsMatch(item.Name, "(app|web).config", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    configFilePath = item.get_FileNames(0);
                }
            }

            if (null == configFilePath)
            {
                throw new FileNotFoundException("Configuration file cannot be found!");
            }

            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = configFilePath;
            return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }
    }
}
