namespace OrderFlow.Application.IServices
{
    public interface IServiceBase
    {
        public IProductService ProductService { get; }
        public ICategoryService CategoryService { get; }
        public IUserService UserService { get; }
        public IOrderService OrderService { get; }
    }
}
