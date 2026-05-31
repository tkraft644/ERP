using ErpSystem.Domain.Modules.Contractors;

namespace ErpSystem.Domain.Modules.Transport;

public sealed class Carrier : Common.AuditableEntity
{
    private Carrier()
    {
    }

    public Carrier(int contractorId, string code, bool isPreferred = false, bool isActive = true)
    {
        ContractorId = contractorId;
        Code = code;
        IsPreferred = isPreferred;
        IsActive = isActive;
    }

    public int ContractorId { get; private set; }
    public Contractor Contractor { get; private set; } = null!;
    public string Code { get; private set; } = string.Empty;
    public bool IsPreferred { get; private set; }
    public bool IsActive { get; private set; }
}
