using Casbin.Model;

namespace Casbin.Persist;

public class PersistantPolicy : IPersistantPolicy
{
    private IPolicyValues _values;

    public PersistantPolicy()
    {
    }

    public PersistantPolicy(string type, IPolicyValues values)
    {
        _values = values;
        Type = type;
        switch (values.Count)
        {
            case 1:
                Value1 = values[0];
                break;
            case 2:
                Value1 = values[0];
                Value2 = values[1];
                break;
            case 3:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                break;
            case 4:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                break;
            case 5:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                break;
            case 6:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                break;
            case 7:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                Value7 = values[6];
                break;
            case 8:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                Value7 = values[6];
                Value8 = values[7];
                break;
            case 9:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                Value7 = values[6];
                Value8 = values[7];
                Value9 = values[8];
                break;
            case 10:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                Value7 = values[6];
                Value8 = values[7];
                Value9 = values[8];
                Value10 = values[9];
                break;
            case 11:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                Value7 = values[6];
                Value8 = values[7];
                Value9 = values[8];
                Value10 = values[9];
                Value11 = values[10];
                break;
            case 12:
                Value1 = values[0];
                Value2 = values[1];
                Value3 = values[2];
                Value4 = values[3];
                Value5 = values[4];
                Value6 = values[5];
                Value7 = values[6];
                Value8 = values[7];
                Value9 = values[8];
                Value10 = values[9];
                Value11 = values[10];
                Value12 = values[11];
                break;
        }
    }

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

    public IPolicyValues Values => _values ??= new StringPolicyValues(
        Value1, Value2, Value3, Value4, Value5, Value6, Value7, Value8, Value9, Value10, Value11, Value12);
}

