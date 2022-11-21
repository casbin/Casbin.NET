using Casbin.Persist;

namespace Casbin;

public class EnforcerOptions
{
    public bool Enabled { get; set; } = true;
    public bool EnabledCache { get; set; } = true;
    public bool AutoBuildRoleLinks { get; set; } = true;
    public bool AutoNotifyWatcher { get; set; } = true;
    public bool AutoCleanEnforceCache { get; set; } = true;
    public bool AutoLoadPolicy { get; set; } = true;
    public IPolicyFilter AutoLoadPolicyFilter { get; set; } = null;
}




