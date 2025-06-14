namespace HumanResources.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; } // Navigation property
    }
}