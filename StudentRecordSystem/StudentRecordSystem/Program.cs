using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;

namespace StudentRecordSystem
{
    /// <summary>
    /// Student Record System
    /// </summary>
    class Program
    {
        /** Connection To SQL Server **/
        public static string connectionString = "Server=LAPTOP-H3HOBEA5\\SQLEXPRESS;Database=student_records;Trusted_Connection=True;";
        //public static int counter = 0;
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
        //static void Main(string[] args)
        public void studentRecords()
        {
            /*
             * Before the Admin User can access the Student Record Base, they will need to
             * sign into their account. This can be done in the LoginSystem class.
             */
            //var loginSystemInstance = new LoginSystem();
            //loginSystemInstance.setCredentials();

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
                    /** Create A New Student User **/
                    case "c":
                        createStudentUser();
                        break;
                    /** Edit A Student User's Details **/
                    case "e":
                        Edit();
                        break;
                    /** Delete A Student User **/
                    case "d":
                        Delete();
                        break;
                    /** Read A Student User **/
                    case "r":
                        Read();
                        break;
                    /** View All Student Users (Softdeleted And Non-Deleted) **/
                    case "a":
                        All();
                        break;
                    /** View Only Non-Deleted Student Users **/
                    case "a+":
                        ReadNonDeleted();
                        break;
                    /** View Only Soft-Deleted Student Users **/
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

        /** 
         * Upon Creating A New Student Record, Also Generate
         * A Student Record And A Student Profile.
        **/
        public static void createStudentUser()
        {
            Console.Write("Enter Student First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Student Last Name: ");
            string lname = Console.ReadLine();
            Console.Write("Enter Student Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Student Contact Number: ");
            string contactNumber = Console.ReadLine();

            /** Generate A Username Based On first character of Firstname; concatenating Lastname **/
            char[] fnameChar = fname.ToCharArray();
            string uname = fnameChar[0] + lname;
            /** Randomly Generate A Password **/
            string pword = KeyGenerator.GetUniqueKey();

            Console.WriteLine("Inserted.");

            SqlConnection conn = new SqlConnection(connectionString);

            // When inserting a new record into the table, the id doesn't auto-increment as unique value.
            // For the time being, calculate the amount of rows within the records table and set this counter to the new record id.
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password, firstname, lastname, created_at, updated_at, user_type, email, contact_number) VALUES (@username, @password, @firstname, @lastname, @localDate, @localDate, @type, @email, @contactNumber)", conn);
                cmd.Parameters.AddWithValue("@username", uname.ToLower());
                cmd.Parameters.AddWithValue("@password", pword);
                cmd.Parameters.AddWithValue("@firstname", fname);
                cmd.Parameters.AddWithValue("@lastname", lname);
                cmd.Parameters.AddWithValue("@localDate", localDate);
                cmd.Parameters.AddWithValue("@type", 1);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@contactNumber", contactNumber);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataTable dbtl = new DataTable();
                sqlda.Fill(dbtl);

                conn.Close();
                /** Generate A Student Record For This Student User **/
                var generateSR = new GenerateStudentRecord();
                //generateSR.generate(fname, lname, uname);
                generateSR.generate(uname);

                /** Generate A Student Profile For This Student User **/
                var generateSP = new GenerateStudentProfile();
                generateSP.generate(uname);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
                Console.WriteLine(e.Message);
            }
        }

        /** Edit An Existing Student Record **/
        public static void Edit()
        {
            Console.WriteLine("Enter the record id you would like to edit: ");
            var searchId = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Please specific which column you would like to edit: ");
            var column = Console.ReadLine();

            Console.WriteLine();
            Console.Write("Replace with: ");
            string value = Console.ReadLine();

            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE records SET [" + column + "]=@value WHERE id=@searchId", conn);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@searchId", searchId);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataTable dbtl = new DataTable();
                sqlda.Fill(dbtl);

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
                Console.WriteLine(e.Message);
            }
        }

        /** Delete An Existing Student Record **/
        public static void Delete()
        {
            Console.Write("Enter the record id you would like to delete: ");
            var searchId = Console.ReadLine();

            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE records SET deleted_at=@localDate WHERE id=@searchId", conn);
                cmd.Parameters.AddWithValue("@localDate", localDate);
                cmd.Parameters.AddWithValue("@searchId", searchId);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataTable dtbl = new DataTable();
                sqlda.Fill(dtbl);

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
                Console.WriteLine(e.Message);
            }
        }

        /** Read A Student Record From Database With A Specified ID **/
        public static void Read()
        {
            Console.Write("Enter the record id you would like to read: ");
            var searchId = Console.ReadLine();

            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM records WHERE id=@searchId", conn);
                cmd.Parameters.AddWithValue("@searchId", searchId);

                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);

                DataTable dtbl = new DataTable();
                sqlda.Fill(dtbl);

                foreach (DataRow row in dtbl.Rows)
                {
                    Console.WriteLine(String.Format("|{0,3}|{1,6}|{2,6}|{3,5}|{4,6}|{5,6}|{6,6}", row["id"], row["first_name"], row["last_name"], row["grade"], row["created_at"], row["updated_at"], row["deleted_at"]));
                }

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
                Console.WriteLine(e.Message);
            }
        }

        /** Only Read Records Which Haven't Been Soft-Deleted **/
        public static void ReadNonDeleted()
        {
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM records WHERE deleted_at IS NULL", conn);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataTable dbtl = new DataTable();
                sqlda.Fill(dbtl);
                foreach (DataRow row in dbtl.Rows)
                {
                    Console.WriteLine(String.Format("|{0,3}|{1,6}|{2,6}|{3,5}|{4,6}|{5,6}|{6,6}", row["id"], row["first_name"], row["last_name"], row["grade"], row["created_at"], row["updated_at"], row["deleted_at"]));
                }

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
                Console.WriteLine(e.Message);
            }
        }

        /** Only Read Records Which Have Been Soft-Deleted **/
        public static void ReadSoftDeleted()
        {
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM records WHERE deleted_at IS NOT NULL", conn);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataTable dbtl = new DataTable();
                sqlda.Fill(dbtl);
                foreach (DataRow row in dbtl.Rows)
                {
                    Console.WriteLine(String.Format("|{0,3}|{1,6}|{2,6}|{3,5}|{4,6}|{5,6}|{6,6}", row["id"], row["first_name"], row["last_name"], row["grade"], row["created_at"], row["updated_at"], row["deleted_at"]));
                }

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
                Console.WriteLine(e.Message);
            }
        }

        /** Read All Student Records From Database **/
        public static void All()
        {
            Console.WriteLine(string.Format("|{0,3}|{1,6}|{2,6}|{3,5}|{4,6}|{5,6}", "First Name", "Last Name", "Grade", "Created At", "Updated At", "Deleted At"));
            Console.WriteLine("------------------------------");

            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                SqlCommand cmd = new SqlCommand("" +
                    "SELECT users.firstname, users.lastname, records.grade, users.created_at, users.updated_at, users.deleted_at " +
                    "FROM users " +
                    "INNER JOIN records ON users.id = records.user_id"
                    ,conn);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataTable dtbl = new DataTable();
                sqlda.Fill(dtbl);
                foreach (DataRow row in dtbl.Rows)
                {
                    Console.WriteLine(String.Format("|{0,3}|{1,6}|{2,6}|{3,5}|{4,5}|{5,5}", row["firstname"], row["lastname"], row["grade"], row["created_at"], row["updated_at"], row["deleted_at"]));
                }

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Connection.");
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
