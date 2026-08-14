using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Helper.Attributes;
using OrderFlow.Application.IServices;

namespace OrderFlow.Application.Services
{
    [Scoped]
    public class ServiceBase(IServiceProvider serviceProvider) : IServiceBase
    {
        private IProductService _productService;
        public IProductService ProductService 
            => _productService ??= serviceProvider.GetService<IProductService>();

        private ICategoryService _categoryService;
        public ICategoryService CategoryService
            => _categoryService ??= serviceProvider.GetService<ICategoryService>();

        private IUserService _userService;
        public IUserService UserService
            => _userService ??= serviceProvider.GetService<IUserService>();

        private IOrderService _orderService;
        public IOrderService OrderService
            => _orderService ??= serviceProvider.GetService<IOrderService>();
    }
}
