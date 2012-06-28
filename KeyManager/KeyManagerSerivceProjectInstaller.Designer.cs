namespace KeyManager
{
    partial class KeyManagerSerivceProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.keyManagerSerivceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.keyManagerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // keyManagerSerivceProcessInstaller
            // 
            this.keyManagerSerivceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.keyManagerSerivceProcessInstaller.Password = null;
            this.keyManagerSerivceProcessInstaller.Username = null;
            // 
            // keyManagerServiceInstaller
            // 
            this.keyManagerServiceInstaller.ServiceName = "OvationKeyManager";
            // 
            // KeyManagerSerivceProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.keyManagerSerivceProcessInstaller,
            this.keyManagerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller keyManagerSerivceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller keyManagerServiceInstaller;
    }
}