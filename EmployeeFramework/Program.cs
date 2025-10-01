using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;
using EmployeeFramework.Application.Services;
using EmployeeFramework.Application.DTOs;
using EmployeeFramework.Infrastructure.Repositories;
using EmployeeFramework.Documentation;

namespace EmployeeFramework
{
    internal class Program
    {
        private static HttpListener listener;
        private static IEmployeeService employeeService;

        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:8080";
            
            var employeeRepository = new InMemoryEmployeeRepository();
            employeeService = new EmployeeService(employeeRepository);

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(baseAddress + "/");
                listener.Start();

                Console.WriteLine($"Employee Management API running at {baseAddress}");
                Console.WriteLine($"Swagger UI: {baseAddress}/swagger");
                Console.WriteLine("Press CTRL+C to stop...");

                while (listener.IsListening)
                {
                    var context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Run as Administrator required.");
                Console.ReadLine();
            }
            finally
            {
                listener?.Stop();
            }
        }

        private static async void ProcessRequest(object state)
        {
            var context = (HttpListenerContext)state;
            var request = context.Request;
            var response = context.Response;

            try
            {
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                var path = request.Url.AbsolutePath.ToLower();
                var method = request.HttpMethod.ToUpper();
                string responseString = "";

                if (path == "/swagger" || path == "/swagger/")
                {
                    response.ContentType = "text/html";
                    responseString = SwaggerDocumentGenerator.GenerateSwaggerUI("http://localhost:8080");
                }
                else if (path == "/swagger.json")
                {
                    response.ContentType = "application/json";
                    responseString = SwaggerDocumentGenerator.GenerateSwaggerJson("http://localhost:8080");
                }
                else if (path.StartsWith("/api/employees"))
                {
                    response.ContentType = "application/json";
                    responseString = await HandleEmployeeRequest(method, path, request);
                }
                else if (path == "/" || path == "")
                {
                    response.ContentType = "text/html";
                    responseString = GenerateWelcomePage();
                }
                else
                {
                    response.StatusCode = 404;
                    response.ContentType = "application/json";
                    responseString = SimpleJsonSerializer.Serialize(new { error = "Not found" });
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    response.StatusCode = 500;
                    response.ContentType = "application/json";
                    var errorResponse = SimpleJsonSerializer.Serialize(new { error = ex.Message });
                    byte[] buffer = Encoding.UTF8.GetBytes(errorResponse);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                }
                catch
                {
                    response.Close();
                }
            }
        }

        private static string GenerateWelcomePage()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <title>Employee Management API</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
        .container { max-width: 800px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        h1 { color: #333; border-bottom: 3px solid #007acc; padding-bottom: 10px; }
        .btn { display: inline-block; background: #007acc; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 10px 5px 0 0; }
        .btn:hover { background: #005a9e; }
    </style>
</head>

</html>";
        }

        private static async Task<string> HandleEmployeeRequest(string method, string path, HttpListenerRequest request)
        {
            var pathParts = path.Split('/').Where(p => !string.IsNullOrEmpty(p)).ToArray();
            Guid? employeeId = null;

            if (pathParts.Length > 2)
            {
                if (Guid.TryParse(pathParts[2], out var id))
                {
                    employeeId = id;
                }
            }

            try
            {
                switch (method)
                {
                    case "GET":
                        if (employeeId.HasValue)
                        {
                            var employee = await employeeService.GetByIdAsync(employeeId.Value);
                            if (employee == null)
                            {
                                return SimpleJsonSerializer.Serialize(new { error = "Employee not found" });
                            }
                            return SimpleJsonSerializer.Serialize(employee);
                        }
                        else
                        {
                            var queryString = request.Url.Query;
                            int page = 1, pageSize = 10;
                            
                            if (!string.IsNullOrEmpty(queryString))
                            {
                                var queryParams = ParseQueryString(queryString);
                                if (queryParams.ContainsKey("page"))
                                {
                                    int.TryParse(queryParams["page"], out page);
                                }
                                if (queryParams.ContainsKey("pagesize"))
                                {
                                    int.TryParse(queryParams["pagesize"], out pageSize);
                                }
                            }

                            if (page <= 0) page = 1;
                            if (pageSize <= 0 || pageSize > 100) pageSize = 10;

                            var result = await employeeService.GetAllAsync(page, pageSize);
                            return SimpleJsonSerializer.Serialize(result);
                        }

                    case "POST":
                        var createBody = await ReadRequestBodyAsync(request);
                        var createDto = SimpleJsonSerializer.Deserialize<CreateEmployeeDto>(createBody);
                        var createdEmployee = await employeeService.CreateAsync(createDto);
                        return SimpleJsonSerializer.Serialize(createdEmployee);

                    case "PUT":
                        if (!employeeId.HasValue)
                        {
                            return SimpleJsonSerializer.Serialize(new { error = "Employee ID is required for update" });
                        }
                        var updateBody = await ReadRequestBodyAsync(request);
                        var updateDto = SimpleJsonSerializer.Deserialize<UpdateEmployeeDto>(updateBody);
                        var updatedEmployee = await employeeService.UpdateAsync(employeeId.Value, updateDto);
                        if (updatedEmployee == null)
                        {
                            return SimpleJsonSerializer.Serialize(new { error = "Employee not found" });
                        }
                        return SimpleJsonSerializer.Serialize(updatedEmployee);

                    case "DELETE":
                        if (!employeeId.HasValue)
                        {
                            return SimpleJsonSerializer.Serialize(new { error = "Employee ID is required for delete" });
                        }
                        var deleted = await employeeService.DeleteAsync(employeeId.Value);
                        if (!deleted)
                        {
                            return SimpleJsonSerializer.Serialize(new { error = "Employee not found" });
                        }
                        return SimpleJsonSerializer.Serialize(new { message = "Employee deleted successfully" });

                    default:
                        return SimpleJsonSerializer.Serialize(new { error = "Method not allowed" });
                }
            }
            catch (ArgumentException ex)
            {
                return SimpleJsonSerializer.Serialize(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return SimpleJsonSerializer.Serialize(new { error = "Internal server error: " + ex.Message });
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            if (string.IsNullOrEmpty(queryString))
                return result;

            if (queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            var pairs = queryString.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length >= 2)
                {
                    var key = Uri.UnescapeDataString(keyValue[0]);
                    var value = Uri.UnescapeDataString(keyValue[1]);
                    result[key] = value;
                }
            }

            return result;
        }
    }
}
