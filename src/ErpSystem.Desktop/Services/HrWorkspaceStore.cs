using System.Globalization;

namespace ErpSystem.Desktop.Services;

public sealed class HrWorkspaceStore
{
    private readonly List<HrDepartmentRecord> departments = [];
    private readonly List<HrPositionRecord> positions = [];
    private readonly List<HrEmployeeRecord> employees = [];
    private readonly List<HrContractRecord> contracts = [];
    private readonly List<HrLeaveTypeRecord> leaveTypes = [];
    private readonly List<HrLeaveRequestRecord> leaveRequests = [];
    private readonly List<HrEmployeeDocumentRecord> documents = [];
    private int nextDepartmentId = 10;
    private int nextPositionId = 10;
    private int nextEmployeeId = 20;
    private int nextContractId = 50;
    private int nextLeaveRequestId = 100;
    private int nextDocumentId = 200;

    private HrWorkspaceStore()
    {
        Seed();
    }

    public static HrWorkspaceStore Instance { get; } = new();

    public event EventHandler? Changed;

    public IReadOnlyList<HrDepartmentRecord> GetDepartments()
        => departments.OrderBy(item => item.Name).ToArray();

    public IReadOnlyList<HrPositionRecord> GetPositions()
        => positions.OrderBy(item => item.Name).ToArray();

    public IReadOnlyList<HrEmployeeRecord> GetEmployees()
        => employees.OrderBy(item => item.LastName).ThenBy(item => item.FirstName).ToArray();

    public IReadOnlyList<HrContractRecord> GetContracts()
        => contracts.OrderByDescending(item => item.StartDate).ThenBy(item => item.ContractNumber).ToArray();

    public IReadOnlyList<HrLeaveTypeRecord> GetLeaveTypes()
        => leaveTypes.OrderBy(item => item.SortOrder).ThenBy(item => item.Name).ToArray();

    public IReadOnlyList<HrLeaveRequestRecord> GetLeaveRequests()
        => leaveRequests.OrderByDescending(item => item.DateFrom).ThenByDescending(item => item.Id).ToArray();

    public IReadOnlyList<HrEmployeeDocumentRecord> GetDocuments()
        => documents.OrderBy(item => item.EmployeeId).ThenBy(item => item.DocumentType).ThenBy(item => item.Title).ToArray();

    public string GetDepartmentName(int? departmentId)
        => departmentId is null
            ? "-"
            : departments.FirstOrDefault(item => item.Id == departmentId)?.Name ?? "-";

    public string GetPositionName(int? positionId)
        => positionId is null
            ? "-"
            : positions.FirstOrDefault(item => item.Id == positionId)?.Name ?? "-";

    public string GetEmployeeName(int employeeId)
        => employees.FirstOrDefault(item => item.Id == employeeId)?.FullName ?? "-";

    public string GetLeaveTypeName(int leaveTypeId)
        => leaveTypes.FirstOrDefault(item => item.Id == leaveTypeId)?.Name ?? "-";

    public HrDepartmentRecord CreateDepartment(string code, string name, string description, bool isActive)
    {
        ValidateDepartment(code, name, null);
        var department = new HrDepartmentRecord(nextDepartmentId++, code.Trim().ToUpperInvariant(), name.Trim(), description.Trim(), isActive);
        departments.Add(department);
        RaiseChanged();
        return department;
    }

    public HrDepartmentRecord UpdateDepartment(int id, string code, string name, string description, bool isActive)
    {
        var current = departments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono działu.");
        ValidateDepartment(code, name, id);
        var updated = current with
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = description.Trim(),
            IsActive = isActive
        };
        ReplaceDepartment(updated);
        RaiseChanged();
        return updated;
    }

    public void ToggleDepartment(int id)
    {
        var current = departments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono działu.");
        ReplaceDepartment(current with { IsActive = !current.IsActive });
        RaiseChanged();
    }

    public void DeleteDepartment(int id)
    {
        var current = departments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono działu.");
        if (employees.Any(item => item.DepartmentId == id) || contracts.Any(item => item.DepartmentId == id && !item.IsArchived))
        {
            throw new InvalidOperationException("Nie można usunąć działu używanego przez pracowników lub umowy.");
        }

        departments.RemoveAll(item => item.Id == id);
        RaiseChanged();
    }

    public HrPositionRecord CreatePosition(string code, string name, string description, bool isActive)
    {
        ValidatePosition(code, name, null);
        var position = new HrPositionRecord(nextPositionId++, code.Trim().ToUpperInvariant(), name.Trim(), description.Trim(), isActive);
        positions.Add(position);
        RaiseChanged();
        return position;
    }

    public HrPositionRecord UpdatePosition(int id, string code, string name, string description, bool isActive)
    {
        var current = positions.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono stanowiska.");
        ValidatePosition(code, name, id);
        var updated = current with
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = description.Trim(),
            IsActive = isActive
        };
        ReplacePosition(updated);
        RaiseChanged();
        return updated;
    }

    public void TogglePosition(int id)
    {
        var current = positions.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono stanowiska.");
        ReplacePosition(current with { IsActive = !current.IsActive });
        RaiseChanged();
    }

    public void DeletePosition(int id)
    {
        var current = positions.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono stanowiska.");
        if (employees.Any(item => item.PositionId == id) || contracts.Any(item => item.PositionId == id && !item.IsArchived))
        {
            throw new InvalidOperationException("Nie można usunąć stanowiska używanego przez pracowników lub umowy.");
        }

        positions.RemoveAll(item => item.Id == id);
        RaiseChanged();
    }

    public HrEmployeeRecord CreateEmployee(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string personalId,
        int? departmentId,
        int? positionId,
        string primaryContractType,
        bool isActive,
        bool hasDriverProfile)
    {
        ValidateEmployee(employeeNumber, email, null);
        ValidateDepartmentPosition(departmentId, positionId);

        var employee = new HrEmployeeRecord(
            nextEmployeeId++,
            employeeNumber.Trim().ToUpperInvariant(),
            firstName.Trim(),
            lastName.Trim(),
            email.Trim(),
            NormalizeOptional(phoneNumber),
            NormalizeOptional(personalId),
            departmentId,
            positionId,
            NormalizeOptional(primaryContractType),
            isActive,
            hasDriverProfile,
            DateTime.Today);

        employees.Add(employee);
        RaiseChanged();
        return employee;
    }

    public HrEmployeeRecord UpdateEmployee(
        int id,
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string personalId,
        int? departmentId,
        int? positionId,
        string primaryContractType,
        bool isActive,
        bool hasDriverProfile)
    {
        var current = employees.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono pracownika.");

        ValidateEmployee(employeeNumber, email, id);
        ValidateDepartmentPosition(departmentId, positionId);

        var updated = current with
        {
            EmployeeNumber = employeeNumber.Trim().ToUpperInvariant(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim(),
            PhoneNumber = NormalizeOptional(phoneNumber),
            PersonalId = NormalizeOptional(personalId),
            DepartmentId = departmentId,
            PositionId = positionId,
            PrimaryContractType = NormalizeOptional(primaryContractType),
            IsActive = isActive,
            HasDriverProfile = hasDriverProfile
        };

        ReplaceEmployee(updated);
        RaiseChanged();
        return updated;
    }

    public void ToggleEmployee(int id)
    {
        var current = employees.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono pracownika.");
        ReplaceEmployee(current with { IsActive = !current.IsActive });
        RaiseChanged();
    }

    public void DeleteEmployee(int id)
    {
        var current = employees.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono pracownika.");

        employees.RemoveAll(item => item.Id == id);
        contracts.RemoveAll(item => item.EmployeeId == id);
        leaveRequests.RemoveAll(item => item.EmployeeId == id);
        documents.RemoveAll(item => item.EmployeeId == id);
        RaiseChanged();
    }

    public HrContractRecord CreateContract(
        int employeeId,
        string contractNumber,
        string contractType,
        int departmentId,
        int positionId,
        DateTime startDate,
        DateTime? endDate,
        decimal employmentRate,
        decimal? monthlySalary,
        string notes)
    {
        ValidateContract(employeeId, contractNumber, departmentId, positionId, startDate, endDate, employmentRate, null);

        var contract = new HrContractRecord(
            nextContractId++,
            employeeId,
            contractNumber.Trim().ToUpperInvariant(),
            contractType.Trim(),
            departmentId,
            positionId,
            startDate,
            endDate,
            employmentRate,
            monthlySalary,
            NormalizeOptional(notes),
            true,
            false);

        contracts.Add(contract);
        SyncEmployeeFromContract(contract);
        RaiseChanged();
        return contract;
    }

    public HrContractRecord UpdateContract(
        int id,
        int employeeId,
        string contractNumber,
        string contractType,
        int departmentId,
        int positionId,
        DateTime startDate,
        DateTime? endDate,
        decimal employmentRate,
        decimal? monthlySalary,
        string notes,
        bool isActive)
    {
        var current = contracts.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono umowy.");
        ValidateContract(employeeId, contractNumber, departmentId, positionId, startDate, endDate, employmentRate, id);

        var updated = current with
        {
            EmployeeId = employeeId,
            ContractNumber = contractNumber.Trim().ToUpperInvariant(),
            ContractType = contractType.Trim(),
            DepartmentId = departmentId,
            PositionId = positionId,
            StartDate = startDate,
            EndDate = endDate,
            EmploymentRate = employmentRate,
            MonthlySalary = monthlySalary,
            Notes = NormalizeOptional(notes),
            IsActive = isActive
        };

        ReplaceContract(updated);
        SyncEmployeeFromContract(updated);
        RaiseChanged();
        return updated;
    }

    public void ArchiveContract(int id)
    {
        var current = contracts.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono umowy.");
        ReplaceContract(current with { IsActive = false, IsArchived = true });
        RaiseChanged();
    }

    public void DeleteContract(int id)
    {
        var current = contracts.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono umowy.");
        contracts.RemoveAll(item => item.Id == id);
        RaiseChanged();
    }

    public HrLeaveRequestRecord CreateLeaveRequest(
        int employeeId,
        int leaveTypeId,
        DateTime dateFrom,
        DateTime dateTo,
        decimal dayCount,
        string reason)
    {
        ValidateLeaveRequest(employeeId, leaveTypeId, dateFrom, dateTo, dayCount);

        var request = new HrLeaveRequestRecord(
            nextLeaveRequestId++,
            employeeId,
            leaveTypeId,
            "Submitted",
            dateFrom,
            dateTo,
            dayCount,
            NormalizeOptional(reason),
            null,
            null,
            null,
            DateTime.UtcNow);

        leaveRequests.Add(request);
        RaiseChanged();
        return request;
    }

    public HrLeaveRequestRecord UpdateLeaveRequest(
        int id,
        int employeeId,
        int leaveTypeId,
        DateTime dateFrom,
        DateTime dateTo,
        decimal dayCount,
        string reason)
    {
        var current = leaveRequests.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wniosku urlopowego.");
        ValidateLeaveRequest(employeeId, leaveTypeId, dateFrom, dateTo, dayCount);

        var updated = current with
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            DayCount = dayCount,
            Reason = NormalizeOptional(reason)
        };

        ReplaceLeaveRequest(updated);
        RaiseChanged();
        return updated;
    }

    public void CancelLeaveRequest(int id)
    {
        var current = leaveRequests.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wniosku urlopowego.");
        ReplaceLeaveRequest(current with
        {
            Status = "Cancelled",
            DecisionNote = "Anulowane przez operatora HR.",
            DecidedBy = "HR",
            DecidedAt = DateTime.UtcNow
        });
        RaiseChanged();
    }

    public void DecideLeaveRequest(int id, string status, string decisionNote, string decidedBy)
    {
        var current = leaveRequests.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wniosku urlopowego.");
        if (!string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Nieprawidłowy status decyzji.");
        }

        ReplaceLeaveRequest(current with
        {
            Status = status,
            DecisionNote = NormalizeOptional(decisionNote),
            DecidedBy = NormalizeOptional(decidedBy) ?? "Kierownik",
            DecidedAt = DateTime.UtcNow
        });
        RaiseChanged();
    }

    public HrEmployeeDocumentRecord CreateDocument(
        int employeeId,
        string documentType,
        string title,
        string documentNumber,
        DateTime? issuedAt,
        DateTime? validUntil,
        string notes,
        bool isActive)
    {
        ValidateDocument(employeeId, documentType, title);

        var document = new HrEmployeeDocumentRecord(
            nextDocumentId++,
            employeeId,
            documentType.Trim(),
            title.Trim(),
            NormalizeOptional(documentNumber),
            issuedAt,
            validUntil,
            NormalizeOptional(notes),
            isActive,
            false);

        documents.Add(document);
        RaiseChanged();
        return document;
    }

    public HrEmployeeDocumentRecord UpdateDocument(
        int id,
        int employeeId,
        string documentType,
        string title,
        string documentNumber,
        DateTime? issuedAt,
        DateTime? validUntil,
        string notes,
        bool isActive)
    {
        var current = documents.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono dokumentu pracownika.");
        ValidateDocument(employeeId, documentType, title);

        var updated = current with
        {
            EmployeeId = employeeId,
            DocumentType = documentType.Trim(),
            Title = title.Trim(),
            DocumentNumber = NormalizeOptional(documentNumber),
            IssuedAt = issuedAt,
            ValidUntil = validUntil,
            Notes = NormalizeOptional(notes),
            IsActive = isActive
        };

        ReplaceDocument(updated);
        RaiseChanged();
        return updated;
    }

    public void ArchiveDocument(int id)
    {
        var current = documents.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono dokumentu pracownika.");
        ReplaceDocument(current with { IsArchived = true, IsActive = false });
        RaiseChanged();
    }

    public void RestoreDocument(int id)
    {
        var current = documents.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono dokumentu pracownika.");
        ReplaceDocument(current with { IsArchived = false, IsActive = true });
        RaiseChanged();
    }

    public void DeleteDocument(int id)
    {
        documents.RemoveAll(item => item.Id == id);
        RaiseChanged();
    }

    private void SyncEmployeeFromContract(HrContractRecord contract)
    {
        var employee = employees.FirstOrDefault(item => item.Id == contract.EmployeeId);
        if (employee is null || contract.IsArchived)
        {
            return;
        }

        ReplaceEmployee(employee with
        {
            DepartmentId = contract.DepartmentId,
            PositionId = contract.PositionId,
            PrimaryContractType = contract.ContractType
        });
    }

    private void ValidateDepartment(string code, string name, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Kod i nazwa działu są wymagane.");
        }

        if (departments.Any(item => item.Id != existingId && string.Equals(item.Code, code.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki kod działu już istnieje.");
        }
    }

    private void ValidatePosition(string code, string name, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Kod i nazwa stanowiska są wymagane.");
        }

        if (positions.Any(item => item.Id != existingId && string.Equals(item.Code, code.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki kod stanowiska już istnieje.");
        }
    }

    private void ValidateEmployee(string employeeNumber, string email, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            throw new InvalidOperationException("Numer pracownika jest wymagany.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Email pracownika jest wymagany.");
        }

        if (employees.Any(item => item.Id != existingId && string.Equals(item.EmployeeNumber, employeeNumber.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki numer pracownika już istnieje.");
        }

        if (employees.Any(item => item.Id != existingId && string.Equals(item.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki email pracownika już istnieje.");
        }
    }

    private void ValidateDepartmentPosition(int? departmentId, int? positionId)
    {
        if (departmentId is not null && departments.All(item => item.Id != departmentId.Value))
        {
            throw new InvalidOperationException("Wybrany dział nie istnieje.");
        }

        if (positionId is not null && positions.All(item => item.Id != positionId.Value))
        {
            throw new InvalidOperationException("Wybrane stanowisko nie istnieje.");
        }
    }

    private void ValidateContract(int employeeId, string contractNumber, int departmentId, int positionId, DateTime startDate, DateTime? endDate, decimal employmentRate, int? existingId)
    {
        if (employees.All(item => item.Id != employeeId))
        {
            throw new InvalidOperationException("Wybrany pracownik nie istnieje.");
        }

        if (string.IsNullOrWhiteSpace(contractNumber))
        {
            throw new InvalidOperationException("Numer umowy jest wymagany.");
        }

        if (departments.All(item => item.Id != departmentId))
        {
            throw new InvalidOperationException("Wybrany dział nie istnieje.");
        }

        if (positions.All(item => item.Id != positionId))
        {
            throw new InvalidOperationException("Wybrane stanowisko nie istnieje.");
        }

        if (endDate is not null && endDate < startDate)
        {
            throw new InvalidOperationException("Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
        }

        if (employmentRate <= 0)
        {
            throw new InvalidOperationException("Wymiar etatu musi być większy od zera.");
        }

        if (contracts.Any(item => item.Id != existingId && string.Equals(item.ContractNumber, contractNumber.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki numer umowy już istnieje.");
        }
    }

    private void ValidateLeaveRequest(int employeeId, int leaveTypeId, DateTime dateFrom, DateTime dateTo, decimal dayCount)
    {
        if (employees.All(item => item.Id != employeeId))
        {
            throw new InvalidOperationException("Wybrany pracownik nie istnieje.");
        }

        if (leaveTypes.All(item => item.Id != leaveTypeId))
        {
            throw new InvalidOperationException("Wybrany typ urlopu nie istnieje.");
        }

        if (dateTo < dateFrom)
        {
            throw new InvalidOperationException("Data końcowa nie może być wcześniejsza niż początkowa.");
        }

        if (dayCount <= 0)
        {
            throw new InvalidOperationException("Liczba dni musi być większa od zera.");
        }
    }

    private void ValidateDocument(int employeeId, string documentType, string title)
    {
        if (employees.All(item => item.Id != employeeId))
        {
            throw new InvalidOperationException("Wybrany pracownik nie istnieje.");
        }

        if (string.IsNullOrWhiteSpace(documentType) || string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Typ i tytuł dokumentu są wymagane.");
        }
    }

    private void ReplaceDepartment(HrDepartmentRecord department)
    {
        var index = departments.FindIndex(item => item.Id == department.Id);
        departments[index] = department;
    }

    private void ReplacePosition(HrPositionRecord position)
    {
        var index = positions.FindIndex(item => item.Id == position.Id);
        positions[index] = position;
    }

    private void ReplaceEmployee(HrEmployeeRecord employee)
    {
        var index = employees.FindIndex(item => item.Id == employee.Id);
        employees[index] = employee;
    }

    private void ReplaceContract(HrContractRecord contract)
    {
        var index = contracts.FindIndex(item => item.Id == contract.Id);
        contracts[index] = contract;
    }

    private void ReplaceLeaveRequest(HrLeaveRequestRecord request)
    {
        var index = leaveRequests.FindIndex(item => item.Id == request.Id);
        leaveRequests[index] = request;
    }

    private void ReplaceDocument(HrEmployeeDocumentRecord document)
    {
        var index = documents.FindIndex(item => item.Id == document.Id);
        documents[index] = document;
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void Seed()
    {
        departments.Add(new HrDepartmentRecord(1, "HR", "Kadry i administracja", "Obsługa personalna i procesy kadrowe.", true));
        departments.Add(new HrDepartmentRecord(2, "TRN", "Transport", "Kierowcy, dyspozytorzy i operacje przewozowe.", true));
        departments.Add(new HrDepartmentRecord(3, "WH", "Magazyn", "Operacje magazynowe i logistyka wewnętrzna.", true));
        departments.Add(new HrDepartmentRecord(4, "FIN", "Finanse", "Koszty, płatności i rozliczenia.", true));
        departments.Add(new HrDepartmentRecord(5, "ADM", "Administracja", "Biuro, recepcja i wsparcie organizacji.", false));

        positions.Add(new HrPositionRecord(1, "HR_SPEC", "Specjalista HR", "Kadry, onboarding i dokumenty.", true));
        positions.Add(new HrPositionRecord(2, "DRIVER", "Kierowca", "Transport krajowy i zagraniczny.", true));
        positions.Add(new HrPositionRecord(3, "DISPATCH", "Dyspozytor", "Planowanie tras i obsad.", true));
        positions.Add(new HrPositionRecord(4, "WH_LEAD", "Brygadzista magazynu", "Zmiana magazynowa i dokumenty.", true));
        positions.Add(new HrPositionRecord(5, "FIN_CTRL", "Kontroler finansowy", "Koszty, faktury i płatności.", true));
        positions.Add(new HrPositionRecord(6, "ASST", "Asystent administracyjny", "Recepcja i wsparcie operacyjne.", false));

        employees.Add(new HrEmployeeRecord(1, "EMP-001", "Anna", "Nowak", "anna.nowak@erp.local", "+48 600 100 101", "92010112345", 1, 1, "Employment", true, false, DateTime.Today.AddYears(-4)));
        employees.Add(new HrEmployeeRecord(2, "EMP-002", "Piotr", "Kaczmarek", "piotr.kaczmarek@erp.local", "+48 600 100 102", "89022412345", 2, 2, "Employment", true, true, DateTime.Today.AddYears(-6)));
        employees.Add(new HrEmployeeRecord(3, "EMP-003", "Michał", "Wójcik", "michal.wojcik@erp.local", "+48 600 100 103", "95031212345", 2, 3, "B2B", true, false, DateTime.Today.AddYears(-2)));
        employees.Add(new HrEmployeeRecord(4, "EMP-004", "Katarzyna", "Mazur", "katarzyna.mazur@erp.local", "+48 600 100 104", "93080712345", 3, 4, "Employment", true, false, DateTime.Today.AddYears(-3)));
        employees.Add(new HrEmployeeRecord(5, "EMP-005", "Tomasz", "Lewandowski", "tomasz.lewandowski@erp.local", "+48 600 100 105", "88061112345", 4, 5, "Employment", false, false, DateTime.Today.AddYears(-5)));

        contracts.Add(new HrContractRecord(1, 1, "HR/2022/001", "Employment", 1, 1, DateTime.Today.AddYears(-4), null, 1.0m, 9800m, "Obsługa procesów HR i dokumentów.", true, false));
        contracts.Add(new HrContractRecord(2, 2, "TR/2020/014", "Employment", 2, 2, DateTime.Today.AddYears(-6), null, 1.0m, 7600m, "Kierowca zestawu międzynarodowego.", true, false));
        contracts.Add(new HrContractRecord(3, 3, "TR/2024/B2B/03", "B2B", 2, 3, DateTime.Today.AddYears(-2), null, 1.0m, 14800m, "Dyspozytor odpowiedzialny za dispatch.", true, false));
        contracts.Add(new HrContractRecord(4, 4, "WH/2023/008", "Employment", 3, 4, DateTime.Today.AddYears(-3), null, 1.0m, 8200m, "Brygadzista zmiany popołudniowej.", true, false));
        contracts.Add(new HrContractRecord(5, 5, "FIN/2021/005", "Employment", 4, 5, DateTime.Today.AddYears(-5), DateTime.Today.AddMonths(-2), 1.0m, 11200m, "Umowa wygaszona.", false, true));

        leaveTypes.Add(new HrLeaveTypeRecord(1, "VAC", "Urlop wypoczynkowy", true, true, 1));
        leaveTypes.Add(new HrLeaveTypeRecord(2, "SICK", "Zwolnienie lekarskie", true, false, 2));
        leaveTypes.Add(new HrLeaveTypeRecord(3, "OCC", "Urlop okolicznościowy", true, true, 3));

        leaveRequests.Add(new HrLeaveRequestRecord(1, 1, 1, "Approved", DateTime.Today.AddDays(10), DateTime.Today.AddDays(14), 5m, "Urlop rodzinny", "Zatwierdzono bez kolizji.", "Kierownik HR", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-8)));
        leaveRequests.Add(new HrLeaveRequestRecord(2, 2, 1, "Submitted", DateTime.Today.AddDays(4), DateTime.Today.AddDays(6), 3m, "Krótki wypoczynek", null, null, null, DateTime.UtcNow.AddDays(-1)));
        leaveRequests.Add(new HrLeaveRequestRecord(3, 3, 3, "Submitted", DateTime.Today.AddDays(2), DateTime.Today.AddDays(2), 1m, "Sprawy urzędowe", null, null, null, DateTime.UtcNow.AddHours(-16)));
        leaveRequests.Add(new HrLeaveRequestRecord(4, 4, 2, "Rejected", DateTime.Today.AddDays(-5), DateTime.Today.AddDays(-2), 4m, "L4", "Brak kompletu dokumentów.", "HR", DateTime.UtcNow.AddDays(-4), DateTime.UtcNow.AddDays(-6)));
        leaveRequests.Add(new HrLeaveRequestRecord(5, 5, 1, "Cancelled", DateTime.Today.AddDays(20), DateTime.Today.AddDays(22), 3m, "Wyjazd prywatny", "Anulowane po zmianie grafiku.", "HR", DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(-5)));

        documents.Add(new HrEmployeeDocumentRecord(1, 2, "Prawo jazdy", "Prawo jazdy C+E", "DL-554120", DateTime.Today.AddYears(-4), DateTime.Today.AddMonths(8), "Kierowca floty międzynarodowej.", true, false));
        documents.Add(new HrEmployeeDocumentRecord(2, 2, "Karta kierowcy", "Karta kierowcy", "CARD-88211", DateTime.Today.AddYears(-2), DateTime.Today.AddMonths(2), "Wymaga odnowienia przed sezonem.", true, false));
        documents.Add(new HrEmployeeDocumentRecord(3, 1, "Badania", "Badania okresowe", "MED-2026-09", DateTime.Today.AddMonths(-2), DateTime.Today.AddMonths(10), "Badania ważne do końca roku.", true, false));
        documents.Add(new HrEmployeeDocumentRecord(4, 4, "Uprawnienia UDT", "Wózek widłowy", "UDT-4412", DateTime.Today.AddYears(-1), DateTime.Today.AddMonths(1), "Kończy się za 30 dni.", true, false));
        documents.Add(new HrEmployeeDocumentRecord(5, 5, "Archiwum", "Poprzednia umowa o pracę", "ARCH-005", DateTime.Today.AddYears(-5), DateTime.Today.AddMonths(-2), "Pozycja historyczna.", false, true));
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}

public sealed record HrDepartmentRecord(
    int Id,
    string Code,
    string Name,
    string Description,
    bool IsActive)
{
    public string StatusLabel => IsActive ? "Aktywny" : "Nieaktywny";
    public string StatusColor => IsActive ? "#1F8A5B" : "#7B8794";
}

public sealed record HrPositionRecord(
    int Id,
    string Code,
    string Name,
    string Description,
    bool IsActive)
{
    public string StatusLabel => IsActive ? "Aktywne" : "Nieaktywne";
    public string StatusColor => IsActive ? "#1F8A5B" : "#7B8794";
}

public sealed record HrEmployeeRecord(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? PersonalId,
    int? DepartmentId,
    int? PositionId,
    string? PrimaryContractType,
    bool IsActive,
    bool HasDriverProfile,
    DateTime HireDate)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string StatusLabel => IsActive ? "Aktywny" : "Nieaktywny";
    public string StatusColor => IsActive ? "#1F8A5B" : "#7B8794";
    public string DriverLabel => HasDriverProfile ? "Tak" : "Nie";
}

public sealed record HrContractRecord(
    int Id,
    int EmployeeId,
    string ContractNumber,
    string ContractType,
    int DepartmentId,
    int PositionId,
    DateTime StartDate,
    DateTime? EndDate,
    decimal EmploymentRate,
    decimal? MonthlySalary,
    string? Notes,
    bool IsActive,
    bool IsArchived)
{
    public string StatusLabel =>
        IsArchived ? "Archiwalna" :
        !IsActive ? "Nieaktywna" :
        EndDate is not null && EndDate.Value.Date < DateTime.Today ? "Wygasła" :
        "Aktywna";

    public string StatusColor =>
        IsArchived ? "#7B8794" :
        !IsActive ? "#7B8794" :
        EndDate is not null && EndDate.Value.Date < DateTime.Today ? "#D14343" :
        "#1F8A5B";

    public string EmploymentRateLabel => EmploymentRate.ToString("0.##", CultureInfo.InvariantCulture);
    public string SalaryLabel => MonthlySalary is null ? "-" : $"{MonthlySalary.Value:N0} zł";
}

public sealed record HrLeaveTypeRecord(
    int Id,
    string Code,
    string Name,
    bool IsPaid,
    bool RequiresApproval,
    int SortOrder);

public sealed record HrLeaveRequestRecord(
    int Id,
    int EmployeeId,
    int LeaveTypeId,
    string Status,
    DateTime DateFrom,
    DateTime DateTo,
    decimal DayCount,
    string? Reason,
    string? DecisionNote,
    string? DecidedBy,
    DateTime? DecidedAt,
    DateTime SubmittedAt)
{
    public string StatusColor => Status switch
    {
        "Approved" => "#1F8A5B",
        "Rejected" => "#D14343",
        "Cancelled" => "#7B8794",
        _ => "#D97706"
    };

    public string StatusLabel => Status switch
    {
        "Approved" => "Zatwierdzony",
        "Rejected" => "Odrzucony",
        "Cancelled" => "Anulowany",
        _ => "Oczekuje"
    };
}

public sealed record HrEmployeeDocumentRecord(
    int Id,
    int EmployeeId,
    string DocumentType,
    string Title,
    string? DocumentNumber,
    DateTime? IssuedAt,
    DateTime? ValidUntil,
    string? Notes,
    bool IsActive,
    bool IsArchived)
{
    public bool IsExpiringSoon => !IsArchived && ValidUntil is not null && ValidUntil.Value.Date <= DateTime.Today.AddDays(45);
    public string StatusLabel =>
        IsArchived ? "Archiwalny" :
        !IsActive ? "Nieaktywny" :
        ValidUntil is not null && ValidUntil.Value.Date < DateTime.Today ? "Wygasł" :
        IsExpiringSoon ? "Do odnowienia" :
        "Aktualny";

    public string StatusColor =>
        IsArchived ? "#7B8794" :
        !IsActive ? "#7B8794" :
        ValidUntil is not null && ValidUntil.Value.Date < DateTime.Today ? "#D14343" :
        IsExpiringSoon ? "#D97706" :
        "#1F8A5B";
}

public sealed record HrSummaryCardViewModel(string Label, string Value, string Caption, string AccentColor);
