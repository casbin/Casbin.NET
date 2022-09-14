using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.File
{
    public class StreamFilteredAdapter : StreamAdapter, IFilteredAdapter
    {
        public bool IsFiltered { get; private set; }

        public StreamFilteredAdapter(Stream inputStream, Stream outputStream) : base(inputStream, outputStream)
        {
        }

        public void LoadFilteredPolicy(IPolicyStore store, Filter filter)
        {
            if (filter is null)
            {
                LoadPolicy(store);
                return;
            }

            LoadFilteredPolicyFile(store, filter);
        }

        public Task LoadFilteredPolicyAsync(IPolicyStore store, Filter filter)
        {
            if (filter is null)
            {
                return LoadPolicyAsync(store);
            }

            return LoadFilteredPolicyFileAsync(store, filter);
        }

        private void LoadFilteredPolicyFile(IPolicyStore store, Filter filter)
        {
            var reader = new StreamReader(_byteArrayInputStream);
            while (reader.EndOfStream is false)
            {
                string line = reader.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(line) || FilterLine(line, filter))
                {
                    return;
                }
                store.TryLoadPolicyLine(line);
            }
            IsFiltered = true;
        }

        private async Task LoadFilteredPolicyFileAsync(IPolicyStore store, Filter filter)
        {
            var reader = new StreamReader(_byteArrayInputStream);
            while (reader.EndOfStream is false)
            {
                string line = (await reader.ReadLineAsync())?.Trim();
                if (string.IsNullOrWhiteSpace(line) || FilterLine(line, filter))
                {
                    return;
                }
                store.TryLoadPolicyLine(line);
            }
            IsFiltered = true;
        }

        private static bool FilterLine(string line, Filter filter)
        {
            if (filter == null)
            {
                return false;
            }

            string[] p = line.Split(',');
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
            string[] filterArray = filter.ToArray();
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
