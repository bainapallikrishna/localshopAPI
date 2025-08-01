using LocalShop.DBContext;
using LocalShop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Repository
{
    public interface IProductRepository // Changed from class to interface  
    {
        Task<Product> GetByIdAsync(int id);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly AppDataContext _context;
        public ProductRepository(AppDataContext context) => _context = context;

        public async Task<Product> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
    }

}
