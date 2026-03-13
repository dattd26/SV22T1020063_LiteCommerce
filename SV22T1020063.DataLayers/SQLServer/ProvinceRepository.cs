using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.Models.DataDictionary;

namespace SV22T1020063.DataLayers.SQLServer
{
    /// <summary>
    /// Repository đọc dữ liệu tỉnh/thành
    /// </summary>
    public class ProvinceRepository : IDataDictionaryRepository<Province>
    {
        private readonly string _connectionString;

        public ProvinceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách tất cả tỉnh/thành
        /// </summary>
        public async Task<List<Province>> ListAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"SELECT ProvinceName
                           FROM Provinces
                           ORDER BY ProvinceName";

            var data = await connection.QueryAsync<Province>(sql);
            return data.ToList();
        }
    }
}