﻿using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using VehiclesControl.Domain.Interfaces.Dapper;

namespace VehiclesControl.Data.Repositories.Dapper
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private readonly ISqlToolsProvider _sqlToolsProvider; 
        private readonly IDapperContext _dapperContext; 
        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(ISqlToolsProvider sqlToolsProvider, IDapperContext dapperContext, ILogger<GenericRepository<T>> logger)
        {
            _sqlToolsProvider = sqlToolsProvider;
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                using (IDbConnection sqlConnection = _dapperContext.CreateConnection())
                {
                    string tableName = _sqlToolsProvider.GetTableName<T>();
                    (string? columnName, string propertyName) = _sqlToolsProvider.GetKeyColumnAndPropertyName<T>();

                    var sql = $"DELETE FROM {tableName} WHERE {columnName} = @{propertyName};";
                    var parameters = new DynamicParameters();
                    parameters.Add($"@{propertyName}", id);

                    int result = await sqlConnection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting entity with ID {Id}", id);
                return false;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                using (IDbConnection sqlConnection = _dapperContext.CreateConnection())
                {
                    string tableName = _sqlToolsProvider.GetTableName<T>();
                    Dictionary<string, string> columnNamePropertyNameDict = _sqlToolsProvider.GetColumnAndPropertyNames<T>();

                    string columnNamesPropNames = _sqlToolsProvider.GetFormattedColumnsAndPropertyNames<T>(columnNamePropertyNameDict, "{0} AS {1}");

                    var sql = $"SELECT {columnNamesPropNames} FROM {tableName}";

                    return await sqlConnection.QueryAsync<T>(sql);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all entities.");
                return Enumerable.Empty<T>();
            }
        }

        public async Task<T> GetByIdAsync(long id)
        {
            try
            {
                using (IDbConnection sqlConnection = _dapperContext.CreateConnection())
                {
                    string tableName = _sqlToolsProvider.GetTableName<T>();
                    Dictionary<string, string> columnNamePropertyNameDict = _sqlToolsProvider.GetColumnAndPropertyNames<T>();

                    string aliasedColumnNamesPropNames = _sqlToolsProvider.GetFormattedColumnsAndPropertyNames<T>(columnNamePropertyNameDict, "{0} AS {1}");

                    (string? columnName, string propertyName) = _sqlToolsProvider.GetKeyColumnAndPropertyName<T>();

                    var sql = $"SELECT {aliasedColumnNamesPropNames} FROM {tableName} WHERE {columnName} = @{propertyName}";

                    var parameters = new DynamicParameters();
                    parameters.Add($"@{propertyName}", id);

                    return await sqlConnection.QueryFirstOrDefaultAsync<T>(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving entity with ID {Id}", id);
                return null!;
            }
        }

        public async Task<bool> InsertAsync(T entity)
        {
            try
            {
                using (IDbConnection sqlConnection = _dapperContext.CreateConnection())
                {
                    string tableName = _sqlToolsProvider.GetTableName<T>();
                    Dictionary<string, string> columnNamePropertyNameDict = _sqlToolsProvider.GetColumnAndPropertyNames<T>(key: false);

                    var sql = $"INSERT INTO {tableName} ({string.Join(", ", columnNamePropertyNameDict.Keys)}) " +
                              $"VALUES ({string.Join(", ", columnNamePropertyNameDict.Values.Select(propName => $"@{propName}"))})";

                    var parameters = new DynamicParameters(entity);
                    int rowsAffected = await sqlConnection.ExecuteAsync(sql, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while inserting entity.");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                using (IDbConnection sqlConnection = _dapperContext.CreateConnection())
                {
                    string tableName = _sqlToolsProvider.GetTableName<T>();
                    Dictionary<string, string> columnNamePropertyNameDict = _sqlToolsProvider.GetColumnAndPropertyNames<T>(key: false);

                    (string? columnName, string propertyName) = _sqlToolsProvider.GetKeyColumnAndPropertyName<T>();

                    string aliasedColumnNamesPropNames = _sqlToolsProvider.GetFormattedColumnsAndPropertyNames<T>(columnNamePropertyNameDict, "{0} = @{1}");

                    var sql = $"UPDATE {tableName} SET {aliasedColumnNamesPropNames} WHERE {columnName} = @{propertyName}";

                    var parameters = new DynamicParameters();

                    foreach (var prop in typeof(T).GetProperties())
                    {
                        var propValue = prop.GetValue(entity);

                        // Eğer property DateTime ise, SQL Server'ın kabul ettiği aralığı kontrol edelim
                        if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            var dateTimeValue = (DateTime?)propValue;

                            // Eğer değer null değilse ve minimum tarihten küçükse, değeri null olarak ayarlayalım
                            if (dateTimeValue.HasValue && dateTimeValue.Value < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue)
                            {
                                parameters.Add(prop.Name, null); // DBNull.Value yerine null kullanıyoruz
                            }
                            else
                            {
                                parameters.Add(prop.Name, propValue);
                            }
                        }
                        else
                        {
                            // Eğer değer DBNull ise null olarak ekle
                            if (propValue == DBNull.Value)
                            {
                                parameters.Add(prop.Name, null);
                            }
                            else
                            {
                                parameters.Add(prop.Name, propValue);
                            }
                        }
                    }

                    var keyProperty = typeof(T).GetProperty(propertyName);
                    if (keyProperty != null)
                    {
                        var keyValue = keyProperty.GetValue(entity);
                        parameters.Add(propertyName, keyValue);
                    }

                    int rowsAffected = await sqlConnection.ExecuteAsync(sql, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating entity.");
                return false;
            }

        }
    }
}
