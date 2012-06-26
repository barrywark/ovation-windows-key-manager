using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Physion.Ovation.KeyRepositoryService
{
    public class KeyRepositoryTests
    {
        [Fact]
        public void ShouldWriteKeyToLocalAppStorage()
        {
            var repo = new FileSystemKeyRepository();

            const string institution = "Some Institution";
            const string group = "Some Group";
            const string product = "Some Product";

            const string key = "abc123";

            repo.WriteKey(institution, group, product, key);    

            Assert.Equal(key, ReadKey(repo, institution, group, product));
            
        }

        [Fact]
        public void ShouldStoreInPhysionOvationKeysDirectory()
        {
            const string path = "somePath";
            var repo = new FileSystemKeyRepository(path);

            Assert.Equal(repo.RepositoryPath, Path.Combine(path, "Physion", "Ovation", "keys"));
        }

        [Fact]
        public void ShouldComputeDifferentEntropyForDifferentInstitutionAndGroup()
        {
            const string institution = "some inst";
            const string group = "group";
            const string product = "Ovation";

            const string institution2 = "other inst";
            const string group2 = "group2";

            var repo = new FileSystemKeyRepository();

            Assert.NotEqual(repo.EntropyBytes(institution2, group2, product), 
                repo.EntropyBytes(institution, group, product));
        }

        [Fact]
        public void ShouldWriteEntropyToFile()
        {
            const string institution = "Some Institution";
            const string group = "Some Group";
            const string product = "Some Product";

            var repo = new FileSystemKeyRepository();

            var entropyBytes = repo.EntropyBytes(institution, group, product);

            var entropyFileName = String.Format("entropy_{0}__{1}__{2}", institution, group, product);

            using(var stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Physion", "Ovation", "keys", entropyFileName), 
                FileMode.Open, 
                FileAccess.Read)
                )
            {
                using(var reader = new BinaryReader(stream))
                {
                    var fileBytes = reader.ReadBytes((int) stream.Length);
                    Assert.Equal(entropyBytes, fileBytes);
                }
            }
        }


        private static string ReadKey(FileSystemKeyRepository repo, string institution, string group, string product)
        {
            var keyName = String.Format("key_{0}__{1}__{2}", institution, group, product);

            using(var stream = new FileStream(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Physion", "Ovation", "keys", keyName), 
                FileMode.Open, 
                FileAccess.Read))
            {
                using(var reader = new BinaryReader(stream))
                {
                    var encryptedBytes = reader.ReadBytes((int) stream.Length);
                    var bytes = ProtectedData.Unprotect(encryptedBytes, 
                        repo.EntropyBytes(institution, group, product), 
                        DataProtectionScope.CurrentUser);

                    return Encoding.UTF8.GetString(bytes);
                }
            }

        }
    }
}
