using RestApi.Models;
using RestApi.Models.ViewModels;

namespace RestApi.Repositories.Interfaces
{
	public interface IOrderRepository
	{
		public Task<List<OrderModel>> GetOrdersByUserAsync(int id_User);
		public Task<int> AddOrderAsync(int id_User, OrderVM order);
		public Task<bool> DeleteAOrderByUserAsync(int id_User, int id_Order);
		public Task<bool> DeleteAllOrdersByUserAsync(int id_User);
		public Task<List<OrderDetailModel>> GetOrderDetailsAsync(int id_User, int id_Order);
		public Task<int> AddOrderDetailAsync(int id_User, int id_Order, OrderDetailVM orderDetail);
		public Task<int> DeleteAOrderDetailByOrderIdAsync(int id_User, int id_Order, int id_OrderDetail);
		public Task<int> DeleteAllOrdersDetailByOrderIdAsync(int id_User, int id_Order);

	}
}
