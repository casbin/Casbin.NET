using NetCasbin.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCasbin.Persist.FileAdapter
{
    public class DefaultFileAdapter : IAdapter
    {
        protected readonly string filePath;
        private readonly Boolean _readOnly = false;
        private readonly StreamReader _byteArrayInputStream;

        public DefaultFileAdapter(string filePath)
        {
            this.filePath = filePath;
        }

        public DefaultFileAdapter(Stream inputStream)
        {
            _readOnly = true;
            try
            {
                _byteArrayInputStream = new StreamReader(inputStream);
            }
            catch (IOException e)
            {
                throw new Exception("File adapter init error");
            }
        }

        public void LoadPolicy(Model.Model model)
        {
            if (filePath != null && !"".Equals(filePath))
            {
                using (var sr = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    LoadPolicyData(model, Helper.LoadPolicyLine, sr);
                }
            }

            if (_byteArrayInputStream != null)
            {
                LoadPolicyData(model, Helper.LoadPolicyLine, _byteArrayInputStream);
            }
        }

        public Task LoadPolicyAsync(Model.Model model)
        {
            throw new NotImplementedException();
        }

        public void SavePolicy(Model.Model model)
        {
            var policy = ConvertToPolicy(model);
            SavePolicyFile(string.Join("\n", policy));
        }

        public async Task SavePolicyAsync(Model.Model model)
        {
            var policy = ConvertToPolicy(model);
            await SavePolicyFileAsync(string.Join("\n", policy));
        }

        public void AddPolicy(string sec, string ptype, IList<string> rule)
        {
            throw new NotImplementedException("not implemented");
        }

        public Task AddPolicyAsync(string sec, string ptype, IList<string> rule)
        {
            throw new NotImplementedException();
        }

        public void RemovePolicy(string sec, string ptype, IList<string> rule)
        {
            throw new NotImplementedException("not implemented");
        }

        public void RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            throw new NotImplementedException("not implemented");
        }

        private List<string> GetModelPolicy(Model.Model model, string ptype)
        {
            List<string> policy = new List<string>();
            model.Model[ptype].ToList().ForEach(item =>
            {
                var k = item.Key;
                var v = item.Value;
                List<string> p = v.Policy.Select(x => $"{k}, {Utility.ArrayToString(x)}").ToList();
                policy.AddRange(p);
            });
            return policy;
        }

        private void LoadPolicyData(Model.Model model, Helper.LoadPolicyLineHandler<string, Model.Model> handler, StreamReader inputStream)
        {
            while (!inputStream.EndOfStream)
            {
                var line = inputStream.ReadLine();
                handler(line, model);
            }
        }

        private IList<string> ConvertToPolicy(Model.Model model)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }
            if (filePath == null || "".Equals(filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            List<string> policy = new List<string>();
            policy.AddRange(GetModelPolicy(model, "p"));
            if (model.Model.ContainsKey("g"))
            {
                policy.AddRange(GetModelPolicy(model, "g"));
            }
            return policy;
        }

        private void SavePolicyFile(string text)
        {
            File.WriteAllText(filePath, text, Encoding.UTF8);
        }

        private async Task SavePolicyFileAsync(string text)
        {
            text = text ?? "";
            var content = Encoding.UTF8.GetBytes(text);
            using (var fs = new FileStream(
                   path: filePath,
                   mode: FileMode.Create,
                   access: FileAccess.Write,
                   share: FileShare.None,
                   bufferSize: 4096,
                   useAsync: true))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }
    }
}
