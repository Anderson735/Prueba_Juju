using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class CustomerService
    {
        private readonly string _connectionString;

        public CustomerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Development");
        }

        public bool CustomerExists(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Customer WHERE CustomerId = @CustomerId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }
        public void DeleteCustomerAndPosts(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Incluir transacción para asegurar que ambas operaciones (eliminación de posts y cliente) sean exitosas
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Eliminar todos los posts asociados al cliente
                    string deletePostsQuery = "DELETE FROM Post WHERE CustomerId = @CustomerId";
                    SqlCommand deletePostsCommand = new SqlCommand(deletePostsQuery, connection, transaction);
                    deletePostsCommand.Parameters.AddWithValue("@CustomerId", customerId);
                    deletePostsCommand.ExecuteNonQuery();

                    // Luego de eliminar los posts, eliminar el cliente
                    string deleteCustomerQuery = "DELETE FROM Customer WHERE CustomerId = @CustomerId";
                    SqlCommand deleteCustomerCommand = new SqlCommand(deleteCustomerQuery, connection, transaction);
                    deleteCustomerCommand.Parameters.AddWithValue("@CustomerId", customerId);
                    deleteCustomerCommand.ExecuteNonQuery();

                    // Commit de la transacción si todo ha sido exitoso
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // En caso de error, hacer rollback de la transacción
                    Console.WriteLine($"Error al eliminar cliente y posts: {ex.Message}");
                    transaction.Rollback();
                    throw; // Relanza la excepción para manejarla en el código que llama a este método
                }
            }
        }
    }
}
