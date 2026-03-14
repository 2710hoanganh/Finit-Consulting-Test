using TechnicalTest.Api.Entities;

namespace TechnicalTest.Api.Seed;

public static class ProductSeeder
{
    private static readonly Random _random = new(42); // Fixed seed for reproducibility

    private static readonly Dictionary<string, string[]> _productNames = new()
    {
        ["Electronics"] = new[] { "Smartphone X", "Laptop Pro", "Tablet Mini", "Wireless Earbuds", "4K Camera", "Smart Watch", "Gaming Console", "Portable Speaker", "Wireless Mouse", "USB-C Hub" },
        ["Smartphones"] = new[] { "iPhone 15", "Samsung Galaxy S24", "Google Pixel 8", "OnePlus 12", "Xiaomi 14", "Sony Xperia 1", "Oppo Find X7", "Vivo X100", "Realme GT5", "Motorola Edge" },
        ["Laptops"] = new[] { "MacBook Pro 14", "Dell XPS 15", "HP Spectre", "Lenovo ThinkPad", "Asus ROG", "Acer Swift", "Microsoft Surface", "Razer Blade", "MSI Prestige", "Samsung Galaxy Book" },
        ["Tablets"] = new[] { "iPad Pro", "iPad Air", "Samsung Tab S9", "Galaxy Tab A", "Fire HD 10", "Lenovo Tab P12", "Surface Go", "Xiaomi Pad 6", "Oppo Pad Air", "Nokia T21" },
        ["Headphones"] = new[] { "AirPods Pro", "Sony WH-1000XM5", "Bose QC45", "Sennheiser HD660S", "Beats Studio3", "JBL Tune", "Anker Soundcore", "Skullcandy", "Audio-Technica", "Bang & Olufsen" },
        ["Cameras"] = new[] { "Canon EOS R5", "Sony A7 IV", "Nikon Z8", "Fujifilm X-T5", "Olympus OM-1", "Panasonic S5", "Canon G7X", "Sony ZV-1", "GoPro Hero12", "Insta360 X4" },
        ["Clothing"] = new[] { "Classic T-Shirt", "Slim Fit Jeans", "Casual Hoodie", "Summer Dress", "Winter Jacket", "Silk Blouse", "Wool Sweater", "Denim Shorts", "Polo Shirt", "Linen Pants" },
        ["Men's Clothing"] = new[] { "Oxford Shirt", "Chino Pants", "Leather Belt", "Wool Blazer", "Cashmere Sweater", "Boxer Briefs", "Dress Socks", "Leather Shoes", "Denim Jacket", "V-Neck Tee" },
        ["Women's Clothing"] = new[] { "Maxi Dress", "High-Waist Jeans", "Silk Blouse", "Trench Coat", "A-Line Skirt", "Cashmere Cardigan", "Leather Handbag", "Ankle Boots", "Pearl Necklace", "Sunglasses" },
        ["Kids' Clothing"] = new[] { "Cotton Onesie", "Kids Jeans", "卡通 T-Shirt", "Winter Coat", "School Uniform", "Sport Shorts", "Dress Set", "Pajama Set", "Rain Boots", "Baby Beanie" },
        ["Sportswear"] = new[] { "Running Shoes", "Yoga Leggings", "Gym Shorts", "Sports Bra", "Training Tank", "Compression Shirt", "Running Jacket", "Workout Hoodie", "Athletic Socks", "Fitness Tracker" },
        ["Underwear"] = new[] { "Cotton Briefs", "Boxer Shorts", "Bikini Set", "Thong", "Thermal Underwear", "Lace Panties", "Sports Underwear", "Silk Pajamas", "Bra Set", "Maternity Underwear" },
        ["Cosmetics"] = new[] { "Moisturizing Cream", "Anti-Aging Serum", "Sunscreen SPF50", "Face Cleanser", "Hair Shampoo", "Body Lotion", "Lip Balm", "Eye Cream", "Face Mask", "Toner" },
        ["Skincare"] = new[] { "Vitamin C Serum", "Retinol Cream", "Hyaluronic Acid", "Niacinamide Toner", "Peptide Serum", "Sunscreen Gel", "Cleansing Oil", "Exfoliating Scrub", "Sheet Mask", "Eye Serum" },
        ["Makeup"] = new[] { "Foundation", "Concealer", "Eyeshadow Palette", "Mascara", "Lipstick", "Blush", "Bronzer", "Highlighter", "Setting Spray", "Makeup Brush Set" },
        ["Hair Care"] = new[] { "Shampoo", "Conditioner", "Hair Mask", "Hair Serum", "Styling Gel", "Hair Spray", "Hair Oil", "Heat Protectant", "Hair Dye", "Scalp Treatment" },
        ["Perfume"] = new[] { "Chanel No.5", "Dior Sauvage", "Tom Ford Oud", "Gucci Bloom", "YSL Libre", "Versace Crystal", "Dolce Gabbana", "Prada Paradoxe", "Armani My Way", "Valentino Donna" },
        ["Body Care"] = new[] { "Body Wash", "Body Scrub", "Body Butter", "Hand Cream", "Foot Cream", "Deodorant", "Shaving Cream", "After Shave", "Body Oil", "Stretch Mark Cream" },
        ["Books"] = new[] { "The Great Gatsby", "1984", "To Kill a Mockingbird", "Pride and Prejudice", "The Catcher in the Rye", "Lord of the Flies", "Animal Farm", "Brave New World", "The Hobbit", "Fahrenheit 451" },
        ["Fiction"] = new[] { "Harry Potter", "The Da Vinci Code", "The Hunger Games", "Twilight", "The Kite Runner", "A Thousand Splendid Suns", "Life of Pi", "The Fault in Our Stars", "The Alchemist", "The Book Thief" },
        ["Non-Fiction"] = new[] { "Sapiens", "Thinking Fast and Slow", "The Power of Habit", "Atomic Habits", "Educated", "Becoming", "Steve Jobs", "The 7 Habits", "Rich Dad Poor Dad", "Quiet" },
        ["Children's Books"] = new[] { "Charlotte's Web", "Matilda", "The Very Hungry Caterpillar", "Where the Wild Things Are", "Green Eggs and Ham", "Curious George", "Winnie the Pooh", "Peter Rabbit", "Goodnight Moon", "The Cat in the Hat" },
        ["Textbooks"] = new[] { "Calculus Early Transcendentals", "Introduction to Psychology", "Principles of Economics", "Organic Chemistry", "Physics for Scientists", "Campbell Biology", "Linear Algebra", "Statistics", "Microeconomics", "Macroeconomics" },
        ["Comics"] = new[] { "Spider-Man", "Batman", "X-Men", "The Avengers", "Watchmen", "V for Vendetta", "Maus", "Saga", "Walking Dead", "One Piece" },
        ["Home & Garden"] = new[] { "Coffee Table", "Floor Lamp", "Area Rug", "Wall Art", "Cushion Covers", "Curtain Panels", "Throw Blanket", "Vase Set", "Wall Clock", "Candle Holder" },
        ["Furniture"] = new[] { "Leather Sofa", "Dining Table", "Bookshelf", "TV Stand", "Bed Frame", "Wardrobe", "Office Chair", "Coffee Table", "Side Table", "Bench" },
        ["Home Decor"] = new[] { "Wall Mirror", "Photo Frame", "Decorative Vase", "Table Lamp", "Wall Clock", "Area Rug", "Cushion", "Curtains", "Wall Art", "Plant Pot" },
        ["Kitchen"] = new[] { "Blender", "Coffee Maker", "Toaster", "Microwave", "Air Fryer", "Electric Kettle", "Food Processor", "Stand Mixer", "Slow Cooker", "Rice Cooker" },
        ["Bedding"] = new[] { "Queen Sheet Set", "Duvet Cover", "Pillowcases", "Mattress Topper", "Bedspread", "Throw Pillow", "Blanket", "Sleeping Pillow", "Bed Skirt", "Comforter" },
        ["Garden"] = new[] { "Garden Tools Set", "Plant Pots", "Garden Hose", "Lawn Mower", "Garden Chairs", "Solar Lights", "Bird Feeder", "Garden Gloves", "Fertilizer", "Seeds Pack" },
        ["Sports & Outdoors"] = new[] { "Yoga Mat", "Dumbbells", "Tennis Racket", "Camping Tent", "Hiking Boots", "Cycling Helmet", "Running Shoes", "Swim Goggles", "Golf Clubs", "Ski Equipment" },
        ["Fitness Equipment"] = new[] { "Dumbbells Set", "Barbell", "Kettlebell", "Pull-Up Bar", "Resistance Bands", "Exercise Ball", "Weight Bench", "Jump Rope", "Ab Roller", "Foam Roller" },
        ["Outdoor Gear"] = new[] { "Hiking Backpack", "Camping Tent", "Sleeping Bag", "Flashlight", "Compass", "Water Bottle", "Binoculars", "Camp Chair", "Portable Stove", "First Aid Kit" },
        ["Team Sports"] = new[] { "Soccer Ball", "Basketball", "Football", "Baseball Bat", "Tennis Rackets", "Volleyball", "Hockey Stick", "Golf Clubs", "Cricket Bat", "Rugby Ball" },
        ["Camping"] = new[] { "4-Person Tent", "Sleeping Bag", "Camping Pillow", "Camp Stove", "Cooler Box", "Camping Chairs", "Lantern", "Hammock", "Mosquito Net", "Camping Table" },
        ["Cycling"] = new[] { "Road Bike", "Mountain Bike", "Helmet", "Cycling Gloves", "Cycling Shoes", "Bike Lights", "Water Bottle Cage", "Bike Pump", "Repair Kit", "Cycling Jersey" },
        ["Toys & Games"] = new[] { "LEGO Set", "Barbie Doll", "Remote Control Car", "Puzzle 1000", "Board Game", "Plush Teddy", "Building Blocks", "Train Set", "Doctor Kit", "Art Set" },
        ["Action Figures"] = new[] { "Spider-Man Figure", "Batman Figure", "Iron Man Figure", "Star Wars Figure", "Transformers", "Pokemon Figure", "Dragon Ball Figure", "Naruto Figure", "Avengers Figure", "Marvel Legends" },
        ["Board Games"] = new[] { "Monopoly", "Scrabble", "Risk", "Catan", "Ticket to Ride", "Carcassonne", "Pandemic", "Chess Set", "Backgammon", "Uno" },
        ["Puzzles"] = new[] { "1000 Piece Puzzle", "3D Puzzle", "Jigsaw Puzzle", "Brain Teaser", "Rubik's Cube", "Wooden Puzzle", "Metal Puzzle", "Logic Puzzle", "Crossword Puzzle", "Sudoku" },
        ["Video Games"] = new[] { "PS5 Game", "Xbox Game", "Nintendo Switch Game", "PC Game", "VR Game", "Gaming Headset", "Controller", "Gaming Chair", "Gaming Desk", "Stream Deck" },
        ["Educational Toys"] = new[] { "STEM Kit", "Math Blocks", "Science Set", "Reading Kit", "Drawing Set", "Music Toy", "Building Kit", "Coding Robot", "Geography Puzzle", "History Game" }
    };

    public static List<Product> GetProducts(List<Category> categories)
    {
        var products = new List<Product>();
        var productCounter = 1;

        foreach (var category in categories)
        {
            var categoryName = GetCategoryName(category);
            var names = _productNames.TryGetValue(categoryName, out var n) ? n : GenerateGenericNames(categoryName, 10);

            for (int i = 0; i < 10; i++)
            {
                var price = Math.Round((decimal)(_random.NextDouble() * 990 + 10), 2);
                var discountPrice = _random.NextDouble() > 0.5 ? Math.Round(price * (decimal)(_random.NextDouble() * 0.3 + 0.7), 2) : (decimal?)null;

                products.Add(new Product
                {
                    Name = $"{names[i]} #{i + 1}",
                    Description = $"High quality {names[i].ToLower()} from {categoryName} category. Premium product with excellent features.",
                    Price = price,
                    DiscountPrice = discountPrice,
                    Stock = _random.Next(0, 200),
                    SKU = $"SKU-{productCounter:D5}",
                    CategoryId = category.Id,
                    IsActive = _random.NextDouble() > 0.1,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365))
                });
                productCounter++;
            }
        }

        return products;
    }

    private static string GetCategoryName(Category category)
    {
        // Try to find the category name in our dictionary
        foreach (var key in _productNames.Keys)
        {
            if (category.Name.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                key.Contains(category.Name, StringComparison.OrdinalIgnoreCase))
            {
                return key;
            }
        }
        return category.Name;
    }

    private static string[] GenerateGenericNames(string categoryName, int count)
    {
        var names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = $"{categoryName} Product {i + 1}";
        }
        return names;
    }
}
