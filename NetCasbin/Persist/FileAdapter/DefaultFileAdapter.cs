using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCasbin.Persist.FileAdapter
{
    public class DefaultFileAdapter : IAdapter
    {
        protected readonly String filePath;
        private readonly Boolean _readOnly = false;
        private readonly StreamReader _byteArrayInputStream;
    

        public DefaultFileAdapter(String filePath)
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

        public void LoadPolicy(Model model)
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

        public void SavePolicy(Model model)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }
            if (filePath == null || "".Equals(filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            List<String> policy = new List<string>();
            policy.AddRange(GetModelPolicy(model, "p"));
            policy.AddRange(GetModelPolicy(model, "g"));

            SavePolicyFile(String.Join("\n", policy));
        }

        public void AddPolicy(String sec, String ptype, IList<String> rule)
        {
            throw new NotImplementedException("not implemented");
        }

        public void RemovePolicy(String sec, String ptype, IList<String> rule)
        {
            throw new NotImplementedException("not implemented");
        }

        public void RemoveFilteredPolicy(String sec, String ptype, int fieldIndex, params string[] fieldValues)
        {
            throw new NotImplementedException("not implemented");
        }

        private List<String> GetModelPolicy(Model model, String ptype)
        {
            List<String> policy = new List<string>();
            model.Model[ptype].ToList().ForEach(item =>
            {
                var k = item.Key;
                var v = item.Value;
                List<String> p = v.Policy.Select(x => $"{k}, {Util.ArrayToString(x)}").ToList();
                policy.AddRange(p);
            });
            return policy;
        }

        private void LoadPolicyData(Model model, Helper.LoadPolicyLineHandler<String, Model> handler, StreamReader inputStream)
        {
            while (!inputStream.EndOfStream)
            {
                var line = inputStream.ReadLine();
                handler(line, model);
            }
        }

        private void SavePolicyFile(String text)
        {
            File.WriteAllText(filePath, text, UTF8Encoding.UTF8);
        }


    }
}
