namespace ErpSystem.Application.Modules.System.Foundation;

public interface IMenuBuilder
{
    IReadOnlyList<MenuSectionView> Build(IReadOnlyCollection<string> permissions);
}
