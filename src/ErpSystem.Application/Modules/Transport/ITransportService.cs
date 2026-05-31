namespace ErpSystem.Application.Modules.Transport;

public interface ITransportService
{
    Task<TransportReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransportOrderListItemView>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<TransportOrderDetailsView?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<TransportOrderDetailsView> CreateOrderAsync(SaveTransportOrderRequest request, CancellationToken cancellationToken = default);
    Task<TransportOrderDetailsView?> UpdateOrderAsync(int orderId, SaveTransportOrderRequest request, CancellationToken cancellationToken = default);
    Task<TransportOrderDetailsView?> ChangeStatusAsync(int orderId, ChangeTransportOrderStatusRequest request, CancellationToken cancellationToken = default);
}
