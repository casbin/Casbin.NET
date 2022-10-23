using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.File;

public class FileFilteredAdapter : FileAdapter, IFilteredAdapter
{
    public FileFilteredAdapter(string filePath) : base(filePath)
    {
    }

    public FileFilteredAdapter(System.IO.Stream inputStream) : base(inputStream)
    {
    }

    public bool IsFiltered { get; private set; }

    public void LoadFilteredPolicy(IPolicyStore store, IPolicyFilter filter)
    {
        if (filter is null)
        {
            LoadPolicy(store);
            return;
        }

        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new InvalidOperationException("invalid file path, file path cannot be empty");
        }

        LoadFilteredPolicyFile(store, filter);
    }

    public Task LoadFilteredPolicyAsync(IPolicyStore store, IPolicyFilter filter)
    {
        if (filter is null)
        {
            return LoadPolicyAsync(store);
        }

        if (string.IsNullOrWhiteSpace(FilePath))
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
        IEnumerable<IPersistantPolicy> policies = ReadPersistantPolicy(FilePath);
        policies = filter.ApplyFilter(policies.AsQueryable());
        foreach (IPersistantPolicy policy in policies)
        {
            string section = policy.Type.Substring(0, 1);
            store.AddPolicy(section, policy.Type, policy.Values);
        }

        IsFiltered = true;
    }

    private Task LoadFilteredPolicyFileAsync(IPolicyStore store, IPolicyFilter filter)
    {
        LoadFilteredPolicyFile(store, filter);
#if NET452
            return Task.FromResult(true);
#else
        return Task.CompletedTask;
#endif
    }

    private static IEnumerable<IPersistantPolicy> ReadPersistantPolicy(string filePath)
    {
        using StreamReader reader = new(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        while (reader.EndOfStream is false)
        {
            string line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("/") || line.StartsWith("#"))
            {
                continue;
            }

            string[] tokens = line.Split(PermConstants.PolicySeparatorChar).Select(x => x.Trim()).ToArray();
            string type = tokens[0];
            IPolicyValues values = Policy.ValuesFrom(tokens.Skip(1));
            yield return new PersistantPolicy(type, values);
        }
    }
}

