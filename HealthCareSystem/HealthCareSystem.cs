using System;
using System.Collections.Generic;
using System.Linq;

// -------------------- Generic Repository --------------------

public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item) => items.Add(item);

    public List<T> GetAll() => new(items);

    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// -------------------- Entity Classes --------------------

public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Patient {Name} (ID: {Id}) - Age: {Age}, Gender: {Gender}";
    }
}

public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:d}";
    }
}

// -------------------- Healthcare System App --------------------

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Johnson", 28, "Female"));
        _patientRepo.Add(new Patient(2, "John Mensah", 35, "Male"));
        _patientRepo.Add(new Patient(3, "Fatima Issah", 42, "Female"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Amoxicillin", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Ibuprofen", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(104, 3, "Cough Syrup", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(105, 3, "Vitamin C", DateTime.Now.AddDays(-4)));
    }

    public void BuildPrescriptionMap()
    {
        var prescriptions = _prescriptionRepo.GetAll();
        _prescriptionMap.Clear();

        foreach (var prescription in prescriptions)
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        var patients = _patientRepo.GetAll();
        Console.WriteLine("All Patients:");
        foreach (var patient in patients)
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        if (_prescriptionMap.ContainsKey(patientId))
        {
            Console.WriteLine($"\nPrescriptions for Patient ID {patientId}:");
            foreach (var prescription in _prescriptionMap[patientId])
            {
                Console.WriteLine(prescription);
            }
        }
        else
        {
            Console.WriteLine($"No prescriptions found for Patient ID {patientId}.");
        }
    }
}

// -------------------- Main Method --------------------

class Program
{
    static void Main(string[] args)
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();

        bool continueProgram = true;
        while (continueProgram)
        {
            Console.WriteLine("\n===== Healthcare Management System =====");
            Console.WriteLine("1. View All Patients");
            Console.WriteLine("2. View Patient Prescriptions");
            Console.WriteLine("3. Exit");
            Console.Write("\nSelect an option (1-3): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        app.PrintAllPatients();
                        break;
                    case 2:
                        Console.Write("\nEnter Patient ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            app.PrintPrescriptionsForPatient(id);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Patient ID entered.");
                        }
                        break;
                    case 3:
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
        Console.WriteLine("Thank you for using the Healthcare Management System!");
    }
}
