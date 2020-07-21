using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetCasbin.Model;

namespace NetCasbin.Persist.FileAdapter
{
    public class DefaultFilteredAdapter : DefaultFileAdapter, IFilteredAdapter
    {
        public bool IsFiltered { get; private set; }

        public DefaultFilteredAdapter(string filePath) : base(filePath)
        {
        }

        public DefaultFilteredAdapter(Stream inputStream) : base(inputStream)
        {
        }

        public void LoadFilteredPolicy(Model.Model model, Filter filter)
        {
            if (filter == null)
            {
                LoadPolicy(model);
                return;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            LoadFilteredPolicyFile(model, filter, Helper.LoadPolicyLine);
            IsFiltered = true;
        }

        public async Task LoadFilteredPolicyAsync(Model.Model model, Filter filter)
        {
            if (filter == null)
            {
                await LoadPolicyAsync(model);
                return;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            await LoadFilteredPolicyFileAsync(model, filter, Helper.LoadPolicyLine);
            IsFiltered = true;
        }

        private void LoadFilteredPolicyFile(Model.Model model, Filter filter, Action<string, Model.Model> handler)
        {
            var reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(line) || FilterLine(line, filter))
                {
                    return;
                }
                handler(line, model);
            }
        }

        private async Task LoadFilteredPolicyFileAsync(Model.Model model, Filter filter, Action<string, Model.Model> handler)
        {
            var reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            while (!reader.EndOfStream)
            {
                string line = (await reader.ReadLineAsync())?.Trim();
                if (string.IsNullOrWhiteSpace(line) || FilterLine(line, filter))
                {
                    return;
                }
                handler(line, model);
            }
        }

        private static bool FilterLine(string line, Filter filter)
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
                case PermConstants.DefaultPolicyType:
                    filterSlice = filter.P;
                    break;
                case PermConstants.DefaultGroupingPolicyType:
                    filterSlice = filter.G;
                    break;
            }

            return FilterWords(p, filterSlice);
        }

        private static bool FilterWords(string[] line, IEnumerable<string> filter)
        {
            var filterArray = filter.ToArray();
            int length = filterArray.Length;

            if (line.Length < length + 1)
            {
                return true;
            }

            bool skipLine = false;
            for (int i = 0; i < length; i++)
            {
                string current = filterArray.ElementAt(i).Trim();
                string next = filterArray.ElementAt(i + 1);

                if (string.IsNullOrEmpty(current) || current == next)
                {
                    continue;
                }

                skipLine = true;
                break;
            }
            return skipLine;
        }
    }
}
