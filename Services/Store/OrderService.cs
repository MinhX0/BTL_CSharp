using backend.Entities.Store;
using backend.Repositories.Store;

namespace backend.Services.Store
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetOrderWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        {
            return await _orderRepository.GetOrdersByCustomerAsync(customerId);
        }

        public async Task<Order> CreateAsync(Order order, IEnumerable<OrderDetail> orderDetails)
        {
            var detailsList = orderDetails?.ToList() ?? new List<OrderDetail>();
            if (!detailsList.Any())
            {
                throw new ArgumentException("Order must contain at least one order detail.", nameof(orderDetails));
            }

            if (order.TotalAmount <= 0)
            {
                order.TotalAmount = detailsList.Sum(d => d.Price * d.Quantity);
            }

            await _orderRepository.AddAsync(order);

            foreach (var detail in detailsList)
            {
                detail.OrderId = order.OrderId;
                await _orderDetailRepository.AddAsync(detail);
            }

            return order;
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
            {
                return false;
            }

            order.Status = status;
            _orderRepository.Update(order);
            return true;
        }
    }
}
