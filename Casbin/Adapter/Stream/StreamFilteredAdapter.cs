using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.Stream;

internal class StreamFilteredAdapter : StreamAdapter, IFilteredAdapter
{
    public StreamFilteredAdapter(System.IO.Stream inputStream, System.IO.Stream outputStream) : base(inputStream,
        outputStream)
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

        LoadFilteredPolicyFile(store, filter);
    }

    public Task LoadFilteredPolicyAsync(IPolicyStore store, IPolicyFilter filter)
    {
        if (filter is null)
        {
            return LoadPolicyAsync(store);
        }

        return LoadFilteredPolicyFileAsync(store, filter);
    }

    private void LoadFilteredPolicyFile(IPolicyStore store, IPolicyFilter filter)
    {
        IEnumerable<IPersistantPolicy> policies = ReadPersistantPolicy(InputStream);
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

    private static IEnumerable<IPersistantPolicy> ReadPersistantPolicy(System.IO.Stream inputStream)
    {
        using StreamReader reader = new(inputStream);
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

