using Microsoft.AspNetCore.Mvc;
using Vertica.Data.VerticaClient;
using GetDataAPI.Models;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
public class DataController : ControllerBase
{
    // Connection string to your Vertica database
    private readonly string connectionString = "Host=10.10.4.231;Database=test;Port=5433;User=bootcamp2;Password=bootcamp22023";

    [HttpPost]
    public IActionResult GetData([FromBody] DataRequestModel request)
    {
        try
        {
            // Extract values from the request model
            string TableName = request.timeFilter;
            string NE = request.neRadio;
            DateTime DateFrom = request.dateFrom;
            DateTime DateTo = request.dateTo;
            
     

            // SQL query to select data from the table
            string query = $"SELECT   {NE} , RSL_INPUT_POWER, MAX_RX_LEVEL , RSL_DEVIATION, Time FROM {TableName}  WHERE Time BETWEEN '{DateFrom}' AND '{DateTo}'  ";

            // Create a list to store the results
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            // Open a connection to the Vertica database
            using (VerticaConnection connection = new VerticaConnection(connectionString))
            {
                connection.Open();

                // Execute the SQL query
                using (VerticaCommand command = new VerticaCommand(query, connection))
                {
                    using (VerticaDataReader reader = command.ExecuteReader())
                    {
                        // Iterate through the result set
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            // Iterate through each column in the result set
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                // Add column name and value to the dictionary
                                row[reader.GetName(i)] = reader[i];
                            }

                            // Add the row to the results list
                            results.Add(row);
                        }
                    }
                }
            }

            // Return the results as JSON
            return Ok(results);
        }
        catch (Exception ex)
        {
            // Handle exceptions and return an error response
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
