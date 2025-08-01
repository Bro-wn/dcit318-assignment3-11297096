using System;
using System.Collections.Generic;
using System.IO;

class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Mark { get; set; }
    public string Grade { get; set; }

    public Student(int id, string name, int mark)
    {
        Id = id;
        Name = name;
        Mark = mark;
        Grade = AssignGrade(mark);
    }

    private string AssignGrade(int mark)
    {
        return mark switch
        {
            >= 80 => "A",
            >= 70 => "B",
            >= 60 => "C",
            >= 50 => "D",
            _ => "F",
        };
    }

    public override string ToString()
    {
        return $"{Id},{Name},{Mark},{Grade}";
    }
}

class StudentGradingSystem
{
    public List<Student> ReadFromFile(string inputPath)
    {
        var students = new List<Student>();
        try
        {
            string[] lines = File.ReadAllLines(inputPath);
            foreach (var line in lines)
            {
                try
                {
                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        throw new FormatException("Line must contain exactly 3 values.");

                    int id = int.Parse(parts[0]);
                    string name = parts[1];
                    int mark = int.Parse(parts[2]);

                    if (mark < 0 || mark > 100)
                        throw new ArgumentOutOfRangeException("Mark must be between 0 and 100.");

                    students.Add(new Student(id, name, mark));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipping invalid line: \"{line}\" - Reason: {ex.Message}");
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"File \"{inputPath}\" not found.");
        }
        return students;
    }

    public void WriteToFile(string outputPath, List<Student> students)
    {
        try
        {
            var lines = new List<string>();
            foreach (var s in students)
                lines.Add(s.ToString());

            File.WriteAllLines(outputPath, lines);
            Console.WriteLine($"Output written to {outputPath}");
        }
        catch (IOException ex)
        {
            Console.WriteLine("File write error: " + ex.Message);
        }
    }

    public void Process(string inputPath, string outputPath)
    {
        var students = ReadFromFile(inputPath);
        WriteToFile(outputPath, students);
    }
}

class Program
{
    static void Main()
    {
        var system = new StudentGradingSystem();
        List<Student> students = new();
        bool continueProgram = true;

        while (continueProgram)
        {
            Console.WriteLine("\n===== Student Grading System =====");
            Console.WriteLine("1. Load Student Data from File");
            Console.WriteLine("2. View All Students");
            Console.WriteLine("3. Save Results to File");
            Console.WriteLine("4. Exit");
            Console.Write("\nSelect an option (1-4): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter input file path: ");
                        string inputPath = Console.ReadLine() ?? "students_input.txt";
                        students = system.ReadFromFile(inputPath);
                        if (students.Count > 0)
                            Console.WriteLine($"Loaded {students.Count} student records.");
                        break;

                    case 2:
                        if (students.Count == 0)
                            Console.WriteLine("No student data loaded. Please load data first.");
                        else
                        {
                            Console.WriteLine("\nStudent Records:");
                            Console.WriteLine("ID\tName\t\tMark\tGrade");
                            Console.WriteLine("----------------------------------------");
                            foreach (var student in students)
                                Console.WriteLine($"{student.Id}\t{student.Name,-15}\t{student.Mark}\t{student.Grade}");
                        }
                        break;

                    case 3:
                        if (students.Count == 0)
                            Console.WriteLine("No student data to save. Please load data first.");
                        else
                        {
                            Console.Write("Enter output file path: ");
                            string outputPath = Console.ReadLine() ?? "students_output.txt";
                            system.WriteToFile(outputPath, students);
                        }
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
        Console.WriteLine("Thank you for using the Student Grading System!");
    }
}
