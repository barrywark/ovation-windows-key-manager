using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace Physion.Ovation.KeyRepositoryService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class FileSystemKeyRepository : IKeyRepository
    {
        public string RepositoryPath { get; private set; }

        public FileSystemKeyRepository()
            : this(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
        {
            
        }

        public FileSystemKeyRepository(string repositoryFolder)
        {
            this.RepositoryPath = Path.Combine(repositoryFolder, "Physion", "Ovation", "keys");
        }


        public void WriteKey(string institution, string @group, string product, string key)
        {
            var encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(key),
                EntropyBytes(institution, group, product),
                DataProtectionScope.CurrentUser);

            if(!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }

            using(var stream = new FileStream(Path.Combine(RepositoryPath, KeyFileName(institution, group, product)), 
                FileMode.Create, 
                FileAccess.Write))
            {
                using(var writer = new BinaryWriter(stream))
                {
                    writer.Write(encryptedBytes);
                }
            }
        }

        public static byte[] EntropyBytes(string institution, string group, string product)
        {
            return BitConverter.GetBytes(KeyFileName(institution, group, product).GetHashCode());

        }

        private static string KeyFileName(string institution, string group, string product)
        {
            return String.Format("key_{0}__{1}__{2}", institution, group, product);
        }
    }
}
