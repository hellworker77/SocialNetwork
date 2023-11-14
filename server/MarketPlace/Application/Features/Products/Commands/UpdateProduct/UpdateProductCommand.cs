﻿using Application.Features.Products.Commands.DeleteProduct;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public float Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public byte Rate { get; set; }
    public int RemainingCount { get; set; }
}

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(command.Id);

        if (product is not null)
        {
            product.Title = command.Title;
            product.Description = command.Description;
            product.Category = command.Category;
            product.Price = command.Price;
            product.Rate = command.Rate;
            product.RemainingCount = command.RemainingCount;
            
            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            product.AddDomainEvent(new ProductUpdatedEvent(product));

            await _unitOfWork.Save(cancellationToken);

            return await Result<Guid>.SuccessAsync(product.Id, "Product updated.");
        }
        else
        {
            return await Result<Guid>.FailureAsync("Product not fount");
        }
    }
}