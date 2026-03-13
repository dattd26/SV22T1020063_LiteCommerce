using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Common;
using SV22T1020063.Models.Sales;

namespace SV22T1020063.DataLayers.SQLServer
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(Order data)
        {
            int orderID = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Orders(CustomerID,OrderTime,DeliveryProvince,DeliveryAddress,Status)
                               VALUES(@CustomerID,@OrderTime,@DeliveryProvince,@DeliveryAddress,@Status);
                               SELECT SCOPE_IDENTITY();";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@CustomerID", data.CustomerID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OrderTime", data.OrderTime);
                cmd.Parameters.AddWithValue("@DeliveryProvince", data.DeliveryProvince ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryAddress", data.DeliveryAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (int)data.Status);

                object result = await cmd.ExecuteScalarAsync();
                orderID = Convert.ToInt32(result);
            }
            return orderID;
        }

        public async Task<bool> UpdateAsync(Order data)
        {
            int rows;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Orders 
                               SET CustomerID=@CustomerID,
                                   DeliveryProvince=@DeliveryProvince,
                                   DeliveryAddress=@DeliveryAddress,
                                   EmployeeID=@EmployeeID,
                                   AcceptTime=@AcceptTime,
                                   ShipperID=@ShipperID,
                                   ShippedTime=@ShippedTime,
                                   FinishedTime=@FinishedTime,
                                   Status=@Status
                               WHERE OrderID=@OrderID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@OrderID", data.OrderID);
                cmd.Parameters.AddWithValue("@CustomerID", data.CustomerID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryProvince", data.DeliveryProvince ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryAddress", data.DeliveryAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeID", data.EmployeeID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcceptTime", data.AcceptTime ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipperID", data.ShipperID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippedTime", data.ShippedTime ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FinishedTime", data.FinishedTime ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (int)data.Status);

                rows = await cmd.ExecuteNonQueryAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int orderID)
        {
            int rows;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Orders WHERE OrderID=@OrderID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);

                rows = await cmd.ExecuteNonQueryAsync();
            }
            return rows > 0;
        }

        public async Task<OrderViewInfo?> GetAsync(int orderID)
        {
            OrderViewInfo? data = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Orders WHERE OrderID=@OrderID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    data = new OrderViewInfo()
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        CustomerID = reader["CustomerID"] as int?,
                        OrderTime = Convert.ToDateTime(reader["OrderTime"]),
                        DeliveryProvince = reader["DeliveryProvince"]?.ToString(),
                        DeliveryAddress = reader["DeliveryAddress"]?.ToString(),
                        Status = (OrderStatusEnum)Convert.ToInt32(reader["Status"])
                    };
                }
            }
            return data;
        }

        public async Task<PagedResult<OrderViewInfo>> ListAsync(OrderSearchInput input)
        {
            PagedResult<OrderViewInfo> result = new PagedResult<OrderViewInfo>();

            List<OrderViewInfo> data = new List<OrderViewInfo>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string sqlCount = "SELECT COUNT(*) FROM Orders";
                SqlCommand cmdCount = new SqlCommand(sqlCount, conn);
                result.RowCount = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                string sql = @"SELECT *
                               FROM Orders
                               ORDER BY OrderTime DESC
                               OFFSET @Offset ROWS
                               FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Offset", (input.Page - 1) * input.PageSize);
                cmd.Parameters.AddWithValue("@PageSize", input.PageSize);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    data.Add(new OrderViewInfo()
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        CustomerID = reader["CustomerID"] as int?,
                        OrderTime = Convert.ToDateTime(reader["OrderTime"]),
                        DeliveryProvince = reader["DeliveryProvince"]?.ToString(),
                        DeliveryAddress = reader["DeliveryAddress"]?.ToString(),
                        Status = (OrderStatusEnum)Convert.ToInt32(reader["Status"])
                    });
                }
            }

            result.DataItems = data;
            result.Page = input.Page;
            result.PageSize = input.PageSize;

            return result;
        }

        public async Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID)
        {
            List<OrderDetailViewInfo> data = new List<OrderDetailViewInfo>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM OrderDetails WHERE OrderID=@OrderID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    data.Add(new OrderDetailViewInfo()
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        SalePrice = Convert.ToDecimal(reader["SalePrice"])
                    });
                }
            }

            return data;
        }

        public async Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID)
        {
            OrderDetailViewInfo? data = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM OrderDetails 
                               WHERE OrderID=@OrderID AND ProductID=@ProductID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Parameters.AddWithValue("@ProductID", productID);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    data = new OrderDetailViewInfo()
                    {
                        OrderID = orderID,
                        ProductID = productID,
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        SalePrice = Convert.ToDecimal(reader["SalePrice"])
                    };
                }
            }

            return data;
        }

        public async Task<bool> AddDetailAsync(OrderDetail data)
        {
            int rows;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO OrderDetails(OrderID,ProductID,Quantity,SalePrice)
                               VALUES(@OrderID,@ProductID,@Quantity,@SalePrice)";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@OrderID", data.OrderID);
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@Quantity", data.Quantity);
                cmd.Parameters.AddWithValue("@SalePrice", data.SalePrice);

                rows = await cmd.ExecuteNonQueryAsync();
            }

            return rows > 0;
        }

        public async Task<bool> UpdateDetailAsync(OrderDetail data)
        {
            int rows;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE OrderDetails
                               SET Quantity=@Quantity, SalePrice=@SalePrice
                               WHERE OrderID=@OrderID AND ProductID=@ProductID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@OrderID", data.OrderID);
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@Quantity", data.Quantity);
                cmd.Parameters.AddWithValue("@SalePrice", data.SalePrice);

                rows = await cmd.ExecuteNonQueryAsync();
            }

            return rows > 0;
        }

        public async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            int rows;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"DELETE FROM OrderDetails
                               WHERE OrderID=@OrderID AND ProductID=@ProductID";

                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Parameters.AddWithValue("@ProductID", productID);

                rows = await cmd.ExecuteNonQueryAsync();
            }

            return rows > 0;
        }
    }
}