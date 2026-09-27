using System.Collections.Generic;

namespace STB_Exports.Models;

public static class ProductCatalogue
{
    public static List<Product> Products { get; } = new()
    {
        // =====================================================
        // FRUITS
        // =====================================================

        new Product
        {
            ProductId = 101,
            Name = "Apple",
            Image = "apple.png",
            Images = new List<string>
            {
                "apple.png",
                "applebenefits.png"
            },
            Category = "Fruits",
            Price = 180,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh premium quality apples."
        },

        new Product
        {
            ProductId = 102,
            Name = "Mango",
            Image = "mangoslide.png",
            Images = new List<string>
            {
                "mangoslide.png"
            },
            Category = "Fruits",
            Price = 250,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh premium quality mangoes."
        },

        new Product
        {
            ProductId = 103,
            Name = "Pomegranate",
            Image = "pomogranate.png",
            Images = new List<string>
            {
                "pomogranate.png",
                "pomogranate2.png"
            },
            Category = "Fruits",
            Price = 220,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh export quality pomegranates."
        },

        new Product
        {
            ProductId = 104,
            Name = "Kiwi",
            Image = "kiwi.png",
            Images = new List<string>
            {
                "kiwi.png",
                "kiwibenefits.png"
            },
            Category = "Fruits",
            Price = 300,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh premium quality kiwi."
        },

        // =====================================================
        // VEGETABLES
        // =====================================================

        new Product
        {
            ProductId = 201,
            Name = "Green Chillies",
            Image = "greenchillies.png",
            Images = new List<string>
            {
                "greenchillies.png"
            },
            Category = "Vegetables",
            Price = 120,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh green chillies."
        },

        new Product
        {
            ProductId = 202,
            Name = "Red Chillies",
            Image = "redchillies.png",
            Images = new List<string>
            {
                "redchillies.png",
                "redchillies3.png"
            },
            Category = "Vegetables",
            Price = 160,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality red chillies."
        },

        new Product
        {
            ProductId = 203,
            Name = "Onion",
            Image = "onion.png",
            Images = new List<string>
            {
                "onion.png",
                "onion2.png",
                "onion3.png"
            },
            Category = "Vegetables",
            Price = 80,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh export quality onions."
        },

        new Product
        {
            ProductId = 204,
            Name = "Tomato",
            Image = "tomato.png",
            Images = new List<string>
            {
                "tomato.png"
            },
            Category = "Vegetables",
            Price = 90,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Fresh premium tomatoes."
        },

        // =====================================================
        // DRY FRUITS
        // =====================================================

        new Product
        {
            ProductId = 301,
            Name = "Almonds",
            Image = "almonds.png",
            Images = new List<string>
            {
                "almonds.png"
            },
            Category = "Dry Fruits",
            Price = 850,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality almonds."
        },

        new Product
        {
            ProductId = 302,
            Name = "Cashew",
            Image = "cashew.png",
            Images = new List<string>
            {
                "cashew.png",
                "cashew3.png"
            },
            Category = "Dry Fruits",
            Price = 900,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium export quality cashews."
        },

        new Product
        {
            ProductId = 303,
            Name = "Pistachio",
            Image = "pistachio.png",
            Images = new List<string>
            {
                "pistachio.png"
            },
            Category = "Dry Fruits",
            Price = 1100,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality pistachios."
        },

        new Product
        {
            ProductId = 304,
            Name = "Walnuts",
            Image = "walnuts.png",
            Images = new List<string>
            {
                "walnuts.png"
            },
            Category = "Dry Fruits",
            Price = 1000,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality walnuts."
        },

        // =====================================================
        // CLAY
        // =====================================================

        new Product
        {
            ProductId = 401,
            Name = "Clay Cooking Pot",
            Image = "claycookingpot.png",
            Images = new List<string>
            {
                "claycookingpot.png",
                "cookingpot2.png"
            },
            BenefitsImage = "benefitsofcookingpot.png",
            Category = "Clay",
            Price = 750,
            Quantity = 1,
            QuantityUnit = "Piece",
            Description = "Traditional clay cooking pot."
        },

        new Product
        {
            ProductId = 402,
            Name = "Traditional Light Lamp",
            Image = "claylatern.png",
            Images = new List<string>
            {
                "claylatern.png"
            },
            Category = "Clay",
            Price = 350,
            Quantity = 1,
            QuantityUnit = "Piece",
            Description = "Traditional handcrafted clay lamp."
        },

        new Product
        {
            ProductId = 403,
            Name = "Clay Bowl",
            Image = "bowl.png",
            Images = new List<string>
            {
                "bowl.png",
                "bowl2.png"
            },
            Category = "Clay",
            Price = 250,
            Quantity = 1,
            QuantityUnit = "Piece",
            Description = "Handcrafted clay bowl."
        },

        // =====================================================
        // SPICES
        // =====================================================

        new Product
        {
            ProductId = 501,
            Name = "Black Pepper",
            Image = "blackpaper.png",
            Images = new List<string>
            {
                "blackpaper.png"
            },
            Category = "Spices",
            Price = 650,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality black pepper."
        },

        new Product
        {
            ProductId = 502,
            Name = "Cumin",
            Image = "cumin.png",
            Images = new List<string>
            {
                "cumin.png"
            },
            Category = "Spices",
            Price = 450,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality cumin seeds."
        },

        new Product
        {
            ProductId = 503,
            Name = "Coriander Seeds",
            Image = "corianderseeds.png",
            Images = new List<string>
            {
                "corianderseeds.png",
                "corianderseeds2.png",
                "corianderseeds3.png"
            },
            Category = "Spices",
            Price = 300,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium coriander seeds."
        },

        new Product
        {
            ProductId = 504,
            Name = "Dried Turmeric",
            Image = "driedturmeric.png",
            Images = new List<string>
            {
                "driedturmeric.png",
                "driedturmeric2.png",
                "driedturmeric3.png"
            },
            Category = "Spices",
            Price = 350,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium dried turmeric."
        },

        // =====================================================
        // COCONUT
        // =====================================================

        new Product
        {
            ProductId = 601,
            Name = "Fresh Coconut",
            Image = "coconut.png",
            Images = new List<string>
            {
                "coconut.png"
            },
            Category = "Coconut",
            Price = 100,
            Quantity = 1,
            QuantityUnit = "Piece",
            Description = "Fresh export quality coconut."
        },

        new Product
        {
            ProductId = 602,
            Name = "Coconut Copra",
            Image = "coconutcopra.png",
            Images = new List<string>
            {
                "coconutcopra.png",
                "coconutcopra2.png"
            },
            Category = "Coconut",
            Price = 280,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium quality coconut copra."
        },

        new Product
        {
            ProductId = 603,
            Name = "Coconut Husks",
            Image = "coconuthusks.png",
            Images = new List<string>
            {
                "coconuthusks.png",
                "coconuthusk2.png"
            },
            Category = "Coconut",
            Price = 150,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Natural coconut husks."
        },

        new Product
        {
            ProductId = 604,
            Name = "Coconut Pieces",
            Image = "coconutpieces.png",
            Images = new List<string>
            {
                "coconutpieces.png"
            },
            Category = "Coconut",
            Price = 250,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium coconut pieces."
        },

        new Product
        {
            ProductId = 605,
            Name = "Coconut Shell",
            Image = "coconutshell.png",
            Images = new List<string>
            {
                "coconutshell.png",
                "coconutshell3.png"
            },
            Category = "Coconut",
            Price = 180,
            Quantity = 1,
            QuantityUnit = "Piece",
            Description = "Natural coconut shells."
        },

        new Product
        {
            ProductId = 606,
            Name = "Desiccated Coconut Powder",
            Image = "desiccatedcoconut.png",
            Images = new List<string>
            {
                "desiccatedcoconut.png"
            },
            Category = "Coconut",
            Price = 400,
            Quantity = 1,
            QuantityUnit = "Weight",
            Description = "Premium desiccated coconut powder."
        }
    };
}