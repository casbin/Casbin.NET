using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Adapter.File;

public class FileAdapter : IEpochAdapter
{
    private readonly StreamReader _byteArrayInputStream;
    private readonly bool _readOnly;
    protected readonly string FilePath;

    public FileAdapter(string filePath) => FilePath = filePath;

    public FileAdapter(System.IO.Stream inputStream)
    {
        _readOnly = true;
        try
        {
            _byteArrayInputStream = new StreamReader(inputStream);
        }
        catch (IOException e)
        {
            throw new IOException("File adapter init error", e);
        }
    }

    public void LoadPolicy(IPolicyStore model)
    {
        if (string.IsNullOrWhiteSpace(FilePath) is false)
        {
            using StreamReader sr = new StreamReader(new FileStream(
                FilePath, FileMode.Open, FileAccess.Read, FileShare.Read));
            LoadPolicyLine(model, sr);
        }

        if (_byteArrayInputStream is not null)
        {
            LoadPolicyLine(model, _byteArrayInputStream);
        }
    }

    public async Task LoadPolicyAsync(IPolicyStore store)
    {
        if (string.IsNullOrWhiteSpace(FilePath) is false)
        {
            using StreamReader sr = new StreamReader(new FileStream(
                FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            await LoadPolicyLineAsync(store, sr);
            Debug.WriteLine("xxx");
        }

        if (_byteArrayInputStream is not null)
        {
            await LoadPolicyLineAsync(store, _byteArrayInputStream);
        }
    }

    public void SavePolicy(IPolicyStore store)
    {
        if (_byteArrayInputStream != null && _readOnly)
        {
            throw new Exception("Store file can not write, because use inputStream is readOnly");
        }

        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new ArgumentException("Invalid file path, file path cannot be empty");
        }

        IEnumerable<string> policy = ConvertToPolicyStrings(store);
        SavePolicyFile(string.Join("\n", policy));
    }

    public Task SavePolicyAsync(IPolicyStore store)
    {
        if (_byteArrayInputStream != null && _readOnly)
        {
            throw new Exception("Store file can not write, because use inputStream is readOnly");
        }

        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new ArgumentException("Invalid file path, file path cannot be empty");
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

    private static void LoadPolicyLine(IPolicyStore store, StreamReader inputStream)
    {
        while (inputStream.EndOfStream is false)
        {
            string line = inputStream.ReadLine();
            store.TryLoadPolicyLine(line);
        }
    }

    private static async Task LoadPolicyLineAsync(IPolicyStore store, StreamReader inputStream)
    {
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

    private void SavePolicyFile(string text) => System.IO.File.WriteAllText(FilePath, text, Encoding.UTF8);

    private async Task SavePolicyFileAsync(string text)
    {
        text ??= string.Empty;
        byte[] content = Encoding.UTF8.GetBytes(text);
#if !NETFRAMEWORK && !NETSTANDARD2_0
        await using FileStream fs = new(
            FilePath, FileMode.Create, FileAccess.Write,
            FileShare.None, 4096, true);
#else
            using var fs = new FileStream(
                FilePath, FileMode.Create, FileAccess.Write,
                FileShare.None, bufferSize: 4096, useAsync: true);
#endif
        await fs.WriteAsync(content, 0, content.Length);
    }
}

