using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using CsvHelper;
using CsvHelper.Configuration;

namespace Casbin.Adapter.File;

public class FileAdapter : IEpochAdapter, IFilteredAdapter
{
    private readonly System.IO.Stream _inputStream;
    private readonly bool _readOnly;

    public string OriginalPath { get; }
    public FileAdapter(string filePath) => OriginalPath = filePath;

    public FileAdapter(System.IO.Stream inputStream)
    {
        _readOnly = true;
        try
        {
            _inputStream = inputStream;
        }
        catch (IOException e)
        {
            throw new IOException("File adapter init error", e);
        }
    }

    public void LoadPolicy(IPolicyStore store)
    {
        if (string.IsNullOrWhiteSpace(OriginalPath) is false)
        {
            IEnumerable<IPersistPolicy> policies = ReadPersistPolicy(OriginalPath);
            foreach (IPersistPolicy policy in policies)
            {
                IPolicyValues values = Policy.ValuesFrom(policy);
                store.AddPolicy(policy.Section, policy.Type, values);
            }
        }

        if (_inputStream is not null)
        {
            IEnumerable<IPersistPolicy> policies = ReadPersistPolicy(_inputStream);
            foreach (IPersistPolicy policy in policies)
            {
                IPolicyValues values = Policy.ValuesFrom(policy);
                store.AddPolicy(policy.Section, policy.Type, values);
            }
        }
    }

#if !NET452
    public async Task LoadPolicyAsync(IPolicyStore store)
    {
        if (string.IsNullOrWhiteSpace(OriginalPath) is false)
        {
            IAsyncEnumerable<IPersistPolicy> policies = ReadPersistPolicyAsync(OriginalPath);
            await foreach (IPersistPolicy policy in policies)
            {
                IPolicyValues values = Policy.ValuesFrom(policy);
                store.AddPolicy(policy.Section, policy.Type, values);
            }
        }

        if (_inputStream is not null)
        {
            IAsyncEnumerable<IPersistPolicy> policies = ReadPersistPolicyAsync(_inputStream);
            await foreach (IPersistPolicy policy in policies)
            {
                IPolicyValues values = Policy.ValuesFrom(policy);
                store.AddPolicy(policy.Section, policy.Type, values);
            }
        }
    }
#else
    public async Task LoadPolicyAsync(IPolicyStore store)
    {
        if (string.IsNullOrWhiteSpace(OriginalPath) is false)
        {
            var policies = await ReadPersistPolicyAsync(OriginalPath);
            foreach (IPersistPolicy policy in policies)
            {
                IPolicyValues values = Policy.ValuesFrom(policy);
                store.AddPolicy(policy.Section, policy.Type, values);
            }
        }

        if (_inputStream is not null)
        {
            var policies = await ReadPersistPolicyAsync(_inputStream);
            foreach (IPersistPolicy policy in policies)
            {
                IPolicyValues values = Policy.ValuesFrom(policy);
                store.AddPolicy(policy.Section, policy.Type, values);
            }
        }
    }
#endif

    public void SavePolicy(IPolicyStore store)
    {
        if (_inputStream != null && _readOnly)
        {
            throw new Exception("Store file can not write, because use inputStream is readOnly");
        }

        if (string.IsNullOrWhiteSpace(OriginalPath))
        {
            throw new ArgumentException("Invalid file path, file path cannot be empty");
        }

        IEnumerable<string> policy = ConvertToPolicyStrings(store);
        SavePolicyFile(string.Join("\n", policy));
    }

    public Task SavePolicyAsync(IPolicyStore store)
    {
        if (_inputStream != null && _readOnly)
        {
            throw new InvalidOperationException("Store file can not write, because use inputStream is readOnly");
        }

        if (string.IsNullOrWhiteSpace(OriginalPath))
        {
            throw new ArgumentException("Invalid file path, file path cannot be empty");
        }

        IEnumerable<string> policy = ConvertToPolicyStrings(store);
        return SavePolicyFileAsync(string.Join(Environment.NewLine, policy));
    }

    private static IEnumerable<string> GetModelPolicy(IReadOnlyPolicyStore store, string section)
    {
        List<string> policy = new List<string>();
        foreach (KeyValuePair<string, IEnumerable<IPolicyValues>> kv in store.GetPolicyAllType(section))
        {
            string key = kv.Key;
            IEnumerable<IPolicyValues> value = kv.Value;
            policy.AddRange(value.Select(p => $"{key}, {p.ToText()}"));
        }

        return policy;
    }

    private static IEnumerable<string> ConvertToPolicyStrings(IPolicyStore store)
    {
        List<string> policy = new();
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

    private void SavePolicyFile(string text) => System.IO.File.WriteAllText(OriginalPath, text, Encoding.UTF8);

    private async Task SavePolicyFileAsync(string text)
    {
        text ??= string.Empty;
        byte[] content = Encoding.UTF8.GetBytes(text);
#if !NETFRAMEWORK && !NETSTANDARD2_0
        await using FileStream fs = new(
            OriginalPath, FileMode.Create, FileAccess.Write,
            FileShare.None, 4096, true);
#else
        using var fs = new FileStream(
            OriginalPath, FileMode.Create, FileAccess.Write,
            FileShare.None, bufferSize: 4096, useAsync: true);
#endif
        await fs.WriteAsync(content, 0, content.Length);
    }

    #region FilteredAdapter

    public bool IsFiltered { get; private set; }

    public void LoadFilteredPolicy(IPolicyStore store, IPolicyFilter filter)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (filter is null)
        {
            LoadPolicy(store);
            return;
        }

        if (string.IsNullOrWhiteSpace(OriginalPath))
        {
            throw new InvalidOperationException("invalid file path, file path cannot be empty");
        }

        LoadFilteredPolicyFile(store, filter);
    }

    public Task LoadFilteredPolicyAsync(IPolicyStore store, IPolicyFilter filter)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (filter is null)
        {
            return LoadPolicyAsync(store);
        }

        if (string.IsNullOrWhiteSpace(OriginalPath))
        {
            throw new InvalidOperationException("invalid file path, file path cannot be empty");
        }

        return LoadFilteredPolicyFileAsync(store, filter);
    }

    public void LoadFilteredPolicy(IPolicyStore store, Filter filter)
    {
        IPolicyFilter policyFilter = filter;
        LoadFilteredPolicy(store, policyFilter);
    }

    public Task LoadFilteredPolicyAsync(IPolicyStore store, Filter filter)
    {
        IPolicyFilter policyFilter = filter;
        return LoadFilteredPolicyAsync(store, policyFilter);
    }

    private void LoadFilteredPolicyFile(IPolicyStore store, IPolicyFilter filter)
    {
        IEnumerable<IPersistPolicy> policies = ReadPersistPolicy(OriginalPath);
        policies = filter.Apply(policies.AsQueryable());
        foreach (IPersistPolicy policy in policies)
        {
            string section = policy.Section;
            IPolicyValues values = Policy.ValuesFrom(policy);
            store.AddPolicy(section, policy.Type, values);
        }

        IsFiltered = true;
    }

    private Task LoadFilteredPolicyFileAsync(IPolicyStore store, IPolicyFilter filter)
    {
        LoadFilteredPolicyFile(store, filter);
#if !NET452
        return Task.CompletedTask;
#else
        return Task.FromResult(true);
#endif
    }

    private static IEnumerable<IPersistPolicy> ReadPersistPolicy(string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });

        while (parser.Read())
        {
            string[] tokens = parser.Record;
            if (tokens is null || tokens.Length is 0)
            {
                continue;
            }

            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            yield return PersistPolicy.Create<PersistPolicy>(type, values);
        }
    }

    private static IEnumerable<IPersistPolicy> ReadPersistPolicy(System.IO.Stream stream)
    {
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });

        while (parser.Read())
        {
            string[] tokens = parser.Record;
            if (tokens is null || tokens.Length is 0)
            {
                continue;
            }

            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            yield return PersistPolicy.Create<PersistPolicy>(type, values);
        }
    }

#if !NET452
    private static async IAsyncEnumerable<IPersistPolicy> ReadPersistPolicyAsync(string filePath)
    {
#if NET6_0_OR_GREATER
        await using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
#else
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
#endif

        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });

        while (await parser.ReadAsync())
        {
            string[] tokens = parser.Record;
            if (tokens is null || tokens.Length is 0)
            {
                continue;
            }

            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            yield return PersistPolicy.Create<PersistPolicy>(type, values);
        }
    }
#else
        private static async Task<IEnumerable<IPersistPolicy>> ReadPersistPolicyAsync(string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });
        var list = new List<PersistPolicy>();
        while (await parser.ReadAsync())
        {
            string[]? tokens = parser.Record;
            if (tokens is null || tokens.Length is 0)
            {
                continue;
            }

            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            var policy = PersistPolicy.Create<PersistPolicy>(type, values);
            list.Add(policy);
        }
        return list;
    }
#endif

#if !NET452
    private static async IAsyncEnumerable<IPersistPolicy> ReadPersistPolicyAsync(System.IO.Stream stream)
    {
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });

        while (await parser.ReadAsync())
        {
            string[] tokens = parser.Record;
            if (tokens is null || tokens.Length is 0)
            {
                continue;
            }

            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            yield return PersistPolicy.Create<PersistPolicy>(type, values);
        }
    }
#else
    private static async Task<IEnumerable<IPersistPolicy>> ReadPersistPolicyAsync(System.IO.Stream stream)
    {
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });
        var list = new List<PersistPolicy>();
        while (await parser.ReadAsync())
        {
            string[] tokens = parser.Record;
            if (tokens is null || tokens.Length is 0)
            {
                continue;
            }

            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            var policy = PersistPolicy.Create<PersistPolicy>(type, values);
            list.Add(policy);
        }
        return list;
    }
#endif

    #endregion
}
