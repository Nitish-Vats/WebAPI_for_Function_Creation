using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CategoryRepository
    {
        private SqlConnection con;
        public string ConnectionStr { get; set; }
        private void Connection()
        {
            con = new SqlConnection(ConnectionStr);
        }
        public CategoryRepository(string connectionString)
        {
            ConnectionStr = connectionString;
        }
        public List<Category> GetCategories()
        {
            Connection();
            List<Category> categories = new List<Category>();
            con.Open();
            var command = "Select * from Categories";
            using (SqlCommand cmd = new SqlCommand(command, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Category cate = new Category();
                    cate.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    cate.Name = reader["Name"].ToString();
                    categories.Add(cate);
                }
            }
            con.Close();
            return categories;
        }
    }
}
