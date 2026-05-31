namespace ErpSystem.Domain.Modules.Contractors;

[Flags]
public enum ContractorType
{
    None = 0,
    Client = 1,
    Supplier = 2,
    Carrier = 4,
    Receiver = 8,
    Sender = 16,
    CustomsAgency = 32,
    Service = 64
}
