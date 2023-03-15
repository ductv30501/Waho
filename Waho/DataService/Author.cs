using Waho.WahoModels;
using System.Text.Json;
namespace Waho.DataService
{
    public class Author
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Author(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Boolean IsAuthor(int role)
        {
            var employeeJson = _httpContextAccessor.HttpContext.Session.GetString("Employee");
            Employee employee = JsonSerializer.Deserialize<Employee>(employeeJson);
            
            if (employee == null)
            {
                return false;
            }
            else
            {
                if (employee.Role == 1)
                {
                    return true;
                }
                else
                {
                    if (employee.Role == role)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }
    }
}
