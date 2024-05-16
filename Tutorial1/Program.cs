using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    class Programme
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Hello World");

            // Prompt user for input
            Console.Out.WriteLine("Enter name:");
            string name = Console.ReadLine();

            Console.Out.WriteLine("Enter age:");
            int age = Convert.ToInt32(Console.ReadLine());

            Console.Out.WriteLine("Enter email:");
            string email = Console.ReadLine();

            Console.Out.WriteLine("Enter address line 1:");
            string address1 = Console.ReadLine();

            Console.Out.WriteLine("Enter address line 2:");
            string address2 = Console.ReadLine();

            Console.Out.WriteLine("Enter postcode:");
            string postcode = Console.ReadLine();

            // Create a Person object and set its properties
            Person person = new Person();
            person.Name = name;
            person.Age = age;
            person.Email = email;
            person.AddressLine1 = address1;
            person.AddressLine2 = address2;
            person.Postcode = postcode;

            // Print all details using the printDetails method
            person.printDetails();
        }
    }


  

}
