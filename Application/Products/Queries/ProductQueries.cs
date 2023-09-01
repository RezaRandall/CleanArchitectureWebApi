using Domain.Models;
using MediatR;

namespace Application.Products.Queries;

public class GetAllProductQuery : IRequest<List<Product>>
{
}

public class GetProductByNameQuery : IRequest<Product> 
{
    public string Name { get; set; }
}
