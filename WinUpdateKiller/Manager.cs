using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
           Task.Factory.StartNew(delegate
           {
               List<string> servicenames = new List<string>(){"wuauserv", "UsoSvc", "WaaSMedicSvc", "WSearch" };
               if(File.Exists("services.cfg"))
               using (var file = new System.IO.StreamReader("services.cfg"))
               {
                   var split = file.ReadLine().Split(';');
                   servicenames = split.ToList();
               }
               else
               {
                   using (var file = new StreamWriter("services.cfg"))
                   {
                       file.WriteLine($"wuauserv;UsoSvc;WaaSMedicSvc;WSearch");
                   }
               }
               while (true)
               {
                   //var serviceName = "wuauserv";//"Windows Update"
                   foreach (string serviceName in servicenames)
                   {
                       ServiceController service = new ServiceController(serviceName);
                  
                       if ((service.Status.Equals(ServiceControllerStatus.Stopped)) ||

                           (service.Status.Equals(ServiceControllerStatus.StopPending)))
                       {
                           using (var m = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", serviceName)))
                           {
                               try
                               {
                                   m.InvokeMethod("ChangeStartMode", new object[] { "Disabled" });
                               }
                               catch (Exception e)
                               {
                                   
                               }
                           }
                       }
                       else
                           try
                           {
                               service.Stop();
                           }
                           catch (Exception e)
                           {

                           }
                   }
                   Thread.Sleep(1000);
               }
           });
        }
    }
}
