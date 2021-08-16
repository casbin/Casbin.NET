using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.File
{
    public class FileFilteredAdapter : FileAdapter, IFilteredAdapter
    {
        public bool IsFiltered { get; private set; }

        public FileFilteredAdapter(string filePath) : base(filePath)
        {
        }

        public FileFilteredAdapter(Stream inputStream) : base(inputStream)
        {
        }

        public void LoadFilteredPolicy(IModel model, Filter filter)
        {
            if (filter == null)
            {
                LoadPolicy(model);
                return;
            }

            if (string.IsNullOrWhiteSpace(_filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            LoadFilteredPolicyFile(model, filter, Helper.LoadPolicyLine);
            IsFiltered = true;
        }

        public async Task LoadFilteredPolicyAsync(IModel model, Filter filter)
        {
            if (filter == null)
            {
                await LoadPolicyAsync(model);
                return;
            }

            if (string.IsNullOrWhiteSpace(_filePath))
            {
                throw new Exception("invalid file path, file path cannot be empty");
            }

            await LoadFilteredPolicyFileAsync(model, filter, Helper.LoadPolicyLine);
            IsFiltered = true;
        }

        private void LoadFilteredPolicyFile(IModel model, Filter filter, Action<string, IModel> handler)
        {
            var reader = new StreamReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
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

        private async Task LoadFilteredPolicyFileAsync(IModel model, Filter filter, Action<string, IModel> handler)
        {
            var reader = new StreamReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
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
