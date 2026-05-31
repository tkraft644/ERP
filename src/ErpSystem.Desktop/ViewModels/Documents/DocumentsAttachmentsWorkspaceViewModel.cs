using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.Documents;

public sealed partial class DocumentsAttachmentsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly DocumentsWorkspaceStore store = DocumentsWorkspaceStore.Instance;

    public DocumentsAttachmentsWorkspaceViewModel()
        : base("/documents/attachments", "Dokumenty", "Załączniki", "Wspólny obieg plików dla transportu, magazynu, HR i finansów.", false)
    {
        SummaryCards = new ObservableCollection<SummaryCardViewModel>();
        VisibleAttachments = new ObservableCollection<DocumentAttachmentRowViewModel>();
        RunActionCommand = new RelayCommand<string>(RunAction);
        SelectAttachmentCommand = new RelayCommand<DocumentAttachmentRowViewModel>(SelectAttachment);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<DocumentAttachmentRowViewModel> VisibleAttachments { get; }
    public IRelayCommand<string> RunActionCommand { get; }
    public IRelayCommand<DocumentAttachmentRowViewModel> SelectAttachmentCommand { get; }

    public IReadOnlyList<string> ModuleFilterOptions => ["Wszystkie moduły", .. store.GetModules()];

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedModuleFilter = "Wszystkie moduły";
    [ObservableProperty] private bool showArchived;
    [ObservableProperty] private DocumentAttachmentRowViewModel? selectedAttachment;
    [ObservableProperty] private int? selectedAttachmentId;
    [ObservableProperty] private string editorModule = "Magazyn";
    [ObservableProperty] private string editorOwnerDocument = string.Empty;
    [ObservableProperty] private string editorFileName = string.Empty;
    [ObservableProperty] private string editorContentType = "application/pdf";
    [ObservableProperty] private string editorSizeLabel = "0 KB";
    [ObservableProperty] private string editorUploadedBy = "Operator";
    [ObservableProperty] private string editorTags = string.Empty;
    [ObservableProperty] private string editorDescription = string.Empty;
    [ObservableProperty] private string lastActionMessage = "Zarządzaj załącznikami i ich wykorzystaniem między modułami.";

    public bool HasSelectedAttachment => SelectedAttachmentId is not null;
    public DocumentAttachmentRecord? SelectedRecord => SelectedAttachmentId is null ? null : store.GetAttachments().FirstOrDefault(item => item.Id == SelectedAttachmentId);
    public string SelectedUploadedAt => SelectedRecord?.UploadedAt.ToString("dd.MM.yyyy HH:mm") ?? "-";
    public string SelectedArchiveLabel => SelectedRecord?.IsArchived == true ? "Archiwum" : "Aktywny";

    partial void OnSearchTextChanged(string value) => RefreshRows();
    partial void OnSelectedModuleFilterChanged(string value) => RefreshRows();
    partial void OnShowArchivedChanged(bool value) => RefreshRows();

    partial void OnSelectedAttachmentChanged(DocumentAttachmentRowViewModel? value)
    {
        SelectedAttachmentId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedAttachmentIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedAttachment));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(SelectedUploadedAt));
        OnPropertyChanged(nameof(SelectedArchiveLabel));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshRows();
        LoadEditor();
        OnPropertyChanged(nameof(ModuleFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var rows = store.GetAttachments();
        SummaryCards.Clear();
        SummaryCards.Add(new SummaryCardViewModel("Aktywne pliki", rows.Count(item => !item.IsArchived).ToString(), "Załączniki dostępne dla bieżących procesów.", "#2563EB"));
        SummaryCards.Add(new SummaryCardViewModel("PDF / skany", rows.Count(item => item.ContentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) || item.ContentType.Contains("image", StringComparison.OrdinalIgnoreCase)).ToString(), "Dokumenty wizualne gotowe do obiegu.", "#1F8A5B"));
        SummaryCards.Add(new SummaryCardViewModel("Moduły", rows.Select(item => item.Module).Distinct().Count().ToString(), "Obszary korzystające ze wspólnego repozytorium.", "#D97706"));
        SummaryCards.Add(new SummaryCardViewModel("Archiwum", rows.Count(item => item.IsArchived).ToString(), "Pliki wyłączone z aktywnego obiegu.", "#7B8794"));
    }

    private void RefreshRows()
    {
        var rows = store.GetAttachments()
            .Where(item =>
                (ShowArchived || !item.IsArchived) &&
                (SelectedModuleFilter == "Wszystkie moduły" || string.Equals(item.Module, SelectedModuleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.OwnerDocument.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 (item.Tags?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)))
            .Select(item => new DocumentAttachmentRowViewModel(
                item.Id,
                item.Module,
                item.OwnerDocument,
                item.FileName,
                item.ContentType,
                item.SizeLabel,
                item.UploadedBy,
                item.UploadedAt.ToString("dd.MM HH:mm"),
                item.IsArchived ? "Archiwum" : "Aktywny",
                item.IsArchived ? "#7B8794" : "#1F8A5B"))
            .ToArray();

        VisibleAttachments.Clear();
        foreach (var row in rows)
        {
            VisibleAttachments.Add(row);
        }

        SelectedAttachment = SelectedAttachmentId is null
            ? VisibleAttachments.FirstOrDefault()
            : VisibleAttachments.FirstOrDefault(item => item.Id == SelectedAttachmentId) ?? VisibleAttachments.FirstOrDefault();
    }

    private void LoadEditor()
    {
        var record = SelectedRecord;
        if (record is null)
        {
            EditorModule = "Magazyn";
            EditorOwnerDocument = string.Empty;
            EditorFileName = string.Empty;
            EditorContentType = "application/pdf";
            EditorSizeLabel = "0 KB";
            EditorUploadedBy = "Operator";
            EditorTags = string.Empty;
            EditorDescription = string.Empty;
            return;
        }

        EditorModule = record.Module;
        EditorOwnerDocument = record.OwnerDocument;
        EditorFileName = record.FileName;
        EditorContentType = record.ContentType;
        EditorSizeLabel = record.SizeLabel;
        EditorUploadedBy = record.UploadedBy;
        EditorTags = record.Tags ?? string.Empty;
        EditorDescription = record.Description ?? string.Empty;
    }

    private void SelectAttachment(DocumentAttachmentRowViewModel? row)
    {
        if (row is not null)
        {
            SelectedAttachment = row;
        }
    }

    private void RunAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedAttachment = null;
                    SelectedAttachmentId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy załącznik.";
                    break;
                case "Save":
                    Save();
                    break;
                case "Delete":
                    Delete();
                    break;
                case "Archive":
                    ToggleArchive();
                    break;
                case "Export":
                    LastActionMessage = $"Wyeksportowano rejestr {VisibleAttachments.Count} załączników.";
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void Save()
    {
        if (SelectedAttachmentId is null)
        {
            var created = store.CreateAttachment(EditorModule, EditorOwnerDocument, EditorFileName, EditorContentType, EditorSizeLabel, EditorUploadedBy, EditorTags, EditorDescription);
            SelectedAttachmentId = created.Id;
            LastActionMessage = $"Dodano plik {created.FileName}.";
            return;
        }

        var updated = store.UpdateAttachment(SelectedAttachmentId.Value, EditorModule, EditorOwnerDocument, EditorFileName, EditorContentType, EditorSizeLabel, EditorUploadedBy, EditorTags, EditorDescription);
        LastActionMessage = $"Zapisano plik {updated.FileName}.";
    }

    private void Delete()
    {
        if (SelectedAttachmentId is null)
        {
            throw new InvalidOperationException("Wybierz załącznik do usunięcia.");
        }

        store.DeleteAttachment(SelectedAttachmentId.Value);
        SelectedAttachmentId = null;
        LastActionMessage = "Usunięto załącznik.";
    }

    private void ToggleArchive()
    {
        if (SelectedAttachmentId is null)
        {
            throw new InvalidOperationException("Wybierz załącznik do archiwizacji.");
        }

        var record = SelectedRecord;
        store.ArchiveAttachment(SelectedAttachmentId.Value);
        LastActionMessage = record?.IsArchived == true
            ? $"Przywrócono plik {record.FileName}."
            : $"Zarchiwizowano plik {record?.FileName}.";
    }
}

public sealed record DocumentAttachmentRowViewModel(
    int Id,
    string Module,
    string OwnerDocument,
    string FileName,
    string ContentType,
    string SizeLabel,
    string UploadedBy,
    string UploadedAtLabel,
    string StatusLabel,
    string StatusColor);
