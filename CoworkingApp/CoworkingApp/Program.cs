using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingApp {
    class Program 
    {
        static string connectionString = "Server=DESKTOP-IB6PI2C;Database=CoworkingDB;Trusted_Connection=True;";

        static void Main(string[] args)
        {
            DisplayTable("Users");
            DisplayTable("Workspaces");
            DisplayTable("Bookings");
            DisplayTable("Payments");
            DisplayTable("Memberships");

            Console.WriteLine();
            AddUser("Alice Johnson", "alicejohnson@example.com", "1122334455", 1);
            DisplayTable("Users");

            Console.WriteLine();
            DisplayJoinQuery();
            DisplayFilteredQuery("Premium");
            DisplayAggregateQuery();

            Console.ReadKey();
        }

        static void DisplayTable(string tableName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                Console.WriteLine($"--- {tableName} ---");
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item}\t");
                    }
                    Console.WriteLine();
                }
            }
        }

        static void AddUser(string name, string email, string phone, int membershipID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Users (Name, Email, Phone, MembershipID) VALUES (@Name, @Email, @Phone, @MembershipID)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@MembershipID", membershipID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static void DisplayJoinQuery()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT u.Name, w.Name AS Workspace, b.BookingDate, b.StartTime, b.EndTime 
                    FROM Bookings b
                    JOIN Users u ON b.UserID = u.UserID
                    JOIN Workspaces w ON b.WorkspaceID = w.WorkspaceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                Console.WriteLine("--- Join Query Results ---");
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item}\t");
                    }
                    Console.WriteLine();
                }
            }
        }

        static void DisplayFilteredQuery(string membershipType)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT u.Name, u.Email, m.Type 
                    FROM Users u
                    JOIN Memberships m ON u.MembershipID = m.MembershipID
                    WHERE m.Type = @Type";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Type", membershipType);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                Console.WriteLine("--- Filtered Query Results ---");
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item}\t");
                    }
                    Console.WriteLine();
                }
            }
        }

        static void DisplayAggregateQuery()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT m.Type, COUNT(u.UserID) AS UserCount
                    FROM Users u
                    JOIN Memberships m ON u.MembershipID = m.MembershipID
                    GROUP BY m.Type";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                Console.WriteLine("--- Aggregate Query Results ---");
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item}\t");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
