using ECommerce513.Data;
using ECommerce513.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerce513.Repository
{
    public class CategoryRepository
    {

        private readonly ApplicationDbContext _context = new();

        // CRUD
        public async Task<bool> CreateAsync(Category category)
        {
            try
            {

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return true;

            }
            catch(Exception ex)
            {

                Console.WriteLine($"{ex.Message}");
                return false;

            }
        }


        public async Task<bool> EditAsync(Category category)
        {
            try
            {

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"{ex.Message}");
                return false;

            }
        }

        public async Task<bool> DeleteAsync(Category category)
        {
            try
            {

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"{ex.Message}");
                return false;

            }
        }

        public async Task<IEnumerable<Category>> GetAsync(Expression<Func<Category, bool>>? expression = null,
            Expression<Func<Category, object>>[]? includes = null, bool tracked = true)
        {
            IQueryable<Category> categories = _context.Categories;

            if(expression is not null)
            {
                categories = categories.Where(expression);
            }

            if(includes is not null)
            {
                foreach (var item in includes)
                {
                    categories = categories.Include(item);
                }
            }

            if(!tracked)
            {
                categories = categories.AsNoTracking();
            }

            return (await categories.ToListAsync());
        }

        public Category? GetOne(Expression<Func<Category, bool>>? expression = null,
            Expression<Func<Category, object>>[]? includes = null, bool tracked = true)
        {
            return GetAsync(expression, includes, tracked).GetAwaiter().GetResult().FirstOrDefault();
        }

    }
}
