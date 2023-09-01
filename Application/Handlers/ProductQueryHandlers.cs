using Application.Interfaces;
using Application.Products.Queries;
using Domain.Models;
using MediatR;

namespace Application.Handlers
{
    public class ProductQueryHandlers :
        IRequestHandler<GetAllProductQuery, List<Product>>,
        IRequestHandler<GetProductByNameQuery, Product>

    {
        private readonly IProductRepository _productRepository;

        public ProductQueryHandlers(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetAllProducts();
        }

        public async Task<Product> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductByName(request.Name);
        }
    }
}
