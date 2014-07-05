using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TodoListImage
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

#if DEBUG
            (new TodoListImage()).MyOnStart();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else

            if (args.Count() > 0)
            {
                if (args[0].Equals("/install", StringComparison.CurrentCultureIgnoreCase))
                {
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                }
                else if (args[0].Equals("/uninstall", StringComparison.CurrentCultureIgnoreCase)) ;
                {
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new TodoListImage() 
                };
                ServiceBase.Run(ServicesToRun);
            }
#endif
        }
    }
}
