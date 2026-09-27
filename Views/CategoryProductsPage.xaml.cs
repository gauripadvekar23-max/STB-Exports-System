using STB_Exports.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace STB_Exports.Views;

public partial class CategoryProductsPage : ContentPage
{
    private readonly HttpClient _httpClient;

    private readonly ObservableCollection<CategoryItem> _categories =
        new();

    private string? _selectedCategory;

    public ObservableCollection<Product> Products { get; } =
        new();

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public CategoryProductsPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7197")
        };

        BindingContext = this;

        CategoriesCollection.ItemsSource = _categories;
    }

    public CategoryProductsPage(string category)
        : this()
    {
        _selectedCategory = category;
    }

    // =========================================================
    // PAGE APPEARING
    // =========================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadCategoriesAsync();

        if (!string.IsNullOrWhiteSpace(_selectedCategory))
        {
            CategoryTitle.Text = _selectedCategory;

            await LoadProductsAsync(_selectedCategory);
        }
        else
        {
            CategoryTitle.Text = "All Categories";

            await LoadProductsAsync();
        }
    }

    // =========================================================
    // CATEGORY API
    // =========================================================

    private async Task LoadCategoriesAsync()
    {
        try
        {
            List<ApiCategory>? categories =
                await _httpClient.GetFromJsonAsync<List<ApiCategory>>(
                    "/api/Categories");

            _categories.Clear();

            if (categories == null || categories.Count == 0)
            {
                return;
            }

            foreach (ApiCategory category in categories)
            {
                if (!category.IsActive)
                    continue;

                _categories.Add(
                    new CategoryItem
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,

                        Image =
                            !string.IsNullOrWhiteSpace(category.Image)
                                ? GetApiMediaUrl(category.Image)
                                : GetCategoryImage(category.Name)
                    });
            }
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"Unable to load categories.\n\n{ex.Message}",
                "OK");
        }
    }

    // =========================================================
    // PRODUCT API
    // =========================================================

    private async Task LoadProductsAsync(string? category = null)
    {
        try
        {
            List<ApiProduct>? apiProducts =
                await _httpClient.GetFromJsonAsync<List<ApiProduct>>(
                    "/api/Products");

            Products.Clear();

            if (apiProducts == null || apiProducts.Count == 0)
            {
                NoProductsLabel.IsVisible = true;
                return;
            }

            IEnumerable<ApiProduct> filteredProducts =
                apiProducts.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(category))
            {
                filteredProducts =
                    filteredProducts.Where(
                        p => string.Equals(
                            p.Category,
                            category,
                            StringComparison.OrdinalIgnoreCase));
            }

            foreach (ApiProduct apiProduct in filteredProducts)
            {
                Product product =
                    ConvertApiProduct(apiProduct);

                System.Diagnostics.Debug.WriteLine(
                    $"API PRODUCT: {apiProduct.ProductId} | " +
                    $"{apiProduct.Name} | " +
                    $"LOCAL IMAGE: {product.Image}");

                Products.Add(product);
            }

            NoProductsLabel.IsVisible =
                Products.Count == 0;
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");

            NoProductsLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"Unable to load products.\n\n{ex.Message}",
                "OK");

            NoProductsLabel.IsVisible = true;
        }
    }

    // =========================================================
    // CATEGORY CLICK
    // =========================================================

    private async void Category_Tapped(
        object sender,
        TappedEventArgs e)
    {
        if (sender is not Border border)
            return;

        if (border.BindingContext is not CategoryItem category)
            return;

        _selectedCategory = category.Name;

        CategoryTitle.Text = category.Name;

        await LoadProductsAsync(category.Name);
    }

    // =========================================================
    // API PRODUCT → CUSTOMER PRODUCT
    // =========================================================

    private Product ConvertApiProduct(ApiProduct apiProduct)
    {
        Product? localProduct = null;

        // ---------------------------------------------------------
        // 1. MATCH BY API PRODUCT ID
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(apiProduct.ProductId))
        {
            localProduct =
                ProductCatalogue.Products.FirstOrDefault(
                    p => string.Equals(
                        p.ApiProductId,
                        apiProduct.ProductId,
                        StringComparison.OrdinalIgnoreCase));
        }

        // ---------------------------------------------------------
        // 2. MATCH BY LOCAL NUMERIC PRODUCT ID
        // ---------------------------------------------------------

        if (localProduct == null &&
            int.TryParse(
                apiProduct.ProductId,
                out int numericId))
        {
            localProduct =
                ProductCatalogue.Products.FirstOrDefault(
                    p => p.ProductId == numericId);
        }

        // ---------------------------------------------------------
        // 3. MATCH BY PRODUCT NAME
        // ---------------------------------------------------------

        if (localProduct == null &&
            !string.IsNullOrWhiteSpace(apiProduct.Name))
        {
            localProduct =
                ProductCatalogue.Products.FirstOrDefault(
                    p => string.Equals(
                        p.Name?.Trim(),
                        apiProduct.Name.Trim(),
                        StringComparison.OrdinalIgnoreCase));
        }

        // ---------------------------------------------------------
        // 4. MAIN IMAGE
        // ---------------------------------------------------------

        string mainImage = string.Empty;

        if (!string.IsNullOrWhiteSpace(apiProduct.Image))
        {
            if (IsApiMediaPath(apiProduct.Image))
            {
                mainImage =
                    GetApiMediaUrl(apiProduct.Image);
            }
            else
            {
                // API contains a simple filename such as:
                // apple.png
                //
                // Use local MAUI image if available.

                mainImage =
                    localProduct?.Image ??
                    apiProduct.Image.Trim();
            }
        }

        // ---------------------------------------------------------
        // 5. ADDITIONAL IMAGES
        // ---------------------------------------------------------

        List<string> images = new();

        if (!string.IsNullOrWhiteSpace(mainImage))
        {
            images.Add(mainImage);
        }

        if (!string.IsNullOrWhiteSpace(
            apiProduct.AdditionalImages))
        {
            try
            {
                List<string>? additionalImages =
                    JsonSerializer.Deserialize<List<string>>(
                        apiProduct.AdditionalImages);

                if (additionalImages != null)
                {
                    foreach (string image in additionalImages)
                    {
                        if (string.IsNullOrWhiteSpace(image))
                            continue;

                        string imageUrl;

                        if (IsApiMediaPath(image))
                        {
                            imageUrl =
                                GetApiMediaUrl(image);
                        }
                        else
                        {
                            imageUrl = image.Trim();
                        }

                        if (!images.Contains(
                            imageUrl,
                            StringComparer.OrdinalIgnoreCase))
                        {
                            images.Add(imageUrl);
                        }
                    }
                }
            }
            catch
            {
                // Ignore invalid AdditionalImages JSON.
            }
        }

        // ---------------------------------------------------------
        // 6. FALLBACK TO LOCAL PRODUCT IMAGES
        // ---------------------------------------------------------

        if (images.Count == 0 &&
            localProduct?.Images != null)
        {
            foreach (string image in localProduct.Images)
            {
                if (!string.IsNullOrWhiteSpace(image))
                {
                    images.Add(image);
                }
            }
        }

        // ---------------------------------------------------------
        // 7. PRODUCT VIDEO
        // ---------------------------------------------------------

        string videoUrl =
            GetApiMediaUrl(apiProduct.Video);

        // ---------------------------------------------------------
        // 8. CREATE CUSTOMER PRODUCT
        // ---------------------------------------------------------

        return new Product
        {
            ProductId =
                localProduct?.ProductId
                ?? GetCustomerProductId(
                    apiProduct.ProductId),

            ApiProductId =
                apiProduct.ProductId ?? string.Empty,

            Name =
                apiProduct.Name ?? string.Empty,

            Category =
                apiProduct.Category ?? string.Empty,

            Price =
                (decimal)apiProduct.Price,

            QuantityUnit =
                apiProduct.QuantityUnit ?? "Weight",

            Quantity =
                (int)apiProduct.Quantity,

            Image =
                mainImage,

            Images =
                images,

            BenefitsImage =
                localProduct?.BenefitsImage ??
                string.Empty,

            Video =
                videoUrl,

            Description =
                !string.IsNullOrWhiteSpace(
                    apiProduct.Description)
                    ? apiProduct.Description
                    : localProduct?.Description
                      ?? string.Empty
        };
    }

    // =========================================================
    // CHECK WHETHER PATH IS AN API MEDIA PATH
    // =========================================================

    private bool IsApiMediaPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        path = path.Trim();

        // Complete HTTP/HTTPS URL
        if (path.StartsWith(
                "http://",
                StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(
                "https://",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // API-relative uploads path
        if (path.StartsWith(
                "/uploads/",
                StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(
                "uploads/",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Other common API media folders
        if (path.StartsWith(
                "/media/",
                StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(
                "media/",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (path.StartsWith(
                "/images/",
                StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(
                "images/",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    // =========================================================
    // API MEDIA URL
    // =========================================================

    private string GetApiMediaUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        path = path.Trim();

        // Already complete URL
        if (path.StartsWith(
                "http://",
                StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(
                "https://",
                StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        // API-relative path
        if (path.StartsWith("/"))
        {
            return $"https://localhost:7197{path}";
        }

        // Relative API path
        return $"https://localhost:7197/{path}";
    }

    // =========================================================
    // CUSTOMER PRODUCT ID
    // =========================================================

    private int GetCustomerProductId(
        string apiProductId)
    {
        if (int.TryParse(
            apiProductId,
            out int numericId))
        {
            return numericId;
        }

        unchecked
        {
            int hash = 17;

            foreach (char c in apiProductId)
            {
                hash = hash * 31 + c;
            }

            return Math.Abs(hash);
        }
    }

    // =========================================================
    // GET REAL API PRODUCT ID
    // =========================================================

    private string GetApiProductId(Product product)
    {
        return product.ApiProductId;
    }

    // =========================================================
    // CATEGORY IMAGES
    // =========================================================

    private string GetCategoryImage(
        string categoryName)
    {
        return categoryName
            .ToLowerInvariant()
            .Trim() switch
        {
            "fruits" =>
                "fruits.png",

            "vegetables" =>
                "vegetables.png",

            "dry fruits" =>
                "dryfruits.png",

            "clay" =>
                "clay.png",

            "spices" =>
                "spices.png",

            "coconut" =>
                "coconut.png",

            _ =>
                "default_category.png"
        };
    }

    // =========================================================
    // ADD TO CART
    // =========================================================

    private async void AddToCartButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Product product)
            return;

        string customerId =
            Preferences.Get(
                "ApiCustomerId",
                string.Empty);

        if (string.IsNullOrWhiteSpace(customerId))
        {
            await DisplayAlert(
                "Login Required",
                "Please login first to add products to your cart.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(
            product.ApiProductId))
        {
            await DisplayAlert(
                "Error",
                "API Product ID is missing.",
                "OK");

            return;
        }

        try
        {
            ApiCartItem cartItem =
                new ApiCartItem
                {
                    CustomerId =
                        customerId,

                    ProductId =
                        product.ApiProductId,

                    Quantity =
                        product.QuantityUnit.Equals(
                            "Weight",
                            StringComparison.OrdinalIgnoreCase)
                            ? 250
                            : 1,

                    QuantityUnit =
                        product.QuantityUnit,

                    Price =
                        (double)product.Price
                };

            using HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "/api/CartItems",
                    cartItem);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert(
                    "Added to Cart",
                    $"{product.Name} has been added to your cart.",
                    "OK");

                return;
            }

            string error =
                await response.Content.ReadAsStringAsync();

            await DisplayAlert(
                "Cart Error",
                string.IsNullOrWhiteSpace(error)
                    ? $"API returned status code {(int)response.StatusCode}."
                    : error,
                "OK");
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Cart Error",
                ex.Message,
                "OK");
        }
    }

    // =========================================================
    // WISHLIST API
    // =========================================================

    private async void WishlistButton_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Product product)
            return;

        string customerId =
            Preferences.Get(
                "ApiCustomerId",
                string.Empty)
            .Trim();

        if (string.IsNullOrWhiteSpace(customerId))
        {
            await DisplayAlert(
                "Login Required",
                "Please login first to add products to your wishlist.",
                "OK");

            return;
        }

        string apiProductId =
            product.ApiProductId?.Trim() ??
            string.Empty;

        if (string.IsNullOrWhiteSpace(apiProductId))
        {
            await DisplayAlert(
                "Error",
                "API Product ID is missing.",
                "OK");

            return;
        }

        try
        {
            using HttpResponseMessage getResponse =
                await _httpClient.GetAsync(
                    $"/api/Wishlist/customer/{customerId}");

            if (!getResponse.IsSuccessStatusCode)
            {
                string error =
                    await getResponse.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "Wishlist Error",
                    string.IsNullOrWhiteSpace(error)
                        ? "Unable to check wishlist."
                        : error,
                    "OK");

                return;
            }

            List<ApiWishlistItem>? wishlist =
                await getResponse.Content
                    .ReadFromJsonAsync<List<ApiWishlistItem>>();

            bool isAlreadyInWishlist =
                wishlist?.Any(
                    w => string.Equals(
                        w.ProductId?.Trim(),
                        apiProductId,
                        StringComparison.OrdinalIgnoreCase)) == true;

            // -----------------------------------------------------
            // REMOVE
            // -----------------------------------------------------

            if (isAlreadyInWishlist)
            {
                using HttpResponseMessage deleteResponse =
                    await _httpClient.DeleteAsync(
                        $"/api/Wishlist/{customerId}/{apiProductId}");

                if (deleteResponse.IsSuccessStatusCode)
                {
                    await DisplayAlert(
                        "Wishlist",
                        $"{product.Name} has been removed from your wishlist.",
                        "OK");
                }
                else
                {
                    string error =
                        await deleteResponse.Content.ReadAsStringAsync();

                    await DisplayAlert(
                        "Wishlist Error",
                        string.IsNullOrWhiteSpace(error)
                            ? $"Unable to remove {product.Name}."
                            : error,
                        "OK");
                }

                return;
            }

            // -----------------------------------------------------
            // ADD
            // -----------------------------------------------------

            ApiWishlistItem wishlistItem =
                new ApiWishlistItem
                {
                    CustomerId =
                        customerId,

                    ProductId =
                        apiProductId,

                    AddedAt =
                        DateTime.UtcNow
                };

            using HttpResponseMessage postResponse =
                await _httpClient.PostAsJsonAsync(
                    "/api/Wishlist",
                    wishlistItem);

            if (postResponse.IsSuccessStatusCode)
            {
                await DisplayAlert(
                    "Wishlist",
                    $"{product.Name} has been added to your wishlist.",
                    "OK");

                return;
            }

            if (postResponse.StatusCode ==
                HttpStatusCode.Conflict)
            {
                await DisplayAlert(
                    "Wishlist",
                    $"{product.Name} is already in your wishlist.",
                    "OK");

                return;
            }

            string postError =
                await postResponse.Content.ReadAsStringAsync();

            await DisplayAlert(
                "Wishlist Error",
                string.IsNullOrWhiteSpace(postError)
                    ? $"API returned status code {(int)postResponse.StatusCode}."
                    : postError,
                "OK");
        }
        catch (HttpRequestException)
        {
            await DisplayAlert(
                "Connection Error",
                "Unable to connect to the API.\n\n" +
                "Please make sure STB_Exports_API is running.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Wishlist Error",
                ex.Message,
                "OK");
        }
    }

    // =========================================================
    // PRODUCT DETAILS
    // =========================================================

    private async void ProductDetails_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Product product)
            return;

        await Navigation.PushAsync(
            new ProductsDetailsPage(product));
    }
}

// =============================================================
// CUSTOMER CATEGORY ITEM
// =============================================================

public class CategoryItem
{
    public string CategoryId { get; set; } =
        string.Empty;

    public string Name { get; set; } =
        string.Empty;

    public string Image { get; set; } =
        string.Empty;
}