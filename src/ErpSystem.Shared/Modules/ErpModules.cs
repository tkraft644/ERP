namespace ErpSystem.Shared.Modules;

public static class ErpModules
{
    public static IReadOnlyList<ErpModuleDescriptor> All { get; } =
    [
        new("system", "System", "Core"),
        new("contractors", "Contractors", "Operations"),
        new("warehouse", "Warehouse", "Operations"),
        new("transport", "Transport", "Operations"),
        new("hr", "HR", "Support"),
        new("finance", "Finance / Costs", "Support"),
        new("documents", "Documents", "Support"),
        new("reports", "Reports", "Analytics"),
        new("administration", "Administration", "Core")
    ];
}
