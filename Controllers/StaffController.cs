using MySql.Data.MySqlClient;
using Sales_user.Models;
using System;
using System.Data;

namespace Sales_user.Controllers
{
    public class StaffController
    {
        public Staff Login(string email, string password)
        {
            string sql = @"SELECT staffID, username, firstName, lastName, title, department, email, status
                           FROM Staff
                           WHERE email = @email AND password = @password AND (status IS NULL OR status = 1)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@email", email),
                new MySqlParameter("@password", password)
            };

            DataTable dt = DatabaseConnect.ExecuteQuery(sql, parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Staff
                {
                    StaffID = Convert.ToInt64(row["staffID"]),
                    Username = row["username"].ToString(),
                    FirstName = row["firstName"].ToString(),
                    LastName = row["lastName"].ToString(),
                    Title = row["title"].ToString(),
                    Department = row["department"].ToString(),
                    Email = row["email"].ToString(),
                    Status = row["status"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["status"])
                };
            }

            return null;
        }

        public DataTable GetAllStaff()
        {
            string sql = @"SELECT staffID AS 'Staff ID',
                                  username AS 'Username',
                                  CONCAT(firstName, ' ', lastName) AS 'Name',
                                  title AS 'Title',
                                  department AS 'Department',
                                  email AS 'Email',
                                  phone AS 'Phone',
                                  employDate AS 'Employ Date',
                                  status AS 'Status'
                           FROM Staff
                           ORDER BY employDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public long Insert(Staff staff)
        {
            string sql = @"INSERT INTO Staff
                (username, password, title, department, firstName, lastName, employDate, phone, email, status)
                VALUES (@user, @pass, @title, @dept, @first, @last, @employDate, @phone, @email, @status)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@user", staff.Username),
                new MySqlParameter("@pass", staff.Password ?? ""),
                new MySqlParameter("@title", staff.Title),
                new MySqlParameter("@dept", staff.Department),
                new MySqlParameter("@first", staff.FirstName),
                new MySqlParameter("@last", staff.LastName),
                new MySqlParameter("@employDate", staff.EmployDate),
                new MySqlParameter("@phone", staff.Phone),
                new MySqlParameter("@email", staff.Email),
                new MySqlParameter("@status", staff.Status ?? 1)
            });
        }
    }
}
