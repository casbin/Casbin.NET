using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.Stream;

internal class StreamAdapter : IEpochAdapter
{
    protected readonly System.IO.Stream InputStream;
    protected readonly System.IO.Stream OutputStream;

    public StreamAdapter(System.IO.Stream inputStream, System.IO.Stream outputStream)
    {
        InputStream = inputStream;
        OutputStream = outputStream;
    }

    public void LoadPolicy(IPolicyStore store)
    {
        using StreamReader streamReader = new StreamReader(InputStream);
        if (InputStream is not null)
        {
            LoadPolicyData(store, streamReader);
        }
    }

    public async Task LoadPolicyAsync(IPolicyStore store)
    {
        using StreamReader streamReader = new StreamReader(InputStream);
        if (InputStream is not null)
        {
            await LoadPolicyDataAsync(store, streamReader);
        }
    }

    public void SavePolicy(IPolicyStore store)
    {
        if (OutputStream is null)
        {
            throw new Exception("Store file can not write, because use outputStream has not been set.");
        }

        IEnumerable<string> policy = ConvertToPolicyStrings(store);
        SavePolicyFile(string.Join("\n", policy));
    }

    public Task SavePolicyAsync(IPolicyStore store)
    {
        if (OutputStream is null)
        {
            throw new Exception("Store file can not write, because use outputStream has not been set.");
        }

        IEnumerable<string> policy = ConvertToPolicyStrings(store);
        return SavePolicyFileAsync(string.Join("\n", policy));
    }

    private static IEnumerable<string> GetModelPolicy(IPolicyStore store, string section)
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

    private static void LoadPolicyData(IPolicyStore store, StreamReader inputStream)
    {
        if (inputStream.EndOfStream is true)
        {
            inputStream.BaseStream.Position = 0;
        }

        while (inputStream.EndOfStream is false)
        {
            string line = inputStream.ReadLine();
            store.TryLoadPolicyLine(line);
        }
    }

    private static async Task LoadPolicyDataAsync(IPolicyStore store, StreamReader inputStream)
    {
        if (inputStream.EndOfStream is true)
        {
            inputStream.BaseStream.Position = 0;
        }

        while (inputStream.EndOfStream is false)
        {
            string line = await inputStream.ReadLineAsync();
            store.TryLoadPolicyLine(line);
        }
    }

    private static IEnumerable<string> ConvertToPolicyStrings(IPolicyStore store)
    {
        List<string> policy = new List<string>();
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

    private void SavePolicyFile(string text)
    {
        StreamWriter streamWriter = new StreamWriter(OutputStream);
#if (NET6_0 || NET5_0 || NETCOREAPP3_1)
            streamWriter.Write(text.AsSpan());
#else
        streamWriter.Write(text.ToCharArray());
#endif
        streamWriter.Dispose();
    }

    private async Task SavePolicyFileAsync(string text)
    {
        StreamWriter streamWriter = new StreamWriter(OutputStream);
        await streamWriter.WriteAsync(text);
        streamWriter.Dispose();
    }
}

