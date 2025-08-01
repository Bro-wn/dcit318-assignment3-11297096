using System;
using System.Collections.Generic;

// -------------------- Marker Interface --------------------
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// -------------------- Product Classes --------------------
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString() =>
        $"[Electronics] {Name} (ID: {Id}) - {Quantity} in stock, Brand: {Brand}, Warranty: {WarrantyMonths} months";
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString() =>
        $"[Grocery] {Name} (ID: {Id}) - {Quantity} in stock, Expires: {ExpiryDate:yyyy-MM-dd}";
}

// -------------------- Custom Exceptions --------------------
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// -------------------- Generic Inventory Repository --------------------
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items.Remove(id);
    }

    public List<T> GetAllItems() => new(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");

        item.Quantity = newQuantity;
    }
}

// -------------------- Warehouse Manager --------------------
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new();
    private InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Smart TV", 5, "Samsung", 24));
            _electronics.AddItem(new ElectronicItem(2, "Laptop", 10, "Dell", 12));

            _groceries.AddItem(new GroceryItem(1, "Milk", 20, DateTime.Now.AddDays(10)));
            _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Updated quantity of {item.Name}: {item.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item with ID {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    // Accessors for testing in Main
    public InventoryRepository<ElectronicItem> GetElectronicsRepo() => _electronics;
    public InventoryRepository<GroceryItem> GetGroceriesRepo() => _groceries;
}

// -------------------- Main Program --------------------
class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();
        bool continueProgram = true;

        while (continueProgram)
        {
            Console.WriteLine("\n===== Warehouse Inventory System =====");
            Console.WriteLine("1. View All Inventory");
            Console.WriteLine("2. Add New Item");
            Console.WriteLine("3. Update Quantity");
            Console.WriteLine("4. Remove Item");
            Console.WriteLine("5. Exit");
            Console.Write("\nSelect an option (1-5): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("\n---- Grocery Inventory ----");
                        manager.PrintAllItems(manager.GetGroceriesRepo());
                        Console.WriteLine("\n---- Electronic Inventory ----");
                        manager.PrintAllItems(manager.GetElectronicsRepo());
                        break;

                    case 2:
                        AddNewItem(manager);
                        break;

                    case 3:
                        UpdateItemQuantity(manager);
                        break;

                    case 4:
                        RemoveItem(manager);
                        break;

                    case 5:
                        continueProgram = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }

            if (continueProgram)
            {
                Console.Write("\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }
        Console.WriteLine("Thank you for using the Warehouse Inventory System!");
    }

    private static void AddNewItem(WareHouseManager manager)
    {
        Console.WriteLine("\nSelect item type:");
        Console.WriteLine("1. Electronic Item");
        Console.WriteLine("2. Grocery Item");
        
        if (int.TryParse(Console.ReadLine(), out int type))
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            try
            {
                if (type == 1)
                {
                    Console.Write("Enter Brand: ");
                    string brand = Console.ReadLine() ?? "";
                    Console.Write("Enter Warranty (months): ");
                    int warranty = int.Parse(Console.ReadLine() ?? "0");
                    manager.GetElectronicsRepo().AddItem(
                        new ElectronicItem(id, name, quantity, brand, warranty));
                }
                else if (type == 2)
                {
                    Console.Write("Enter Expiry Date (yyyy-MM-dd): ");
                    DateTime expiry = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());
                    manager.GetGroceriesRepo().AddItem(
                        new GroceryItem(id, name, quantity, expiry));
                }
                Console.WriteLine("Item added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void UpdateItemQuantity(WareHouseManager manager)
    {
        Console.WriteLine("\nSelect item type:");
        Console.WriteLine("1. Electronic Item");
        Console.WriteLine("2. Grocery Item");
        
        if (int.TryParse(Console.ReadLine(), out int type))
        {
            Console.Write("Enter Item ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Enter New Quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity))
                {
                    if (type == 1)
                        manager.IncreaseStock(manager.GetElectronicsRepo(), id, quantity);
                    else if (type == 2)
                        manager.IncreaseStock(manager.GetGroceriesRepo(), id, quantity);
                }
            }
        }
    }

    private static void RemoveItem(WareHouseManager manager)
    {
        Console.WriteLine("\nSelect item type:");
        Console.WriteLine("1. Electronic Item");
        Console.WriteLine("2. Grocery Item");
        
        if (int.TryParse(Console.ReadLine(), out int type))
        {
            Console.Write("Enter Item ID to remove: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (type == 1)
                    manager.RemoveItemById(manager.GetElectronicsRepo(), id);
                else if (type == 2)
                    manager.RemoveItemById(manager.GetGroceriesRepo(), id);
            }
        }
    }
}
