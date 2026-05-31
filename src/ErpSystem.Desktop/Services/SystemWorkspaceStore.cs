using ErpSystem.Shared.Security;

namespace ErpSystem.Desktop.Services;

public sealed class SystemWorkspaceStore
{
    private readonly List<SystemUserRecord> users = [];
    private readonly List<SystemRoleRecord> roles = [];
    private readonly List<SystemDictionaryRecord> dictionaries = [];
    private readonly List<SystemDocumentSequenceRecord> sequences = [];
    private readonly List<SystemAuditRecord> auditLogs = [];
    private int nextUserId = 10;
    private int nextRoleId = 10;
    private int nextDictionaryId = 20;
    private int nextSequenceId = 10;
    private int nextAuditId = 1000;

    private SystemWorkspaceStore()
    {
        Seed();
    }

    public static SystemWorkspaceStore Instance { get; } = new();

    public event EventHandler? Changed;

    public IReadOnlyList<SystemUserRecord> GetUsers()
        => users.OrderBy(item => item.DisplayName).ToArray();

    public IReadOnlyList<SystemRoleRecord> GetRoles()
        => roles.OrderBy(item => item.Name).ToArray();

    public IReadOnlyList<PermissionDefinition> GetPermissions()
        => PermissionCatalog.All
            .OrderBy(item => item.Module)
            .ThenBy(item => item.Resource)
            .ThenBy(item => item.Action)
            .ToArray();

    public IReadOnlyList<SystemDictionaryRecord> GetDictionaries()
        => dictionaries
            .OrderBy(item => item.Area)
            .ThenBy(item => item.Category)
            .ThenBy(item => item.Code)
            .ToArray();

    public IReadOnlyList<SystemDocumentSequenceRecord> GetSequences()
        => sequences
            .OrderBy(item => item.Module)
            .ThenBy(item => item.Key)
            .ToArray();

    public IReadOnlyList<SystemAuditRecord> GetAuditLogs()
        => auditLogs
            .OrderByDescending(item => item.At)
            .ToArray();

    public IReadOnlyList<string> GetRoleNames(IReadOnlyList<int> roleIds)
        => roles
            .Where(role => roleIds.Contains(role.Id))
            .OrderBy(role => role.Name)
            .Select(role => role.Name)
            .ToArray();

    public int GetAssignedRoleCount(string permissionName)
        => roles.Count(role => role.PermissionNames.Contains(permissionName, StringComparer.OrdinalIgnoreCase));

    public IReadOnlyList<SystemRoleRecord> GetRolesForPermission(string permissionName)
        => roles
            .Where(role => role.PermissionNames.Contains(permissionName, StringComparer.OrdinalIgnoreCase))
            .OrderBy(role => role.Name)
            .ToArray();

    public IReadOnlyList<SystemUserRecord> GetUsersForRole(int roleId)
        => users
            .Where(user => user.RoleIds.Contains(roleId))
            .OrderBy(user => user.DisplayName)
            .ToArray();

    public SystemUserRecord CreateUser(string userName, string displayName, string email, bool isActive, IReadOnlyList<int> roleIds)
    {
        ValidateUser(userName, email, null);

        var user = new SystemUserRecord(
            nextUserId++,
            userName.Trim(),
            displayName.Trim(),
            email.Trim(),
            isActive,
            [.. roleIds.Distinct().OrderBy(item => item)],
            DateTime.UtcNow.AddMinutes(-12),
            DateTime.UtcNow);

        users.Add(user);
        Log("System", "User", "Create", $"Dodano użytkownika {user.DisplayName}.", "Administrator ERP");
        RaiseChanged();
        return user;
    }

    public SystemUserRecord UpdateUser(int userId, string userName, string displayName, string email, bool isActive)
    {
        var current = users.FirstOrDefault(item => item.Id == userId)
            ?? throw new InvalidOperationException("Nie znaleziono użytkownika.");

        ValidateUser(userName, email, userId);

        var updated = current with
        {
            UserName = userName.Trim(),
            DisplayName = displayName.Trim(),
            Email = email.Trim(),
            IsActive = isActive
        };

        ReplaceUser(updated);
        Log("System", "User", "Edit", $"Zmieniono dane użytkownika {updated.DisplayName}.", "Administrator ERP");
        RaiseChanged();
        return updated;
    }

    public void DeleteUser(int userId)
    {
        var current = users.FirstOrDefault(item => item.Id == userId)
            ?? throw new InvalidOperationException("Nie znaleziono użytkownika.");

        if (string.Equals(current.UserName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Nie można usunąć głównego administratora.");
        }

        users.RemoveAll(item => item.Id == userId);
        Log("System", "User", "Delete", $"Usunięto użytkownika {current.DisplayName}.", "Administrator ERP");
        RaiseChanged();
    }

    public void ToggleUserActive(int userId)
    {
        var current = users.FirstOrDefault(item => item.Id == userId)
            ?? throw new InvalidOperationException("Nie znaleziono użytkownika.");

        ReplaceUser(current with { IsActive = !current.IsActive });
        Log("System", "User", current.IsActive ? "Deactivate" : "Activate", $"Zmieniono aktywność użytkownika {current.DisplayName}.", "Administrator ERP");
        RaiseChanged();
    }

    public void ResetPassword(int userId)
    {
        var current = users.FirstOrDefault(item => item.Id == userId)
            ?? throw new InvalidOperationException("Nie znaleziono użytkownika.");

        ReplaceUser(current with { LastPasswordResetAt = DateTime.UtcNow });
        Log("System", "User", "ResetPassword", $"Zresetowano hasło użytkownika {current.DisplayName}.", "Administrator ERP");
        RaiseChanged();
    }

    public void AssignRoleToUser(int userId, int roleId)
    {
        var current = users.FirstOrDefault(item => item.Id == userId)
            ?? throw new InvalidOperationException("Nie znaleziono użytkownika.");
        var role = roles.FirstOrDefault(item => item.Id == roleId)
            ?? throw new InvalidOperationException("Nie znaleziono roli.");

        if (current.RoleIds.Contains(roleId))
        {
            return;
        }

        ReplaceUser(current with { RoleIds = [.. current.RoleIds, roleId] });
        Log("System", "UserRole", "Assign", $"Przypisano rolę {role.Name} do użytkownika {current.DisplayName}.", "Administrator ERP");
        RaiseChanged();
    }

    public void RemoveRoleFromUser(int userId, int roleId)
    {
        var current = users.FirstOrDefault(item => item.Id == userId)
            ?? throw new InvalidOperationException("Nie znaleziono użytkownika.");
        var role = roles.FirstOrDefault(item => item.Id == roleId)
            ?? throw new InvalidOperationException("Nie znaleziono roli.");

        var newRoles = current.RoleIds.Where(item => item != roleId).ToArray();
        if (newRoles.Length == current.RoleIds.Count)
        {
            return;
        }

        ReplaceUser(current with { RoleIds = newRoles });
        Log("System", "UserRole", "Remove", $"Odebrano rolę {role.Name} użytkownikowi {current.DisplayName}.", "Administrator ERP");
        RaiseChanged();
    }

    public SystemRoleRecord CreateRole(string code, string name, string description, bool isActive)
    {
        ValidateRole(code, null);

        var role = new SystemRoleRecord(
            nextRoleId++,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            description.Trim(),
            isActive,
            false,
            []);

        roles.Add(role);
        Log("System", "Role", "Create", $"Dodano rolę {role.Name}.", "Administrator ERP");
        RaiseChanged();
        return role;
    }

    public SystemRoleRecord UpdateRole(int roleId, string code, string name, string description, bool isActive)
    {
        var current = roles.FirstOrDefault(item => item.Id == roleId)
            ?? throw new InvalidOperationException("Nie znaleziono roli.");

        ValidateRole(code, roleId);

        var updated = current with
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = description.Trim(),
            IsActive = isActive
        };

        ReplaceRole(updated);
        Log("System", "Role", "Edit", $"Zmieniono rolę {updated.Name}.", "Administrator ERP");
        RaiseChanged();
        return updated;
    }

    public void DeleteRole(int roleId)
    {
        var current = roles.FirstOrDefault(item => item.Id == roleId)
            ?? throw new InvalidOperationException("Nie znaleziono roli.");

        if (current.IsSystem)
        {
            throw new InvalidOperationException("Ról systemowych nie można usunąć.");
        }

        roles.RemoveAll(item => item.Id == roleId);
        for (var i = 0; i < users.Count; i++)
        {
            if (!users[i].RoleIds.Contains(roleId))
            {
                continue;
            }

            users[i] = users[i] with { RoleIds = users[i].RoleIds.Where(item => item != roleId).ToArray() };
        }

        Log("System", "Role", "Delete", $"Usunięto rolę {current.Name}.", "Administrator ERP");
        RaiseChanged();
    }

    public void AssignPermissionToRole(int roleId, string permissionName)
    {
        var current = roles.FirstOrDefault(item => item.Id == roleId)
            ?? throw new InvalidOperationException("Nie znaleziono roli.");

        if (current.PermissionNames.Contains(permissionName, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        ReplaceRole(current with
        {
            PermissionNames = [.. current.PermissionNames.OrderBy(item => item), permissionName]
        });

        Log("System", "RolePermission", "Assign", $"Nadano uprawnienie {permissionName} roli {current.Name}.", "Administrator ERP");
        RaiseChanged();
    }

    public void RemovePermissionFromRole(int roleId, string permissionName)
    {
        var current = roles.FirstOrDefault(item => item.Id == roleId)
            ?? throw new InvalidOperationException("Nie znaleziono roli.");

        if (!current.PermissionNames.Contains(permissionName, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        ReplaceRole(current with
        {
            PermissionNames = current.PermissionNames
                .Where(item => !string.Equals(item, permissionName, StringComparison.OrdinalIgnoreCase))
                .ToArray()
        });

        Log("System", "RolePermission", "Remove", $"Odebrano uprawnienie {permissionName} roli {current.Name}.", "Administrator ERP");
        RaiseChanged();
    }

    public SystemDictionaryRecord CreateDictionary(string area, string category, string code, string value, string description, bool isActive)
    {
        ValidateDictionary(area, category, code, null);

        var item = new SystemDictionaryRecord(
            nextDictionaryId++,
            area.Trim(),
            category.Trim(),
            code.Trim().ToUpperInvariant(),
            value.Trim(),
            description.Trim(),
            isActive,
            false);

        dictionaries.Add(item);
        Log("System", "Dictionary", "Create", $"Dodano wpis słownikowy {item.Code}.", "Administrator ERP");
        RaiseChanged();
        return item;
    }

    public SystemDictionaryRecord UpdateDictionary(int id, string area, string category, string code, string value, string description, bool isActive)
    {
        var current = dictionaries.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wpisu słownikowego.");

        ValidateDictionary(area, category, code, id);

        var updated = current with
        {
            Area = area.Trim(),
            Category = category.Trim(),
            Code = code.Trim().ToUpperInvariant(),
            Value = value.Trim(),
            Description = description.Trim(),
            IsActive = isActive
        };

        ReplaceDictionary(updated);
        Log("System", "Dictionary", "Edit", $"Zmieniono wpis słownikowy {updated.Code}.", "Administrator ERP");
        RaiseChanged();
        return updated;
    }

    public void ArchiveDictionary(int id)
    {
        var current = dictionaries.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wpisu słownikowego.");

        ReplaceDictionary(current with { IsArchived = true, IsActive = false });
        Log("System", "Dictionary", "Archive", $"Zarchiwizowano wpis słownikowy {current.Code}.", "Administrator ERP");
        RaiseChanged();
    }

    public void RestoreDictionary(int id)
    {
        var current = dictionaries.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wpisu słownikowego.");

        ReplaceDictionary(current with { IsArchived = false, IsActive = true });
        Log("System", "Dictionary", "Restore", $"Przywrócono wpis słownikowy {current.Code}.", "Administrator ERP");
        RaiseChanged();
    }

    public void DeleteDictionary(int id)
    {
        var current = dictionaries.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wpisu słownikowego.");

        dictionaries.RemoveAll(item => item.Id == id);
        Log("System", "Dictionary", "Delete", $"Usunięto wpis słownikowy {current.Code}.", "Administrator ERP");
        RaiseChanged();
    }

    public SystemDocumentSequenceRecord CreateSequence(string key, string module, string prefix, int numberLength, int currentNumber, string resetPolicy, bool isActive)
    {
        ValidateSequence(key, null);

        var sequence = new SystemDocumentSequenceRecord(
            nextSequenceId++,
            key.Trim().ToUpperInvariant(),
            module.Trim(),
            prefix.Trim().ToUpperInvariant(),
            numberLength,
            currentNumber,
            resetPolicy.Trim(),
            isActive,
            DateTime.UtcNow.AddHours(-6));

        sequences.Add(sequence);
        Log("System", "Sequence", "Create", $"Dodano sekwencję {sequence.Key}.", "Administrator ERP");
        RaiseChanged();
        return sequence;
    }

    public SystemDocumentSequenceRecord UpdateSequence(int id, string key, string module, string prefix, int numberLength, int currentNumber, string resetPolicy, bool isActive)
    {
        var current = sequences.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono sekwencji.");

        ValidateSequence(key, id);

        var updated = current with
        {
            Key = key.Trim().ToUpperInvariant(),
            Module = module.Trim(),
            Prefix = prefix.Trim().ToUpperInvariant(),
            NumberLength = numberLength,
            CurrentNumber = currentNumber,
            ResetPolicy = resetPolicy.Trim(),
            IsActive = isActive
        };

        ReplaceSequence(updated);
        Log("System", "Sequence", "Edit", $"Zmieniono sekwencję {updated.Key}.", "Administrator ERP");
        RaiseChanged();
        return updated;
    }

    public string GenerateNextDocumentNumber(int id)
    {
        var current = sequences.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono sekwencji.");

        var nextNumber = current.CurrentNumber + 1;
        var preview = $"{current.Prefix}/{DateTime.Now:yyyyMM}/{nextNumber.ToString().PadLeft(current.NumberLength, '0')}";
        ReplaceSequence(current with
        {
            CurrentNumber = nextNumber,
            LastGeneratedAt = DateTime.UtcNow
        });
        Log("System", "Sequence", "Generate", $"Wygenerowano numer {preview}.", "Administrator ERP");
        RaiseChanged();
        return preview;
    }

    public void DeleteSequence(int id)
    {
        var current = sequences.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono sekwencji.");

        sequences.RemoveAll(item => item.Id == id);
        Log("System", "Sequence", "Delete", $"Usunięto sekwencję {current.Key}.", "Administrator ERP");
        RaiseChanged();
    }

    private void Seed()
    {
        roles.Add(new SystemRoleRecord(1, "ADMIN", "Administrator", "Pełny dostęp do wszystkich modułów i konfiguracji.", true, true, PermissionCatalog.All.Select(item => item.Name).ToArray()));
        roles.Add(new SystemRoleRecord(2, "WAREHOUSE", "Magazyn", "Obsługa dokumentów magazynowych, towarów i stanów.", true, true,
        [
            PermissionNames.System.Menu.Read,
            PermissionNames.Warehouse.Goods.Read,
            PermissionNames.Warehouse.Goods.Create,
            PermissionNames.Warehouse.Goods.Edit,
            PermissionNames.Warehouse.Documents.Read,
            PermissionNames.Warehouse.Documents.Create,
            PermissionNames.Warehouse.Documents.Post,
            PermissionNames.Warehouse.Stock.Read
        ]));
        roles.Add(new SystemRoleRecord(3, "DISPATCH", "Dispatch", "Planowanie zleceń, obsady i monitorowanie wyjazdów.", true, true,
        [
            PermissionNames.System.Menu.Read,
            PermissionNames.Transport.Order.Read,
            PermissionNames.Transport.Order.Create,
            PermissionNames.Transport.Order.Edit,
            PermissionNames.Transport.Order.ChangeStatus,
            PermissionNames.Transport.Order.AssignDriver,
            PermissionNames.Contractors.Contractor.Read
        ]));
        roles.Add(new SystemRoleRecord(4, "HR", "HR", "Kadry, dokumenty i nieobecności pracowników.", true, true,
        [
            PermissionNames.System.Menu.Read,
            PermissionNames.HR.Employee.Read,
            PermissionNames.HR.Employee.Create,
            PermissionNames.HR.Employee.Edit,
            PermissionNames.HR.Department.Read,
            PermissionNames.HR.Department.Manage,
            PermissionNames.HR.Position.Read,
            PermissionNames.HR.Position.Manage,
            PermissionNames.HR.Contract.Read,
            PermissionNames.HR.Contract.Manage,
            PermissionNames.HR.Leave.Read,
            PermissionNames.HR.Leave.Create,
            PermissionNames.HR.Leave.Approve,
            PermissionNames.HR.Document.Read,
            PermissionNames.HR.Document.Manage
        ]));
        roles.Add(new SystemRoleRecord(5, "FINANCE", "Finanse", "Koszty, faktury, kursy i rozliczenia.", true, true,
        [
            PermissionNames.System.Menu.Read,
            PermissionNames.Finance.Cost.Read,
            PermissionNames.Finance.Cost.Create,
            PermissionNames.Finance.Cost.Edit,
            PermissionNames.Finance.Invoice.Read,
            PermissionNames.Finance.Invoice.Create,
            PermissionNames.Finance.Invoice.Edit,
            PermissionNames.Finance.Payment.Read,
            PermissionNames.Finance.Payment.Create,
            PermissionNames.Finance.CurrencyRate.Read,
            PermissionNames.Finance.CurrencyRate.Manage,
            PermissionNames.Finance.Settlement.Read,
            PermissionNames.Finance.Settlement.Manage
        ]));
        roles.Add(new SystemRoleRecord(6, "VIEWER", "Podgląd", "Dostęp tylko do odczytu wybranych modułów.", true, true,
        [
            PermissionNames.System.Menu.Read,
            PermissionNames.Contractors.Contractor.Read,
            PermissionNames.Warehouse.Goods.Read,
            PermissionNames.Warehouse.Documents.Read,
            PermissionNames.Warehouse.Stock.Read,
            PermissionNames.Transport.Order.Read,
            PermissionNames.HR.Employee.Read,
            PermissionNames.Finance.Cost.Read,
            PermissionNames.Finance.Invoice.Read
        ]));

        users.Add(new SystemUserRecord(1, "admin", "Administrator ERP", "admin@erp.local", true, [1], DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddHours(-3)));
        users.Add(new SystemUserRecord(2, "magazyn", "Kierownik magazynu", "warehouse@erp.local", true, [2], DateTime.UtcNow.AddMinutes(-18), DateTime.UtcNow.AddDays(-1)));
        users.Add(new SystemUserRecord(3, "dispatch", "Dyspozytor krajowy", "dispatch@erp.local", true, [3], DateTime.UtcNow.AddMinutes(-9), DateTime.UtcNow.AddHours(-5)));
        users.Add(new SystemUserRecord(4, "hr", "Specjalista HR", "hr@erp.local", true, [4], DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddDays(-2)));
        users.Add(new SystemUserRecord(5, "finance", "Kontroler finansowy", "finance@erp.local", false, [5, 6], DateTime.UtcNow.AddDays(-4), DateTime.UtcNow.AddDays(-7)));

        dictionaries.Add(new SystemDictionaryRecord(1, "System", "Currencies", "PLN", "Polski zloty", "Waluta bazowa systemu.", true, false));
        dictionaries.Add(new SystemDictionaryRecord(2, "System", "Currencies", "EUR", "Euro", "Waluta przewozow zagranicznych.", true, false));
        dictionaries.Add(new SystemDictionaryRecord(3, "Transport", "Incoterms", "DAP", "Delivered At Place", "Warunki dostawy dla importu i eksportu.", true, false));
        dictionaries.Add(new SystemDictionaryRecord(4, "Transport", "VehicleTypes", "MEGA", "Mega", "Naczepa mega.", true, false));
        dictionaries.Add(new SystemDictionaryRecord(5, "HR", "LeaveTypes", "URL", "Urlop wypoczynkowy", "Podstawowy typ nieobecnosci.", true, false));
        dictionaries.Add(new SystemDictionaryRecord(6, "Warehouse", "Units", "SZT", "Sztuka", "Jednostka magazynowa.", true, false));

        sequences.Add(new SystemDocumentSequenceRecord(1, "WH_PZ", "Warehouse", "PZ", 4, 48, "Miesięczny", true, DateTime.UtcNow.AddHours(-12)));
        sequences.Add(new SystemDocumentSequenceRecord(2, "WH_WZ", "Warehouse", "WZ", 4, 12, "Miesięczny", true, DateTime.UtcNow.AddHours(-8)));
        sequences.Add(new SystemDocumentSequenceRecord(3, "TR_ORD", "Transport", "TR", 4, 54, "Miesięczny", true, DateTime.UtcNow.AddHours(-2)));
        sequences.Add(new SystemDocumentSequenceRecord(4, "FIN_INV", "Finance", "FV", 5, 182, "Roczny", true, DateTime.UtcNow.AddDays(-1)));
        sequences.Add(new SystemDocumentSequenceRecord(5, "HR_DOC", "HR", "HRD", 4, 17, "Roczny", false, DateTime.UtcNow.AddDays(-14)));

        auditLogs.Add(new SystemAuditRecord(nextAuditId++, DateTime.UtcNow.AddMinutes(-55), "Administrator ERP", "System", "Role", "Assign", "Nadano pełne uprawnienia roli Administrator."));
        auditLogs.Add(new SystemAuditRecord(nextAuditId++, DateTime.UtcNow.AddMinutes(-34), "Dyspozytor krajowy", "Transport", "Order", "ChangeStatus", "Zmieniono status TR/202605/0042 na Zaplanowane."));
        auditLogs.Add(new SystemAuditRecord(nextAuditId++, DateTime.UtcNow.AddMinutes(-12), "Kierownik magazynu", "Warehouse", "Document", "Post", "Zaksięgowano dokument PZ/2026/0048."));
    }

    private void ReplaceUser(SystemUserRecord updated)
    {
        var index = users.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            users[index] = updated;
        }
    }

    private void ReplaceRole(SystemRoleRecord updated)
    {
        var index = roles.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            roles[index] = updated;
        }
    }

    private void ReplaceDictionary(SystemDictionaryRecord updated)
    {
        var index = dictionaries.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            dictionaries[index] = updated;
        }
    }

    private void ReplaceSequence(SystemDocumentSequenceRecord updated)
    {
        var index = sequences.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            sequences[index] = updated;
        }
    }

    private void ValidateUser(string userName, string email, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidOperationException("Login użytkownika jest wymagany.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Email użytkownika jest wymagany.");
        }

        if (users.Any(item => item.Id != existingId && string.Equals(item.UserName, userName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki login użytkownika już istnieje.");
        }

        if (users.Any(item => item.Id != existingId && string.Equals(item.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki email użytkownika już istnieje.");
        }
    }

    private void ValidateRole(string code, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("Kod roli jest wymagany.");
        }

        if (roles.Any(item => item.Id != existingId && string.Equals(item.Code, code.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki kod roli już istnieje.");
        }
    }

    private void ValidateDictionary(string area, string category, string code, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(area) || string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("Obszar, kategoria i kod słownika są wymagane.");
        }

        if (dictionaries.Any(item =>
                item.Id != existingId &&
                string.Equals(item.Area, area.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Category, category.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Code, code.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taki wpis słownikowy już istnieje.");
        }
    }

    private void ValidateSequence(string key, int? existingId)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Klucz sekwencji jest wymagany.");
        }

        if (sequences.Any(item => item.Id != existingId && string.Equals(item.Key, key.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Taka sekwencja już istnieje.");
        }
    }

    private void Log(string module, string entity, string action, string detail, string user)
    {
        auditLogs.Add(new SystemAuditRecord(nextAuditId++, DateTime.UtcNow, user, module, entity, action, detail));
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}

public sealed record SystemUserRecord(
    int Id,
    string UserName,
    string DisplayName,
    string Email,
    bool IsActive,
    IReadOnlyList<int> RoleIds,
    DateTime LastSignInAt,
    DateTime LastPasswordResetAt);

public sealed record SystemRoleRecord(
    int Id,
    string Code,
    string Name,
    string Description,
    bool IsActive,
    bool IsSystem,
    IReadOnlyList<string> PermissionNames);

public sealed record SystemDictionaryRecord(
    int Id,
    string Area,
    string Category,
    string Code,
    string Value,
    string Description,
    bool IsActive,
    bool IsArchived)
{
    public string StatusLabel => IsActive ? "Aktywny" : "Nieaktywny";
    public string ArchiveLabel => IsArchived ? "Tak" : "Nie";
}

public sealed record SystemDocumentSequenceRecord(
    int Id,
    string Key,
    string Module,
    string Prefix,
    int NumberLength,
    int CurrentNumber,
    string ResetPolicy,
    bool IsActive,
    DateTime LastGeneratedAt)
{
    public string StatusLabel => IsActive ? "Aktywna" : "Nieaktywna";
}

public sealed record SystemAuditRecord(
    int Id,
    DateTime At,
    string User,
    string Module,
    string Entity,
    string Action,
    string Detail);

public sealed record SystemSummaryCardViewModel(string Label, string Value, string Caption, string AccentColor);
