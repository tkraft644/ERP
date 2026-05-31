namespace ErpSystem.Desktop.Services;

public sealed class DocumentsWorkspaceStore
{
    private readonly List<DocumentAttachmentRecord> attachments = [];
    private readonly List<DocumentHistoryRecord> historyEntries = [];
    private int nextAttachmentId = 30;
    private int nextHistoryId = 70;

    private DocumentsWorkspaceStore()
    {
        Seed();
    }

    public static DocumentsWorkspaceStore Instance { get; } = new();

    public event EventHandler? Changed;

    public IReadOnlyList<DocumentAttachmentRecord> GetAttachments()
        => attachments.OrderByDescending(item => item.UploadedAt).ThenBy(item => item.FileName).ToArray();

    public IReadOnlyList<DocumentHistoryRecord> GetHistoryEntries()
        => historyEntries.OrderByDescending(item => item.OccurredAt).ThenByDescending(item => item.Id).ToArray();

    public IReadOnlyList<string> GetModules()
        => attachments.Select(item => item.Module)
            .Concat(historyEntries.Select(item => item.Module))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .ToArray();

    public DocumentAttachmentRecord CreateAttachment(
        string module,
        string ownerDocument,
        string fileName,
        string contentType,
        string sizeLabel,
        string uploadedBy,
        string tags,
        string description)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException("Nazwa pliku jest wymagana.");
        }

        var created = new DocumentAttachmentRecord(
            nextAttachmentId++,
            NormalizeRequired(module, "Moduł"),
            NormalizeRequired(ownerDocument, "Dokument"),
            fileName.Trim(),
            NormalizeRequired(contentType, "Typ pliku"),
            NormalizeRequired(sizeLabel, "Rozmiar"),
            NormalizeRequired(uploadedBy, "Operator"),
            DateTime.Now,
            NormalizeOptional(tags),
            NormalizeOptional(description),
            false);
        attachments.Add(created);
        RaiseChanged();
        return created;
    }

    public DocumentAttachmentRecord UpdateAttachment(
        int id,
        string module,
        string ownerDocument,
        string fileName,
        string contentType,
        string sizeLabel,
        string uploadedBy,
        string tags,
        string description)
    {
        var current = attachments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono załącznika.");
        var updated = current with
        {
            Module = NormalizeRequired(module, "Moduł"),
            OwnerDocument = NormalizeRequired(ownerDocument, "Dokument"),
            FileName = fileName.Trim(),
            ContentType = NormalizeRequired(contentType, "Typ pliku"),
            SizeLabel = NormalizeRequired(sizeLabel, "Rozmiar"),
            UploadedBy = NormalizeRequired(uploadedBy, "Operator"),
            Tags = NormalizeOptional(tags),
            Description = NormalizeOptional(description)
        };
        ReplaceAttachment(updated);
        RaiseChanged();
        return updated;
    }

    public void ArchiveAttachment(int id)
    {
        var current = attachments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono załącznika.");
        ReplaceAttachment(current with { IsArchived = !current.IsArchived });
        RaiseChanged();
    }

    public void DeleteAttachment(int id)
    {
        if (attachments.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono załącznika.");
        }
        RaiseChanged();
    }

    public DocumentHistoryRecord CreateHistoryEntry(
        string module,
        string documentType,
        string documentNumber,
        string entryType,
        string status,
        string actor,
        string description)
    {
        var created = new DocumentHistoryRecord(
            nextHistoryId++,
            NormalizeRequired(module, "Moduł"),
            NormalizeRequired(documentType, "Typ dokumentu"),
            NormalizeRequired(documentNumber, "Numer dokumentu"),
            NormalizeRequired(entryType, "Typ wpisu"),
            NormalizeRequired(status, "Status"),
            NormalizeRequired(actor, "Operator"),
            NormalizeRequired(description, "Opis"),
            DateTime.Now);
        historyEntries.Add(created);
        RaiseChanged();
        return created;
    }

    public DocumentHistoryRecord UpdateHistoryEntry(
        int id,
        string module,
        string documentType,
        string documentNumber,
        string entryType,
        string status,
        string actor,
        string description)
    {
        var current = historyEntries.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono wpisu historii.");
        var updated = current with
        {
            Module = NormalizeRequired(module, "Moduł"),
            DocumentType = NormalizeRequired(documentType, "Typ dokumentu"),
            DocumentNumber = NormalizeRequired(documentNumber, "Numer dokumentu"),
            EntryType = NormalizeRequired(entryType, "Typ wpisu"),
            Status = NormalizeRequired(status, "Status"),
            Actor = NormalizeRequired(actor, "Operator"),
            Description = NormalizeRequired(description, "Opis")
        };
        ReplaceHistory(updated);
        RaiseChanged();
        return updated;
    }

    public void DeleteHistoryEntry(int id)
    {
        if (historyEntries.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono wpisu historii.");
        }
        RaiseChanged();
    }

    private static string NormalizeRequired(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{fieldName} jest wymagany.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void ReplaceAttachment(DocumentAttachmentRecord updated)
    {
        var index = attachments.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            attachments[index] = updated;
        }
    }

    private void ReplaceHistory(DocumentHistoryRecord updated)
    {
        var index = historyEntries.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            historyEntries[index] = updated;
        }
    }

    private void Seed()
    {
        attachments.AddRange(
        [
            new DocumentAttachmentRecord(nextAttachmentId++, "Transport", "TR-INT-2026-018", "cmr-scan.pdf", "application/pdf", "245 KB", "Dyspozytor", new DateTime(2026, 5, 14, 8, 45, 0), "CMR, eksport", "Skan CMR dla przewozu zagranicznego.", false),
            new DocumentAttachmentRecord(nextAttachmentId++, "Magazyn", "PZ/2026/0048", "delivery-photo.jpg", "image/jpeg", "180 KB", "Magazynier B", new DateTime(2026, 5, 14, 7, 58, 0), "PZ, dostawa", "Zdjęcie palety przyjętej do magazynu.", false),
            new DocumentAttachmentRecord(nextAttachmentId++, "HR", "EMP-2026-014", "badania-okresowe.pdf", "application/pdf", "92 KB", "Kadry", new DateTime(2026, 5, 10, 10, 15, 0), "HR, dokument", "Zaświadczenie badań okresowych.", false),
            new DocumentAttachmentRecord(nextAttachmentId++, "Finanse", "FV/2026/0142", "invoice-export.xml", "application/xml", "48 KB", "Finanse", new DateTime(2026, 5, 14, 13, 24, 0), "JPK, faktura", "Eksport faktury do zewnętrznego obiegu.", true)
        ]);

        historyEntries.AddRange(
        [
            new DocumentHistoryRecord(nextHistoryId++, "Transport", "TransportOrder", "TR-INT-2026-018", "Status", "W realizacji", "Dyspozytor", "Potwierdzono wyjazd kierowcy i odprawę celną.", new DateTime(2026, 5, 14, 9, 10, 0)),
            new DocumentHistoryRecord(nextHistoryId++, "Magazyn", "WarehouseDocument", "WZ/2026/0012", "Rezerwacja", "W buforze", "Magazynier A", "Utworzono rezerwację brakujących filtrów.", new DateTime(2026, 5, 14, 11, 28, 0)),
            new DocumentHistoryRecord(nextHistoryId++, "Finanse", "Invoice", "FV/2026/0135", "Rozliczenie", "Częściowo rozliczona", "Finanse", "Powiązano wpływ 3 500 PLN z fakturą eksportową.", new DateTime(2026, 5, 14, 15, 30, 0)),
            new DocumentHistoryRecord(nextHistoryId++, "HR", "LeaveRequest", "LR/2026/0081", "Decyzja", "Zatwierdzony", "Kierownik HR", "Wniosek urlopowy zaakceptowany po weryfikacji obsady.", new DateTime(2026, 5, 13, 14, 5, 0))
        ]);
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}

public sealed record DocumentAttachmentRecord(
    int Id,
    string Module,
    string OwnerDocument,
    string FileName,
    string ContentType,
    string SizeLabel,
    string UploadedBy,
    DateTime UploadedAt,
    string? Tags,
    string? Description,
    bool IsArchived);

public sealed record DocumentHistoryRecord(
    int Id,
    string Module,
    string DocumentType,
    string DocumentNumber,
    string EntryType,
    string Status,
    string Actor,
    string Description,
    DateTime OccurredAt);
