using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCasbin.Persist.FileAdapter
{
    public class DefaultFilteredAdapter : DefaultFileAdapter, IAdapter, IFilteredAdapter
    {
        private bool _filtered;

        public Boolean IsFiltered => _filtered;
        public DefaultFilteredAdapter(string filePath) : base(filePath)
        {
        }

        public DefaultFilteredAdapter(Stream inputStream) : base(inputStream)
        {
        }

        public void LoadFilteredPolicy(Model model, Filter filter)
        {
            if (filter == null)
            {
                this.LoadPolicy(model);
                return;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            this.LoadFilteredPolicyFile(model, filter, Helper.LoadPolicyLine);
            this._filtered = true;
        }

        private void LoadFilteredPolicyFile(Model model, Filter filter, Action<string, Model> handler)
        {
            var reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (string.IsNullOrEmpty(line) || FilterLine(line, filter))
                {
                    return;
                }
                handler(line, model);
            }
        }

        private static Boolean FilterLine(string line, Filter filter)
        {
            if (filter == null)
            {
                return false;
            }

            var p = line.Split(',');
            if (p.Length == 0)
            {
                return true;
            }

            IEnumerable<string> filterSlice = new List<string>();
            switch (p[0].Trim())
            {
                case "p":
                    filterSlice = filter.P;
                    break;
                case "g":
                    filterSlice = filter.G;
                    break;
            }

            return FilterWords(p, filterSlice);
        }

        private static bool FilterWords(string[] line, IEnumerable<string> filter)
        {
            if (line.Length < filter.Count() + 1)
            {
                return true;
            }
            var skipLine = false;
            for (var i = 0; i < filter.Count(); i++)
            {
                var current = filter.ElementAt(i).Trim();
                var next = filter.ElementAt(i + 1);
                if (!string.IsNullOrEmpty(current) && current != next)
                {
                    skipLine = true;
                    break;
                }
            }
            return skipLine;
        }

    }
}
