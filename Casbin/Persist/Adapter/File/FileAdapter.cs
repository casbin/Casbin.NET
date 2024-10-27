using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casbin.Model;
using CsvHelper;
using CsvHelper.Configuration;

namespace Casbin.Persist.Adapter.File;

internal enum ReadSource
{
    File,
    Text,
    Stream
}

public class FileAdapter : IEpochAdapter, IFilteredAdapter
{
    public string OriginalPath { get; private set; }

    internal ReadSource ReadSource { get; private set; }
    private System.IO.Stream OriginalStream { get; set; }
    private string OriginalText { get; set; }
    private bool CanWrite { get; set; }

    private FileAdapter()
    {
    }

    public FileAdapter(string filePath)
    {
        SetLoadFromPath(filePath);
    }

    public FileAdapter(System.IO.Stream originalStream)
    {
        SetLoadFromStream(originalStream);
    }

    private void SetLoadFromPath(string path)
    {
        ReadSource = ReadSource.File;
        OriginalPath = path;
        CanWrite = true;
    }

    private void SetLoadFromText(string text)
    {
        ReadSource = ReadSource.Text;
        OriginalText = text;
        CanWrite = false;
    }

    private void SetLoadFromStream(System.IO.Stream stream)
    {
        ReadSource = ReadSource.Stream;
        OriginalStream = stream;
        CanWrite = false;
    }

    public static FileAdapter CreateFromFile(string filePath)
    {
        var adapter = new FileAdapter();
        adapter.SetLoadFromPath(filePath);
        return adapter;
    }

    public static FileAdapter CreateFromText(string text)
    {
        var adapter = new FileAdapter();
        adapter.SetLoadFromText(text);
        return adapter;
    }

    public void LoadPolicy(IPolicyStore store)
    {
        foreach (IPersistPolicy policy in ReadPersistPolicy())
        {
            int requiredCount = store.GetRequiredValuesCount(policy.Section, policy.Type);
            IPolicyValues values = Policy.ValuesFrom(policy, requiredCount);
            store.AddPolicy(policy.Section, policy.Type, values);
        }
    }

#if !NET452
    public async Task LoadPolicyAsync(IPolicyStore store)
    {
        await foreach (IPersistPolicy policy in ReadPersistPolicyAsync())
        {
            int requiredCount = store.GetRequiredValuesCount(policy.Section, policy.Type);
            IPolicyValues values = Policy.ValuesFrom(policy, requiredCount);
            store.AddPolicy(policy.Section, policy.Type, values);
        }
    }
#else
    public async Task LoadPolicyAsync(IPolicyStore store)
    {
        foreach (IPersistPolicy policy in await ReadPersistPolicyAsync())
        {
            int requiredCount = store.GetRequiredValuesCount(policy.Section, policy.Type);
            IPolicyValues values = Policy.ValuesFrom(policy, requiredCount);
            store.AddPolicy(policy.Section, policy.Type, values);
        }
    }
#endif

    public void SavePolicy(IPolicyStore store)
    {
        if (CanWrite is false)
        {
            throw new InvalidOperationException("Store file can not write, because use inputStream is readOnly");
        }

        if (string.IsNullOrWhiteSpace(OriginalPath))
        {
            throw new ArgumentException("Invalid file path, file path cannot be empty");
        }

        IEnumerable<string> policy = ConvertToPolicyStrings(store);
        SavePolicyFile(string.Join(Environment.NewLine, policy));
    }

    public Task SavePolicyAsync(IPolicyStore store)
    {
        if (CanWrite is false)
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
        List<string> policy = new();
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

        IEnumerable<IPersistPolicy> policies = ReadPersistPolicy();
        policies = filter.Apply(policies.AsQueryable());
        foreach (IPersistPolicy policy in policies)
        {
            int requiredCount = store.GetRequiredValuesCount(policy.Section, policy.Type);
            IPolicyValues values = Policy.ValuesFrom(policy, requiredCount);
            store.AddPolicy(policy.Section, policy.Type, values);
        }
        IsFiltered = true;
    }

    public Task LoadFilteredPolicyAsync(IPolicyStore store, IPolicyFilter filter)
    {
        LoadFilteredPolicy(store, filter);
#if !NET452
        return Task.CompletedTask;
#else
        return Task.FromResult(true);
#endif
    }

    public void LoadFilteredPolicy(IPolicyStore store, Filter filter)
    {
        IPolicyFilter policyFilter = filter;
        LoadFilteredPolicy(store, policyFilter);
    }

    public Task LoadFilteredPolicyAsync(IPolicyStore store, Filter filter)
    {
        LoadFilteredPolicy(store, filter);
#if !NET452
        return Task.CompletedTask;
#else
        return Task.FromResult(true);
#endif
    }


    private IEnumerable<IPersistPolicy> ReadPersistPolicy()
    {
        switch (ReadSource)
        {
            case ReadSource.File:
                FileStream fileStream = new(OriginalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return ReadPersistPolicyStream(fileStream);
            case ReadSource.Text:
                MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(OriginalText));
                return ReadPersistPolicyStream(memoryStream);
            case ReadSource.Stream:
                return ReadPersistPolicyStream(OriginalStream);
            default:
                throw new InvalidOperationException("Invalid read source");
        }
    }

    private static IEnumerable<IPersistPolicy> ReadPersistPolicyStream(System.IO.Stream stream)
    {
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null,
                WhiteSpaceChars = new[] { ' ', '\t' }
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
    private IAsyncEnumerable<IPersistPolicy> ReadPersistPolicyAsync()
    {
        switch (ReadSource)
        {
            case ReadSource.File:
                FileStream fileStream = new(OriginalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return ReadPersistPolicyStreamAsync(fileStream);
            case ReadSource.Text:
                MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(OriginalText));
                return ReadPersistPolicyStreamAsync(memoryStream);
            case ReadSource.Stream:
                return ReadPersistPolicyStreamAsync(OriginalStream);
            default:
                throw new InvalidOperationException("Invalid read source");
        }
    }
#else
    private Task<IEnumerable<IPersistPolicy>> ReadPersistPolicyAsync()
    {
        switch (ReadSource)
        {
            case ReadSource.File:
                FileStream fileStream = new(OriginalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return ReadPersistPolicyStreamAsync(fileStream);
            case ReadSource.Text:
                MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(OriginalText));
                return ReadPersistPolicyStreamAsync(memoryStream);
            case ReadSource.Stream:
                return ReadPersistPolicyStreamAsync(OriginalStream);
            default:
                throw new InvalidOperationException("Invalid read source");
        }
    }
#endif

#if !NET452
    private static async IAsyncEnumerable<IPersistPolicy> ReadPersistPolicyStreamAsync(System.IO.Stream stream)
    {
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null,
                WhiteSpaceChars = new[] { ' ', '\t' }
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
    private static async Task<IEnumerable<IPersistPolicy>> ReadPersistPolicyStreamAsync(System.IO.Stream stream)
    {
        using StreamReader reader = new(stream);
        CsvParser parser = new(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null,
                WhiteSpaceChars = new []{' ', '\t'}
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
