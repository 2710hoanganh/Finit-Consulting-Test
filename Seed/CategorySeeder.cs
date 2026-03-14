using TechnicalTest.Api.Entities;

namespace TechnicalTest.Api.Seed;

public static class CategorySeeder
{
    public static List<Category> GetParentCategories()
    {
        return new List<Category>
        {
            new() { Name = "Electronics", Description = "Electronic devices and gadgets", SortOrder = 1, IsActive = true },
            new() { Name = "Clothing", Description = "Fashion and apparel", SortOrder = 2, IsActive = true },
            new() { Name = "Cosmetics", Description = "Beauty and skincare products", SortOrder = 3, IsActive = true },
            new() { Name = "Books", Description = "Books and publications", SortOrder = 4, IsActive = true },
            new() { Name = "Home & Garden", Description = "Home and garden products", SortOrder = 5, IsActive = true },
            new() { Name = "Sports & Outdoors", Description = "Sports and outdoor equipment", SortOrder = 6, IsActive = true },
            new() { Name = "Toys & Games", Description = "Toys and board games", SortOrder = 7, IsActive = true }
        };
    }

    public static List<Category> GetChildCategories(List<Category> parents)
    {
        var children = new List<Category>();
        var parentMap = parents.ToDictionary(p => p.Name, p => p.Id);

        // Electronics children
        children.AddRange(new Category[]
        {
            new() { Name = "Smartphones", Description = "Mobile phones and accessories", ParentCategoryId = parentMap["Electronics"], SortOrder = 1, IsActive = true },
            new() { Name = "Laptops", Description = "Computers and laptops", ParentCategoryId = parentMap["Electronics"], SortOrder = 2, IsActive = true },
            new() { Name = "Tablets", Description = "Tablet devices", ParentCategoryId = parentMap["Electronics"], SortOrder = 3, IsActive = true },
            new() { Name = "Headphones", Description = "Audio headphones and earphones", ParentCategoryId = parentMap["Electronics"], SortOrder = 4, IsActive = true },
            new() { Name = "Cameras", Description = "Digital cameras and video cameras", ParentCategoryId = parentMap["Electronics"], SortOrder = 5, IsActive = true }
        });

        // Clothing children
        children.AddRange(new Category[]
        {
            new() { Name = "Men's Clothing", Description = "Clothing for men", ParentCategoryId = parentMap["Clothing"], SortOrder = 1, IsActive = true },
            new() { Name = "Women's Clothing", Description = "Clothing for women", ParentCategoryId = parentMap["Clothing"], SortOrder = 2, IsActive = true },
            new() { Name = "Kids' Clothing", Description = "Clothing for children", ParentCategoryId = parentMap["Clothing"], SortOrder = 3, IsActive = true },
            new() { Name = "Sportswear", Description = "Athletic and sports clothing", ParentCategoryId = parentMap["Clothing"], SortOrder = 4, IsActive = true },
            new() { Name = "Underwear", Description = "Undergarments and intimate apparel", ParentCategoryId = parentMap["Clothing"], SortOrder = 5, IsActive = true }
        });

        // Cosmetics children
        children.AddRange(new Category[]
        {
            new() { Name = "Skincare", Description = "Skin care products and treatments", ParentCategoryId = parentMap["Cosmetics"], SortOrder = 1, IsActive = true },
            new() { Name = "Makeup", Description = "Cosmetics and makeup products", ParentCategoryId = parentMap["Cosmetics"], SortOrder = 2, IsActive = true },
            new() { Name = "Hair Care", Description = "Hair products and treatments", ParentCategoryId = parentMap["Cosmetics"], SortOrder = 3, IsActive = true },
            new() { Name = "Perfume", Description = "Fragrances and perfumes", ParentCategoryId = parentMap["Cosmetics"], SortOrder = 4, IsActive = true },
            new() { Name = "Body Care", Description = "Body care and bath products", ParentCategoryId = parentMap["Cosmetics"], SortOrder = 5, IsActive = true }
        });

        // Books children
        children.AddRange(new Category[]
        {
            new() { Name = "Fiction", Description = "Fiction books and novels", ParentCategoryId = parentMap["Books"], SortOrder = 1, IsActive = true },
            new() { Name = "Non-Fiction", Description = "Non-fiction books", ParentCategoryId = parentMap["Books"], SortOrder = 2, IsActive = true },
            new() { Name = "Children's Books", Description = "Books for children", ParentCategoryId = parentMap["Books"], SortOrder = 3, IsActive = true },
            new() { Name = "Textbooks", Description = "Educational textbooks", ParentCategoryId = parentMap["Books"], SortOrder = 4, IsActive = true },
            new() { Name = "Comics", Description = "Comic books and graphic novels", ParentCategoryId = parentMap["Books"], SortOrder = 5, IsActive = true }
        });

        // Home & Garden children
        children.AddRange(new Category[]
        {
            new() { Name = "Furniture", Description = "Home furniture", ParentCategoryId = parentMap["Home & Garden"], SortOrder = 1, IsActive = true },
            new() { Name = "Home Decor", Description = "Home decoration items", ParentCategoryId = parentMap["Home & Garden"], SortOrder = 2, IsActive = true },
            new() { Name = "Kitchen", Description = "Kitchen appliances and tools", ParentCategoryId = parentMap["Home & Garden"], SortOrder = 3, IsActive = true },
            new() { Name = "Bedding", Description = "Beds and bedding", ParentCategoryId = parentMap["Home & Garden"], SortOrder = 4, IsActive = true },
            new() { Name = "Garden", Description = "Garden tools and plants", ParentCategoryId = parentMap["Home & Garden"], SortOrder = 5, IsActive = true }
        });

        // Sports & Outdoors children
        children.AddRange(new Category[]
        {
            new() { Name = "Fitness Equipment", Description = "Gym and fitness equipment", ParentCategoryId = parentMap["Sports & Outdoors"], SortOrder = 1, IsActive = true },
            new() { Name = "Outdoor Gear", Description = "Camping and hiking gear", ParentCategoryId = parentMap["Sports & Outdoors"], SortOrder = 2, IsActive = true },
            new() { Name = "Team Sports", Description = "Sports equipment for teams", ParentCategoryId = parentMap["Sports & Outdoors"], SortOrder = 3, IsActive = true },
            new() { Name = "Camping", Description = "Camping equipment", ParentCategoryId = parentMap["Sports & Outdoors"], SortOrder = 4, IsActive = true },
            new() { Name = "Cycling", Description = "Bicycles and cycling gear", ParentCategoryId = parentMap["Sports & Outdoors"], SortOrder = 5, IsActive = true }
        });

        // Toys & Games children
        children.AddRange(new Category[]
        {
            new() { Name = "Action Figures", Description = "Action figures and statues", ParentCategoryId = parentMap["Toys & Games"], SortOrder = 1, IsActive = true },
            new() { Name = "Board Games", Description = "Board games and card games", ParentCategoryId = parentMap["Toys & Games"], SortOrder = 2, IsActive = true },
            new() { Name = "Puzzles", Description = "Puzzles and brain teasers", ParentCategoryId = parentMap["Toys & Games"], SortOrder = 3, IsActive = true },
            new() { Name = "Video Games", Description = "Video games and consoles", ParentCategoryId = parentMap["Toys & Games"], SortOrder = 4, IsActive = true },
            new() { Name = "Educational Toys", Description = "Learning and educational toys", ParentCategoryId = parentMap["Toys & Games"], SortOrder = 5, IsActive = true }
        });

        return children;
    }
}
