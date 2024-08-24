namespace WebApplication1.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private List<Employee> employees;
        public EmployeeRepository()
        {
            employees = new List<Employee>() { 
                new Employee(){Id =1,Name="Mary",Department=Dept.Hr,Email="kekw@gmail.com"},
                new Employee(){Id =2,Name="hassan",Department=Dept.IT,Email="kekw@gmail.com"},
                new Employee(){Id =3,Name="basha",Department=Dept.Payroll,Email="kekw@gmail.com"},
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = employees.Max(e => e.Id) + 1;
            employees.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = employees.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                employees.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return employees;
        }

        public Employee GetEmployee(int id)
        {
            return employees.FirstOrDefault(e => e.Id == id);
        }

        public Employee Update(Employee employee)
        {
            Employee _employee = employees.FirstOrDefault(e => e.Id == employee.Id);
            if (_employee != null)
            {
                _employee.Name = employee.Name;
                _employee.Department = employee.Department;
                _employee.Email = employee.Email;
            }
            return _employee;
        }
    }
}
