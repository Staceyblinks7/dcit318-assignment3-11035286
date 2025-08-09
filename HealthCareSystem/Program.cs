using System;
using System.Collections.Generic;
using System.Linq;

#region Repository<T> - Generic Repository for Entity Management
public class Repository<T>
{
    private List<T> items = new List<T>();

    // Add an item
    public void Add(T item) => items.Add(item);

    // Return a copy of all items
    public List<T> GetAll() => new List<T>(items);

    // Return first match or null
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    // Remove by condition, return true if removed
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
#endregion

#region Domain Models: Patient and Prescription
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString() => $"ID: {Id} | {Name} | Age: {Age} | Gender: {Gender}";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString() => $"Prescription ID: {Id} | Medication: {MedicationName} | Issued: {DateIssued:d}";
}
#endregion

#region HealthSystemApp
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    // Seed sample patients and prescriptions
    public void SeedData()
    {
        // Patients (2-3)
        _patientRepo.Add(new Patient(101, "Alice Smith", 30, "Female"));
        _patientRepo.Add(new Patient(102, "Bob Johnson", 45, "Male"));
        _patientRepo.Add(new Patient(103, "Clara Williams", 29, "Female"));

        // Prescriptions (4-5) with valid PatientIds
        _prescriptionRepo.Add(new Prescription(1, 101, "Amoxicillin 500mg", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 101, "Ibuprofen 200mg", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 102, "Paracetamol 500mg", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(4, 103, "Vitamin C 1000mg", DateTime.Now.AddDays(-7)));
        _prescriptionRepo.Add(new Prescription(5, 102, "Metformin 500mg", DateTime.Now.AddDays(-1)));
    }

    // Build the dictionary map PatientId -> List<Prescription>
    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    // Return prescriptions for a patient (empty list if none)
    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        if (_prescriptionMap.TryGetValue(patientId, out var list))
            return new List<Prescription>(list); // return a copy
        return new List<Prescription>();
    }

    // Print all patients from repository
    public void PrintAllPatients()
    {
        Console.WriteLine("=== All Patients ===");
        var patients = _patientRepo.GetAll();
        if (patients.Count == 0)
        {
            Console.WriteLine("No patients found.");
            return;
        }
        foreach (var p in patients)
            Console.WriteLine(p);
    }

    // Print prescriptions for a specific patient id
    public void PrintPrescriptionsForPatient(int id)
    {
        var patient = _patientRepo.GetById(p => (p as Patient)!.Id == id);
        if (patient == null)
        {
            Console.WriteLine($"Patient with ID {id} not found.");
            return;
        }

        var prescriptions = GetPrescriptionsByPatientId(id);
        Console.WriteLine($"\nPrescriptions for {((Patient)patient).Name} (ID: {id}):");

        if (prescriptions.Count == 0)
        {
            Console.WriteLine("  No prescriptions found for this patient.");
            return;
        }

        foreach (var pres in prescriptions)
            Console.WriteLine($"  - {pres}");
    }
}
#endregion

#region Main Program Flow
public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();

        // i. Seed data
        app.SeedData();

        // ii. Build map
        app.BuildPrescriptionMap();

        // iii. Print all patients
        app.PrintAllPatients();

        // iv. Choose a patient id and display prescriptions
        Console.WriteLine("\nEnter a Patient ID to display their prescriptions (e.g., 101):");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out int patientId))
        {
            app.PrintPrescriptionsForPatient(patientId);
        }
        else
        {
            // If user input is invalid, show prescriptions of the first patient as a fallback
            Console.WriteLine("Invalid input. Displaying prescriptions for Patient ID 101 (fallback).");
            app.PrintPrescriptionsForPatient(101);
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
#endregion
