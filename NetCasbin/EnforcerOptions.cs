using NetCasbin.Persist;

namespace NetCasbin;

public class EnforcerOptions
{
    public bool Enabled { get; set; } = true;
    public bool AutoBuildRoleLinks { get; set; } = true;
    public bool AutoNotifyWatcher { get; set; } = true;
    public bool AutoLoadPolicy { get; set; } = true;
    public Filter AutoLoadPolicyFilter { get; set; } = null;
}

