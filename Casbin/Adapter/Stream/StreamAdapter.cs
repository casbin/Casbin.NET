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
        protected readonly Stream _byteArrayInputStream;
        protected readonly Stream _byteArrayOutputStream;
        public StreamAdapter(Stream inputStream, Stream outputStream)
        {
            _byteArrayInputStream = inputStream;
            _byteArrayOutputStream = outputStream;
        }

        public void LoadPolicy(IPolicyStore store)
        {
            try
            {
                var streamReader = new StreamReader(_byteArrayInputStream);
                if (_byteArrayInputStream is not null)
                {
                    LoadPolicyData(store, streamReader);
                }
                streamReader.Dispose();
            }
            catch (Exception)
            {

            }
        }

        public async Task LoadPolicyAsync(IPolicyStore store)
        {
            try
            {
                var streamReader = new StreamReader(_byteArrayInputStream);
                if (_byteArrayInputStream is not null)
                {
                    await LoadPolicyDataAsync(store, streamReader);
                }
                streamReader.Dispose();
            }
            catch (Exception)
            {

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
            var streamWriter = new StreamWriter(_byteArrayOutputStream);
#if (NET6_0 || NET5_0 || NETCOREAPP3_1)
            streamWriter.Write(text.AsSpan());
#else
            streamWriter.Write(text.ToCharArray());
#endif
            streamWriter.Dispose();
        }

        private async Task SavePolicyFileAsync(string text)
        {
            var streamWriter = new StreamWriter(_byteArrayOutputStream);
            await streamWriter.WriteAsync(text);
            streamWriter.Dispose();
        }
    }
}
