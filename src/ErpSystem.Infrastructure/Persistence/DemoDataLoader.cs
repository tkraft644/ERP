using ErpSystem.Application.Modules.Contractors;
using ErpSystem.Application.Modules.Finance;
using ErpSystem.Application.Modules.HR;
using ErpSystem.Application.Modules.Transport;
using ErpSystem.Application.Modules.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence;

public sealed class DemoDataLoader
{
    private readonly ErpSystemDbContext dbContext;
    private readonly IContractorService contractorService;
    private readonly IHrService hrService;
    private readonly ITransportService transportService;
    private readonly IFinanceService financeService;
    private readonly IWarehouseService warehouseService;

    public DemoDataLoader(
        ErpSystemDbContext dbContext,
        IContractorService contractorService,
        IHrService hrService,
        ITransportService transportService,
        IFinanceService financeService,
        IWarehouseService warehouseService)
    {
        this.dbContext = dbContext;
        this.contractorService = contractorService;
        this.hrService = hrService;
        this.transportService = transportService;
        this.financeService = financeService;
        this.warehouseService = warehouseService;
    }

    public async Task<DemoDataLoadResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        var modules = new List<DemoDataModuleResult>();

        modules.Add(await EnsureContractorsAsync(cancellationToken));
        modules.Add(await EnsureHrAsync(cancellationToken));
        modules.Add(await EnsureWarehouseAsync(cancellationToken));
        modules.Add(await EnsureTransportAsync(cancellationToken));
        modules.Add(await EnsureFinanceAsync(cancellationToken));

        var createdCount = modules.Sum(item => item.CreatedCount);
        var summary = createdCount == 0
            ? "Demo data już były dostępne albo brakuje danych referencyjnych do części modułów."
            : $"Załadowano {createdCount} rekordów demonstracyjnych do bazy SQL.";

        return new DemoDataLoadResult(DateTime.UtcNow, createdCount, modules, summary);
    }

    private async Task<DemoDataModuleResult> EnsureContractorsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Contractors.AnyAsync(cancellationToken))
        {
            return new DemoDataModuleResult("Contractors", 0, "Kontrahenci już istnieją w bazie.");
        }

        SaveContractorRequest[] requests =
        [
            new SaveContractorRequest(
                "KLI-001",
                "Nord Steel Sp. z o.o.",
                "Nord Steel",
                "5253001122",
                true,
                ["Client", "Receiver"],
                [
                    new SaveContractorAddressRequest("Registered", "Siedziba", "PL", "61-248", "Poznań", "Metalowa", "18", null, true),
                    new SaveContractorAddressRequest("Unloading", "Magazyn odbiorcy", "PL", "62-030", "Luboń", "Magazynowa", "7", null, false)
                ],
                [
                    new SaveContractorContactRequest("Joanna Krawiec", "Zakupy", "zakupy@nordsteel.pl", "+48 600 120 120", true)
                ],
                [
                    new SaveContractorBankAccountRequest("Bank Polski", "12 1140 2004 0000 3002 0001 1234", "PLN", "BREXPLPW", true)
                ],
                [
                    new SaveContractorNoteRequest("Obsługa", "Kluczowy klient dla wysyłek krajowych.")
                ]),
            new SaveContractorRequest(
                "DST-001",
                "Volvo Parts Polska",
                "Volvo Parts",
                "7012234455",
                true,
                ["Supplier", "Sender"],
                [
                    new SaveContractorAddressRequest("Registered", "Siedziba", "PL", "50-429", "Wrocław", "Przemysłowa", "44", null, true),
                    new SaveContractorAddressRequest("Loading", "Magazyn wysyłkowy", "PL", "55-095", "Długołęka", "Logistyczna", "2", null, false)
                ],
                [
                    new SaveContractorContactRequest("Marek Domański", "Logistyka", "logistyka@volvoparts.pl", "+48 600 130 130", true)
                ],
                [
                    new SaveContractorBankAccountRequest("Bank Handlowy", "33 1030 1508 0000 0005 5000 5678", "PLN", "CITIPLPX", true)
                ],
                [
                    new SaveContractorNoteRequest("Dostawy", "Główny dostawca części do magazynu centralnego.")
                ]),
            new SaveContractorRequest(
                "PRZ-001",
                "Baltic Carrier S.A.",
                "Baltic Carrier",
                "5882300991",
                true,
                ["Carrier", "Service"],
                [
                    new SaveContractorAddressRequest("Registered", "Siedziba", "PL", "80-309", "Gdańsk", "Portowa", "91", null, true)
                ],
                [
                    new SaveContractorContactRequest("Paweł Urban", "Dispatch", "dispatch@balticcarrier.pl", "+48 600 140 140", true)
                ],
                [
                    new SaveContractorBankAccountRequest("Santander", "54 1090 1098 0000 0001 9000 9012", "PLN", "WBKPPLPP", true)
                ],
                [
                    new SaveContractorNoteRequest("Transport", "Przewoźnik obsługujący trasy krajowe i eksportowe.")
                ])
        ];

        foreach (var request in requests)
        {
            await contractorService.CreateContractorAsync(request, cancellationToken);
        }

        return new DemoDataModuleResult("Contractors", requests.Length, "Dodano podstawowych kontrahentów dla magazynu, transportu i finansów.");
    }

    private async Task<DemoDataModuleResult> EnsureHrAsync(CancellationToken cancellationToken)
    {
        var createdCount = 0;
        var referenceData = await hrService.GetReferenceDataAsync(cancellationToken);
        if (referenceData.Departments.Count == 0 || referenceData.Positions.Count == 0)
        {
            return new DemoDataModuleResult("HR", 0, "Pominięto HR, bo brak działów albo stanowisk referencyjnych.");
        }

        if (!await dbContext.Employees.AnyAsync(cancellationToken))
        {
            var logisticsDepartment = referenceData.Departments.First();
            var warehousePosition = referenceData.Positions.First();
            var contractType = referenceData.ContractTypes.FirstOrDefault()?.Key ?? "Employment";

            SaveEmployeeRequest[] employees =
            [
                new SaveEmployeeRequest(
                    "EMP-3001",
                    "Anna",
                    "Nowak",
                    "anna.nowak@erp.demo",
                    "+48 600 200 200",
                    "92031411223",
                    true,
                    [
                        new SaveEmploymentContractRequest(
                            "UOP/2026/3001",
                            contractType,
                            logisticsDepartment.Id,
                            warehousePosition.Id,
                            new DateTime(2026, 1, 2),
                            null,
                            1.0m,
                            7200m,
                            "Koordynacja pracy magazynu.")
                    ],
                    [
                        new SaveWorkScheduleRequest("Standard 8h", 40m, new DateTime(2026, 1, 2), null, "Zmiana dzienna.")
                    ],
                    [
                        new SaveEmployeeDocumentRequest("Medical", "Badania wstępne", "MED-3001", new DateTime(2026, 1, 2), new DateTime(2027, 1, 2), "Ważne rok.", true)
                    ],
                    null),
                new SaveEmployeeRequest(
                    "EMP-3002",
                    "Piotr",
                    "Lis",
                    "piotr.lis@erp.demo",
                    "+48 600 210 210",
                    "89120155678",
                    true,
                    [
                        new SaveEmploymentContractRequest(
                            "UOP/2026/3002",
                            contractType,
                            logisticsDepartment.Id,
                            warehousePosition.Id,
                            new DateTime(2026, 2, 1),
                            null,
                            1.0m,
                            6800m,
                            "Operator z profilem kierowcy.")
                    ],
                    [
                        new SaveWorkScheduleRequest("Standard 8h", 40m, new DateTime(2026, 2, 1), null, "Zmiana poranna.")
                    ],
                    [],
                    new SaveDriverProfileRequest("DRV-2026-9002", "B,C,E", new DateTime(2029, 2, 1), "CARD-9002", new DateTime(2029, 2, 1), null))
            ];

            foreach (var employee in employees)
            {
                await hrService.CreateEmployeeAsync(employee, cancellationToken);
                createdCount++;
            }

            referenceData = await hrService.GetReferenceDataAsync(cancellationToken);
        }

        if (!await dbContext.LeaveRequests.AnyAsync(cancellationToken) &&
            referenceData.Employees.Count > 0 &&
            referenceData.LeaveTypes.Count > 0)
        {
            await hrService.CreateLeaveRequestAsync(
                new SaveLeaveRequestRequest(
                    referenceData.Employees.First().Id,
                    referenceData.LeaveTypes.First().Id,
                    new DateTime(2026, 7, 14),
                    new DateTime(2026, 7, 18),
                    5m,
                    "Przykładowy urlop wypoczynkowy."),
                cancellationToken);
            createdCount++;
        }

        return new DemoDataModuleResult("HR", createdCount, createdCount == 0
            ? "HR już posiada dane."
            : "Dodano pracowników i przykładowy wniosek urlopowy.");
    }

    private async Task<DemoDataModuleResult> EnsureWarehouseAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.WarehouseDocuments.AnyAsync(cancellationToken))
        {
            return new DemoDataModuleResult("Warehouse", 0, "Dokumenty magazynowe już istnieją w bazie.");
        }

        var referenceData = await warehouseService.GetReferenceDataAsync(cancellationToken);
        if (referenceData.Warehouses.Count < 2 || referenceData.Locations.Count == 0 || referenceData.Products.Count < 3)
        {
            return new DemoDataModuleResult("Warehouse", 0, "Pominięto magazyn, bo brakuje referencyjnych magazynów, lokalizacji albo produktów.");
        }

        var contractors = await dbContext.Contractors
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToArrayAsync(cancellationToken);

        var mainWarehouse = referenceData.Warehouses.First();
        var transitWarehouse = referenceData.Warehouses.Skip(1).FirstOrDefault() ?? mainWarehouse;
        var targetLocation = referenceData.Locations.First(item => item.WarehouseId == mainWarehouse.Id);
        var transitLocation = referenceData.Locations.FirstOrDefault(item => item.WarehouseId == transitWarehouse.Id) ?? targetLocation;

        var supplierId = contractors.FirstOrDefault(item => item.Types.HasFlag(Domain.Modules.Contractors.ContractorType.Supplier))?.Id;
        var clientId = contractors.FirstOrDefault(item => item.Types.HasFlag(Domain.Modules.Contractors.ContractorType.Client))?.Id;

        var firstProduct = referenceData.Products[0];
        var secondProduct = referenceData.Products[Math.Min(1, referenceData.Products.Count - 1)];
        var thirdProduct = referenceData.Products[Math.Min(2, referenceData.Products.Count - 1)];

        await CreateAndMaybePostAsync(
            new SaveWarehouseDocumentRequest(
                "PZ",
                new DateTime(2026, 5, 14, 7, 40, 0),
                supplierId,
                null,
                null,
                mainWarehouse.Id,
                targetLocation.Id,
                "DEMO-PZ-001",
                "Przyjęcie startowe dla danych magazynowych.",
                [
                    new SaveWarehouseDocumentPositionRequest(firstProduct.Id, 50m, 12.50m, null, targetLocation.Id, "Pozycja startowa.")
                ]),
            shouldPost: true,
            cancellationToken);

        await CreateAndMaybePostAsync(
            new SaveWarehouseDocumentRequest(
                "PZ",
                new DateTime(2026, 5, 14, 8, 15, 0),
                supplierId,
                null,
                null,
                transitWarehouse.Id,
                transitLocation.Id,
                "DEMO-PZ-002",
                "Dostawa opakowań do magazynu pomocniczego.",
                [
                    new SaveWarehouseDocumentPositionRequest(secondProduct.Id, 25m, 34m, null, transitLocation.Id, null)
                ]),
            shouldPost: true,
            cancellationToken);

        await CreateAndMaybePostAsync(
            new SaveWarehouseDocumentRequest(
                "PW",
                new DateTime(2026, 5, 14, 6, 30, 0),
                null,
                null,
                null,
                mainWarehouse.Id,
                targetLocation.Id,
                "DEMO-PW-001",
                "Przyjęcie wewnętrzne materiału pomocniczego.",
                [
                    new SaveWarehouseDocumentPositionRequest(thirdProduct.Id, 6m, 18m, null, targetLocation.Id, null)
                ]),
            shouldPost: true,
            cancellationToken);

        await CreateAndMaybePostAsync(
            new SaveWarehouseDocumentRequest(
                "WZ",
                new DateTime(2026, 5, 15, 6, 45, 0),
                clientId,
                mainWarehouse.Id,
                targetLocation.Id,
                null,
                null,
                "DEMO-WZ-001",
                "Wydanie oczekujące na zatwierdzenie.",
                [
                    new SaveWarehouseDocumentPositionRequest(firstProduct.Id, 8m, 12.50m, targetLocation.Id, null, null)
                ]),
            shouldPost: false,
            cancellationToken);

        return new DemoDataModuleResult("Warehouse", 4, "Dodano dokumenty magazynowe i stany startowe.");
    }

    private async Task<DemoDataModuleResult> EnsureTransportAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.TransportOrders.AnyAsync(cancellationToken))
        {
            return new DemoDataModuleResult("Transport", 0, "Zlecenia transportowe już istnieją w bazie.");
        }

        var referenceData = await transportService.GetReferenceDataAsync(cancellationToken);
        if (referenceData.Partners.Count < 2 || referenceData.Carriers.Count == 0 || referenceData.Vehicles.Count == 0 || referenceData.Drivers.Count == 0)
        {
            return new DemoDataModuleResult("Transport", 0, "Pominięto transport, bo brakuje partnerów, kierowców albo floty referencyjnej.");
        }

        var carrier = referenceData.Carriers.First();
        var vehicle = referenceData.Vehicles.FirstOrDefault(item => item.CarrierId == carrier.Id) ?? referenceData.Vehicles.First();
        var trailer = referenceData.Trailers.FirstOrDefault(item => item.CarrierId == carrier.Id) ?? referenceData.Trailers.FirstOrDefault();
        var driver = referenceData.Drivers.First();
        var sender = referenceData.Partners.First();
        var receiver = referenceData.Partners.Skip(1).FirstOrDefault() ?? sender;

        SaveTransportOrderRequest[] requests =
        [
            new SaveTransportOrderRequest(
                "Domestic",
                new DateTime(2026, 5, 15, 7, 0, 0),
                carrier.Id,
                vehicle.Id,
                trailer?.Id,
                driver.Id,
                null,
                null,
                null,
                null,
                null,
                false,
                false,
                "Wielkopolskie",
                "Całopojazdowy",
                "TR-DEMO-001",
                "Dystrybucja krajowa dla klienta hurtowego.",
                new SaveTransportRouteRequest(320m, 4200m, new DateTime(2026, 5, 16, 5, 30, 0, DateTimeKind.Utc), new DateTime(2026, 5, 16, 14, 0, 0, DateTimeKind.Utc), "Poznań -> Łódź"),
                [
                    new SaveTransportStopRequest(1, "Loading", sender.Id, sender.Name, "PL", "Poznań", "Magazynowa 7", new DateTime(2026, 5, 16, 5, 30, 0, DateTimeKind.Utc), null, null),
                    new SaveTransportStopRequest(2, "Unloading", receiver.Id, receiver.Name, "PL", "Łódź", "Przemysłowa 12", new DateTime(2026, 5, 16, 14, 0, 0, DateTimeKind.Utc), null, null)
                ],
                [
                    new SaveTransportDocumentRequest("Other", "TRDOC-001", "zlecenie-krajowe.pdf", DateTime.UtcNow, null, true)
                ],
                [
                    new SaveTransportCostRequest("Paliwo", "Koszt paliwa", "PLN", 950m, null, true)
                ]),
            new SaveTransportOrderRequest(
                "International",
                new DateTime(2026, 5, 15, 8, 20, 0),
                carrier.Id,
                vehicle.Id,
                trailer?.Id,
                driver.Id,
                "PL",
                "DE",
                "DAP",
                "EUR",
                4.28m,
                true,
                true,
                null,
                null,
                "TR-DEMO-002",
                "Transport eksportowy z odprawą celną i CMR.",
                new SaveTransportRouteRequest(840m, 2100m, new DateTime(2026, 5, 17, 4, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 17, 18, 30, 0, DateTimeKind.Utc), "Wrocław -> Berlin"),
                [
                    new SaveTransportStopRequest(1, "Loading", sender.Id, sender.Name, "PL", "Wrocław", "Logistyczna 4", new DateTime(2026, 5, 17, 4, 0, 0, DateTimeKind.Utc), null, null),
                    new SaveTransportStopRequest(2, "Unloading", receiver.Id, receiver.Name, "DE", "Berlin", "Ring 18", new DateTime(2026, 5, 17, 18, 30, 0, DateTimeKind.Utc), null, null)
                ],
                [
                    new SaveTransportDocumentRequest("CMR", "CMR-2026-0001", "cmr-berlin.pdf", DateTime.UtcNow, null, true)
                ],
                [
                    new SaveTransportCostRequest("Opłata drogowa", "Myto zagraniczne", "EUR", 180m, 4.28m, false)
                ])
        ];

        foreach (var request in requests)
        {
            await transportService.CreateOrderAsync(request, cancellationToken);
        }

        return new DemoDataModuleResult("Transport", requests.Length, "Dodano krajowe i zagraniczne zlecenia transportowe.");
    }

    private async Task<DemoDataModuleResult> EnsureFinanceAsync(CancellationToken cancellationToken)
    {
        var createdCount = 0;
        var referenceData = await financeService.GetReferenceDataAsync(cancellationToken);
        if (referenceData.Contractors.Count == 0 || referenceData.Currencies.Count == 0)
        {
            return new DemoDataModuleResult("Finance", 0, "Pominięto finanse, bo brakuje kontrahentów albo walut referencyjnych.");
        }

        var contractorId = referenceData.Contractors.First().Id;
        var currencyCode = referenceData.Currencies.First().Code;
        var transportOrderId = referenceData.TransportOrders.FirstOrDefault()?.Id;
        var vehicleId = referenceData.Vehicles.FirstOrDefault()?.Id;
        var employeeId = referenceData.Employees.FirstOrDefault()?.Id;
        var warehouseId = referenceData.Warehouses.FirstOrDefault()?.Id;
        var departmentId = referenceData.Departments.FirstOrDefault()?.Id;

        if (!await dbContext.CostDocuments.AnyAsync(cancellationToken))
        {
            await financeService.CreateCostDocumentAsync(
                new SaveCostDocumentRequest(
                    new DateTime(2026, 5, 15),
                    new DateTime(2026, 5, 15),
                    new DateTime(2026, 5, 29),
                    contractorId,
                    currencyCode,
                    currencyCode == "PLN" ? null : 4.28m,
                    "KOSZT-DEMO-001",
                    "Koszty serwisu i operacji transportowej.",
                    [
                        new SaveCostPositionRequest(1, "Serwis", "Przegląd zestawu", 1m, 1850m, 23m, transportOrderId, vehicleId, null, null, contractorId, null, "Koszt floty"),
                        new SaveCostPositionRequest(2, "Delegacja", "Obsługa załadunku", 1m, 420m, 23m, null, null, employeeId, warehouseId, null, departmentId, "Koszt operacyjny")
                    ]),
                cancellationToken);
            createdCount++;
        }

        if (!await dbContext.Invoices.AnyAsync(cancellationToken))
        {
            await financeService.CreateInvoiceAsync(
                new SaveInvoiceRequest(
                    new DateTime(2026, 5, 15),
                    new DateTime(2026, 5, 15),
                    new DateTime(2026, 5, 30),
                    contractorId,
                    transportOrderId,
                    currencyCode,
                    currencyCode == "PLN" ? null : 4.28m,
                    "FV-DEMO-001",
                    "Faktura sprzedaży za obsługę zlecenia transportowego.",
                    [
                        new SaveInvoicePositionRequest(1, "Transport", "Obsługa pełnego zlecenia", 1m, 5400m, 23m, null)
                    ]),
                cancellationToken);
            createdCount++;
        }

        if (!await dbContext.Payments.AnyAsync(cancellationToken))
        {
            await financeService.CreatePaymentAsync(
                new SavePaymentRequest(
                    new DateTime(2026, 5, 18),
                    "Incoming",
                    contractorId,
                    currencyCode,
                    currencyCode == "PLN" ? null : 4.28m,
                    6642m,
                    "Przelew",
                    "PAY-DEMO-001",
                    "Wpływ do przykładowej faktury."),
                cancellationToken);
            createdCount++;
        }

        if (!await dbContext.Settlements.AnyAsync(cancellationToken))
        {
            var paymentId = await dbContext.Payments
                .AsNoTracking()
                .OrderBy(item => item.Id)
                .Select(item => item.Id)
                .FirstOrDefaultAsync(cancellationToken);
            var invoiceId = await dbContext.Invoices
                .AsNoTracking()
                .OrderBy(item => item.Id)
                .Select(item => item.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (paymentId > 0 && invoiceId > 0)
            {
                await financeService.CreateSettlementAsync(
                    new SaveSettlementRequest(paymentId, invoiceId, null, 6642m, "Pełne rozliczenie przykładowej faktury."),
                    cancellationToken);
                createdCount++;
            }
        }

        return new DemoDataModuleResult("Finance", createdCount, createdCount == 0
            ? "Finanse już posiadają dane demonstracyjne."
            : "Dodano koszty, faktury, płatności i rozliczenia.");
    }

    private async Task CreateAndMaybePostAsync(
        SaveWarehouseDocumentRequest request,
        bool shouldPost,
        CancellationToken cancellationToken)
    {
        var createdDocument = await warehouseService.CreateDocumentAsync(request, cancellationToken);
        if (!shouldPost)
        {
            return;
        }

        await warehouseService.PostDocumentAsync(
            createdDocument.Id,
            new PostWarehouseDocumentRequest(createdDocument.RowVersion),
            cancellationToken);
    }
}

public sealed record DemoDataLoadResult(
    DateTime LoadedAtUtc,
    int CreatedCount,
    IReadOnlyList<DemoDataModuleResult> Modules,
    string Summary);

public sealed record DemoDataModuleResult(
    string ModuleKey,
    int CreatedCount,
    string Message);
