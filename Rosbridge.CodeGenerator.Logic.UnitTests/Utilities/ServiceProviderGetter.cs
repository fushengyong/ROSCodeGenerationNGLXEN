namespace Rosbridge.CodeGenerator.Logic.UnitTests.Utilities
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using System;

    public class ServiceProviderGetter
    {
        public static IServiceProvider GetServiceProvider(string visualStudioProgID)
        {
            Type dteType = Type.GetTypeFromProgID(visualStudioProgID, true);
            DTE dte = (DTE)Activator.CreateInstance(dteType);
            IServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
            return serviceProvider;
        }
    }
}
