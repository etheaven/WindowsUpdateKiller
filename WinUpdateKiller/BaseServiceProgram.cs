using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WinUpdateKiller
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public long dwServiceType;
        public ServiceState dwCurrentState;
        public long dwControlsAccepted;
        public long dwWin32ExitCode;
        public long dwServiceSpecificExitCode;
        public long dwCheckPoint;
        public long dwWaitHint;
    };
    public abstract class BaseServiceProgram : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        string m_serviceName = "";
        bool stop = false;
        public BaseServiceProgram(string serviceName)
        {
            m_serviceName = serviceName;
        }
        
        public static bool ParseCommandLine(string[] args, Assembly assembly)
        {
            try
            {
                String sCurrent = "";
                var enumerator = args.GetEnumerator();
                bool bChangedSettings = false;
                string serviceName = "";
                string assInfo = assembly.FullName;
                var splitName = assInfo.Split(',');
                if (splitName.Length != 0)
                    serviceName = "WinUpdateKiller";
                while (enumerator.MoveNext())
                {
                    sCurrent = enumerator.Current.ToString();
                    if (sCurrent.Length < 2 || sCurrent[0] != '-') //ocakavame parameter zadany omlckou a za nim nejaky znak
                    {
                        continue;
                    }
                    switch (sCurrent)
                    {
                        case "-install":
                            {
                                try
                                {
                                    ManagedInstallerClass.InstallHelper(new string[] { assembly.Location });
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                                try
                                {
                                    SetRecoveryOptions(serviceName);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);

                                }
                                return true;
                            }
                        case "-remove":
                            {
                                try
                                {
                                    ManagedInstallerClass.InstallHelper(new string[] { "/u", assembly.Location });
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);

                                }
                                return true;
                            }
                        case "-start":
                            {
                                StartService(serviceName);
                                return true;
                            }
                        case "-stop":
                            StopService(serviceName);
                            return true;
                        default:
                            break;


                    }
                }
                
                if (bChangedSettings)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;

        }

        static void SetRecoveryOptions(string serviceName)
        {
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                

                // tell Windows that the service should restart if it fails
                startInfo.Arguments = string.Format("failure \"{0}\"  actions= restart/0/restart/0/restart/0 reset= 86400", serviceName);

                process.Start();
               process.WaitForExit();

                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
                throw new InvalidOperationException();
        }

        static void StartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        static void StopService(string serviceName)
        {
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(30000);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Update the service state to Start Pending.
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
                serviceStatus.dwWaitHint = 100000;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                OnServiceStarting();


                // Update the service state to Running.
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
                OnServiceStarted();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        protected ManualResetEvent m_shutdownEvent2;
        protected override void OnStop()
        {
            try
            {

                // Update the service state to Start Pending.
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
                serviceStatus.dwWaitHint = 100000;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                stop = true;

                OnServiceStoping();

                //listenThread?.Abort();

                //// Update the service state to Running.
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
                OnServiceStopped();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        public void TestStartupAndStop(string[] args)
        {
            try
            {
                OnStart(args);
                Console.WriteLine("Press any key to stop service.");
                Console.ReadLine();
                OnStop();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        

        public abstract void OnServiceStarting();

        public abstract void OnServiceStarted();

        public abstract void OnServiceStoping();

        public abstract void OnServiceStopped();

        public delegate void OnNewClientConnection(object client);
        protected OnNewClientConnection m_newClientDelegate;

    }
}
