using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.HR;

public sealed partial class HrEmployeeDocumentsWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly HrWorkspaceStore store = HrWorkspaceStore.Instance;

    public HrEmployeeDocumentsWorkspaceViewModel()
        : base(
            "/hr/documents",
            "HR",
            "Dokumenty pracownika",
            "Badania, uprawnienia i pliki z terminami ważności oraz archiwum dokumentów.",
            false)
    {
        SummaryCards = new ObservableCollection<HrSummaryCardViewModel>();
        VisibleDocuments = new ObservableCollection<HrEmployeeDocumentRowViewModel>();
        RunDocumentActionCommand = new RelayCommand<string>(RunDocumentAction);
        SelectDocumentCommand = new RelayCommand<HrEmployeeDocumentRowViewModel>(SelectDocument);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<HrSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<HrEmployeeDocumentRowViewModel> VisibleDocuments { get; }
    public IRelayCommand<string> RunDocumentActionCommand { get; }
    public IRelayCommand<HrEmployeeDocumentRowViewModel> SelectDocumentCommand { get; }

    public IReadOnlyList<HrEmployeeRecord> EmployeeOptions => store.GetEmployees();
    public IReadOnlyList<string> DocumentTypeFilterOptions => ["Wszystkie typy", .. store.GetDocuments().Select(item => item.DocumentType).Distinct().OrderBy(item => item)];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedDocumentTypeFilter = "Wszystkie typy";

    [ObservableProperty]
    private bool showArchived = true;

    [ObservableProperty]
    private bool onlyExpiringSoon;

    [ObservableProperty]
    private HrEmployeeDocumentRowViewModel? selectedDocument;

    [ObservableProperty]
    private int? selectedDocumentId;

    [ObservableProperty]
    private HrEmployeeRecord? selectedEmployee;

    [ObservableProperty]
    private string editorDocumentType = string.Empty;

    [ObservableProperty]
    private string editorTitle = string.Empty;

    [ObservableProperty]
    private string editorDocumentNumber = string.Empty;

    [ObservableProperty]
    private string editorIssuedAt = string.Empty;

    [ObservableProperty]
    private string editorValidUntil = string.Empty;

    [ObservableProperty]
    private string editorNotes = string.Empty;

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz dokument lub dodaj nowy wpis pracowniczy.";

    public bool HasSelectedDocument => SelectedDocumentId is not null;

    partial void OnSearchTextChanged(string value) => RefreshDocuments();
    partial void OnSelectedDocumentTypeFilterChanged(string value) => RefreshDocuments();
    partial void OnShowArchivedChanged(bool value) => RefreshDocuments();
    partial void OnOnlyExpiringSoonChanged(bool value) => RefreshDocuments();

    partial void OnSelectedDocumentChanged(HrEmployeeDocumentRowViewModel? value)
    {
        SelectedDocumentId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedDocumentIdChanged(int? value) => OnPropertyChanged(nameof(HasSelectedDocument));

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshDocuments();
        LoadEditor();
        OnPropertyChanged(nameof(EmployeeOptions));
        OnPropertyChanged(nameof(DocumentTypeFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var documents = store.GetDocuments();
        SummaryCards.Clear();
        SummaryCards.Add(new HrSummaryCardViewModel("Dokumenty aktywne", documents.Count(item => item.IsActive && !item.IsArchived).ToString(), "Wpisy aktualnie używane przez HR i transport.", "#1D4ED8"));
        SummaryCards.Add(new HrSummaryCardViewModel("Do odnowienia", documents.Count(item => item.IsExpiringSoon).ToString(), "Uprawnienia i badania z terminem do 45 dni.", "#D97706"));
        SummaryCards.Add(new HrSummaryCardViewModel("Archiwum", documents.Count(item => item.IsArchived).ToString(), "Dokumenty historyczne i wygasłe.", "#7B8794"));
        SummaryCards.Add(new HrSummaryCardViewModel("Kierowcy", documents.Count(item => store.GetEmployees().FirstOrDefault(employee => employee.Id == item.EmployeeId)?.HasDriverProfile == true).ToString(), "Dokumenty przypisane do profili kierowców.", "#1F8A5B"));
    }

    private void RefreshDocuments()
    {
        var rows = store.GetDocuments()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 store.GetEmployeeName(item.EmployeeId).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.DocumentType.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedDocumentTypeFilter == "Wszystkie typy" ||
                 string.Equals(item.DocumentType, SelectedDocumentTypeFilter, StringComparison.OrdinalIgnoreCase)) &&
                (ShowArchived || !item.IsArchived) &&
                (!OnlyExpiringSoon || item.IsExpiringSoon))
            .Select(item => new HrEmployeeDocumentRowViewModel(
                item.Id,
                store.GetEmployeeName(item.EmployeeId),
                item.DocumentType,
                item.Title,
                item.DocumentNumber ?? "-",
                item.ValidUntil?.ToString("dd.MM.yyyy") ?? "-",
                item.StatusLabel,
                item.StatusColor,
                item.Notes ?? "-"))
            .ToArray();

        VisibleDocuments.Clear();
        foreach (var row in rows)
        {
            VisibleDocuments.Add(row);
        }

        SelectedDocument = SelectedDocumentId is not null
            ? VisibleDocuments.FirstOrDefault(item => item.Id == SelectedDocumentId) ?? VisibleDocuments.FirstOrDefault()
            : VisibleDocuments.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedDocumentId is null)
        {
            SelectedEmployee = EmployeeOptions.FirstOrDefault();
            EditorDocumentType = string.Empty;
            EditorTitle = string.Empty;
            EditorDocumentNumber = string.Empty;
            EditorIssuedAt = string.Empty;
            EditorValidUntil = string.Empty;
            EditorNotes = string.Empty;
            EditorIsActive = true;
            return;
        }

        var document = store.GetDocuments().FirstOrDefault(item => item.Id == SelectedDocumentId);
        if (document is null)
        {
            return;
        }

        SelectedEmployee = EmployeeOptions.FirstOrDefault(item => item.Id == document.EmployeeId) ?? EmployeeOptions.FirstOrDefault();
        EditorDocumentType = document.DocumentType;
        EditorTitle = document.Title;
        EditorDocumentNumber = document.DocumentNumber ?? string.Empty;
        EditorIssuedAt = document.IssuedAt?.ToString("dd.MM.yyyy") ?? string.Empty;
        EditorValidUntil = document.ValidUntil?.ToString("dd.MM.yyyy") ?? string.Empty;
        EditorNotes = document.Notes ?? string.Empty;
        EditorIsActive = document.IsActive;
    }

    private void RunDocumentAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedDocument = null;
                    SelectedDocumentId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nowy dokument pracowniczy.";
                    break;
                case "Save":
                    SaveDocument();
                    break;
                case "Archive":
                    if (SelectedDocumentId is not null)
                    {
                        store.ArchiveDocument(SelectedDocumentId.Value);
                        LastActionMessage = "Zarchiwizowano dokument.";
                    }
                    break;
                case "Restore":
                    if (SelectedDocumentId is not null)
                    {
                        store.RestoreDocument(SelectedDocumentId.Value);
                        LastActionMessage = "Przywrócono dokument.";
                    }
                    break;
                case "Delete":
                    if (SelectedDocumentId is not null)
                    {
                        store.DeleteDocument(SelectedDocumentId.Value);
                        SelectedDocumentId = null;
                        SelectedDocument = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto dokument.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveDocument()
    {
        var employee = SelectedEmployee ?? throw new InvalidOperationException("Wybierz pracownika.");
        var issuedAt = HrEditorParsers.ParseOptionalDate(EditorIssuedAt);
        var validUntil = HrEditorParsers.ParseOptionalDate(EditorValidUntil);

        if (SelectedDocumentId is null)
        {
            var created = store.CreateDocument(employee.Id, EditorDocumentType, EditorTitle, EditorDocumentNumber, issuedAt, validUntil, EditorNotes, EditorIsActive);
            SelectedDocumentId = created.Id;
            LastActionMessage = $"Dodano dokument {created.Title}.";
            return;
        }

        var updated = store.UpdateDocument(SelectedDocumentId.Value, employee.Id, EditorDocumentType, EditorTitle, EditorDocumentNumber, issuedAt, validUntil, EditorNotes, EditorIsActive);
        LastActionMessage = $"Zapisano dokument {updated.Title}.";
    }

    private void SelectDocument(HrEmployeeDocumentRowViewModel? document)
    {
        SelectedDocument = document;
    }
}

public sealed record HrEmployeeDocumentRowViewModel(
    int Id,
    string EmployeeName,
    string DocumentType,
    string Title,
    string DocumentNumber,
    string ValidUntilLabel,
    string StatusLabel,
    string StatusColor,
    string Notes);
