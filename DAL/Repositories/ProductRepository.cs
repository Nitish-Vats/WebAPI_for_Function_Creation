using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductRepository
    {
        private SqlConnection con;
        public string ConnectionStr { get; set; }
        private void Connection()
        {
            con = new SqlConnection(ConnectionStr);
        }
        public ProductRepository(string connectionString)
        {
            ConnectionStr = connectionString;
        }

        public bool AddProduct(Product model)
        {
            Connection();
            using (SqlCommand cmd = new SqlCommand("AddNewProductDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@UnitPrice", model.UnitPrice);
                cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                con.Open();
                bool retResult = cmd.ExecuteNonQuery() >= 1;
                con.Close();
                return retResult;
            }
        }
        public bool UpdateProduct(Product model)
        {
            Connection();
            using (SqlCommand cmd = new SqlCommand("UpdateProductDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", model.ProductId);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@UnitPrice", model.UnitPrice);
                cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                con.Open();
                bool retResult = cmd.ExecuteNonQuery() >= 1;
                con.Close();
                return retResult;
            }
        }

        public bool DeleteProduct(int Id)
        {
            Connection();
            using (SqlCommand cmd = new SqlCommand("DeleteProductById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", Id);
                con.Open();
                bool retResult = cmd.ExecuteNonQuery() >= 1;
                con.Close();
                return retResult;
            }
        }
        public async Task<List<Product>> GetProducts()
        {
            Connection();
            List<Product> products = new List<Product>();
            con.Open();
            var command = "Select * from Products";
            using (SqlCommand cmd = new SqlCommand(command, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product prod = new Product();
                    prod.ProductId = Convert.ToInt32(reader["ProductId"]);
                    prod.Name = reader["Name"].ToString();
                    prod.Description = reader["Description"].ToString();
                    prod.UnitPrice = Convert.ToDecimal(reader["UnitPrice"]);
                    prod.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                    products.Add(prod);
                }
            }
            con.Close();
            return products;
        }


        public Product GetProductById(int Id)
        {
            Connection();
            Product prod = new Product();
            con.Open();
            using (SqlCommand cmd = new SqlCommand("usp_getproduct", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", Id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    prod.ProductId = Convert.ToInt32(reader["ProductId"]);
                    prod.Name = reader["Name"].ToString();
                    prod.Description = reader["Description"].ToString();
                    prod.UnitPrice = Convert.ToDecimal(reader["UnitPrice"]);
                    prod.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    prod.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                }
            }

            con.Close();
            return prod;


        }




    }
}
