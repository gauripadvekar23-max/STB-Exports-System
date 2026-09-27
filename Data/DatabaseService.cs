using System.Linq;
using SQLite;
using STB_Exports.Models;

namespace STB_Exports.Data;
public class DatabaseService
{
    private SQLiteAsyncConnection? _database;


        // =====================================================
        // DATABASE INITIALIZATION
        // =====================================================
        private string GetDatabasePath()
        {
            string folderPath = @"C:\STB_Exports_Data";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return Path.Combine(
                folderPath,
                "STBExports.db3");
        }
        private async Task InitializeAsync()
        {
            if (_database != null)
                return;

            string databasePath = GetDatabasePath();


            _database =
                new SQLiteAsyncConnection(databasePath);

            // =================================================
            // CREATE TABLES
            // =================================================

            await _database.CreateTableAsync<Customer>();

            await _database.CreateTableAsync<CartItem>();

            await _database.CreateTableAsync<CustomerProfile>();

            await _database.CreateTableAsync<SavedAddress>();

            await _database.CreateTableAsync<Order>();

            await _database.CreateTableAsync<OrderItem>();

            await _database.CreateTableAsync<WishlistItem>();

            await _database.CreateTableAsync<Review>();

            await _database.CreateTableAsync<OrderNotification>();

        // =================================================
        // DATABASE MIGRATIONS
        // =================================================

        // -------------------------------------------------
        // CartItem - QuantityUnit
        // -------------------------------------------------

        try
            {
                await _database.ExecuteAsync(
                    "ALTER TABLE CartItem ADD COLUMN QuantityUnit TEXT DEFAULT 'Weight'");
            }
            catch
            {
                // Column already exists.
            }

            // -------------------------------------------------
            // Customer - IsDeactivated
            // -------------------------------------------------

            try
            {
                await _database.ExecuteAsync(
                    "ALTER TABLE Customer ADD COLUMN IsDeactivated INTEGER DEFAULT 0");
            }
            catch
            {
                // Column already exists.
            }

            // -------------------------------------------------
            // CustomerProfile - District
            // -------------------------------------------------

            try
            {
                await _database.ExecuteAsync(
                    "ALTER TABLE CustomerProfile ADD COLUMN District TEXT DEFAULT ''");
            }
            catch
            {
                // Column already exists.
            }

            // -------------------------------------------------
            // Order - ExpectedDeliveryDate
            // -------------------------------------------------

            try
            {
                await _database.ExecuteAsync(
                    "ALTER TABLE [Order] ADD COLUMN ExpectedDeliveryDate TEXT");
            }
            catch
            {
                // Column already exists.
            }

            // =================================================
            // SEED DEMO REVIEWS
            // =================================================

            await SeedDemoReviewsAsync();
        }


        // =====================================================
        // DEMO REVIEWS - SEED SAMPLE REVIEWS
        // =====================================================

        private async Task SeedDemoReviewsAsync()
        {
            // IMPORTANT:
            // InitializeAsync() has already created the tables.
            // Do NOT call InitializeAsync() here because that
            // would cause recursive initialization.

            if (_database == null)
                return;

            // -------------------------------------------------
            // CHECK WHETHER DEMO REVIEWS ALREADY EXIST
            // -------------------------------------------------

            int reviewCount =
                await _database
                    .Table<Review>()
                    .CountAsync();

            if (reviewCount > 0)
                return;

            // =================================================
            // DEMO REVIEWS
            // =================================================

            List<Review> demoReviews = new()
            {
                // -------------------------------------------------
                // APPLE
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9001,
                    ProductId = 1,
                    Rating = 5,
                    ReviewText =
                        "Fresh and excellent quality apples. Packaging was very good.",
                    ReviewDate = "12 Aug 2026",
                    CustomerName = "Rahul"
                },

                new Review
                {
                    CustomerId = 9002,
                    ProductId = 1,
                    Rating = 4,
                    ReviewText =
                        "Good quality and fresh. Really satisfied with the purchase.",
                    ReviewDate = "18 Aug 2026",
                    CustomerName = "Priya"
                },

                new Review
                {
                    CustomerId = 9003,
                    ProductId = 1,
                    Rating = 5,
                    ReviewText =
                        "Very fresh apples with great taste. Would definitely recommend.",
                    ReviewDate = "22 Aug 2026",
                    CustomerName = "Amit"
                },

                // -------------------------------------------------
                // MANGO
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9004,
                    ProductId = 2,
                    Rating = 5,
                    ReviewText =
                        "Sweet, fresh and premium quality mangoes.",
                    ReviewDate = "10 Aug 2026",
                    CustomerName = "Sneha"
                },

                new Review
                {
                    CustomerId = 9005,
                    ProductId = 2,
                    Rating = 4,
                    ReviewText =
                        "Good quality mangoes and nicely packed.",
                    ReviewDate = "16 Aug 2026",
                    CustomerName = "Vikas"
                },

                // -------------------------------------------------
                // POMEGRANATE
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9006,
                    ProductId = 3,
                    Rating = 5,
                    ReviewText =
                        "Very fresh pomegranates. The quality is excellent.",
                    ReviewDate = "14 Aug 2026",
                    CustomerName = "Neha"
                },

                new Review
                {
                    CustomerId = 9007,
                    ProductId = 3,
                    Rating = 5,
                    ReviewText =
                        "Premium export quality. Fresh and juicy.",
                    ReviewDate = "20 Aug 2026",
                    CustomerName = "Karan"
                },

                new Review
                {
                    CustomerId = 9008,
                    ProductId = 3,
                    Rating = 4,
                    ReviewText =
                        "Good product and proper packaging.",
                    ReviewDate = "24 Aug 2026",
                    CustomerName = "Pooja"
                },

                // -------------------------------------------------
                // GREEN CHILLIES
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9009,
                    ProductId = 101,
                    Rating = 4,
                    ReviewText =
                        "Fresh green chillies and good quality.",
                    ReviewDate = "11 Aug 2026",
                    CustomerName = "Sanjay"
                },

                new Review
                {
                    CustomerId = 9010,
                    ProductId = 101,
                    Rating = 5,
                    ReviewText =
                        "Very fresh and nicely packed.",
                    ReviewDate = "19 Aug 2026",
                    CustomerName = "Meena"
                },

                // -------------------------------------------------
                // ONION
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9011,
                    ProductId = 103,
                    Rating = 5,
                    ReviewText =
                        "Fresh onions with excellent quality.",
                    ReviewDate = "09 Aug 2026",
                    CustomerName = "Rohit"
                },

                new Review
                {
                    CustomerId = 9012,
                    ProductId = 103,
                    Rating = 4,
                    ReviewText =
                        "Good quality product. Value for money.",
                    ReviewDate = "21 Aug 2026",
                    CustomerName = "Anjali"
                },

                // -------------------------------------------------
                // ALMONDS
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9013,
                    ProductId = 201,
                    Rating = 5,
                    ReviewText =
                        "Premium quality almonds. Fresh and crunchy.",
                    ReviewDate = "08 Aug 2026",
                    CustomerName = "Arjun"
                },

                new Review
                {
                    CustomerId = 9014,
                    ProductId = 201,
                    Rating = 5,
                    ReviewText =
                        "Excellent quality and good packaging.",
                    ReviewDate = "17 Aug 2026",
                    CustomerName = "Kavita"
                },

                new Review
                {
                    CustomerId = 9015,
                    ProductId = 201,
                    Rating = 4,
                    ReviewText =
                        "Good quality almonds. Happy with the purchase.",
                    ReviewDate = "25 Aug 2026",
                    CustomerName = "Nikhil"
                },

                // -------------------------------------------------
                // CASHEW
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9016,
                    ProductId = 202,
                    Rating = 5,
                    ReviewText =
                        "Excellent cashews. Fresh, crunchy and premium quality.",
                    ReviewDate = "13 Aug 2026",
                    CustomerName = "Riya"
                },

                new Review
                {
                    CustomerId = 9017,
                    ProductId = 202,
                    Rating = 5,
                    ReviewText =
                        "Very good quality cashews. Packaging was also excellent.",
                    ReviewDate = "23 Aug 2026",
                    CustomerName = "Aditya"
                },

                // -------------------------------------------------
                // CLAY COOKING POT
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9018,
                    ProductId = 301,
                    Rating = 5,
                    ReviewText =
                        "Beautiful traditional cooking pot. Strong and well finished.",
                    ReviewDate = "07 Aug 2026",
                    CustomerName = "Shweta"
                },

                new Review
                {
                    CustomerId = 9019,
                    ProductId = 301,
                    Rating = 4,
                    ReviewText =
                        "Good handmade product. Looks beautiful in the kitchen.",
                    ReviewDate = "15 Aug 2026",
                    CustomerName = "Manoj"
                },

                // -------------------------------------------------
                // BLACK PEPPER
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9020,
                    ProductId = 401,
                    Rating = 5,
                    ReviewText =
                        "Excellent quality black pepper with strong aroma.",
                    ReviewDate = "06 Aug 2026",
                    CustomerName = "Deepak"
                },

                new Review
                {
                    CustomerId = 9021,
                    ProductId = 401,
                    Rating = 4,
                    ReviewText =
                        "Good quality and fresh product.",
                    ReviewDate = "20 Aug 2026",
                    CustomerName = "Nandini"
                },

                // -------------------------------------------------
                // COCONUT
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9022,
                    ProductId = 501,
                    Rating = 5,
                    ReviewText =
                        "Fresh coconut and excellent quality.",
                    ReviewDate = "12 Aug 2026",
                    CustomerName = "Sachin"
                },

                new Review
                {
                    CustomerId = 9023,
                    ProductId = 501,
                    Rating = 5,
                    ReviewText =
                        "Very fresh and properly packed.",
                    ReviewDate = "22 Aug 2026",
                    CustomerName = "Komal"
                },

                // -------------------------------------------------
                // COCONUT COPRA
                // -------------------------------------------------

                new Review
                {
                    CustomerId = 9024,
                    ProductId = 502,
                    Rating = 4,
                    ReviewText =
                        "Good quality coconut copra. Satisfied with the product.",
                    ReviewDate = "16 Aug 2026",
                    CustomerName = "Raj"
                },

                new Review
                {
                    CustomerId = 9025,
                    ProductId = 502,
                    Rating = 5,
                    ReviewText =
                        "Premium quality and good packaging.",
                    ReviewDate = "26 Aug 2026",
                    CustomerName = "Madhuri"
                }
            };

            // =================================================
            // INSERT DEMO REVIEWS
            // =================================================

            foreach (Review review in demoReviews)
            {
                await _database.InsertAsync(review);
            }
        }


        // =====================================================
        // CUSTOMER - REGISTER
        // =====================================================

        public async Task<int> RegisterCustomerAsync(Customer customer)
        {
            await InitializeAsync();

            string email =
                customer.Email?.Trim().ToLowerInvariant() ?? "";

            if (string.IsNullOrWhiteSpace(email))
                return 0;

            Customer? existingCustomer =
                await _database!
                    .Table<Customer>()
                    .Where(c => c.Email == email)
                    .FirstOrDefaultAsync();

            if (existingCustomer != null)
                return 0;

            customer.Email = email;

            return await _database.InsertAsync(customer);
        }


        // =====================================================
        // CUSTOMER - GET BY EMAIL
        // =====================================================

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            await InitializeAsync();

            string searchEmail =
                email?.Trim().ToLowerInvariant() ?? "";

            if (string.IsNullOrWhiteSpace(searchEmail))
                return null;

            return await _database!
                .Table<Customer>()
                .Where(c => c.Email == searchEmail)
                .FirstOrDefaultAsync();
        }


        // =====================================================
        // CUSTOMER - LOGIN
        // =====================================================

        public async Task<Customer?> LoginCustomerAsync(
            string email,
            string password)
        {
            await InitializeAsync();

            string searchEmail =
                email?.Trim().ToLowerInvariant() ?? "";

            if (string.IsNullOrWhiteSpace(searchEmail))
                return null;

            Customer? customer =
                await _database!
                    .Table<Customer>()
                    .Where(c => c.Email == searchEmail)
                    .FirstOrDefaultAsync();

            if (customer == null)
                return null;

            if (customer.IsDeactivated)
                return null;

            string storedPassword =
                customer.Password ?? "";

            if (storedPassword != password)
                return null;

            return customer;
        }


    // =====================================================
    // CART - ADD
    // =====================================================

    public async Task<int> AddToCartAsync(CartItem item)
    {
        await InitializeAsync();

        if (item == null)
            return 0;

        if (item.CustomerId <= 0)
            return 0;

        CartItem? existingItem =
            await _database!
                .Table<CartItem>()
                .Where(c =>
                    c.CustomerId == item.CustomerId &&
                    c.ProductId == item.ProductId &&
                    c.QuantityUnit == item.QuantityUnit)
                .FirstOrDefaultAsync();

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;

            existingItem.Price = item.Price;

            existingItem.ProductName =
                item.ProductName;

            existingItem.ProductImage =
                item.ProductImage;

            existingItem.QuantityUnit =
                item.QuantityUnit;

            return await _database.UpdateAsync(
                existingItem);
        }

        return await _database.InsertAsync(item);
    }


    // =====================================================
    // CART - GET
    // =====================================================

    public async Task<List<CartItem>> GetCartItemsAsync(
        int customerId)
    {
        await InitializeAsync();

        if (customerId <= 0)
            return new List<CartItem>();

        List<CartItem> items =
            await _database!
                .Table<CartItem>()
                .Where(c =>
                    c.CustomerId == customerId)
                .ToListAsync();

        foreach (CartItem item in items)
        {
            if (string.IsNullOrWhiteSpace(
                item.QuantityUnit))
            {
                item.QuantityUnit = "Weight";
            }
        }

        return items;
    }


    // =====================================================
    // CART - COUNT
    // =====================================================

    public async Task<int> GetCartItemCountAsync(
        int customerId)
    {
        await InitializeAsync();

        if (customerId <= 0)
            return 0;

        List<CartItem> cartItems =
            await _database!
                .Table<CartItem>()
                .Where(c =>
                    c.CustomerId == customerId)
                .ToListAsync();

        return cartItems.Sum(
            c => c.Quantity);
    }


    // =====================================================
    // CART - UPDATE QUANTITY
    // =====================================================

    public async Task<int> UpdateCartQuantityAsync(
        int cartItemId,
        int quantity)
    {
        await InitializeAsync();

        if (cartItemId <= 0)
            return 0;

        CartItem? item =
            await _database!
                .Table<CartItem>()
                .Where(c =>
                    c.CartItemId == cartItemId)
                .FirstOrDefaultAsync();

        if (item == null)
            return 0;

        if (quantity <= 0)
        {
            return await _database.DeleteAsync(item);
        }

        item.Quantity = quantity;

        return await _database.UpdateAsync(item);
    }


    // =====================================================
    // CART - REMOVE
    // =====================================================

    public async Task<int> RemoveFromCartAsync(
        int cartItemId)
    {
        await InitializeAsync();

        if (cartItemId <= 0)
            return 0;

        CartItem? item =
            await _database!
                .Table<CartItem>()
                .Where(c =>
                    c.CartItemId == cartItemId)
                .FirstOrDefaultAsync();

        if (item == null)
            return 0;

        return await _database.DeleteAsync(item);
    }


    // =====================================================
    // CART - CLEAR
    // =====================================================

    public async Task<int> ClearCartAsync(
        int customerId)
    {
        await InitializeAsync();

        if (customerId <= 0)
            return 0;

        List<CartItem> items =
            await _database!
                .Table<CartItem>()
                .Where(c =>
                    c.CustomerId == customerId)
                .ToListAsync();

        int count = 0;

        foreach (CartItem item in items)
        {
            count +=
                await _database.DeleteAsync(item);
        }

        return count;
    }


    // =====================================================
    // WISHLIST - ADD
    // =====================================================

    public async Task<int> AddToWishlistAsync(WishlistItem item)
        {
            await InitializeAsync();

            if (item.CustomerId <= 0 ||
                item.ProductId <= 0)
            {
                return 0;
            }

            WishlistItem? existingItem =
                await _database!
                    .Table<WishlistItem>()
                    .Where(w =>
                        w.CustomerId == item.CustomerId &&
                        w.ProductId == item.ProductId)
                    .FirstOrDefaultAsync();

            if (existingItem != null)
                return 0;

            return await _database.InsertAsync(item);
        }


        // =====================================================
        // WISHLIST - CHECK PRODUCT
        // =====================================================

        public async Task<bool> IsProductInWishlistAsync(
            int customerId,
            int productId)
        {
            await InitializeAsync();

            if (customerId <= 0 ||
                productId <= 0)
            {
                return false;
            }

            WishlistItem? item =
                await _database!
                    .Table<WishlistItem>()
                    .Where(w =>
                        w.CustomerId == customerId &&
                        w.ProductId == productId)
                    .FirstOrDefaultAsync();

            return item != null;
        }


        // =====================================================
        // WISHLIST - GET CUSTOMER ITEMS
        // =====================================================

        public async Task<List<WishlistItem>> GetWishlistItemsAsync(
            int customerId)
        {
            await InitializeAsync();

            if (customerId <= 0)
                return new List<WishlistItem>();

            return await _database!
                .Table<WishlistItem>()
                .Where(w => w.CustomerId == customerId)
                .OrderByDescending(w => w.WishlistItemId)
                .ToListAsync();
        }


        // =====================================================
        // WISHLIST - REMOVE
        // =====================================================

        public async Task<int> RemoveFromWishlistAsync(
            int customerId,
            int productId)
        {
            await InitializeAsync();

            WishlistItem? item =
                await _database!
                    .Table<WishlistItem>()
                    .Where(w =>
                        w.CustomerId == customerId &&
                        w.ProductId == productId)
                    .FirstOrDefaultAsync();

            if (item == null)
                return 0;

            return await _database.DeleteAsync(item);
        }


        // =====================================================
        // WISHLIST - CLEAR
        // =====================================================

        public async Task<int> ClearWishlistAsync(int customerId)
        {
            await InitializeAsync();

            List<WishlistItem> items =
                await _database!
                    .Table<WishlistItem>()
                    .Where(w => w.CustomerId == customerId)
                    .ToListAsync();

            int count = 0;

            foreach (WishlistItem item in items)
            {
                count += await _database.DeleteAsync(item);
            }

            return count;
        }


        // =====================================================
        // ORDER - CREATE
        // =====================================================

        public async Task<int> CreateOrderAsync(
            Order order,
            List<CartItem> cartItems)
        {
            await InitializeAsync();

            if (cartItems == null ||
                cartItems.Count == 0)
            {
                return 0;
            }

            if (order.CustomerId <= 0)
                return 0;

            order.OrderDate = DateTime.Now;

            DateTime startOfToday = DateTime.Today;

            DateTime startOfTomorrow =
                startOfToday.AddDays(1);

            string datePart =
                order.OrderDate.ToString("yyyyMMdd");

            int existingOrders =
                await _database!
                    .Table<Order>()
                    .Where(o =>
                        o.OrderDate >= startOfToday &&
                        o.OrderDate < startOfTomorrow)
                    .CountAsync();

            int sequence =
                existingOrders + 1;

            order.OrderNumber =
                $"STB{datePart}{sequence:0000}";

            order.ExpectedDeliveryDate =
                order.OrderDate.AddDays(5);

            decimal total = 0m;

            foreach (CartItem item in cartItems)
            {
                total += item.TotalPrice;
            }

            order.TotalAmount = total;

            if (string.IsNullOrWhiteSpace(order.OrderStatus))
            {
                order.OrderStatus = "Pending";
            }

            if (string.IsNullOrWhiteSpace(order.PaymentStatus))
            {
                order.PaymentStatus = "Pending";
            }

            int result =
                await _database.InsertAsync(order);

            if (result <= 0)
                return 0;

            foreach (CartItem cartItem in cartItems)
            {
                OrderItem orderItem =
                    new OrderItem
                    {
                        OrderId = order.OrderId,

                        ProductId = cartItem.ProductId,

                        ProductName = cartItem.ProductName,

                        ProductImage = cartItem.ProductImage,

                        Price = cartItem.Price,

                        Quantity = cartItem.Quantity,

                        QuantityUnit = cartItem.QuantityUnit
                    };

                await _database.InsertAsync(orderItem);
            }

            await ClearCartAsync(order.CustomerId);

            return order.OrderId;
        }


        // =====================================================
        // ORDERS - GET CUSTOMER ORDERS
        // =====================================================

        


        // =====================================================
        // ORDER - GET BY ID
        // =====================================================

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            await InitializeAsync();

            return await _database!
                .Table<Order>()
                .Where(o => o.OrderId == orderId)
                .FirstOrDefaultAsync();
        }


    // =====================================================
    // ORDER ITEMS - GET
    // =====================================================

    public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
    {
        await InitializeAsync();

        if (customerId <= 0)
            return new List<Order>();

        List<Order> orders =
            await _database!
                .Table<Order>()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        foreach (Order order in orders)
        {
            List<OrderItem> items =
                await _database!
                    .Table<OrderItem>()
                    .Where(i => i.OrderId == order.OrderId)
                    .ToListAsync();

            if (items.Count == 0)
            {
                order.ProductNames = "No product information";
                order.ProductDetails = "No product information available.";
                continue;
            }

            order.ProductNames =
                string.Join(
                    ", ",
                    items.Select(i => i.ProductName));

            order.ProductDetails =
                string.Join(
                    "\n",
                    items.Select(i =>
                        $"• {i.ProductName} - {i.DisplayQuantity}"));
        }

        return orders;
    }


    // =====================================================
    // ORDER - CANCEL
    // =====================================================

    public async Task<int> CancelOrderAsync(int orderId)
        {
            await InitializeAsync();

            Order? order =
                await GetOrderByIdAsync(orderId);

            if (order == null)
                return 0;

            if (order.OrderStatus == "Delivered")
                return 0;

            if (order.OrderStatus == "Cancelled")
                return 0;

            order.OrderStatus = "Cancelled";

            return await _database!.UpdateAsync(order);
        }


        // =====================================================
        // ORDER - UPDATE STATUS
        // =====================================================

        public async Task<int> UpdateOrderStatusAsync(
            int orderId,
            string status)
        {
            await InitializeAsync();

            Order? order =
                await GetOrderByIdAsync(orderId);

            if (order == null)
                return 0;

            order.OrderStatus = status;

            return await _database!.UpdateAsync(order);
        }


        // =====================================================
        // ORDER - UPDATE PAYMENT
        // =====================================================

        public async Task<int> UpdatePaymentStatusAsync(
            int orderId,
            string paymentStatus)
        {
            await InitializeAsync();

            Order? order =
                await GetOrderByIdAsync(orderId);

            if (order == null)
                return 0;

            order.PaymentStatus = paymentStatus;

            return await _database!.UpdateAsync(order);
        }

    // =====================================================
    // NOTIFICATIONS
    // =====================================================

    public async Task<List<OrderNotification>> GetCustomerNotificationsAsync(
        int customerId)
    {
        await InitializeAsync();

        if (customerId <= 0)
            return new List<OrderNotification>();

        return await _database!
            .Table<OrderNotification>()
            .Where(n => n.CustomerId == customerId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }


    // =====================================================
    // NOTIFICATION - UNREAD COUNT
    // =====================================================

    public async Task<int> GetUnreadNotificationCountAsync(
        int customerId)
    {
        await InitializeAsync();

        if (customerId <= 0)
            return 0;

        return await _database!
            .Table<OrderNotification>()
            .Where(n =>
                n.CustomerId == customerId &&
                !n.IsRead)
            .CountAsync();
    }


    // =====================================================
    // NOTIFICATION - MARK AS SEEN
    // =====================================================

    public async Task<int> MarkNotificationAsReadAsync(
        int notificationId,
        int customerId)
    {
        await InitializeAsync();

        if (notificationId <= 0 || customerId <= 0)
            return 0;

        OrderNotification? notification =
            await _database!
                .Table<OrderNotification>()
                .Where(n =>
                    n.NotificationId == notificationId &&
                    n.CustomerId == customerId)
                .FirstOrDefaultAsync();

        if (notification == null)
            return 0;

        if (notification.IsRead)
            return 1;

        notification.IsRead = true;

        return await _database.UpdateAsync(notification);
    }

    // =====================================================
    // CUSTOMER PROFILE - GET
    // =====================================================

    public async Task<CustomerProfile?> GetCustomerProfileAsync(
            int customerId)
        {
            await InitializeAsync();

            return await _database!
                .Table<CustomerProfile>()
                .Where(p => p.CustomerId == customerId)
                .FirstOrDefaultAsync();
        }


    // =====================================================
    // CUSTOMER PROFILE - SAVE
    // =====================================================

    public async Task<int> SaveCustomerProfileAsync(
CustomerProfile profile)
    {
        await InitializeAsync();

        if (profile == null || profile.CustomerId <= 0)
            return 0;

        CustomerProfile? existing =
            await _database!
                .Table<CustomerProfile>()
                .Where(p =>
                    p.CustomerId == profile.CustomerId)
                .FirstOrDefaultAsync();

        if (existing != null)
        {
            return await _database.UpdateAsync(profile);
        }

        return await _database.InsertAsync(profile);
    }


    // =====================================================
    // SAVED ADDRESSES - GET
    // =====================================================

    public async Task<List<SavedAddress>> GetSavedAddressesAsync(
            int customerId)
        {
            await InitializeAsync();

            return await _database!
                .Table<SavedAddress>()
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AddressId)
                .ToListAsync();
        }


        // =====================================================
        // SAVED ADDRESSES - COUNT
        // =====================================================

        public async Task<int> GetSavedAddressCountAsync(
            int customerId)
        {
            await InitializeAsync();

            return await _database!
                .Table<SavedAddress>()
                .Where(a => a.CustomerId == customerId)
                .CountAsync();
        }


        // =====================================================
        // SAVED ADDRESSES - ADD
        // =====================================================

        public async Task<int> AddSavedAddressAsync(
            SavedAddress address)
        {
            await InitializeAsync();

            int count =
                await GetSavedAddressCountAsync(
                    address.CustomerId);

            if (count >= 5)
                return -1;

            return await _database!.InsertAsync(address);
        }


        // =====================================================
        // SAVED ADDRESSES - UPDATE
        // =====================================================

        public async Task<int> UpdateSavedAddressAsync(
            SavedAddress address)
        {
            await InitializeAsync();

            if (address.AddressId <= 0)
                return 0;

            SavedAddress? existing =
                await _database!
                    .Table<SavedAddress>()
                    .Where(a =>
                        a.AddressId == address.AddressId &&
                        a.CustomerId == address.CustomerId)
                    .FirstOrDefaultAsync();

            if (existing == null)
                return 0;

            existing.FullName =
                address.FullName;

            existing.PhoneNumber =
                address.PhoneNumber;

            existing.Address =
                address.Address;

            existing.City =
                address.City;

            existing.State =
                address.State;

            existing.Pincode =
                address.Pincode;

            existing.AddressType =
                address.AddressType;

            return await _database.UpdateAsync(existing);
        }


        // =====================================================
        // SAVED ADDRESSES - DELETE
        // =====================================================

        public async Task<int> DeleteSavedAddressAsync(
            int addressId)
        {
            await InitializeAsync();

            SavedAddress? address =
                await _database!
                    .Table<SavedAddress>()
                    .Where(a => a.AddressId == addressId)
                    .FirstOrDefaultAsync();

            if (address == null)
                return 0;

            return await _database.DeleteAsync(address);
        }


        // =====================================================
        // ACCOUNT - DEACTIVATE
        // =====================================================

        public async Task<bool> DeactivateAccountAsync(
            int customerId)
        {
            await InitializeAsync();

            Customer? customer =
                await _database!
                    .Table<Customer>()
                    .Where(c => c.CustomerId == customerId)
                    .FirstOrDefaultAsync();

            if (customer == null)
                return false;

            customer.IsDeactivated = true;

            int result =
                await _database.UpdateAsync(customer);

            return result > 0;
        }


        // =====================================================
        // ACCOUNT - DELETE
        // =====================================================

        public async Task<bool> DeleteCustomerAccountAsync(
            int customerId)
        {
            await InitializeAsync();

            // -------------------------------------------------
            // DELETE CART ITEMS
            // -------------------------------------------------

            List<CartItem> cartItems =
                await _database!
                    .Table<CartItem>()
                    .Where(c => c.CustomerId == customerId)
                    .ToListAsync();

            foreach (CartItem item in cartItems)
            {
                await _database.DeleteAsync(item);
            }


            // -------------------------------------------------
            // DELETE SAVED ADDRESSES
            // -------------------------------------------------

            List<SavedAddress> addresses =
                await _database
                    .Table<SavedAddress>()
                    .Where(a => a.CustomerId == customerId)
                    .ToListAsync();

            foreach (SavedAddress address in addresses)
            {
                await _database.DeleteAsync(address);
            }


            // -------------------------------------------------
            // DELETE PROFILE
            // -------------------------------------------------

            CustomerProfile? profile =
                await _database
                    .Table<CustomerProfile>()
                    .Where(p => p.CustomerId == customerId)
                    .FirstOrDefaultAsync();

            if (profile != null)
            {
                await _database.DeleteAsync(profile);
            }


            // -------------------------------------------------
            // GET ORDERS
            // -------------------------------------------------

            List<Order> orders =
                await _database
                    .Table<Order>()
                    .Where(o => o.CustomerId == customerId)
                    .ToListAsync();


            // -------------------------------------------------
            // DELETE ORDER ITEMS + ORDERS
            // -------------------------------------------------

            foreach (Order order in orders)
            {
                List<OrderItem> orderItems =
                    await _database
                        .Table<OrderItem>()
                        .Where(i => i.OrderId == order.OrderId)
                        .ToListAsync();

                foreach (OrderItem item in orderItems)
                {
                    await _database.DeleteAsync(item);
                }

                await _database.DeleteAsync(order);
            }


            // -------------------------------------------------
            // DELETE WISHLIST ITEMS
            // -------------------------------------------------

            List<WishlistItem> wishlistItems =
                await _database!
                    .Table<WishlistItem>()
                    .Where(w => w.CustomerId == customerId)
                    .ToListAsync();

            foreach (WishlistItem item in wishlistItems)
            {
                await _database.DeleteAsync(item);
            }


            // -------------------------------------------------
            // DELETE REVIEWS
            // -------------------------------------------------

            List<Review> reviews =
                await _database!
                    .Table<Review>()
                    .Where(r => r.CustomerId == customerId)
                    .ToListAsync();

            foreach (Review review in reviews)
            {
                await _database.DeleteAsync(review);
            }


            // -------------------------------------------------
            // DELETE CUSTOMER
            // -------------------------------------------------

            Customer? customer =
                await _database
                    .Table<Customer>()
                    .Where(c => c.CustomerId == customerId)
                    .FirstOrDefaultAsync();

            if (customer == null)
                return false;

            int result =
                await _database.DeleteAsync(customer);

            return result > 0;
        }


        // =====================================================
        // REVIEWS - CHECK IF CUSTOMER CAN REVIEW PRODUCT
        // =====================================================

        public async Task<bool> CanCustomerReviewProductAsync(
            int customerId,
            int productId)
        {
            await InitializeAsync();

            if (customerId <= 0 ||
                productId <= 0)
            {
                return false;
            }

            // -------------------------------------------------
            // CHECK WHETHER CUSTOMER ALREADY REVIEWED PRODUCT
            // -------------------------------------------------

            Review? existingReview =
                await _database!
                    .Table<Review>()
                    .Where(r =>
                        r.CustomerId == customerId &&
                        r.ProductId == productId)
                    .FirstOrDefaultAsync();

            if (existingReview != null)
                return false;


            // -------------------------------------------------
            // GET DELIVERED CUSTOMER ORDERS
            // -------------------------------------------------

            List<Order> orders =
                await _database
                    .Table<Order>()
                    .Where(o =>
                        o.CustomerId == customerId &&
                        o.OrderStatus == "Delivered")
                    .ToListAsync();

            if (orders.Count == 0)
                return false;


            // -------------------------------------------------
            // CHECK DELIVERED ORDER ITEMS
            // -------------------------------------------------

            foreach (Order order in orders)
            {
                OrderItem? orderItem =
                    await _database
                        .Table<OrderItem>()
                        .Where(i =>
                            i.OrderId == order.OrderId &&
                            i.ProductId == productId)
                        .FirstOrDefaultAsync();

                if (orderItem != null)
                {
                    return true;
                }
            }

            return false;
        }


        // =====================================================
        // REVIEWS - ADD REVIEW
        // =====================================================

        public async Task<int> AddReviewAsync(Review review)
        {
            await InitializeAsync();

            if (review.CustomerId <= 0 ||
                review.ProductId <= 0)
            {
                return 0;
            }

            // -------------------------------------------------
            // VALIDATE RATING
            // -------------------------------------------------

            if (review.Rating < 1 ||
                review.Rating > 5)
            {
                return 0;
            }

            // -------------------------------------------------
            // CHECK WHETHER CUSTOMER CAN REVIEW
            // -------------------------------------------------

            bool canReview =
                await CanCustomerReviewProductAsync(
                    review.CustomerId,
                    review.ProductId);

            if (!canReview)
                return 0;

            // -------------------------------------------------
            // REVIEW DATE
            // -------------------------------------------------

            review.ReviewDate =
                DateTime.Now.ToString("dd MMM yyyy");

            // -------------------------------------------------
            // SAVE REVIEW
            // -------------------------------------------------

            return await _database!.InsertAsync(review);
        }


        // =====================================================
        // REVIEWS - GET ALL REVIEWS FOR PRODUCT
        // =====================================================

        public async Task<List<Review>> GetProductReviewsAsync(
            int productId)
        {
            await InitializeAsync();

            if (productId <= 0)
                return new List<Review>();

            List<Review> reviews =
                await _database!
                    .Table<Review>()
                    .Where(r => r.ProductId == productId)
                    .OrderByDescending(r => r.ReviewId)
                    .ToListAsync();

            // -------------------------------------------------
            // LOAD CUSTOMER NAMES + PRODUCT INFORMATION
            // -------------------------------------------------

            foreach (Review review in reviews)
            {
                Customer? customer =
                    await _database
                        .Table<Customer>()
                        .Where(c =>
                            c.CustomerId == review.CustomerId)
                        .FirstOrDefaultAsync();

                if (customer != null)
                {
                    review.CustomerName =
                        customer.Email;
                }
                else if (string.IsNullOrWhiteSpace(review.CustomerName))
                {
                    review.CustomerName =
                        "Customer";
                }


                Product? product =
                    ProductCatalogue.Products
                        .FirstOrDefault(p =>
                            p.ProductId == review.ProductId);

                if (product != null)
                {
                    review.ProductName =
                        product.Name;

                    review.ProductImage =
                        product.Image;
                }
            }

            return reviews;
        }


        // =====================================================
        // REVIEWS - GET CUSTOMER REVIEWS
        // =====================================================

        public async Task<List<Review>> GetCustomerReviewsAsync(
            int customerId)
        {
            await InitializeAsync();

            if (customerId <= 0)
                return new List<Review>();

            List<Review> reviews =
                await _database!
                    .Table<Review>()
                    .Where(r =>
                        r.CustomerId == customerId)
                    .OrderByDescending(r =>
                        r.ReviewId)
                    .ToListAsync();

            // -------------------------------------------------
            // LOAD PRODUCT INFORMATION
            // -------------------------------------------------

            foreach (Review review in reviews)
            {
                Product? product =
                    ProductCatalogue.Products
                        .FirstOrDefault(p =>
                            p.ProductId == review.ProductId);

                if (product != null)
                {
                    review.ProductName =
                        product.Name;

                    review.ProductImage =
                        product.Image;
                }
            }

            return reviews;
        }


        // =====================================================
        // REVIEWS - GET CUSTOMER REVIEW FOR PRODUCT
        // =====================================================

        public async Task<Review?> GetCustomerProductReviewAsync(
            int customerId,
            int productId)
        {
            await InitializeAsync();

            if (customerId <= 0 ||
                productId <= 0)
            {
                return null;
            }

            return await _database!
                .Table<Review>()
                .Where(r =>
                    r.CustomerId == customerId &&
                    r.ProductId == productId)
                .FirstOrDefaultAsync();
        }


        // =====================================================
        // REVIEWS - GET AVERAGE RATING
        // =====================================================

        public async Task<double> GetProductAverageRatingAsync(
            int productId)
        {
            List<Review> reviews =
                await GetProductReviewsAsync(productId);

            if (reviews.Count == 0)
                return 0;

            return reviews.Average(r => r.Rating);
        }


        // =====================================================
        // REVIEWS - DELETE CUSTOMER REVIEWS
        // =====================================================

        public async Task<int> DeleteCustomerReviewsAsync(
            int customerId)
        {
            await InitializeAsync();

            List<Review> reviews =
                await _database!
                    .Table<Review>()
                    .Where(r =>
                        r.CustomerId == customerId)
                    .ToListAsync();

            int count = 0;

            foreach (Review review in reviews)
            {
                count +=
                    await _database.DeleteAsync(review);
            }

            return count;
        }


    // =====================================================
    // DELETE DATABASE
    // =====================================================

    public void DeleteDatabase()
    {
        string databasePath = GetDatabasePath();

        if (File.Exists(databasePath))
        {
            File.Delete(databasePath);
        }

        _database = null;
    }
}