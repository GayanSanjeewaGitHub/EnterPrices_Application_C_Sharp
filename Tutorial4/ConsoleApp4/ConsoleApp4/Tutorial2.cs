using HelloWorld;
using System.Collections.Generic;
using static System.Console;

public class Tutorial2
{
    public void OutputInformation2(object person)
    {
        if (person is Student)
        {
            Student student = (Student)person;
            Console.WriteLine($"Student {student.Name} {student.LastName} is enrolled for courses {String.Join(", ", student.CourseCodes)}");
        }
        else if (person is Professor)
        {
            Professor prof = (Professor)person;
            Console.WriteLine($"Professor {prof.Name} {prof.LastName} teaches {String.Join(",", prof.TeachesSubjects)}");
        }
        else if (person is null)
        {
            Console.WriteLine($"Object {nameof(person)} is null");
        }
    }

    public void OutputInformation(object person)
    {
        switch (person)
        {
            case Student student when student.CourseCodes.Contains(203):
                WriteLine($"Student {student.Name} {student.LastName} is enrolled for course 203.");
                break;
            case Student student:
                WriteLine($"Student {student.Name} {student.LastName} is enrolled for courses {String.Join(", ", student.CourseCodes)}");
                break;
            case Professor prof:
                WriteLine($"Professor {prof.Name} {prof.LastName} teaches {String.Join(",", prof.TeachesSubjects)}");
                break;
            case null:
                WriteLine($"Object {nameof(person)} is null");
                break;
            default:
                WriteLine("Unknown object detected");
                break;
        }
    }
}
