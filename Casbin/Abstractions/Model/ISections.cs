#nullable enable
using System.Collections.Generic;
using Casbin.Config;

namespace Casbin.Model;

public interface ISections
{
    public bool ContainsSection(string section);
    public void LoadSection(IConfig config, string section);
    public bool AddSection(string section, string type, string value);

    public T GetAssertion<T>(string section, string type)
        where T : Assertion;

    public bool TryGetAssertion<T>(string section, string type, out T? outAssertion)
        where T : Assertion;

    public IDictionary<string, T> GetAssertions<T>(string section)
        where T : Assertion;
}
