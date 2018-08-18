using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WinUpdateKiller
{
    static class Program
    {
        static void Main(string[] args)
        {

            //https://stackoverflow.com/questions/1472498/wpf-global-exception-handler/1472562#1472562
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException; 
            

            var ass = Assembly.GetExecutingAssembly();
            if (BaseServiceProgram.ParseCommandLine(args, ass)) 
            {
            }
            else
            {
                string serviceName = "";
                string assInfo = Assembly.GetExecutingAssembly().FullName;
                var splitName = assInfo.Split(',');
                if (splitName.Length != 0)
                    serviceName = splitName[0];
                else
                    return;

                if (Environment.UserInteractive)
                {
                    //launch as exe
                    
                    Process[] pname = Process.GetProcessesByName("WinUpdateKiller");
                    if (pname.Length > 1)
                        Console.WriteLine("Process is already running");
                    else
                    {
                        var serv = new WinUpdateKiller(serviceName);
                        serv.TestStartupAndStop(args);
                    }

                }
                else
                {
                    // toto sa vykonava ked je exac spusteny ako sluzba
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new WinUpdateKiller(serviceName) };
                    ServiceBase.Run(ServicesToRun);
                }
            }
        }


        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Console.WriteLine(unhandledExceptionEventArgs.ToString());
        }
    }
}
    

