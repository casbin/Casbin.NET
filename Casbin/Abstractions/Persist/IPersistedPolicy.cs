using Casbin.Model;

namespace Casbin.Persist;

public interface IPersistantPolicy
{
    public string Type { get; set; }
    public string Value1 { get; set; }
    public string Value2 { get; set; }
    public string Value3 { get; set; }
    public string Value4 { get; set; }
    public string Value5 { get; set; }
    public string Value6 { get; set; }
    public string Value7 { get; set; }
    public string Value8 { get; set; }
    public string Value9 { get; set; }
    public string Value10 { get; set; }
    public string Value11 { get; set; }
    public string Value12 { get; set; }
    public IPolicyValues Values { get; }
}

