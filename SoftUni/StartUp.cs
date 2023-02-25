namespace SoftUni;

using Data;
using Models;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class StartUp
{
	static void Main(string[] args)
	{
		SoftUniContext context = new SoftUniContext();

		Console.WriteLine(GetEmployee147(context));
	}

	//Problem 3

	public static string GetEmployeesFullInformation(SoftUniContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();

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

		foreach (var employee in employees)
			stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");

		return stringBuilder.ToString().TrimEnd();
	}

	//Problem 4

	public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();

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

		foreach (var employee in employees)
			stringBuilder.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");

		return stringBuilder.ToString().TrimEnd();
	}

	//Problem 5

	public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();

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

		foreach (var employee in employees)
			stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Department.Name} - {employee.Salary:f2}");

		return stringBuilder.ToString().TrimEnd();
	}

	//Problem 6

	public static string AddNewAddressToEmployee(SoftUniContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();

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

		foreach (var employeeAddress in employeeAddresses)
			stringBuilder.AppendLine(employeeAddress);

		return stringBuilder.ToString().TrimEnd();
	}

	//Problem 7

	/*public static string GetEmployeesInPeriod(SoftUniContext context)
	//{
	//	StringBuilder stringBuilder = new StringBuilder();

	//	var employeesWithProjects = context.Employees
	//		.AsNoTracking()
	//		.Select(e => new
	//		{
	//			e.FirstName,
	//			e.LastName,
	//			e.Manager,
	//			e.EmployeesProjects
	//		})
	//		.Where(e => e.EmployeesProjects
	//			.Where(ep => ep.Project.StartDate.Year >= 2001 &&
	//						 ep.Project.StartDate.Year <= 2003)
	//		.Take(10)
	//		.ToArray();

	//	foreach (var ep in employeesWithProjects)
	//	{
	//		stringBuilder.AppendLine($"{ep.FirstName} {ep.LastName} - Manager: {ep.Manager.FirstName} {ep.Manager.LastName}");

	//		foreach (var p in ep.EmployeesProjects)
	//			stringBuilder.AppendLine($"--{p.Name} - {p.StartDate}");
	//	}
	  }*/

	//Problem 8

	public static string GetAddressesByTown(SoftUniContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();

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

		foreach (var address in addresses)
			stringBuilder.AppendLine($"{address.AddressText}, {address.Town.Name} - {address.Employees.Count} employees");

		return stringBuilder.ToString().TrimEnd();
	}

	//Problem 9

	public static string GetEmployee147(SoftUniContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();

		int idToFind = 147;

		Employee? employee = context.Employees
			.FirstOrDefault(e => e.EmployeeId == idToFind);

		stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

		foreach (var employeeProject in employee.EmployeesProjects.OrderBy(ep => ep.Project.Name))
			stringBuilder.AppendLine(employeeProject.Project.Name);

		return stringBuilder.ToString().TrimEnd();
	}
}
