namespace ErpSystem.Desktop.Services;

public sealed class ContractorWorkspaceStore
{
    private readonly List<ContractorRecord> contractors = [];
    private int nextContractorId = 10;

    private ContractorWorkspaceStore()
    {
        Seed();
    }

    public static ContractorWorkspaceStore Instance { get; } = new();

    public event EventHandler? Changed;

    public IReadOnlyList<ContractorRecord> GetContractors()
        => contractors
            .OrderBy(item => item.Name)
            .ThenBy(item => item.Code)
            .ToArray();

    public IReadOnlyList<string> GetTypes()
        => contractors
            .SelectMany(item => item.Types)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .ToArray();

    public ContractorRecord CreateContractor(
        string code,
        string name,
        string shortName,
        string taxId,
        string city,
        string email,
        string phone,
        string paymentTerms,
        string creditLimit,
        string bankAccount,
        string typeList,
        bool isActive,
        string primaryAddress,
        string notes)
    {
        Validate(code, name, null);

        var contractor = new ContractorRecord(
            nextContractorId++,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            NormalizeOptional(shortName),
            NormalizeOptional(taxId),
            NormalizeOptional(city),
            NormalizeOptional(email),
            NormalizeOptional(phone),
            NormalizeOptional(paymentTerms),
            NormalizeOptional(creditLimit),
            NormalizeOptional(bankAccount),
            ParseTypes(typeList),
            isActive,
            false,
            NormalizeOptional(primaryAddress) ?? "-",
            NormalizeOptional(notes),
            BuildAddresses(primaryAddress, city),
            BuildContacts(name, email, phone),
            BuildHistory("Utworzono kontrahenta", "Administrator ERP"),
            DateTime.UtcNow);

        contractors.Add(contractor);
        RaiseChanged();
        return contractor;
    }

    public ContractorRecord UpdateContractor(
        int id,
        string code,
        string name,
        string shortName,
        string taxId,
        string city,
        string email,
        string phone,
        string paymentTerms,
        string creditLimit,
        string bankAccount,
        string typeList,
        bool isActive,
        string primaryAddress,
        string notes)
    {
        var current = contractors.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono kontrahenta.");

        Validate(code, name, id);

        var updated = current with
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            ShortName = NormalizeOptional(shortName),
            TaxId = NormalizeOptional(taxId),
            City = NormalizeOptional(city),
            Email = NormalizeOptional(email),
            Phone = NormalizeOptional(phone),
            PaymentTerms = NormalizeOptional(paymentTerms),
            CreditLimit = NormalizeOptional(creditLimit),
            BankAccount = NormalizeOptional(bankAccount),
            Types = ParseTypes(typeList),
            IsActive = isActive,
            PrimaryAddress = NormalizeOptional(primaryAddress) ?? "-",
            Notes = NormalizeOptional(notes),
            Addresses = BuildAddresses(primaryAddress, city),
            Contacts = BuildContacts(name, email, phone),
            History = PrependHistory(current.History, "Zaktualizowano kartotekę", "Koordynator kontrahentów"),
            UpdatedAtUtc = DateTime.UtcNow
        };

        Replace(updated);
        RaiseChanged();
        return updated;
    }

    public void DeleteContractor(int id)
    {
        if (contractors.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono kontrahenta.");
        }

        RaiseChanged();
    }

    public void ArchiveContractor(int id)
    {
        var current = contractors.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono kontrahenta.");

        Replace(current with
        {
            IsArchived = true,
            IsActive = false,
            History = PrependHistory(current.History, "Przeniesiono do archiwum", "Koordynator kontrahentów"),
            UpdatedAtUtc = DateTime.UtcNow
        });
        RaiseChanged();
    }

    public void RestoreContractor(int id)
    {
        var current = contractors.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono kontrahenta.");

        Replace(current with
        {
            IsArchived = false,
            IsActive = true,
            History = PrependHistory(current.History, "Przywrócono do aktywnych", "Koordynator kontrahentów"),
            UpdatedAtUtc = DateTime.UtcNow
        });
        RaiseChanged();
    }

    private void Replace(ContractorRecord updated)
    {
        var index = contractors.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            contractors[index] = updated;
        }
    }

    private void Validate(string code, string name, int? currentId)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("Kod kontrahenta jest wymagany.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Nazwa kontrahenta jest wymagana.");
        }

        if (contractors.Any(item =>
                item.Id != currentId &&
                string.Equals(item.Code, code.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Kontrahent o takim kodzie już istnieje.");
        }
    }

    private static IReadOnlyList<string> ParseTypes(string? value)
    {
        var parsed = (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return parsed.Length == 0 ? ["Klient"] : parsed;
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static IReadOnlyList<ContractorAddressRecord> BuildAddresses(string? primaryAddress, string? city)
    {
        var label = NormalizeOptional(primaryAddress) ?? "Adres podstawowy";
        var cityLabel = NormalizeOptional(city) ?? "Brak miasta";
        return
        [
            new ContractorAddressRecord("Główny", $"{label}, {cityLabel}", true),
            new ContractorAddressRecord("Załadunek", $"{label}, {cityLabel}", false)
        ];
    }

    private static IReadOnlyList<ContractorContactRecord> BuildContacts(string name, string? email, string? phone)
    {
        return
        [
            new ContractorContactRecord(name.Trim(), NormalizeOptional(email) ?? "-", NormalizeOptional(phone) ?? "-", true)
        ];
    }

    private static IReadOnlyList<ContractorHistoryRecord> BuildHistory(string action, string actor)
    {
        return
        [
            new ContractorHistoryRecord(action, actor, DateTime.UtcNow)
        ];
    }

    private static IReadOnlyList<ContractorHistoryRecord> PrependHistory(
        IReadOnlyList<ContractorHistoryRecord> current,
        string action,
        string actor)
    {
        return
        [
            new ContractorHistoryRecord(action, actor, DateTime.UtcNow),
            .. current
        ];
    }

    private void Seed()
    {
        contractors.AddRange(
        [
            new ContractorRecord(
                nextContractorId++,
                "VOL-PARTS",
                "Volvo Parts Polska",
                "Volvo Parts",
                "5251102291",
                "Poznań",
                "zamowienia@volvoparts.pl",
                "+48 61 400 20 20",
                "14 dni",
                "120 000 zł",
                "PL77 1140 2004 0000 3202 1234 0001",
                ["Dostawca", "Nadawca"],
                true,
                false,
                "ul. Magazynowa 12",
                "Główny dostawca części zamiennych dla magazynu centralnego.",
                [
                    new ContractorAddressRecord("Główny", "ul. Magazynowa 12, Poznań", true),
                    new ContractorAddressRecord("Załadunek", "ul. Terminalowa 4, Poznań", false)
                ],
                [
                    new ContractorContactRecord("Agnieszka Pawlak", "zamowienia@volvoparts.pl", "+48 61 400 20 20", true),
                    new ContractorContactRecord("Krzysztof Duda", "logistyka@volvoparts.pl", "+48 61 400 20 44", false)
                ],
                [
                    new ContractorHistoryRecord("Utworzono kartotekę", "Administrator ERP", DateTime.UtcNow.AddDays(-40)),
                    new ContractorHistoryRecord("Zmieniono limity kupieckie", "Dział finansów", DateTime.UtcNow.AddDays(-4))
                ],
                DateTime.UtcNow.AddHours(-3)),
            new ContractorRecord(
                nextContractorId++,
                "SER-POLNOC",
                "Serwis Północ",
                "Serwis Pn.",
                "7792201188",
                "Gdańsk",
                "serwis@polnoc.pl",
                "+48 58 410 99 44",
                "7 dni",
                "45 000 zł",
                "PL58 1020 5558 1111 2222 3333 4444",
                ["Klient", "Odbiorca"],
                true,
                false,
                "ul. Stoczniowa 4",
                "Stały odbiorca części serwisowych i napraw powypadkowych.",
                [
                    new ContractorAddressRecord("Główny", "ul. Stoczniowa 4, Gdańsk", true),
                    new ContractorAddressRecord("Rozładunek", "ul. Portowa 17, Gdańsk", false)
                ],
                [
                    new ContractorContactRecord("Monika Wrona", "serwis@polnoc.pl", "+48 58 410 99 44", true)
                ],
                [
                    new ContractorHistoryRecord("Dodano kontrahenta", "Biuro obsługi klienta", DateTime.UtcNow.AddDays(-18))
                ],
                DateTime.UtcNow.AddHours(-6)),
            new ContractorRecord(
                nextContractorId++,
                "CMR-LOG",
                "CMR Logistic Group",
                "CMR Logistic",
                "7811902215",
                "Wrocław",
                "dispatch@cmrlog.eu",
                "+48 71 800 12 66",
                "21 dni",
                "80 000 zł",
                "PL27 1050 0099 7603 1234 1000 8877",
                ["Przewoźnik", "Agencja celna"],
                true,
                false,
                "ul. Celna 8",
                "Obsługuje przewozy międzynarodowe i odprawy celne.",
                [
                    new ContractorAddressRecord("Główny", "ul. Celna 8, Wrocław", true),
                    new ContractorAddressRecord("Załadunek", "Terminal 3, Bielany Wrocławskie", false)
                ],
                [
                    new ContractorContactRecord("Rafał Jasiński", "dispatch@cmrlog.eu", "+48 71 800 12 66", true)
                ],
                [
                    new ContractorHistoryRecord("Zmieniono adres terminala", "Transport", DateTime.UtcNow.AddDays(-2))
                ],
                DateTime.UtcNow.AddHours(-1)),
            new ContractorRecord(
                nextContractorId++,
                "COLORMIX",
                "ColorMix Service",
                "ColorMix",
                "8943001120",
                "Łódź",
                "biuro@colormix.pl",
                "+48 42 225 10 09",
                "14 dni",
                "20 000 zł",
                "PL65 1940 1076 3000 0550 0000 7654",
                ["Serwis", "Dostawca"],
                false,
                true,
                "ul. Lakiernicza 2",
                "Kartoteka archiwalna po zamknięciu współpracy.",
                [
                    new ContractorAddressRecord("Główny", "ul. Lakiernicza 2, Łódź", true)
                ],
                [
                    new ContractorContactRecord("Dział handlowy", "biuro@colormix.pl", "+48 42 225 10 09", true)
                ],
                [
                    new ContractorHistoryRecord("Przeniesiono do archiwum", "Zakupy", DateTime.UtcNow.AddDays(-12))
                ],
                DateTime.UtcNow.AddDays(-12))
        ]);
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}

public sealed record ContractorRecord(
    int Id,
    string Code,
    string Name,
    string? ShortName,
    string? TaxId,
    string? City,
    string? Email,
    string? Phone,
    string? PaymentTerms,
    string? CreditLimit,
    string? BankAccount,
    IReadOnlyList<string> Types,
    bool IsActive,
    bool IsArchived,
    string PrimaryAddress,
    string? Notes,
    IReadOnlyList<ContractorAddressRecord> Addresses,
    IReadOnlyList<ContractorContactRecord> Contacts,
    IReadOnlyList<ContractorHistoryRecord> History,
    DateTime UpdatedAtUtc);

public sealed record ContractorAddressRecord(string Kind, string Label, bool IsPrimary);

public sealed record ContractorContactRecord(string FullName, string Email, string Phone, bool IsPrimary);

public sealed record ContractorHistoryRecord(string Action, string Actor, DateTime AtUtc);
