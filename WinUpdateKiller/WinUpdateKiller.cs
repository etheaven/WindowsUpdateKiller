using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinUpdateKiller
{
    partial class WinUpdateKiller : BaseServiceProgram
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Manager MyManager { get; set; }

        public WinUpdateKiller():base(System.Reflection.Assembly.GetEntryAssembly().GetName().Name)
        {
            InitializeComponent();
        }

        public WinUpdateKiller(string name ):base(name)
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                MyManager = new Manager();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                OnStop();
            }
            finally
            {
                log.Info("OnServiceStarting Stop");
            }
        }

        
        protected override void OnStop()
        {
        }

        public override void OnServiceStarting()
        {
            Console.WriteLine("Starting");
        }

        public override void OnServiceStarted()
        {
        }

        public override void OnServiceStoping()
        {
        }

        public override void OnServiceStopped()
        {
        }
        
    }
}
