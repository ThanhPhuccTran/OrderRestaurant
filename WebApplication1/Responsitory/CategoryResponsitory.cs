﻿using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderRestaurant.Responsitory
{
    public class CategoryResponsitory : ICategory
    {
        private readonly ApplicationDBContext _context;
        public CategoryResponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public Task<bool> CategoryExit(int id)
        {
            return _context.Categoies.AnyAsync(s => s.CategoryId == id);
        }

        public async Task<Category> CreateCategory(Category categoryFood)
        {
            await _context.Categoies.AddAsync(categoryFood);
            await _context.SaveChangesAsync();
            return categoryFood;
        }

        public async Task<Category?> DeleteCategory(int id)
        {
            var model = await _context.Categoies.FirstOrDefaultAsync(i => i.CategoryId == id);
            if (model == null)
            {
                return null;
            }
            _context.Categoies.Remove(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Category?> GetCategoryFoodById(int id)
        {
            return await _context.Categoies.FindAsync(id);
        }

        public async Task<(int totalItems, int totalPages, List<Category> category)> GetSearch(QuerryObject querry, string search = "")
        {
            var list = _context.Categoies.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                list = list.Where(f => EF.Functions.Like(f.CategoryName, $"%{search}%"));

            }
            var totalItems = await list.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / querry.PageSize);
            var skipNumber = (querry.PageNumber - 1) * querry.PageSize;

            var categories = await list.Select(f => new Category 
            {
                CategoryId = f.CategoryId,
                CategoryName = f.CategoryName,
                Description = f.Description,
                
            
            })  .Skip(skipNumber)
                .Take(querry.PageSize)
                .ToListAsync();
            return (totalItems, totalPages, categories);
        }
        public async Task<List<Category>> GetCategoryFoods()
        {
            return await _context.Categoies.ToListAsync();
        }
        public async Task<Category> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDTO)
        {
            var categoryupdate = await _context.Categoies.FirstOrDefaultAsync(hh => hh.CategoryId == id);
            if (categoryupdate == null)
            {
                return null;
            }

            categoryupdate.CategoryName = updateCategoryDTO.CategoryName;
            categoryupdate.Description = updateCategoryDTO.Description;
            await _context.SaveChangesAsync();
            return categoryupdate;
        }
    }
}
