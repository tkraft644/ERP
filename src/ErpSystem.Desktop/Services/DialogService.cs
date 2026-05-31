namespace ErpSystem.Desktop.Services;

public sealed class DialogService
{
    public Task<bool> ConfirmTabCloseAsync(string title, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(true);
    }
}
