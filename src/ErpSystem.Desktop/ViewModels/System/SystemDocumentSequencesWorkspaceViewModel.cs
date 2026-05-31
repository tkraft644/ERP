using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels.System;

public sealed partial class SystemDocumentSequencesWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly SystemWorkspaceStore store = SystemWorkspaceStore.Instance;

    public SystemDocumentSequencesWorkspaceViewModel()
        : base(
            "/system/document-numbers",
            "System",
            "Document sequences",
            "Centralna numeracja dla dokumentów magazynowych, transportowych, HR i finansów.",
            false)
    {
        SummaryCards = new ObservableCollection<SystemSummaryCardViewModel>();
        VisibleSequences = new ObservableCollection<SystemDocumentSequenceRecord>();
        RunSequenceActionCommand = new RelayCommand<string>(RunSequenceAction);
        SelectSequenceCommand = new RelayCommand<SystemDocumentSequenceRecord>(item => SelectedSequence = item);

        store.Changed += OnStoreChanged;
        Refresh();
    }

    public ObservableCollection<SystemSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<SystemDocumentSequenceRecord> VisibleSequences { get; }
    public IRelayCommand<string> RunSequenceActionCommand { get; }
    public IRelayCommand<SystemDocumentSequenceRecord> SelectSequenceCommand { get; }

    public IReadOnlyList<string> ModuleFilterOptions => ["Wszystkie moduły", .. store.GetSequences().Select(item => item.Module).Distinct().OrderBy(item => item)];
    public IReadOnlyList<int> NumberLengthOptions => [4, 5, 6, 7, 8];
    public IReadOnlyList<string> ResetPolicyOptions => ["Miesięczny", "Roczny", "Bez resetu"];

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedModuleFilter = "Wszystkie moduły";

    [ObservableProperty]
    private bool showInactive = true;

    [ObservableProperty]
    private SystemDocumentSequenceRecord? selectedSequence;

    [ObservableProperty]
    private int? selectedSequenceId;

    [ObservableProperty]
    private string editorKey = string.Empty;

    [ObservableProperty]
    private string editorModule = string.Empty;

    [ObservableProperty]
    private string editorPrefix = string.Empty;

    [ObservableProperty]
    private int editorNumberLength = 4;

    [ObservableProperty]
    private int editorCurrentNumber;

    [ObservableProperty]
    private string editorResetPolicy = "Miesięczny";

    [ObservableProperty]
    private bool editorIsActive = true;

    [ObservableProperty]
    private string lastGeneratedPreview = "-";

    [ObservableProperty]
    private string lastActionMessage = "Wybierz sekwencję albo utwórz nową, aby zarządzać numeracją dokumentów.";

    public bool HasSelectedSequence => SelectedSequenceId is not null;

    partial void OnSearchTextChanged(string value) => RefreshSequences();
    partial void OnSelectedModuleFilterChanged(string value) => RefreshSequences();
    partial void OnShowInactiveChanged(bool value) => RefreshSequences();

    partial void OnSelectedSequenceChanged(SystemDocumentSequenceRecord? value)
    {
        SelectedSequenceId = value?.Id;
        LoadEditor();
    }

    partial void OnSelectedSequenceIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasSelectedSequence));
    }

    private void OnStoreChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RefreshSummaryCards();
        RefreshSequences();
        LoadEditor();
        OnPropertyChanged(nameof(ModuleFilterOptions));
    }

    private void RefreshSummaryCards()
    {
        var sequences = store.GetSequences();
        SummaryCards.Clear();
        SummaryCards.Add(new SystemSummaryCardViewModel("Aktywne sekwencje", sequences.Count(item => item.IsActive).ToString(), "Numeratory dostępne do generowania dokumentów.", "#2563EB"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Moduły", sequences.Select(item => item.Module).Distinct().Count().ToString(), "Obszary korzystające z centralnej numeracji.", "#1F8A5B"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Ostatnie generacje", sequences.Count(item => item.LastGeneratedAt >= DateTime.UtcNow.AddDays(-1)).ToString(), "Sekwencje używane w ostatnich 24 godzinach.", "#D97706"));
        SummaryCards.Add(new SystemSummaryCardViewModel("Nieaktywne", sequences.Count(item => !item.IsActive).ToString(), "Sekwencje wstrzymane lub historyczne.", "#D14343"));
    }

    private void RefreshSequences()
    {
        var items = store.GetSequences()
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Key.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Module.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Prefix.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedModuleFilter == "Wszystkie moduły" ||
                 string.Equals(item.Module, SelectedModuleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (ShowInactive || item.IsActive))
            .ToArray();

        VisibleSequences.Clear();
        foreach (var item in items)
        {
            VisibleSequences.Add(item);
        }

        SelectedSequence = SelectedSequenceId is not null
            ? VisibleSequences.FirstOrDefault(item => item.Id == SelectedSequenceId) ?? VisibleSequences.FirstOrDefault()
            : VisibleSequences.FirstOrDefault();
    }

    private void LoadEditor()
    {
        if (SelectedSequenceId is null)
        {
            EditorKey = string.Empty;
            EditorModule = string.Empty;
            EditorPrefix = string.Empty;
            EditorNumberLength = 4;
            EditorCurrentNumber = 0;
            EditorResetPolicy = "Miesięczny";
            EditorIsActive = true;
            LastGeneratedPreview = "-";
            return;
        }

        var sequence = store.GetSequences().FirstOrDefault(item => item.Id == SelectedSequenceId);
        if (sequence is null)
        {
            return;
        }

        EditorKey = sequence.Key;
        EditorModule = sequence.Module;
        EditorPrefix = sequence.Prefix;
        EditorNumberLength = sequence.NumberLength;
        EditorCurrentNumber = sequence.CurrentNumber;
        EditorResetPolicy = sequence.ResetPolicy;
        EditorIsActive = sequence.IsActive;
        LastGeneratedPreview = $"{sequence.Prefix}/{DateTime.Now:yyyyMM}/{sequence.CurrentNumber.ToString().PadLeft(sequence.NumberLength, '0')}";
    }

    private void RunSequenceAction(string? action)
    {
        try
        {
            switch (action)
            {
                case "New":
                    SelectedSequence = null;
                    SelectedSequenceId = null;
                    LoadEditor();
                    LastActionMessage = "Przygotowano nową sekwencję.";
                    break;
                case "Save":
                    SaveSequence();
                    break;
                case "Generate":
                    if (SelectedSequenceId is not null)
                    {
                        LastGeneratedPreview = store.GenerateNextDocumentNumber(SelectedSequenceId.Value);
                        LastActionMessage = $"Wygenerowano numer {LastGeneratedPreview}.";
                    }
                    break;
                case "Delete":
                    if (SelectedSequenceId is not null)
                    {
                        store.DeleteSequence(SelectedSequenceId.Value);
                        SelectedSequence = null;
                        SelectedSequenceId = null;
                        LoadEditor();
                        LastActionMessage = "Usunięto sekwencję.";
                    }
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            LastActionMessage = exception.Message;
        }
    }

    private void SaveSequence()
    {
        if (SelectedSequenceId is null)
        {
            var created = store.CreateSequence(EditorKey, EditorModule, EditorPrefix, EditorNumberLength, EditorCurrentNumber, EditorResetPolicy, EditorIsActive);
            SelectedSequenceId = created.Id;
            LastActionMessage = $"Dodano sekwencję {created.Key}.";
            return;
        }

        var updated = store.UpdateSequence(SelectedSequenceId.Value, EditorKey, EditorModule, EditorPrefix, EditorNumberLength, EditorCurrentNumber, EditorResetPolicy, EditorIsActive);
        LastActionMessage = $"Zapisano sekwencję {updated.Key}.";
    }
}
