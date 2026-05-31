using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.System.Documents;
using ErpSystem.Domain.Modules.System.Identity;
using ErpSystem.Shared.Security;

namespace ErpSystem.Infrastructure.Persistence.Seed;

internal static class FoundationSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SequenceSeedTimestamp = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public const int AdminUserId = 1;

    public const int AdminRoleId = 1;
    public const int WarehouseRoleId = 2;
    public const int TransportRoleId = 3;
    public const int HrRoleId = 4;
    public const int FinanceRoleId = 5;

    public static Role[] Roles =>
    [
        Create(new Role("SYSTEM_ADMIN", "System administrator", "Full access to the ERP foundation.", true), AdminRoleId),
        Create(new Role("WAREHOUSE_MANAGER", "Warehouse manager", "Warehouse operations and goods maintenance."), WarehouseRoleId),
        Create(new Role("TRANSPORT_DISPATCHER", "Transport dispatcher", "Transport orders and dispatch operations."), TransportRoleId),
        Create(new Role("HR_MANAGER", "HR manager", "Employee records and leave approvals."), HrRoleId),
        Create(new Role("FINANCE_MANAGER", "Finance manager", "Costs, invoices, payments and settlements."), FinanceRoleId)
    ];

    public static Permission[] Permissions =>
        PermissionCatalog.All
            .Select((item, index) => Create(
                new Permission(item.Name, item.ModuleKey, item.Resource, item.Action, item.Description),
                index + 1))
            .ToArray();

    public static RolePermission[] RolePermissions =>
        BuildRolePermissions().ToArray();

    public static DictionaryItem[] DictionaryItems =>
    [
        Create(new DictionaryItem("Currencies", "PLN", "Polish zloty", "PLN", 10), 1),
        Create(new DictionaryItem("Currencies", "EUR", "Euro", "EUR", 20), 2),
        Create(new DictionaryItem("VehicleTypes", "TRUCK", "Truck", "Truck", 10), 3),
        Create(new DictionaryItem("VehicleTypes", "VAN", "Van", "Van", 20), 4),
        Create(new DictionaryItem("LeaveTypes", "ANNUAL", "Annual leave", "Annual", 10), 5),
        Create(new DictionaryItem("LeaveTypes", "SICK", "Sick leave", "Sick", 20), 6),
        Create(new DictionaryItem("WarehouseDocumentTypes", "PZ", "Przyjęcie zewnętrzne", "PZ", 10), 7),
        Create(new DictionaryItem("WarehouseDocumentTypes", "WZ", "Wydanie zewnętrzne", "WZ", 20), 8),
        Create(new DictionaryItem("WarehouseDocumentTypes", "MM", "Przesunięcie międzymagazynowe", "MM", 30), 9),
        Create(new DictionaryItem("WarehouseDocumentTypes", "RW", "Rozchód wewnętrzny", "RW", 40), 10),
        Create(new DictionaryItem("WarehouseDocumentTypes", "PW", "Przyjęcie wewnętrzne", "PW", 50), 11),
        Create(new DictionaryItem("WarehouseDocumentTypes", "INW", "Inwentaryzacja", "INW", 60), 12),
        Create(new DictionaryItem("TransportOrderTypes", "Domestic", "Krajowe", "Domestic", 10), 13),
        Create(new DictionaryItem("TransportOrderTypes", "International", "Międzynarodowe", "International", 20), 14)
    ];

    public static DocumentStatus[] DocumentStatuses =>
    [
        Create(new DocumentStatus("warehouse", "Active", "Active", 10), 1),
        Create(new DocumentStatus("warehouse", "Archived", "Archived", 20, true), 2),
        Create(new DocumentStatus("transport", "New", "Nowe", 10), 3),
        Create(new DocumentStatus("transport", "Accepted", "Przyjęte", 20), 4),
        Create(new DocumentStatus("transport", "Planned", "Zaplanowane", 30), 5),
        Create(new DocumentStatus("transport", "InProgress", "W realizacji", 40), 6),
        Create(new DocumentStatus("transport", "Loaded", "Załadowane", 50), 7),
        Create(new DocumentStatus("transport", "Delivered", "Dostarczone", 60), 8),
        Create(new DocumentStatus("transport", "Closed", "Zamknięte", 70, true), 9),
        Create(new DocumentStatus("transport", "Cancelled", "Anulowane", 80, true), 10),
        Create(new DocumentStatus("hr", "Requested", "Requested", 10), 11),
        Create(new DocumentStatus("hr", "Approved", "Approved", 20, true), 12),
        Create(new DocumentStatus("hr", "Rejected", "Rejected", 30, true), 13)
    ];

    public static DocumentNumberSequence[] DocumentNumberSequences =>
    [
        Create(new DocumentNumberSequence("WAREHOUSE_GOODS", "WG", 128, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 1),
        Create(new DocumentNumberSequence("TR_ORD", "TR", 42, 5, DocumentNumberResetPolicy.Monthly, SequenceSeedTimestamp), 2),
        Create(new DocumentNumberSequence("HR_EMPLOYEE", "HE", 15, 4, DocumentNumberResetPolicy.Never, SequenceSeedTimestamp), 4),
        Create(new DocumentNumberSequence("WH_PZ", "PZ", 0, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 5),
        Create(new DocumentNumberSequence("WH_WZ", "WZ", 0, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 6),
        Create(new DocumentNumberSequence("WH_MM", "MM", 0, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 7),
        Create(new DocumentNumberSequence("WH_RW", "RW", 0, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 8),
        Create(new DocumentNumberSequence("WH_PW", "PW", 0, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 9),
        Create(new DocumentNumberSequence("WH_INW", "INW", 0, 5, DocumentNumberResetPolicy.Yearly, SequenceSeedTimestamp), 10),
        Create(new DocumentNumberSequence("FIN_COST", "KD", 0, 5, DocumentNumberResetPolicy.Monthly, SequenceSeedTimestamp), 11),
        Create(new DocumentNumberSequence("FIN_INV", "FV", 0, 5, DocumentNumberResetPolicy.Monthly, SequenceSeedTimestamp), 12)
    ];

    private static T Create<T>(T entity, int id) where T : Domain.Common.AuditableEntity
    {
        entity.Id = id;
        entity.CreatedAt = SeedTimestamp;
        entity.CreatedByUserId = AdminUserId;
        entity.ModifiedAt = null;
        entity.ModifiedByUserId = null;
        entity.IsDeleted = false;
        entity.RowVersion = [];
        return entity;
    }

    private static IEnumerable<RolePermission> BuildRolePermissions()
    {
        var permissionsByName = Permissions.ToDictionary(item => item.Name, item => item.Id, StringComparer.OrdinalIgnoreCase);
        var items = new List<RolePermission>();
        var nextId = 1;

        void AddPermissions(int roleId, params string[] names)
        {
            foreach (var name in names)
            {
                items.Add(Create(new RolePermission(roleId, permissionsByName[name]), nextId++));
            }
        }

        AddPermissions(AdminRoleId, PermissionCatalog.All.Select(permission => permission.Name).ToArray());
        AddPermissions(
            WarehouseRoleId,
            PermissionNames.System.Menu.Read,
            PermissionNames.System.Attachments.Read,
            PermissionNames.System.Attachments.Create,
            PermissionNames.System.DocumentHistory.Read,
            PermissionNames.System.DocumentNumbers.Read,
            PermissionNames.System.DocumentNumbers.Generate,
            PermissionNames.Contractors.Contractor.Read,
            PermissionNames.Warehouse.Goods.Read,
            PermissionNames.Warehouse.Goods.Create,
            PermissionNames.Warehouse.Goods.Edit,
            PermissionNames.Warehouse.Goods.Delete,
            PermissionNames.Warehouse.Documents.Read,
            PermissionNames.Warehouse.Documents.Create,
            PermissionNames.Warehouse.Documents.Post,
            PermissionNames.Warehouse.Stock.Read);
        AddPermissions(
            TransportRoleId,
            PermissionNames.System.Menu.Read,
            PermissionNames.System.Attachments.Read,
            PermissionNames.System.Attachments.Create,
            PermissionNames.System.DocumentHistory.Read,
            PermissionNames.Contractors.Contractor.Read,
            PermissionNames.Transport.Order.Read,
            PermissionNames.Transport.Order.Create,
            PermissionNames.Transport.Order.Edit,
            PermissionNames.Transport.Order.ChangeStatus,
            PermissionNames.Transport.Order.AssignDriver);
        AddPermissions(
            HrRoleId,
            PermissionNames.System.Menu.Read,
            PermissionNames.System.DocumentHistory.Read,
            PermissionNames.HR.Employee.Read,
            PermissionNames.HR.Employee.Create,
            PermissionNames.HR.Employee.Edit,
            PermissionNames.HR.Department.Read,
            PermissionNames.HR.Department.Manage,
            PermissionNames.HR.Position.Read,
            PermissionNames.HR.Position.Manage,
            PermissionNames.HR.Contract.Read,
            PermissionNames.HR.Contract.Manage,
            PermissionNames.HR.Leave.Read,
            PermissionNames.HR.Leave.Create,
            PermissionNames.HR.Leave.Approve,
            PermissionNames.HR.Schedule.Read,
            PermissionNames.HR.Schedule.Manage,
            PermissionNames.HR.Document.Read,
            PermissionNames.HR.Document.Manage);
        AddPermissions(
            FinanceRoleId,
            PermissionNames.System.Menu.Read,
            PermissionNames.System.DocumentHistory.Read,
            PermissionNames.System.DocumentNumbers.Read,
            PermissionNames.System.DocumentNumbers.Generate,
            PermissionNames.Contractors.Contractor.Read,
            PermissionNames.Transport.Order.Read,
            PermissionNames.HR.Employee.Read,
            PermissionNames.HR.Department.Read,
            PermissionNames.Warehouse.Stock.Read,
            PermissionNames.Finance.Cost.Read,
            PermissionNames.Finance.Cost.Create,
            PermissionNames.Finance.Cost.Edit,
            PermissionNames.Finance.Invoice.Read,
            PermissionNames.Finance.Invoice.Create,
            PermissionNames.Finance.Invoice.Edit,
            PermissionNames.Finance.Payment.Read,
            PermissionNames.Finance.Payment.Create,
            PermissionNames.Finance.CurrencyRate.Read,
            PermissionNames.Finance.CurrencyRate.Manage,
            PermissionNames.Finance.Settlement.Read,
            PermissionNames.Finance.Settlement.Manage);

        return items;
    }
}
