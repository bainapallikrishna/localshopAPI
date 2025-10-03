using LocalShop.Domain.Interfaces;
using LocalShop.Domain.Models;
using LocalShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Infrastructure.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(LocalShopDbContext context) : base(context)
        {
        }
     
        // Inherits GetAllAsync, GetByIdAsync, AddAsync from RepositoryBase
        // Add product-specific queries here if needed
    }
}
