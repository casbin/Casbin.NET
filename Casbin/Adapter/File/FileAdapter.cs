using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.File
{
    public class FileAdapter : IEpochAdapter
    {
        private readonly StreamReader _byteArrayInputStream;
        private readonly bool _readOnly;
        protected readonly string FilePath;

        public FileAdapter(string filePath)
        {
            FilePath = filePath;
        }

        public FileAdapter(Stream inputStream)
        {
            _readOnly = true;
            try
            {
                _byteArrayInputStream = new StreamReader(inputStream);
            }
            catch (IOException e)
            {
                throw new IOException("File adapter init error", e);
            }
        }

        public void LoadPolicy(IPolicyStore model)
        {
            if (string.IsNullOrWhiteSpace(FilePath) is false)
            {
                using var sr = new StreamReader(new FileStream(
                    FilePath, FileMode.Open, FileAccess.Read, FileShare.Read));
                LoadPolicyData(model, sr);
            }

            if (_byteArrayInputStream is not null)
            {
                LoadPolicyData(model, _byteArrayInputStream);
            }
        }

        public async Task LoadPolicyAsync(IPolicyStore store)
        {
            if (string.IsNullOrWhiteSpace(FilePath) is false)
            {
                using var sr = new StreamReader(new FileStream(
                    FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                await LoadPolicyDataAsync(store, sr);
                Debug.WriteLine("xxx");
            }

            if (_byteArrayInputStream is not null)
            {
                await LoadPolicyDataAsync(store, _byteArrayInputStream);
            }
        }

        public void SavePolicy(IPolicyStore store)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Store file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(store);
            SavePolicyFile(string.Join("\n", policy));
        }

        public Task SavePolicyAsync(IPolicyStore store)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Store file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(store);
            return SavePolicyFileAsync(string.Join("\n", policy));
        }

        private static IEnumerable<string> GetModelPolicy(IPolicyStore store, string section)
        {
            var policy = new List<string>();
            foreach (var kv in store.Sections[section])
            {
                string key = kv.Key;
                Assertion value = kv.Value;
                policy.AddRange(value.Policy.Select(p => $"{key}, {p.ToText()}"));
            }

            return policy;
        }

        private static void LoadPolicyData(IPolicyStore store, StreamReader inputStream)
        {
            while (inputStream.EndOfStream is false)
            {
                string line = inputStream.ReadLine();
                store.TryLoadPolicyLine(line);
            }
        }

        private static async Task LoadPolicyDataAsync(IPolicyStore store, StreamReader inputStream)
        {
            while (inputStream.EndOfStream is false)
            {
                string line = await inputStream.ReadLineAsync();
                store.TryLoadPolicyLine(line);
            }
        }

        private static IEnumerable<string> ConvertToPolicyStrings(IPolicyStore store)
        {
            var policy = new List<string>();
            policy.AddRange(GetModelPolicy(store, PermConstants.DefaultPolicyType));
            if (store.Sections.ContainsKey(PermConstants.Section.RoleSection))
            {
                policy.AddRange(GetModelPolicy(store, PermConstants.Section.RoleSection));
            }

            return policy;
        }

        private void SavePolicyFile(string text)
        {
            System.IO.File.WriteAllText(FilePath, text, Encoding.UTF8);
        }

        private async Task SavePolicyFileAsync(string text)
        {
            text = text ?? string.Empty;
            byte[] content = Encoding.UTF8.GetBytes(text);
            using var fs = new FileStream(
                FilePath, FileMode.Create, FileAccess.Write,
                FileShare.None, bufferSize: 4096, useAsync: true);
            await fs.WriteAsync(content, 0, content.Length);
        }
    }
}
