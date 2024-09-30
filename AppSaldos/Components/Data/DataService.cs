using System.Data;
using System.Dynamic;
using Microsoft.Data.SqlClient;

namespace AppSaldos.Data;

public class DataService
{
    private readonly IConfiguration _configuration;

    public DataService(IConfiguration configuration) => _configuration = configuration;

    public async Task<List<ExpandoObject>> GetDataAsync(string query)
    {
        try
        {
            //var connectionString = _configuration.GetConnectionString("BusinessConnection");
            var connectionString = _configuration.GetSection("ConnectionStrings:BusinessConnection").Value;
            var resultList = new List<ExpandoObject>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dynamic expandoObject = new ExpandoObject();
                        var dataRow = expandoObject as IDictionary<string, object>;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            dataRow.Add(columnName, value);
                        }

                        resultList.Add(expandoObject);
                    }
                }
            }

            return resultList;
        }
        catch (Exception w)
        {
            Console.WriteLine("error GetDataAsync:" + w);
            return null;
        }
    }

    public async Task<List<ExpandoObject>> ExecuteStoredProcedureAsync(string storedProcedureName, Dictionary<string, object> parameters)
    {
        try
        {
            var connectionString = _configuration.GetSection("ConnectionStrings:SiaAppConnection").Value;
            var resultList = new List<ExpandoObject>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    // Agregar parámetros al comando
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dynamic expandoObject = new ExpandoObject();
                            var dataRow = expandoObject as IDictionary<string, object>;

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                dataRow.Add(columnName, value);
                            }

                            resultList.Add(expandoObject);
                        }
                    }
                }
            }

            return resultList;
        }
        catch (Exception w)
        {
            Console.WriteLine("error ExecuteStoredProcedureAsync:" + w);
            return null;
        }
    }


}