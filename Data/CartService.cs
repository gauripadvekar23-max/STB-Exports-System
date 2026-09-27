using System.Collections.ObjectModel;
using STB_Exports.Models;

namespace STB_Exports.Data;

public static class CartService
{
    public static ObservableCollection<Product> Cart { get; }
        = new ObservableCollection<Product>();

    public static void Add(Product product)
    {
        if (product == null)
            return;

        Cart.Add(product);
    }

    public static void Remove(Product product)
    {
        if (product == null)
            return;

        Cart.Remove(product);
    }

    public static void Clear()
    {
        Cart.Clear();
    }
}