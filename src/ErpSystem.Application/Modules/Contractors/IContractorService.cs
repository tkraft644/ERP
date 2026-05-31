namespace ErpSystem.Application.Modules.Contractors;

public interface IContractorService
{
    Task<IReadOnlyList<ContractorListItemView>> GetContractorsAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<ContractorDetailsView?> GetContractorAsync(int contractorId, CancellationToken cancellationToken = default);
    Task<ContractorDetailsView> CreateContractorAsync(SaveContractorRequest request, CancellationToken cancellationToken = default);
    Task<ContractorDetailsView?> UpdateContractorAsync(int contractorId, SaveContractorRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContractorHistoryEntryView>> GetHistoryAsync(int contractorId, CancellationToken cancellationToken = default);
    IReadOnlyList<ContractorOptionView> GetAvailableContractorTypes();
    IReadOnlyList<ContractorOptionView> GetAvailableAddressKinds();
}
