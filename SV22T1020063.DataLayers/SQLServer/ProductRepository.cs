using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Models.Common;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt repository thao tác dữ liệu với bảng Products,
    /// ProductAttributes và ProductPhotos trong SQL Server.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo ProductRepository
        /// </summary>
        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(Product data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID,
                                                 Unit, Price, Photo, IsSelling)
                           VALUES (@ProductName,@ProductDescription,@SupplierID,@CategoryID,
                                   @Unit,@Price,@Photo,@IsSelling);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, data);
        }

        public async Task<bool> UpdateAsync(Product data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Products
                           SET ProductName=@ProductName,
                               ProductDescription=@ProductDescription,
                               SupplierID=@SupplierID,
                               CategoryID=@CategoryID,
                               Unit=@Unit,
                               Price=@Price,
                               Photo=@Photo,
                               IsSelling=@IsSelling
                           WHERE ProductID=@ProductID";

            return await connection.ExecuteAsync(sql, data) > 0;
        }

        public async Task<bool> DeleteAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"DELETE FROM Products WHERE ProductID=@productID";
            return await connection.ExecuteAsync(sql, new { productID }) > 0;
        }

        public async Task<Product?> GetAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT * FROM Products WHERE ProductID=@productID";

            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { productID });
        }

        public async Task<bool> IsUsedAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT COUNT(*) FROM OrderDetails WHERE ProductID=@productID";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { productID });
            return count > 0;
        }

        public async Task<PagedResult<Product>> ListAsync(ProductSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = new PagedResult<Product>()
            {
                Page = input.Page,
                PageSize = input.PageSize
            };

            string searchValue = $"%{input.SearchValue}%";

            string where = @"WHERE (ProductName LIKE @searchValue)";

            if (input.CategoryID != 0)
                where += " AND CategoryID = @CategoryID";

            if (input.SupplierID != 0)
                where += " AND SupplierID = @SupplierID";

            if (input.MinPrice > 0)
                where += " AND Price >= @MinPrice";

            if (input.MaxPrice > 0)
                where += " AND Price <= @MaxPrice";

            string countSql = $"SELECT COUNT(*) FROM Products {where}";

            result.RowCount = await connection.ExecuteScalarAsync<int>(countSql, new
            {
                searchValue,
                input.CategoryID,
                input.SupplierID,
                input.MinPrice,
                input.MaxPrice
            });

            if (result.RowCount > 0)
            {
                string sql;

                if (input.PageSize == 0)
                {
                    sql = $@"SELECT * FROM Products
                             {where}
                             ORDER BY ProductName";
                }
                else
                {
                    sql = $@"SELECT * FROM Products
                             {where}
                             ORDER BY ProductName
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                }
                Console.WriteLine(sql);
                var data = await connection.QueryAsync<Product>(sql, new
                {
                    searchValue,
                    input.CategoryID,
                    input.SupplierID,
                    input.MinPrice,
                    input.MaxPrice,
                    Offset = input.Offset,
                    input.PageSize
                });

                result.DataItems = data.ToList();
            }

            return result;
        }

        public async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT * FROM ProductAttributes
                           WHERE ProductID=@productID
                           ORDER BY DisplayOrder";

            var data = await connection.QueryAsync<ProductAttribute>(sql, new { productID });
            return data.ToList();
        }

        public async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT * FROM ProductAttributes
                           WHERE AttributeID=@attributeID";

            return await connection.QueryFirstOrDefaultAsync<ProductAttribute>(sql, new { attributeID });
        }

        public async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                           VALUES(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                           SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, data);
        }

        public async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE ProductAttributes
                           SET AttributeName=@AttributeName,
                               AttributeValue=@AttributeValue,
                               DisplayOrder=@DisplayOrder
                           WHERE AttributeID=@AttributeID";

            return await connection.ExecuteAsync(sql, data) > 0;
        }

        public async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"DELETE FROM ProductAttributes WHERE AttributeID=@attributeID";

            return await connection.ExecuteAsync(sql, new { attributeID }) > 0;
        }

        public async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT * FROM ProductPhotos
                           WHERE ProductID=@productID
                           ORDER BY DisplayOrder";

            var data = await connection.QueryAsync<ProductPhoto>(sql, new { productID });
            return data.ToList();
        }

        public async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"SELECT * FROM ProductPhotos
                           WHERE PhotoID=@photoID";

            return await connection.QueryFirstOrDefaultAsync<ProductPhoto>(sql, new { photoID });
        }

        public async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"INSERT INTO ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                           VALUES(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);
                           SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, data);
        }

        public async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE ProductPhotos
                           SET Photo=@Photo,
                               Description=@Description,
                               DisplayOrder=@DisplayOrder,
                               IsHidden=@IsHidden
                           WHERE PhotoID=@PhotoID";

            return await connection.ExecuteAsync(sql, data) > 0;
        }

        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"DELETE FROM ProductPhotos WHERE PhotoID=@photoID";

            return await connection.ExecuteAsync(sql, new { photoID }) > 0;
        }
    }
}
