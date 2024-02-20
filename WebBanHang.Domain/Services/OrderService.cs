using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Enums;
using WebBanHang.Domain.Model.Cart;
using WebBanHang.Domain.UseCase.Products;
using WebBanHang.Domain.UseCase.Users_Admin;

namespace WebBanHang.Domain.Services
{
    public class OrderService : IOrderCaculate
    {
        private IRepository<Order> _orderRepository;
        private IRepository<OrderDetail> _orderDetailRepository;
        private IRepository<Users> userInfor;
        private IRepository<Product> _productRepository;
        public OrderService(IRepository<Order> orderRepository, IRepository<OrderDetail> orderDetailRepository,
            IRepository<Users> infor, IRepository<Product> repository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            userInfor = infor;
            _productRepository = repository;
        }

        public async Task AddNewOrder(List<CartItem> items)
        {
            try
            {
                // From given username => userID
                Users user = await userInfor.GetByName(items[0].GetOriginEmail());
                // Create new order => add into Order table => get the lastest ID
                Order newItem = new Order()
                {
                    UserID = user.Id,
                    PaymentID = (int)PaymentMethod.Cash_on_store,
                    StatusID = (int)TransactionStatus.In_process,
                    CreateAt = DateTime.Now
                };
                await _orderRepository.Add(newItem);

                int OrderID = newItem.Id;
                foreach (var item in items)
                {
                    // Add item into OrderDetail
                    OrderDetail detailItem = new OrderDetail()
                    {
                        OrderID = OrderID,
                        ProductID = item.Product!.Id,
                        ProductPrice = item.Product.Price - (item.Product.Price * item.Product.Discount)
                    };
                    await _orderDetailRepository.Add(detailItem);
                    // Subtract equivalent sell quantites
                    await UpdateProductStock(item.Product.Id, item.Quantities);
                }
            } catch (Exception ex)
            {
                throw new AggregateException(ex);
            }
        }

        private async Task UpdateProductStock(int id, int quantities)
        {
            Product item = await _productRepository.GetById(id);
            item.Stock -= quantities;
            // Update back the item
            await _productRepository.Update(item);
        }
    }
}
