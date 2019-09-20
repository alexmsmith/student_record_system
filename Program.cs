using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StudentRecordSystem
{
    /// <summary>
    /// Student Record System
    /// </summary>
    class Program
    {
        public static string path = @"C:\Users\alexm\Documents\records.csv";
        public static int counter = 0;
        public static DateTime localDate = DateTime.Now;
        struct Student
        {
            public int Id;
            public string FirstName;
            public string LastName;
            public char Grade;

            public Student(int id, string fname, string lname, char grade)
            {
                Id = id;
                FirstName = fname;
                LastName = lname;
                Grade = grade;
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome To The Student Record Base.");
            Console.WriteLine("For Information On Commands, Use 'h' To List The Descriptions.");
            Console.WriteLine();
            bool exit = false;
            do
            {
                Console.WriteLine();
                Console.Write("What would you like to do? : ");
                string input = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine();

                switch (input)
                {
                    case "i":
                        Insert();
                        break;
                    case "e":
                        Edit();
                        break;
                    case "d":
                        Delete();
                        break;
                    case "r":
                        Read();
                        break;
                    // View all records (softdeleted and non-deleted).
                    case "a":
                        All();
                        break;
                    // View only non-deleted records.
                    case "a+":
                        ReadNonDeleted();
                        break;
                    // View only soft-deleted records.
                    case "a-":
                        ReadSoftDeleted();
                        break;
                    case "h":
                        Help();
                        break;
                    case "z":
                        exit = true;
                        break;
                }
            } while (exit == false);
        }

        // Insert a new student record.
        public static void Insert()
        {
            Console.Write("Enter Student First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Student Last Name: ");
            string lname = Console.ReadLine();
            Console.Write("Enter Student Grade: ");
            char grade = Console.ReadKey().KeyChar;

            Console.WriteLine("Inserted.");

            // Increment counter when a record has been inserted.
            ++counter;

            try
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(counter + "," + fname + "," + lname + "," + grade + "," + localDate + "," + localDate + "," + "(Null)?");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        // Edit an existing student record.
        // Takes in a student id argument.
        public static void Edit()
        {
            Console.WriteLine("Enter the record id you would like to edit: ");
            var searchId = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Please specific which column you would like to edit: ");
            Console.WriteLine("Firstname - 1 : Lastname - 2 : Grade - 3:");
            var columnNumber = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Replace with: ");
            var columnNewValue = Console.ReadLine();

            // Line that we need to edit (found through searchId input).
            string record = "";
            StringBuilder sb = new StringBuilder();

            // Read the individual lines of the .csv file.
            try
            {
                StreamReader sr = new StreamReader(path);

                string line;
                // Loop through each of these lines
                while ((line = sr.ReadLine()) != null) {
                    // If id column matches search id, delete that record
                    if (line.Split(',')[0] == searchId)
                    {
                        // Assign this record to b.
                        record = line;
                    }
                    // Otherwise, append the remaining records to the string builder.
                    else
                    {
                        sb.Append(line);
                    }
                }

                sr.Close();

                // When the record has been updated, we want to update the timestamp of the 'updated' column for that record.

                // Selected column original value to edit in record.
                string columnBefore = record.Split(',')[int.Parse(columnNumber)];

                StreamWriter sw = new StreamWriter(path);
                // Loop through all columns until column to edit, skip this and process to appending the stringbuilder with other columns in loop.
                for (var i = 0; i <= 6; i++)
                {
                    if (i == int.Parse(columnNumber))
                    {
                        sb.Append(columnNewValue + ",");
                    }
                    // Give 'updated' edit record timestamp.
                    else if (i == 5)
                    {
                        sb.Append(localDate + ",");
                    } 
                    // Remove trailing comma of string builder.
                    else if (i == 6)
                    {
                        sb.Append(record.Split(',')[i]);
                    }
                    else
                    {
                        sb.Append(record.Split(',')[i] + ",");
                    }
                }

                // We want to avoid spliting the last record, because this will cause a blank record to begin.
                string[] o = sb.ToString().Split('?');

                for (var i = 0; i < o.Count() - 1; i++)
                {
                    sw.WriteLine(o[i] + '?');
                }
                sw.Close();

            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }

        // Delete an existing student record.
        // Takes in a student id argument.
        public static void Delete()
        {
            Console.Write("Enter the record id you would like to delete: ");
            var searchId = Console.ReadLine();

            // This will store the line which needs to be soft-deleted.
            string record = "";
            StringBuilder sb = new StringBuilder();

            // Read the individual lines of the .csv file.
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                // Loop through each of these lines.
                while ((line = sr.ReadLine()) != null)
                {
                    // If id column matches search id, delete that record.
                    if (line.Split(',')[0] == searchId)
                    {
                        record = line;
                    }
                    else
                        sb.Append(line);
                        continue;
                }
            }
            // Check if record has already been soft-deleted
            if (record.Split(',')[6] != "(Null)?")
                Console.WriteLine("This record has already been soft-deleted.");
            else
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        for (var i = 0; i <= 5; i++)
                        {
                            sb.Append(record.Split(',')[i] + ",");
                        }
                        sb.Append(localDate);

                        string[] o = sb.ToString().Split('?');
                        foreach (var w in o)
                        {
                            sw.WriteLine(w + '?');
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        // Read an existing student record.
        // Takes in a student id argument.
        public static void Read()
        {
            Console.Write("Enter the record id you would like to read: ");
            var searchId = Console.ReadLine();

            // Run an exception, to check if the .csv file can be read.
            try
            {
                var strLines = File.ReadLines(path);
                foreach (var line in strLines)
                {
                    if (line.Split(',')[0].Equals(searchId))
                        Console.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        // Only read records which haven't been soft-deleted.
        public static void ReadNonDeleted()
        {
            try
            {
                StreamReader all = new StreamReader(File.OpenRead(path));

                string line;
                // Loop through each of these lines.
                while ((line = all.ReadLine()) != null)
                {
                    if (line.Split(',')[6] == "(Null)?")
                        Console.WriteLine("LINE: " + line);
                    else
                        continue;
                }
                all.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        // Only read records which have been soft-deleted.
        public static void ReadSoftDeleted()
        {
            try
            {
                StreamReader all = new StreamReader(File.OpenRead(path));

                string line;
                // Skips over the first line, since it contains columns names.
                all.ReadLine();
                // Loop through each of these lines.
                while ((line = all.ReadLine()) != null)
                {
                    if (line.Split(',')[6] != "(Null)?")
                        Console.WriteLine("LINE: " + line);
                    else
                        continue;
                }
                all.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        // Read all student records.
        public static void All()
        {
            try
            {
                StreamReader all = new StreamReader(File.OpenRead(path));

                while (!all.EndOfStream)
                {
                    string line = all.ReadLine();
                    //Console.WriteLine("LINE: " + line);
                    string[] l = line.Split('?');

                    string la = l[0];
                    string[] column = la.Split(',');
                    Console.WriteLine(String.Format("|{0,3}|{1,6}|{2,6}|{3,5}|{4,6}|{5,6}|{6,6}", column[0], column[1].PadRight(25), column[2].PadRight(25), column[3], column[4].PadRight(20), column[5].PadRight(20), column[6].PadRight(20)));
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------");
                }

                all.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        // This is a helper that will return a list of terminal commands.
        public static void Help()
        {
            Console.WriteLine("This is the helper. Below are a list of commands with a given description:");
            Console.WriteLine();
            Console.WriteLine("a : 'View All Records'");
            Console.WriteLine("a+ : 'View Only Non-Deleted Records'");
            Console.WriteLine("a- : 'View Only Soft-Deleted Records'");
            Console.WriteLine("i : 'Insert New Record Into CSV.'");
            Console.WriteLine("d : 'Deleted A Record With A Specified ID'");
            Console.WriteLine("e : 'Edit A Record With A Specified ID'");
            Console.WriteLine("r : 'Read A Record With A Specified ID'");
            Console.WriteLine("z : 'Exit Program'");
        }
    }
}
