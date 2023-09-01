using Domain.Models;
using MediatR;

namespace Application.Products.Commands;

public class CreateProductCommand :IRequest<int>
{
    public Product Product { get; set; }
}

public class UpdateProductCommand : IRequest<bool>
{
    public Product Product { get; set; }
}

public class DeleteProductCommand : IRequest<bool>
{
    public int ProductId { get; set; }
}
