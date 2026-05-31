namespace ErpSystem.Shared.Security;

public static class PermissionNames
{
    public static class System
    {
        public static class Users
        {
            public const string Read = "System.Users.Read";
            public const string Create = "System.Users.Create";
            public const string Edit = "System.Users.Edit";
            public const string Delete = "System.Users.Delete";
        }

        public static class Roles
        {
            public const string Read = "System.Roles.Read";
            public const string Manage = "System.Roles.Manage";
        }

        public static class Permissions
        {
            public const string Read = "System.Permissions.Read";
            public const string Assign = "System.Permissions.Assign";
        }

        public static class Menu
        {
            public const string Read = "System.Menu.Read";
        }

        public static class Audit
        {
            public const string Read = "System.Audit.Read";
        }

        public static class Dictionaries
        {
            public const string Read = "System.Dictionaries.Read";
            public const string Manage = "System.Dictionaries.Manage";
        }

        public static class Attachments
        {
            public const string Read = "System.Attachments.Read";
            public const string Create = "System.Attachments.Create";
            public const string Delete = "System.Attachments.Delete";
        }

        public static class DocumentHistory
        {
            public const string Read = "System.DocumentHistory.Read";
        }

        public static class DocumentNumbers
        {
            public const string Read = "System.DocumentNumbers.Read";
            public const string Generate = "System.DocumentNumbers.Generate";
        }
    }

    public static class Warehouse
    {
        public static class Goods
        {
            public const string Read = "Warehouse.Goods.Read";
            public const string Create = "Warehouse.Goods.Create";
            public const string Edit = "Warehouse.Goods.Edit";
            public const string Delete = "Warehouse.Goods.Delete";
        }

        public static class Documents
        {
            public const string Read = "Warehouse.Documents.Read";
            public const string Create = "Warehouse.Documents.Create";
            public const string Post = "Warehouse.Documents.Post";
        }

        public static class Stock
        {
            public const string Read = "Warehouse.Stock.Read";
        }
    }

    public static class Contractors
    {
        public static class Contractor
        {
            public const string Read = "Contractors.Contractor.Read";
            public const string Create = "Contractors.Contractor.Create";
            public const string Edit = "Contractors.Contractor.Edit";
            public const string ChangeStatus = "Contractors.Contractor.ChangeStatus";
            public const string ViewHistory = "Contractors.Contractor.ViewHistory";
        }
    }

    public static class Transport
    {
        public static class Order
        {
            public const string Read = "Transport.Order.Read";
            public const string Create = "Transport.Order.Create";
            public const string Edit = "Transport.Order.Edit";
            public const string ChangeStatus = "Transport.Order.ChangeStatus";
            public const string AssignDriver = "Transport.Order.AssignDriver";
        }
    }

    public static class HR
    {
        public static class Employee
        {
            public const string Read = "HR.Employee.Read";
            public const string Create = "HR.Employee.Create";
            public const string Edit = "HR.Employee.Edit";
        }

        public static class Department
        {
            public const string Read = "HR.Department.Read";
            public const string Manage = "HR.Department.Manage";
        }

        public static class Position
        {
            public const string Read = "HR.Position.Read";
            public const string Manage = "HR.Position.Manage";
        }

        public static class Contract
        {
            public const string Read = "HR.Contract.Read";
            public const string Manage = "HR.Contract.Manage";
        }

        public static class Leave
        {
            public const string Read = "HR.Leave.Read";
            public const string Create = "HR.Leave.Create";
            public const string Approve = "HR.Leave.Approve";
        }

        public static class Schedule
        {
            public const string Read = "HR.Schedule.Read";
            public const string Manage = "HR.Schedule.Manage";
        }

        public static class Document
        {
            public const string Read = "HR.Document.Read";
            public const string Manage = "HR.Document.Manage";
        }
    }

    public static class Finance
    {
        public static class Cost
        {
            public const string Read = "Finance.Cost.Read";
            public const string Create = "Finance.Cost.Create";
            public const string Edit = "Finance.Cost.Edit";
        }

        public static class Invoice
        {
            public const string Read = "Finance.Invoice.Read";
            public const string Create = "Finance.Invoice.Create";
            public const string Edit = "Finance.Invoice.Edit";
        }

        public static class Payment
        {
            public const string Read = "Finance.Payment.Read";
            public const string Create = "Finance.Payment.Create";
        }

        public static class CurrencyRate
        {
            public const string Read = "Finance.CurrencyRate.Read";
            public const string Manage = "Finance.CurrencyRate.Manage";
        }

        public static class Settlement
        {
            public const string Read = "Finance.Settlement.Read";
            public const string Manage = "Finance.Settlement.Manage";
        }
    }
}
