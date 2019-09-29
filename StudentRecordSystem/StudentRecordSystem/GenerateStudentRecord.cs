using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRecordSystem
{
    class GenerateStudentRecord
    {
        public static string connectionString = "Server=LAPTOP-H3HOBEA5\\SQLEXPRESS;Database=student_records;Trusted_Connection=True;";
        public static DateTime localDate = DateTime.Now;
        public static string uid;
        public void generate(string uname)
        {

            SqlConnection conn = new SqlConnection(connectionString);
            /** Retrieve The Id From Student User And Store As 'user_id' In the Student Records Table **/
            using (conn)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE username=@username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", uname);

                    SqlDataReader reader = cmd.ExecuteReader();
                    {
                        if (reader.Read())
                        {
                            uid = String.Format("{0}", reader["id"]);
                            conn.Close();
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("INSERT INTO records (user_id, created_at, updated_at) VALUES (@userId, @localDate, @localDate)", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", uid);
                    cmd.Parameters.AddWithValue("@localDate", localDate);
                    SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                    DataTable dbtl = new DataTable();
                    sqlda.Fill(dbtl);
                }
            }
        }
    }
}
