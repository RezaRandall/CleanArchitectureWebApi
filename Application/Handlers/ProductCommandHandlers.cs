using Application.Interfaces;
using Application.Products.Commands;
using MediatR;

namespace Application.Handlers;

public class ProductCommandHandlers :
    IRequestHandler<CreateProductCommand, int>,
    IRequestHandler<UpdateProductCommand, bool>,
    IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public ProductCommandHandlers(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productRepository.CreateProduct(request.Product);
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productRepository.UpdateProduct(request.Product);
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productRepository.DeleteProduct(request.ProductId);
    }
}
