﻿using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.CategoryDTO
{
    public static class CategoryMapper
    {
        public static CategoryModel ToCategoryDto(this Category model)
        {
            return new CategoryModel
            {
                CategoryId = model.CategoryId,
                CategoryName = model.CategoryName,
                Description = model.Description
                
            };
        }

        public static Category ToCategoryFromCreate(this CreateCategoryDTO model)
        {
            return new Category
            {
                CategoryName = model.CategoryName,
                Description = model.Description,
            };
        }
    }
}
