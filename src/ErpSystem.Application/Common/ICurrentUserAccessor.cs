namespace ErpSystem.Application.Common;

public interface ICurrentUserAccessor
{
    int? UserId { get; }
}
