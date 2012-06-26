﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Physion.Ovation.KeyRepositoryService;
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

            Assert.Equal(key, ReadKey(institution, group, product));
            
        }

        [Fact]
        public void ShouldStoreInPhysionOvationKeysDirectory()
        {
            const string path = "somePath";
            var repo = new FileSystemKeyRepository(path);

            Assert.Equal(repo.RepositoryPath, Path.Combine(path, "Physion", "Ovation", "keys"));
        }

        [Fact]
        public void ShouldComputeSaltFromKeyName()
        {
            const string institution = "some inst";
            const string group = "group";
            const string product = "Ovation";

            var keyName = String.Format("key_{0}__{1}__{2}", institution, group, product);

            Assert.Equal(BitConverter.GetBytes(keyName.GetHashCode()), FileSystemKeyRepository.EntropyBytes(institution, group, product));
        }

        private string ReadKey(string institution, string group, string product)
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
                        FileSystemKeyRepository.EntropyBytes(institution, group, product), 
                        DataProtectionScope.CurrentUser);

                    return Encoding.UTF8.GetString(bytes);
                }
            }

        }
    }
}
