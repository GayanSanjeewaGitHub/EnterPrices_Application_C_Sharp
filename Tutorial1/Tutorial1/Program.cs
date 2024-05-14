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


    class Person
    {
        // Fields
        String name;
        int age;
        String email;
        String addressline1;
        String addressline2;
        String postcode;

        // Properties using C#'s get and set syntax
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        public String AddressLine1
        {
            get { return addressline1; }
            set { addressline1 = value; }
        }

        public String AddressLine2
        {
            get { return addressline2; }
            set { addressline2 = value; }
        }

        public String Postcode
        {
            get { return postcode; }
            set { postcode = value; }
        }

        // Method to print all details
        public void printDetails()
        {
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("Age: " + Age);
            Console.WriteLine("Email: " + Email);
            Console.WriteLine("Address Line 1: " + AddressLine1);
            Console.WriteLine("Address Line 2: " + AddressLine2);
            Console.WriteLine("Postcode: " + Postcode);
        }
    }

}
