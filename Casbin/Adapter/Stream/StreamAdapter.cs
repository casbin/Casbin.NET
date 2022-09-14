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
    public class StreamAdapter : IEpochAdapter
    {
        protected readonly StreamReader _byteArrayInputStream;
        protected readonly StreamWriter _byteArrayOutputStream;
        public StreamAdapter(string filePath) : this(new FileStream(
                    filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                    new FileStream(
                    filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {

        }

        public StreamAdapter(Stream inputStream)
        {
            try
            {
                _byteArrayInputStream = new StreamReader(inputStream);
            }
            catch (IOException e)
            {
                throw new IOException("File adapter init error", e);
            }
        }

        public StreamAdapter(Stream inputStream, Stream outputStream)
        {
            try
            {
                _byteArrayInputStream = new StreamReader(inputStream);
                _byteArrayOutputStream = new StreamWriter(outputStream);
            }
            catch (IOException e)
            {
                throw new IOException("File adapter init error", e);
            }
        }

        public void LoadPolicy(IPolicyStore model)
        {
            if (_byteArrayInputStream is not null)
            {
                LoadPolicyData(model, _byteArrayInputStream);
            }
        }

        public async Task LoadPolicyAsync(IPolicyStore store)
        {
            if (_byteArrayInputStream is not null)
            {
                await LoadPolicyDataAsync(store, _byteArrayInputStream);
            }
        }

        public void SavePolicy(IPolicyStore store)
        {
            if (_byteArrayOutputStream is null)
            {
                throw new Exception("Store file can not write, because use outputStream has not been set.");
            }

            var policy = ConvertToPolicyStrings(store);
            SavePolicyFile(string.Join("\n", policy));
        }

        public Task SavePolicyAsync(IPolicyStore store)
        {
            if (_byteArrayOutputStream is null)
            {
                throw new Exception("Store file can not write, because use outputStream has not been set.");
            }

            var policy = ConvertToPolicyStrings(store);
            return SavePolicyFileAsync(string.Join("\n", policy));
        }

        private static IEnumerable<string> GetModelPolicy(IPolicyStore store, string section)
        {
            var policy = new List<string>();
            foreach (var kv in store.GetPolicyAllType(section))
            {
                var key = kv.Key;
                var value = kv.Value;
                policy.AddRange(value.Select(p => $"{key}, {p.ToText()}"));
            }

            return policy;
        }

        private static void LoadPolicyData(IPolicyStore store, StreamReader inputStream)
        {
            if (inputStream.EndOfStream is true)
            {
                inputStream.BaseStream.Position = 0;
            }
            while (inputStream.EndOfStream is false)
            {
                string line = inputStream.ReadLine();
                store.TryLoadPolicyLine(line);
            }
        }

        private static async Task LoadPolicyDataAsync(IPolicyStore store, StreamReader inputStream)
        {
            if (inputStream.EndOfStream is true)
            {
                inputStream.BaseStream.Position = 0;
            }
            while (inputStream.EndOfStream is false)
            {
                string line = await inputStream.ReadLineAsync();
                store.TryLoadPolicyLine(line);
            }
        }

        private static IEnumerable<string> ConvertToPolicyStrings(IPolicyStore store)
        {
            var policy = new List<string>();
            if (store.ContainsNodes(PermConstants.Section.PolicySection))
            {
                policy.AddRange(GetModelPolicy(store, PermConstants.Section.PolicySection));
            }

            if (store.ContainsNodes(PermConstants.Section.RoleSection))
            {
                policy.AddRange(GetModelPolicy(store, PermConstants.Section.RoleSection));
            }

            return policy;
        }

        private void SavePolicyFile(string text)
        {
#if (NET6_0 || NET5_0 || NETCOREAPP3_1)
            _byteArrayOutputStream.Write(text.AsSpan());
#else
            _byteArrayOutputStream.Write(text.ToCharArray());
#endif
        }

        private async Task SavePolicyFileAsync(string text)
        {
            await _byteArrayOutputStream.WriteAsync(text);
        }
    }
}
