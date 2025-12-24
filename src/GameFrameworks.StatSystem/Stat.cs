using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem;

#if DEBUG
[System.Diagnostics.DebuggerDisplay("{DisplayName}")]
#endif
public class Stat : IStat
{
    public string DisplayName { get; set; }
}
