namespace ErpSystem.Tests;

public class ArchitectureSmokeTests
{
    [Fact]
    public void ProjectMarkers_AreAccessible()
    {
        _ = typeof(ErpSystem.Domain.AssemblyMarker);
        _ = typeof(ErpSystem.Application.AssemblyMarker);
        _ = typeof(ErpSystem.Infrastructure.AssemblyMarker);
        _ = typeof(ErpSystem.Shared.AssemblyMarker);
    }
}
