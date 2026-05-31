using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.Transport;
using WarehouseEntity = ErpSystem.Domain.Modules.Warehouse.Warehouse;

namespace ErpSystem.Domain.Modules.Finance;

public sealed class CostPosition : Common.AuditableEntity
{
    private CostPosition()
    {
    }

    public CostPosition(
        int lineNumber,
        string costCategory,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate,
        int? transportOrderId,
        int? vehicleId,
        int? employeeId,
        int? warehouseId,
        int? contractorId,
        int? departmentId,
        string? notes)
    {
        LineNumber = lineNumber;
        CostCategory = costCategory;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
        TransportOrderId = transportOrderId;
        VehicleId = vehicleId;
        EmployeeId = employeeId;
        WarehouseId = warehouseId;
        ContractorId = contractorId;
        DepartmentId = departmentId;
        Notes = notes;
    }

    public int CostDocumentId { get; private set; }
    public CostDocument CostDocument { get; private set; } = null!;
    public int LineNumber { get; private set; }
    public string CostCategory { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public int? TransportOrderId { get; private set; }
    public TransportOrder? TransportOrder { get; private set; }
    public int? VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public int? EmployeeId { get; private set; }
    public Employee? Employee { get; private set; }
    public int? WarehouseId { get; private set; }
    public WarehouseEntity? Warehouse { get; private set; }
    public int? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }
    public int? DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public string? Notes { get; private set; }

    public decimal NetAmount => decimal.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);
    public decimal TaxAmount => decimal.Round(NetAmount * TaxRate / 100m, 2, MidpointRounding.AwayFromZero);
    public decimal GrossAmount => NetAmount + TaxAmount;

    internal void AssignTo(CostDocument document)
    {
        CostDocument = document;
        CostDocumentId = document.Id;
    }
}
