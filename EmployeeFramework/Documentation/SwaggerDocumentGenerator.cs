using System;

namespace EmployeeFramework.Documentation
{
    public static class SwaggerDocumentGenerator
    {
        public static string GenerateSwaggerJson(string baseUrl)
        {
            return $@"{{
  ""openapi"": ""3.0.1"",
  ""info"": {{
    ""title"": ""Employee Management API"",
    ""description"": ""A RESTful API for managing employee data (.NET Framework 4.7.2)"",
    ""version"": ""1.0.0""
  }},
  ""servers"": [{{ ""url"": ""{baseUrl}"", ""description"": ""Development server"" }}],
  ""paths"": {{
    ""/api/employees"": {{
      ""get"": {{
        ""responses"": {{
          ""200"": {{
            ""description"": ""Success"",
            ""content"": {{ ""application/json"": {{ ""schema"": {{ ""$ref"": ""#/components/schemas/PagedEmployeeResult"" }} }} }}
          }}
        }}
      }},
      ""post"": {{
        ""requestBody"": {{
          ""required"": true,
          ""content"": {{ ""application/json"": {{ ""schema"": {{ ""$ref"": ""#/components/schemas/CreateEmployee"" }} }} }}
        }},
        ""responses"": {{
          ""201"": {{ ""description"": ""Created"", ""content"": {{ ""application/json"": {{ ""schema"": {{ ""$ref"": ""#/components/schemas/Employee"" }} }} }} }},
          ""400"": {{ ""description"": ""Invalid input"" }}
        }}
      }}
    }},
    ""/api/employees/{{id}}"": {{
      ""get"": {{
        ""parameters"": [{{ ""name"": ""id"", ""in"": ""path"", ""required"": true, ""schema"": {{ ""type"": ""string"", ""format"": ""uuid"" }} }}],
        ""responses"": {{
          ""200"": {{ ""description"": ""Success"", ""content"": {{ ""application/json"": {{ ""schema"": {{ ""$ref"": ""#/components/schemas/Employee"" }} }} }} }},
          ""404"": {{ ""description"": ""Not Found"" }}
        }}
      }},
      ""put"": {{
        ""parameters"": [{{ ""name"": ""id"", ""in"": ""path"", ""required"": true, ""schema"": {{ ""type"": ""string"", ""format"": ""uuid"" }} }}],
        ""requestBody"": {{
          ""required"": true,
          ""content"": {{ ""application/json"": {{ ""schema"": {{ ""$ref"": ""#/components/schemas/UpdateEmployee"" }} }} }}
        }},
        ""responses"": {{
          ""200"": {{ ""description"": ""Updated"", ""content"": {{ ""application/json"": {{ ""schema"": {{ ""$ref"": ""#/components/schemas/Employee"" }} }} }} }},
          ""400"": {{ ""description"": ""Invalid input"" }},
          ""404"": {{ ""description"": ""Not Found"" }}
        }}
      }},
      ""delete"": {{
        ""parameters"": [{{ ""name"": ""id"", ""in"": ""path"", ""required"": true, ""schema"": {{ ""type"": ""string"", ""format"": ""uuid"" }} }}],
        ""responses"": {{
          ""200"": {{ ""description"": ""Deleted"" }},
          ""404"": {{ ""description"": ""Not Found"" }}
        }}
      }}
    }}
  }},
  ""components"": {{
    ""schemas"": {{
      ""Employee"": {{
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {{
          ""id"": {{ ""type"": ""string"", ""format"": ""uuid"" }},
          ""name"": {{ ""type"": ""string"" }},
          ""address"": {{ ""type"": ""string"" }},
          ""age"": {{ ""type"": ""integer"", ""minimum"": 1, ""maximum"": 120 }},
          ""salary"": {{ ""type"": ""number"", ""format"": ""decimal"", ""minimum"": 0 }},
          ""createdAt"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
          ""updatedAt"": {{ ""type"": ""string"", ""format"": ""date-time"", ""nullable"": true }}
        }},
        ""required"": [""id"", ""name"", ""address"", ""age"", ""salary"", ""createdAt""]
      }},
      ""CreateEmployee"": {{
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {{
          ""name"": {{ ""type"": ""string"" }},
          ""address"": {{ ""type"": ""string"" }},
          ""age"": {{ ""type"": ""integer"", ""minimum"": 1, ""maximum"": 120 }},
          ""salary"": {{ ""type"": ""number"", ""format"": ""decimal"", ""minimum"": 0 }}
        }},
        ""required"": [""name"", ""address"", ""age"", ""salary""]
      }},
      ""UpdateEmployee"": {{
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {{
          ""name"": {{ ""type"": ""string"" }},
          ""address"": {{ ""type"": ""string"" }},
          ""age"": {{ ""type"": ""integer"", ""minimum"": 1, ""maximum"": 120 }},
          ""salary"": {{ ""type"": ""number"", ""format"": ""decimal"", ""minimum"": 0 }}
        }},
        ""required"": [""name"", ""address"", ""age"", ""salary""]
      }},
      ""PagedEmployeeResult"": {{
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {{
          ""items"": {{ ""type"": ""array"", ""items"": {{ ""$ref"": ""#/components/schemas/Employee"" }} }},
          ""totalCount"": {{ ""type"": ""integer"" }},
          ""page"": {{ ""type"": ""integer"" }},
          ""pageSize"": {{ ""type"": ""integer"" }},
          ""totalPages"": {{ ""type"": ""integer"" }}
        }},
        ""required"": [""items"", ""totalCount"", ""page"", ""pageSize"", ""totalPages""]
      }}
    }}
  }}
}}";
        }

        public static string GenerateSwaggerUI(string baseUrl)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
  <title>Employee API - Swagger UI</title>
  <link rel=""stylesheet"" href=""https://unpkg.com/swagger-ui-dist/swagger-ui.css"" />
  <style>.swagger-ui .topbar {{ display:none; }}</style>
</head>
<body>
  <div id=""swagger-ui""></div>
  <script src=""https://unpkg.com/swagger-ui-dist/swagger-ui-bundle.js""></script>
  <script>
    window.onload = () => {{
      SwaggerUIBundle({{
        url: '{baseUrl}/swagger.json',
        dom_id: '#swagger-ui'
      }});
    }};
  </script>
</body>
</html>";
        }
    }
}
