﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Util;

namespace Casbin.Adapter.File
{
    public class FileAdapter : IAdapter
    {
        protected readonly string filePath;
        private readonly bool _readOnly;
        private readonly StreamReader _byteArrayInputStream;

        public FileAdapter(string filePath)
        {
            this.filePath = filePath;
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

        public void LoadPolicy(IModel model, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                using (var sr = new StreamReader(new FileStream(
                    filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    LoadPolicyData(model, Helper.LoadPolicyLine, sr);
                }
            }

            if (_byteArrayInputStream != null)
            {
                LoadPolicyData(model, Helper.LoadPolicyLine, _byteArrayInputStream);
            }
        }

        public async Task LoadPolicyAsync(IModel model, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                using (var sr = new StreamReader(new FileStream(
                    filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    await LoadPolicyDataAsync(model, Helper.LoadPolicyLine, sr);
                }
            }

            if (_byteArrayInputStream != null)
            {
                await LoadPolicyDataAsync(model, Helper.LoadPolicyLine, _byteArrayInputStream);
            }
        }

        public void SavePolicy(IModel model, CancellationToken cancellationToken = default)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(model);
            SavePolicyFile(string.Join("\n", policy));
        }

        public async Task SavePolicyAsync(IModel model, CancellationToken cancellationToken = default)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(model);
            await SavePolicyFileAsync(string.Join("\n", policy));
        }

        private static IEnumerable<string> GetModelPolicy(IModel model, string ptype)
        {
            var policy = new List<string>();
            foreach (var pair in model.Sections[ptype])
            {
                string key = pair.Key;
                Assertion value = pair.Value;
                policy.AddRange(value.Policy.Select(p => $"{key}, {Utility.RuleToString(p)}"));
            }
            return policy;
        }

        private static void LoadPolicyData(IModel model, Helper.LoadPolicyLineHandler<string, IModel> handler, StreamReader inputStream)
        {
            while (!inputStream.EndOfStream)
            {
                string line = inputStream.ReadLine();
                handler(line, model);
            }
        }

        private async Task LoadPolicyDataAsync(IModel model, Helper.LoadPolicyLineHandler<string, IModel> handler, StreamReader inputStream)
        {
            while (!inputStream.EndOfStream)
            {
                string line = await inputStream.ReadLineAsync();
                handler(line, model);
            }
        }

        private IEnumerable<string> ConvertToPolicyStrings(IModel model)
        {
            var policy = new List<string>();
            policy.AddRange(GetModelPolicy(model, PermConstants.DefaultPolicyType));
            if (model.Sections.ContainsKey(PermConstants.Section.RoleSection))
            {
                policy.AddRange(GetModelPolicy(model, PermConstants.Section.RoleSection));
            }
            return policy;
        }

        private void SavePolicyFile(string text)
        {
            System.IO.File.WriteAllText(filePath, text, Encoding.UTF8);
        }

        private async Task SavePolicyFileAsync(string text)
        {
            text = text ?? string.Empty;
            var content = Encoding.UTF8.GetBytes(text);
            using (var fs = new FileStream(
                   filePath, FileMode.Create, FileAccess.Write,
                   FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }

        public void AddPolicies(string sec, string ptype, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AddPoliciesAsync(string sec, string ptype, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void RemovePolicies(string sec, string ptype, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task RemovePoliciesAsync(string sec, string ptype, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void AddPolicy(string sec, string ptype, IEnumerable<string> rule, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AddPolicyAsync(string sec, string ptype, IEnumerable<string> rule, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void RemovePolicy(string sec, string ptype, IEnumerable<string> rule, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task RemovePolicyAsync(string sec, string ptype, IEnumerable<string> rule, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues) => throw new NotImplementedException();
        public Task RemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex, params string[] fieldValues) => throw new NotImplementedException();
    }
}
