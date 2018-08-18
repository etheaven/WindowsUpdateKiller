using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace WinUpdateKiller
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
        
        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            serviceInstaller1.Description = "Windows update killer, because there is no other way.";
            serviceProcessInstaller1.Password = null;
            serviceProcessInstaller1.Username = null;
            
        }
    }
}
