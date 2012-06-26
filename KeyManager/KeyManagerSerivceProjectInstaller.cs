using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace KeyManager
{
    [RunInstaller(true)]
    public partial class KeyManagerSerivceProjectInstaller : System.Configuration.Install.Installer
    {
        public KeyManagerSerivceProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
