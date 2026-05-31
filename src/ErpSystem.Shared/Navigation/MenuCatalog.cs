using ErpSystem.Shared.Security;

namespace ErpSystem.Shared.Navigation;

public static class MenuCatalog
{
    public static IReadOnlyList<MenuItemDefinition> All { get; } =
    [
        new("system", "System", "Users", "/system/users", [PermissionNames.System.Users.Read]),
        new("system", "System", "Roles", "/system/roles", [PermissionNames.System.Roles.Read]),
        new("system", "System", "Permissions", "/system/permissions", [PermissionNames.System.Permissions.Read]),
        new("system", "System", "Audit log", "/system/audit", [PermissionNames.System.Audit.Read]),
        new("system", "System", "Dictionaries", "/system/dictionaries", [PermissionNames.System.Dictionaries.Read]),
        new("system", "System", "Document sequences", "/system/document-numbers", [PermissionNames.System.DocumentNumbers.Read]),
        new("contractors", "Contractors", "Contractor list", "/contractors/list", [PermissionNames.Contractors.Contractor.Read]),
        new("warehouse", "Warehouse", "Goods", "/warehouse/goods", [PermissionNames.Warehouse.Goods.Read]),
        new("warehouse", "Warehouse", "Documents", "/warehouse/documents", [PermissionNames.Warehouse.Documents.Read]),
        new("warehouse", "Warehouse", "Stock", "/warehouse/stock", [PermissionNames.Warehouse.Stock.Read]),
        new("transport", "Transport", "Orders", "/transport/orders", [PermissionNames.Transport.Order.Read]),
        new("transport", "Transport", "Dispatch", "/transport/dispatch", [PermissionNames.Transport.Order.AssignDriver]),
        new("hr", "HR", "Employees", "/hr/employees", [PermissionNames.HR.Employee.Read]),
        new("hr", "HR", "Departments", "/hr/departments", [PermissionNames.HR.Department.Read]),
        new("hr", "HR", "Positions", "/hr/positions", [PermissionNames.HR.Position.Read]),
        new("hr", "HR", "Contracts", "/hr/contracts", [PermissionNames.HR.Contract.Read]),
        new("hr", "HR", "Leave requests", "/hr/leave-requests", [PermissionNames.HR.Leave.Read]),
        new("hr", "HR", "Leave approvals", "/hr/leave-approvals", [PermissionNames.HR.Leave.Approve]),
        new("hr", "HR", "Employee documents", "/hr/documents", [PermissionNames.HR.Document.Read]),
        new("finance", "Finance", "Cost documents", "/finance/cost-documents", [PermissionNames.Finance.Cost.Read]),
        new("finance", "Finance", "Invoices", "/finance/invoices", [PermissionNames.Finance.Invoice.Read]),
        new("finance", "Finance", "Payments", "/finance/payments", [PermissionNames.Finance.Payment.Read]),
        new("finance", "Finance", "Settlements", "/finance/settlements", [PermissionNames.Finance.Settlement.Read]),
        new("finance", "Finance", "Currency rates", "/finance/currency-rates", [PermissionNames.Finance.CurrencyRate.Read]),
        new("documents", "Documents", "Attachments", "/documents/attachments", [PermissionNames.System.Attachments.Read]),
        new("documents", "Documents", "History", "/documents/history", [PermissionNames.System.DocumentHistory.Read])
    ];
}
