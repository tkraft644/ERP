using System.Text.Json;
using ErpSystem.Application.Common;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.System.Auditing;

namespace ErpSystem.Application.Modules.Contractors;

public sealed class ContractorService : IContractorService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IContractorRepository repository;
    private readonly ICurrentUserAccessor currentUserAccessor;

    public ContractorService(IContractorRepository repository, ICurrentUserAccessor currentUserAccessor)
    {
        this.repository = repository;
        this.currentUserAccessor = currentUserAccessor;
    }

    public async Task<IReadOnlyList<ContractorListItemView>> GetContractorsAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var contractors = await repository.GetContractorsAsync(includeInactive, cancellationToken);
        return contractors
            .OrderBy(item => item.Name)
            .ThenBy(item => item.Code)
            .Select(MapListItem)
            .ToArray();
    }

    public async Task<ContractorDetailsView?> GetContractorAsync(int contractorId, CancellationToken cancellationToken = default)
    {
        var contractor = await repository.GetContractorAsync(contractorId, cancellationToken);
        return contractor is null ? null : MapDetails(contractor);
    }

    public async Task<ContractorDetailsView> CreateContractorAsync(SaveContractorRequest request, CancellationToken cancellationToken = default)
    {
        var contractor = BuildContractor(request);

        await repository.AddContractorAsync(contractor, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var createdView = MapDetails(contractor);
        await repository.AddAuditLogAsync(CreateAuditLog(
            contractor.Id,
            "Created",
            oldValues: "{}",
            newValues: Serialize(createdView),
            summary: $"Created contractor '{contractor.Name}' ({contractor.Code})."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return createdView;
    }

    public async Task<ContractorDetailsView?> UpdateContractorAsync(int contractorId, SaveContractorRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating a contractor.", nameof(request));
        }

        var contractor = await repository.GetContractorForUpdateAsync(contractorId, cancellationToken);
        if (contractor is null)
        {
            return null;
        }

        var previousView = MapDetails(contractor);
        repository.SetOriginalRowVersion(contractor, request.RowVersion);

        contractor.Update(
            NormalizeRequired(request.Code, nameof(request.Code)),
            NormalizeRequired(request.Name, nameof(request.Name)),
            NormalizeOptional(request.ShortName),
            NormalizeOptional(request.TaxId),
            ParseContractorTypes(request.Types),
            request.IsActive);
        contractor.ReplaceAddresses(MapAddresses(request.Addresses));
        contractor.ReplaceContacts(MapContacts(request.Contacts));
        contractor.ReplaceBankAccounts(MapBankAccounts(request.BankAccounts));
        contractor.ReplaceNotes(MapNotes(request.Notes));

        var updatedViewForAudit = MapDetails(contractor);
        await repository.AddAuditLogAsync(CreateAuditLog(
            contractorId,
            "Updated",
            oldValues: Serialize(previousView),
            newValues: Serialize(updatedViewForAudit),
            summary: $"Updated contractor '{contractor.Name}' ({contractor.Code})."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapDetails(contractor);
    }

    public async Task<IReadOnlyList<ContractorHistoryEntryView>> GetHistoryAsync(int contractorId, CancellationToken cancellationToken = default)
    {
        var entries = await repository.GetContractorHistoryAsync(contractorId, cancellationToken);
        return entries
            .OrderByDescending(item => item.ChangedAtUtc)
            .Select(item => new ContractorHistoryEntryView(
                item.Id,
                item.ActionName,
                item.ChangedByUserId,
                item.ChangedAtUtc,
                item.Summary,
                item.OldValues,
                item.NewValues))
            .ToArray();
    }

    public IReadOnlyList<ContractorOptionView> GetAvailableContractorTypes()
        => Enum.GetValues<ContractorType>()
            .Where(item => item != ContractorType.None)
            .Select(item => new ContractorOptionView(item.ToString(), item switch
            {
                ContractorType.Client => "Klient",
                ContractorType.Supplier => "Dostawca",
                ContractorType.Carrier => "Przewoźnik",
                ContractorType.Receiver => "Odbiorca",
                ContractorType.Sender => "Nadawca",
                ContractorType.CustomsAgency => "Agencja celna",
                ContractorType.Service => "Serwis",
                _ => item.ToString()
            }))
            .ToArray();

    public IReadOnlyList<ContractorOptionView> GetAvailableAddressKinds()
        => Enum.GetValues<ContractorAddressKind>()
            .Select(item => new ContractorOptionView(item.ToString(), item switch
            {
                ContractorAddressKind.Registered => "Adres rejestrowy",
                ContractorAddressKind.Correspondence => "Adres korespondencyjny",
                ContractorAddressKind.Loading => "Adres załadunku",
                ContractorAddressKind.Unloading => "Adres rozładunku",
                ContractorAddressKind.Other => "Inny",
                _ => item.ToString()
            }))
            .ToArray();

    private AuditLog CreateAuditLog(int contractorId, string actionName, string oldValues, string newValues, string summary)
    {
        return new AuditLog(
            "Contractor",
            contractorId,
            actionName,
            currentUserAccessor.UserId,
            oldValues,
            newValues,
            summary)
        {
            ChangedAtUtc = DateTime.UtcNow
        };
    }

    private static Contractor BuildContractor(SaveContractorRequest request)
    {
        var contractor = new Contractor(
            NormalizeRequired(request.Code, nameof(request.Code)),
            NormalizeRequired(request.Name, nameof(request.Name)),
            NormalizeOptional(request.ShortName),
            NormalizeOptional(request.TaxId),
            ParseContractorTypes(request.Types),
            request.IsActive);
        contractor.ReplaceAddresses(MapAddresses(request.Addresses));
        contractor.ReplaceContacts(MapContacts(request.Contacts));
        contractor.ReplaceBankAccounts(MapBankAccounts(request.BankAccounts));
        contractor.ReplaceNotes(MapNotes(request.Notes));
        return contractor;
    }

    private static IReadOnlyList<ContractorAddress> MapAddresses(IReadOnlyList<SaveContractorAddressRequest> requests)
        => requests.Select(item => new ContractorAddress(
                ParseAddressKind(item.Kind),
                NormalizeRequired(item.Label, nameof(item.Label)),
                NormalizeRequired(item.CountryCode, nameof(item.CountryCode)).ToUpperInvariant(),
                NormalizeRequired(item.PostalCode, nameof(item.PostalCode)),
                NormalizeRequired(item.City, nameof(item.City)),
                NormalizeRequired(item.Street, nameof(item.Street)),
                NormalizeRequired(item.BuildingNumber, nameof(item.BuildingNumber)),
                NormalizeOptional(item.ApartmentNumber),
                item.IsPrimary))
            .ToArray();

    private static IReadOnlyList<ContractorContact> MapContacts(IReadOnlyList<SaveContractorContactRequest> requests)
        => requests.Select(item => new ContractorContact(
                NormalizeRequired(item.FullName, nameof(item.FullName)),
                NormalizeOptional(item.Position),
                NormalizeOptional(item.Email),
                NormalizeOptional(item.PhoneNumber),
                item.IsPrimary))
            .ToArray();

    private static IReadOnlyList<ContractorBankAccount> MapBankAccounts(IReadOnlyList<SaveContractorBankAccountRequest> requests)
        => requests.Select(item => new ContractorBankAccount(
                NormalizeRequired(item.BankName, nameof(item.BankName)),
                NormalizeRequired(item.AccountNumber, nameof(item.AccountNumber)),
                NormalizeRequired(item.CurrencyCode, nameof(item.CurrencyCode)).ToUpperInvariant(),
                NormalizeOptional(item.Swift),
                item.IsPrimary))
            .ToArray();

    private static IReadOnlyList<ContractorNote> MapNotes(IReadOnlyList<SaveContractorNoteRequest> requests)
        => requests.Select(item => new ContractorNote(
                NormalizeRequired(item.Title, nameof(item.Title)),
                NormalizeRequired(item.Content, nameof(item.Content))))
            .ToArray();

    private static ContractorListItemView MapListItem(Contractor contractor)
    {
        var primaryAddress = contractor.Addresses.FirstOrDefault(item => item.IsPrimary) ?? contractor.Addresses.FirstOrDefault();
        var primaryContact = contractor.Contacts.FirstOrDefault(item => item.IsPrimary) ?? contractor.Contacts.FirstOrDefault();

        return new ContractorListItemView(
            contractor.Id,
            contractor.Code,
            contractor.Name,
            contractor.ShortName,
            contractor.TaxId,
            contractor.IsActive,
            ExpandTypes(contractor.Types),
            primaryAddress?.City,
            primaryContact?.Email,
            primaryContact?.PhoneNumber,
            contractor.Addresses.Count,
            contractor.Contacts.Count,
            contractor.RowVersion);
    }

    private static ContractorDetailsView MapDetails(Contractor contractor)
    {
        return new ContractorDetailsView(
            contractor.Id,
            contractor.Code,
            contractor.Name,
            contractor.ShortName,
            contractor.TaxId,
            contractor.IsActive,
            ExpandTypes(contractor.Types),
            contractor.Addresses
                .OrderBy(item => item.Kind)
                .ThenByDescending(item => item.IsPrimary)
                .ThenBy(item => item.Label)
                .Select(item => new ContractorAddressView(
                    item.Id,
                    item.Kind.ToString(),
                    item.Label,
                    item.CountryCode,
                    item.PostalCode,
                    item.City,
                    item.Street,
                    item.BuildingNumber,
                    item.ApartmentNumber,
                    item.IsPrimary))
                .ToArray(),
            contractor.Contacts
                .OrderByDescending(item => item.IsPrimary)
                .ThenBy(item => item.FullName)
                .Select(item => new ContractorContactView(
                    item.Id,
                    item.FullName,
                    item.Position,
                    item.Email,
                    item.PhoneNumber,
                    item.IsPrimary))
                .ToArray(),
            contractor.BankAccounts
                .OrderByDescending(item => item.IsPrimary)
                .ThenBy(item => item.BankName)
                .Select(item => new ContractorBankAccountView(
                    item.Id,
                    item.BankName,
                    item.AccountNumber,
                    item.CurrencyCode,
                    item.Swift,
                    item.IsPrimary))
                .ToArray(),
            contractor.Notes
                .OrderBy(item => item.Title)
                .Select(item => new ContractorNoteView(
                    item.Id,
                    item.Title,
                    item.Content))
                .ToArray(),
            contractor.RowVersion);
    }

    private static IReadOnlyList<string> ExpandTypes(ContractorType types)
        => Enum.GetValues<ContractorType>()
            .Where(item => item != ContractorType.None && types.HasFlag(item))
            .Select(item => item.ToString())
            .ToArray();

    private static ContractorType ParseContractorTypes(IReadOnlyList<string> items)
    {
        if (items.Count == 0)
        {
            throw new ArgumentException("At least one contractor type is required.", nameof(items));
        }

        ContractorType result = ContractorType.None;
        foreach (var item in items)
        {
            if (!Enum.TryParse<ContractorType>(item, ignoreCase: true, out var parsed) || parsed == ContractorType.None)
            {
                throw new ArgumentException($"Unknown contractor type '{item}'.", nameof(items));
            }

            result |= parsed;
        }

        return result;
    }

    private static ContractorAddressKind ParseAddressKind(string value)
    {
        if (!Enum.TryParse<ContractorAddressKind>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown address kind '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static string NormalizeRequired(string? value, string fieldName)
    {
        if (value is null)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        var normalized = value.Trim();
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length == 0 ? null : normalized;
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, SerializerOptions);
}
