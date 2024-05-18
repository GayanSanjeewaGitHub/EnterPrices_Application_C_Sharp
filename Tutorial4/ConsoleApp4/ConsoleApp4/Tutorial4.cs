using System.Collections.Generic;


using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
   public class Tutorial4
    {
        static void Main(string[] args) {
            Tutorial2 t2 = new Tutorial2();
            Student student = new Student(); 
            student.Name = "Dirk"; 
            student.LastName = "Strauss";
            student.CourseCodes = new List<int> { 203, 202, 101 };
            
            t2.OutputInformation(student); 
            
            
            Professor prof = new Professor();
            prof.Name = "Reinhardt"; 
            prof.LastName = "Botha"; 
            prof.TeachesSubjects = new List<string> { "Mobile Development", "Cryptography" }; 
            t2.OutputInformation(prof);
            t2.OutputInformation(null );




        }



    }

}