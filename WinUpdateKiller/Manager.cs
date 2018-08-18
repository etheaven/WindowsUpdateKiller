using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinUpdateKiller
{
    class Manager
    {
        public Manager()
        {
           Task.Run(delegate
           {
               while (true)
               {
                   var serviceName = "wuauserv";
                   ServiceController service = new ServiceController("Windows Update");
                  
                   if ((service.Status.Equals(ServiceControllerStatus.Stopped)) ||

                       (service.Status.Equals(ServiceControllerStatus.StopPending)))
                   {
                       using (var m = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", serviceName)))
                       {
                           m.InvokeMethod("ChangeStartMode", new object[] { "Disabled" });
                       }
                   }
                   else service.Stop();
                   Thread.Sleep(1000);
               }
           });
        }
    }
}
