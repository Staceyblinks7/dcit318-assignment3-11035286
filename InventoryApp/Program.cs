using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// 1. Marker Interface
public interface IInventoryEntity
{
    int Id { get; }
}

// 2. Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// 3. Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    // Add item to log
    public void Add(T item)
    {
        _log.Add(item);
    }

    // Get all items
    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    // Save to file (JSON serialization)
    public void SaveToFile()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                sw.Write(json);
            }
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving to file: " + ex.Message);
        }
    }

    // Load from file (JSON deserialization)
    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No saved file found.");
                return;
            }

            using (StreamReader sr = new StreamReader(_filePath))
            {
                string json = sr.ReadToEnd();
                var data = JsonSerializer.Deserialize<List<T>>(json);
                if (data != null)
                    _log = data;
            }
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading from file: " + ex.Message);
        }
    }
}

// 4. Integration Layer – InventoryApp
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Mouse", 15, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 10, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 7, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No inventory data found.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

// 5. Main Application Flow
class Program
{
    static void Main()
    {
        string filePath = "inventory.json";

        // First session: Create, seed, save
        InventoryApp app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session by creating new app instance
        Console.WriteLine("\n--- Simulating New Session ---\n");

        InventoryApp newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}
