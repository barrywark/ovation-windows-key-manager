using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using KeyManager;
using Xunit;
using KeyManagerWindowsServiceTests.ServiceProxy;

namespace KeyManagerWindowsServiceTests
{
    public class KeyManagerAppTests
    {
        [Fact]
        public void KeyRepositoryAuthorizationManager()
        {
            var app = new KeyRepositoryApp();
            try
            {
                app.Start();
                Assert.Equal(app.ServiceHost.Authorization.ServiceAuthorizationManager.GetType().FullName,
                             "KeyManager.KeyRepositoryAuthorizationManager");
            }
            finally
            {
                app.Stop();
            }
        }

        [Fact]
        public void ShouldWriteKey()
        {
            var app = new KeyRepositoryApp(new ServiceAuthorizationManager());
            try
            {
                app.Start();

                var proxy = new KeyRepositoryClient();

                const string institution = "ShouldWriteKeyInstitution";
                const string group = "ShouldWriteKeyGroup";
                const string product = "KeyManager";


                var keyName = String.Format("key_{0}__{1}__{2}", institution, group, product);
                var keyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Physion", "Ovation", "keys", keyName);

                if(File.Exists(keyPath))
                    File.Delete(keyPath);

                proxy.WriteKey(institution, group, product, "some key");

                Assert.True(File.Exists(keyPath));

                if (File.Exists(keyPath))
                    File.Delete(keyPath);
            }
            finally
            {
                app.Stop();
            }
        }
    }
}
