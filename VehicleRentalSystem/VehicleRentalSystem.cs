using System;

// Abstract Class
abstract class Vehicle
{
    public string Model { get; set; }
    public int Year { get; set; }

    protected Vehicle(string model, int year)
    {
        Model = model;
        Year = year;
    }

    public abstract double CalculateRentalPrice(int days);
}

// Interface
interface IRentable
{
    void Rent(int days);
}

// Car Class
class Car : Vehicle, IRentable
{
    private const double DailyRate = 100.0;

    public Car(string model, int year) : base(model, year) { }

    public override double CalculateRentalPrice(int days)
    {
        return days * DailyRate;
    }

    public void Rent(int days)
    {
        Console.WriteLine($"Car: {Model} ({Year}) rented for {days} day(s). Total: ${CalculateRentalPrice(days)}");
    }
}

// Motorcycle Class
class Motorcycle : Vehicle, IRentable
{
    private const double DailyRate = 50.0;

    public Motorcycle(string model, int year) : base(model, year) { }

    public override double CalculateRentalPrice(int days)
    {
        return days * DailyRate;
    }

    public void Rent(int days)
    {
        Console.WriteLine($"Motorcycle: {Model} ({Year}) rented for {days} day(s). Total: ${CalculateRentalPrice(days)}");
    }
}

// Main Application
class Program
{
    static List<IRentable> vehicles = new();

    static void InitializeFleet()
    {
        vehicles.Add(new Car("Toyota Corolla", 2020));
        vehicles.Add(new Car("Honda Civic", 2021));
        vehicles.Add(new Motorcycle("Yamaha MT-07", 2022));
        vehicles.Add(new Motorcycle("Kawasaki Ninja", 2021));
    }

    static void Main()
    {
        InitializeFleet();
        bool continueProgram = true;

        while (continueProgram)
        {
            Console.WriteLine("\n===== Vehicle Rental System =====");
            Console.WriteLine("1. View Available Vehicles");
            Console.WriteLine("2. Rent a Car");
            Console.WriteLine("3. Rent a Motorcycle");
            Console.WriteLine("4. Exit");
            Console.Write("\nSelect an option (1-4): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        DisplayVehicles();
                        break;
                    case 2:
                        RentVehicle("Car");
                        break;
                    case 3:
                        RentVehicle("Motorcycle");
                        break;
                    case 4:
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
        Console.WriteLine("Thank you for using the Vehicle Rental System!");
    }

    static void DisplayVehicles()
    {
        Console.WriteLine("\nAvailable Vehicles:");
        for (int i = 0; i < vehicles.Count; i++)
        {
            var vehicle = vehicles[i] as Vehicle;
            Console.WriteLine($"{i + 1}. {vehicle?.GetType().Name}: {vehicle?.Model} ({vehicle?.Year})");
        }
    }

    static void RentVehicle(string type)
    {
        DisplayVehicles();
        Console.Write("\nSelect vehicle number: ");
        if (int.TryParse(Console.ReadLine(), out int vehicleNumber) && 
            vehicleNumber > 0 && vehicleNumber <= vehicles.Count)
        {
            var selected = vehicles[vehicleNumber - 1] as Vehicle;
            if (selected?.GetType().Name == type)
            {
                Console.Write("Enter number of days: ");
                if (int.TryParse(Console.ReadLine(), out int days) && days > 0)
                {
                    vehicles[vehicleNumber - 1].Rent(days);
                }
                else
                {
                    Console.WriteLine("Invalid number of days.");
                }
            }
            else
            {
                Console.WriteLine($"Selected vehicle is not a {type}.");
            }
        }
        else
        {
            Console.WriteLine("Invalid vehicle number.");
        }
    }
}
