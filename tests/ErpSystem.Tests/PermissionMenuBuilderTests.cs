using ErpSystem.Application.Modules.System.Foundation;
using ErpSystem.Shared.Security;

namespace ErpSystem.Tests;

public class PermissionMenuBuilderTests
{
    [Fact]
    public void Build_ShouldIncludeOnlySectionsAllowedByPermissions()
    {
        var builder = new PermissionMenuBuilder();

        var menu = builder.Build(
        [
            PermissionNames.Contractors.Contractor.Read,
            PermissionNames.System.DocumentHistory.Read,
            PermissionNames.System.Attachments.Read,
            PermissionNames.Warehouse.Goods.Read,
            PermissionNames.Transport.Order.AssignDriver
        ]);

        Assert.Contains(menu, section => section.Key == "contractors");
        Assert.Contains(menu, section => section.Key == "warehouse");
        Assert.Contains(menu, section => section.Key == "transport");
        Assert.Contains(menu, section => section.Key == "documents");
        Assert.DoesNotContain(menu, section => section.Key == "hr");
    }
}
