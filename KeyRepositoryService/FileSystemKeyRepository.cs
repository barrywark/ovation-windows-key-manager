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
            this.random = new Random();
        }


        public void WriteKey(string institution, string group, string product, string key)
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

        public const uint ENTROPY_BYTES = 16;

        private readonly Random random;

        /**
         * TODO, this needs to be written to a side-car file, and OvationEncryption needs to read it from there.
         * If the file doesn't exist, generate random bytes. If it does exist, read it from there.
         */
        public byte[] EntropyBytes(string institution, string group, string product)
        {

            var entropyFilePath = Path.Combine(RepositoryPath, EntropyFileName(institution, group, product));

            if(!File.Exists(entropyFilePath))
            {
                if (!Directory.Exists(RepositoryPath))
                {
                    Directory.CreateDirectory(RepositoryPath);
                }

                var bytes = new byte[ENTROPY_BYTES];
                random.NextBytes(bytes);
                
                using (var stream = new FileStream(entropyFilePath,
                    FileMode.Create,
                    FileAccess.Write))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(bytes);
                    }
                }
            }

            using(var stream = new FileStream(entropyFilePath,
                FileMode.Open,
                FileAccess.Read))
            {
                using(var reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int) stream.Length);
                }
            }

        }

        private static string KeyFileName(string institution, string group, string product)
        {
            return String.Format("{0}__{1}__{2}", institution, group, product);
        }

        private static string EntropyFileName(string institution, string group, string product)
        {
            return String.Format("entropy_{0}__{1}__{2}", institution, group, product);
        }
    }
}
