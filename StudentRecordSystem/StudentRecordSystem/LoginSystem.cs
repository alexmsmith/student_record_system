using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRecordSystem
{
    class LoginSystem
    {
        public static string connectionString = "Server=LAPTOP-H3HOBEA5\\SQLEXPRESS;Database=student_records;Trusted_Connection=True;";

        public static string usernameInput;
        public static string passwordInput;

        static void Main(string[] args)
        {
            bool loggedOut = true;
            /** This do-while loop will only break for successful login attempt **/
            do
            {
                Console.Write("Please Enter Your Username: ");
                usernameInput = Console.ReadLine();
                Console.Clear();

                Console.Write("Please Enter Your Password: ");
                passwordInput = Console.ReadLine();
                Console.Clear();

                SqlConnection conn = new SqlConnection(connectionString);

                conn.Open();

                // When inserting a new record into the table, the id doesn't auto-increment as unique value.
                // For the time being, calculate the amount of rows within the records table and set this counter to the new record id.
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE username=@username", conn);
                    cmd.Parameters.AddWithValue("@username", usernameInput);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check input credentials against this record's password
                            string password = String.Format("{0}", reader["password"]);
                            if (password == passwordInput)
                            {
                                loggedOut = false;

                                //Console.WriteLine(String.Format("{0}", reader["username"]));
                                string userType = String.Format("{0}", reader["user_type"]);

                                /** If the student user attempts to log in successfully,
                                 *  re-direct to their student profile. Otherwise, message the user to try again.
                                 *  user_type 1 = student.
                                 */
                                if (userType == "1")
                                {
                                    Console.WriteLine("Hello Student " + reader["username"] + "!");

                                    var profileInstance = new StudentProfile();
                                    profileInstance.profile();
                                    conn.Close();
                                }

                                /** If an admin user attempts to log in succesfully,
                                 * re-direct to the student records. Otherwise, message the user to try again.
                                 * user_type 0 = admin.
                                 */
                                if (userType == "0")
                                {
                                    Console.WriteLine("Hello Admin " + reader["username"] + "!");

                                    conn.Close();
                                    var recordsInstance = new Program();
                                    recordsInstance.studentRecords();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Incorrect Password.");
                            }
                        }
                        else
                        {
                            // If the select query doesn't find the record.
                            Console.WriteLine("Record Not Found.");
                        }
                    }

                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could Not Open Connection.");
                    Console.WriteLine(e.Message);
                }
            } while (loggedOut);
            
        }

        public string getCredentials()
        {
            return usernameInput + " " + passwordInput;
        }
    }
}
