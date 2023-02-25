namespace SoftUni;

using Data;
using Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

public class StartUp
{
    static void Main(string[] args)
    {
        SoftUniContext context = new SoftUniContext();
    }

    //Problem 3

    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        var employees = context.Employees
            .AsNoTracking()
            .Select(e => new
            {
                e.EmployeeId,
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .OrderBy(e => e.EmployeeId)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var employee in employees)
            stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 4

    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        var employees = context.Employees
            .AsNoTracking()
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            })
            .Where(e => e.Salary > 50000)
            .OrderBy(e => e.FirstName)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var employee in employees)
            stringBuilder.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 5

    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        var employees = context.Employees
            .AsNoTracking()
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Department,
                e.Salary
            })
            .Where(e => e.Department.Name == "Research and Development")
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var employee in employees)
            stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Department.Name} - ${employee.Salary:f2}");

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 6

    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address address = new Address()
        {
            AddressText = "Vitoshka 15",
            TownId = 4
        };

        context.Addresses.Add(address);

        Employee? employee = context.Employees
            .FirstOrDefault(e => e.LastName == "Nakov");

        employee.Address = address;

        context.SaveChanges();

        var employeeAddresses = context.Employees
            .AsNoTracking()
            .OrderByDescending(e => e.AddressId)
            .Take(10)
            .Select(e => e.Address.AddressText)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var employeeAddress in employeeAddresses)
            stringBuilder.AppendLine(employeeAddress);

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 7

    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        var employeesWithProjects = context.Employees
            .AsNoTracking()
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager!.FirstName,
                ManagerLastName = e.Manager!.LastName,
                Projects = e.EmployeesProjects
                    .Where(ep => ep.Project.StartDate.Year >= 2001 &&
                                 ep.Project.StartDate.Year <= 2003)
                    .Select(ep => new
                    {
                        Name = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = ep.Project.EndDate.HasValue
                            ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                            : "not finished"
                    })
                    .ToArray()
            })
            .Take(10)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var e in employeesWithProjects)
        {
            stringBuilder.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

            foreach (var p in e.Projects)
                stringBuilder.AppendLine($"--{p.Name} - {p.StartDate} - {p.EndDate}");
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 8

    public static string GetAddressesByTown(SoftUniContext context)
    {
        var addresses = context.Addresses
            .AsNoTracking()
            .Select(a => new
            {
                a.AddressText,
                a.Town,
                a.Employees
            })
            .OrderByDescending(a => a.Employees.Count)
            .ThenBy(a => a.Town.Name)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var address in addresses)
            stringBuilder.AppendLine($"{address.AddressText}, {address.Town.Name} - {address.Employees.Count} employees");

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 9

    public static string GetEmployee147(SoftUniContext context)
    {
        int idToFind = 147;

        Employee? employee = context.Employees
            .FirstOrDefault(e => e.EmployeeId == idToFind);

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

        foreach (var employeeProject in employee.EmployeesProjects.OrderBy(ep => ep.Project.Name))
            stringBuilder.AppendLine(employeeProject.Project.Name);

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 10

    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        var departmentsWithEmployees = context.Departments
            .AsNoTracking()
            .Where(d => d.Employees.Count > 5)
            .OrderBy(d => d.Employees.Count)
            .ThenBy(d => d.Name)
            .Select(d => new
            {
                d.Name,
                ManagerFirstName = d.Manager.FirstName,
                ManagerLastName = d.Manager.LastName,
                Employees = d.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(e => e.FirstName)
                    .OrderBy(e => e.LastName)
                    .ToArray()
            })
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var department in departmentsWithEmployees)
        {
            stringBuilder.AppendLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastName}");

            foreach (var employee in department.Employees)
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 11

    public static string GetLatestProjects(SoftUniContext context)
    {
        var projects = context.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.StartDate)
            .Take(10)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            })
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var project in projects)
        {
            stringBuilder.AppendLine(project.Name);
            stringBuilder.AppendLine(project.Description);
            stringBuilder.AppendLine(project.StartDate);
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 12

    public static string IncreaseSalaries(SoftUniContext context)
    {
        string[] departmentNames = new string[]
        {
            "Engineering",
            "Tool Design",
            "Marketing",
            "Information Services"
        };

        Employee[] employees = context.Employees
            .Where(e => departmentNames.Contains(e.Department.Name))
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        foreach (Employee employee in employees)
            employee.Salary += employee.Salary * 0.12m;

        context.SaveChanges();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var employee in employees)
            stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 13

    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        string stringToFind = "sa";

        var employees = context.Employees
            .AsNoTracking()
            .Where(e => e.FirstName.Substring(0, 2).ToLower() == stringToFind)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var employee in employees)
            stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 14
    
    public static string DeleteProjectById(SoftUniContext context)
    {
        int projectId = 2;

        context.EmployeesProjects.RemoveRange(context.EmployeesProjects.Where(ep => ep.ProjectId == projectId));

        context.Projects.Remove(context.Projects.FirstOrDefault(p => p.ProjectId == projectId));

        context.SaveChanges();

        var projects = context.Projects
            .AsNoTracking()
            .Take(10)
            .Select(p => p.Name)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var project in projects)
            stringBuilder.AppendLine(project);

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 15

    public static string RemoveTown(SoftUniContext context)
    {
        string townName = "Seattle";

        Employee[] employees = context.Employees
            .Where(e => e.Address.Town.Name == townName)
            .ToArray();

        foreach (Employee employee in employees)
            employee.AddressId = null;

        var addresses = context.Addresses
            .Where(a => a.Town.Name == townName)
            .ToArray();

        context.Addresses.RemoveRange(addresses);

        context.Towns.Remove(context.Towns.FirstOrDefault(t => t.Name == townName));

        context.SaveChanges();

        return $"{addresses.Length} addresses in {townName} were deleted";
    }
}
