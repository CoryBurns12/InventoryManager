using System.Net;
using Microsoft.Identity.Client;
using MySql.Data.MySqlClient;

public class Access
{
    private Connection connection = new();
    private Menu menu = new();
    private Random rand = new Random();
    private BoolMethods boolmethods = new();

    // Main Access Point
    public void MainAccess()
    {
        int _User;
        do
        {
            menu.MainMenu();
            _User = Convert.ToInt32(Console.ReadLine());

            switch (_User)
            {
                case 1:
                    AddEmployee();
                    Console.WriteLine();
                    break;
                case 2:
                    UpdateEmployee();
                    Console.WriteLine();
                    break;
                case 3:
                    DeleteSpecificEmployee();
                    Console.WriteLine();
                    break;
                case 4:
                    DeleteAll();
                    Console.WriteLine();
                    break;
                case 5:
                    ViewEmployees();
                    Console.WriteLine();
                    break;
                case 6:
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid option, try again.");
                    break;
            }

        } while (_User != 6);
    }

    // Add Employee
    public void AddEmployee()
    {
        string _query = "INSERT INTO Employee (EmployeeID, FirstName, LastName, Email, PhoneNumber, HireDate, JobTitle, Department, Salary) " +
                        "VALUES (@EmployeeID, @FirstName, @LastName, @Email, @PhoneNumber, @HireDate, @JobTitle, @Department, @Salary)";
        
        // Generate a random 6-digit EmployeeID
        string employeeID = rand.Next(100000, 1000000).ToString(); 

        // Array with placeholders for other employee details (includes HireDate for first employee)
        string[] _Employee = { "EmployeeID", "First Name", "Last Name", "Email", "Phone Number", "Job Title", "Department", "Salary" };
        object[] _EmployeeValues = new object[_Employee.Length];

        Console.Clear();
        Console.WriteLine("** Add Employee Menu **");

        // Loop starts from index 1 to skip EmployeeID since it's already set
        for (int i = 1; i < _Employee.Length; i++)
        {
            Console.WriteLine($"Enter {_Employee[i]}");

            // Switch to handle data input for other fields
#pragma warning disable CS8601 // Possible null reference assignment.
            _EmployeeValues[i] = _Employee[i] switch
            {
                "First Name" => Console.ReadLine(),
                "Last Name" => Console.ReadLine(),
                "Email" => Console.ReadLine(),
                "Phone Number" => Console.ReadLine(),
                "Job Title" => Console.ReadLine(),
                "Department" => Console.ReadLine(),
                "Salary" => Convert.ToDecimal(Console.ReadLine()),
                _ => throw new NotImplementedException()
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        try
        {
            // Ensure EmployeeID and PhoneNumber are unique
            while (boolmethods.IDExists(employeeID))
            {
                Console.WriteLine("EMPLOYEE ID ALREADY EXISTS! GENERATING NEW ID...");
                employeeID = rand.Next(100000, 1000000).ToString();
            }

            while (boolmethods.PhoneNumberExists(_EmployeeValues[4].ToString()))
            {
                Console.WriteLine("PHONE NUMBER ALREADY EXISTS! ENTER A NEW PHONE NUMBER...");
#pragma warning disable CS8601 // Possible null reference assignment.
                _EmployeeValues[4] = Console.ReadLine();
#pragma warning restore CS8601 // Possible null reference assignment.
            }

            while(boolmethods.EmailExists(_EmployeeValues[3].ToString()))
            {
                Console.WriteLine("EMAIL ALREADY EXISTS! ENTER A DIFFERENT VARIANT OR A NEW EMAIL ENTIRELY!");
                _EmployeeValues[3] = Console.ReadLine();
            }

            // Set current date as HireDate
            DateTime hireDate = DateTime.Now;

            using MySqlCommand cmd = new(_query, connection.Connect());

            // Format PhoneNumber if it's exactly 10 digits
            if (_EmployeeValues[4].ToString().Length == 10)
            {
                string formattedPhoneNumber = string.Format("{0:###-###-####}", long.Parse(_EmployeeValues[4].ToString()));
                cmd.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar).Value = formattedPhoneNumber;
            }
            else
            {
                cmd.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar).Value = _EmployeeValues[4].ToString();
            }

            // Add parameters for other fields
            cmd.Parameters.Add("@EmployeeID", MySqlDbType.Int32).Value = Convert.ToInt32(employeeID);
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = _EmployeeValues[1];
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = _EmployeeValues[2];
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = _EmployeeValues[3];
            cmd.Parameters.Add("@HireDate", MySqlDbType.DateTime).Value = hireDate.ToString("yyyy-MM-dd");
            cmd.Parameters.Add("@JobTitle", MySqlDbType.VarChar).Value = _EmployeeValues[5];
            cmd.Parameters.Add("@Department", MySqlDbType.VarChar).Value = _EmployeeValues[6];
            cmd.Parameters.Add("@Salary", MySqlDbType.Decimal).Value = _EmployeeValues[7];

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Employee added successfully.\n");
            }
            else
            {
                Console.WriteLine("Error: No rows affected.\n");
            }
        }
        catch (MySqlException ex)
        {
            // Print detailed error message to understand what went wrong
            Console.WriteLine($"An error occurred while adding the employee: {ex.Message}");
            Console.WriteLine($"Error Code: {ex.ErrorCode}");
            Console.WriteLine($"SQL State: {ex.SqlState}");
        }
    }

    // Update Employee
    public void UpdateEmployee()
    {
        string _query = "UPDATE Employee SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber, JobTitle = @JobTitle, Department = @Department, Salary = @Salary WHERE EmployeeID = @EmployeeID";
        ViewEmployees();

        Console.WriteLine("\nEnter the EmployeeID of the employee you want to update:");
        string employeeID = Console.ReadLine();

        if (!boolmethods.IDExists(employeeID))
        {
            Console.WriteLine("EmployeeID does not exist.");
            return;
        }

        Console.WriteLine("Enter the new First Name:");
        string firstName = Console.ReadLine();

        Console.WriteLine("Enter the new Last Name:");
        string lastName = Console.ReadLine();

        Console.WriteLine("Enter the new Email:");
        string email = Console.ReadLine();

        Console.WriteLine("Enter the new Phone Number:");
        string phoneNumber = Console.ReadLine();

        Console.WriteLine("Enter the new Job Title:");
        string jobTitle = Console.ReadLine();

        Console.WriteLine("Enter the new Department:");
        string department = Console.ReadLine();

        Console.WriteLine("Enter the new Salary:");
        decimal salary = Convert.ToDecimal(Console.ReadLine());

        try
        {
            using MySqlCommand cmd = new(_query, connection.Connect());

            // Format PhoneNumber if it's exactly 10 digits
            if (phoneNumber.Length == 10)
            {
                string formattedPhoneNumber = string.Format("{0:###-###-####}", int.Parse(phoneNumber));
                cmd.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar).Value = formattedPhoneNumber;
            }
            else
            {
                cmd.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar).Value = phoneNumber;
            }

            // Add other parameters
            cmd.Parameters.Add("@EmployeeID", MySqlDbType.Int32).Value = Convert.ToInt32(employeeID);
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = firstName;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = lastName;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@JobTitle", MySqlDbType.VarChar).Value = jobTitle;
            cmd.Parameters.Add("@Department", MySqlDbType.VarChar).Value = department;
            cmd.Parameters.Add("@Salary", MySqlDbType.Decimal).Value = salary;

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Employee updated successfully.\n");
            }
            else
            {
                Console.WriteLine("Error: No rows affected.\n");
            }
        }
        catch (MySqlException e)
        {
            Console.WriteLine($"An error occurred while updating the employee: {e.Message}");
        }
    }

    // Delete All Employees
    public void DeleteAll()
    {
        string _query = "DELETE FROM Employee";
        try
        {
            using MySqlCommand cmd = new(_query, connection.Connect());
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Successfully deleted!\n");
            }
            else
            {
                Console.WriteLine("Error: No rows have been deleted!\n");
            }
        }
        catch (MySqlException e)
        {
            Console.WriteLine($"An error occurred while deleting one or more of the employee rows {e.Message}");
        }
    }

    // Delete Specific Employee
    public void DeleteSpecificEmployee()
    {
        string _query = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID";
        ViewEmployees();
        Console.WriteLine("\nWhich employee would you like to delete? (Enter ID number)");
        int ID = Convert.ToInt32(Console.ReadLine());
        while(!boolmethods.IDExists(ID.ToString()))
        {
            Console.WriteLine("ID DOES NOT EXIST! PLEASE RE-ENTER...\n");
            ViewEmployees();
            ID = Convert.ToInt32(Console.ReadLine());
        }

        try
        {
            using MySqlCommand cmd = new(_query, connection.Connect());
            cmd.Parameters.Add("@EmployeeID", MySqlDbType.Int32).Value = Convert.ToInt32(ID);
            int rowsAffected = cmd.ExecuteNonQuery();
            while(boolmethods.IDExists(ID.ToString()))
            {
                if(rowsAffected > 0)
                    Console.WriteLine("Employee successfully deleted!");
                else
                    Console.WriteLine("Employee not deleted!");
            }
        }
        catch(MySqlException e)
        {
            Console.WriteLine($"AN ERROR HAS OCCURED! {e.Message}");
            Console.WriteLine($"State: {e.SqlState}");
            Console.WriteLine($"Error Code: {e.ErrorCode}");
        }
    }

    // View Employees
    public void ViewEmployees()
    {
        string _query = "SELECT * FROM Employee";

        try
        {
            using MySqlCommand cmd = new(_query, connection.Connect());
            using MySqlDataReader SELECT = cmd.ExecuteReader();

            if (SELECT.HasRows)
            {
                // Print header with aligned columns
                Console.WriteLine($"{ "ID".PadRight(10) }" +
                                  $"{ "First Name".PadRight(20) }" +
                                  $"{ "Last Name".PadRight(15) }" +
                                  $"{ "Email".PadRight(25) }" +
                                  $"{ "Phone Number".PadRight(15) }" +
                                  $"{ "Job Title".PadRight(20) }" +
                                  $"{ "Department".PadRight(20) }" +
                                  $"{ "Salary".PadRight(15) }");

                // Read and display each employee's data
                while (SELECT.Read())
                {
                    string hireDate = SELECT["HireDate"] != DBNull.Value ? Convert.ToDateTime(SELECT["HireDate"]).ToString("yyyy-MM-dd") : "";

                    Console.WriteLine($"{SELECT["EmployeeID"].ToString().PadRight(10)}" +
                                      $"{SELECT["FirstName"].ToString().PadRight(20)}" +
                                      $"{SELECT["LastName"].ToString().PadRight(15)}" +
                                      $"{SELECT["Email"].ToString().PadRight(25)}" +
                                      $"{SELECT["PhoneNumber"].ToString().PadRight(15)}" +
                                      $"{SELECT["JobTitle"].ToString().PadRight(20)}" +
                                      $"{SELECT["Department"].ToString().PadRight(20)}" +
                                      $"{Convert.ToDecimal(SELECT["Salary"]).ToString("C").PadRight(15)}");
                }
            }
            else
            {
                Console.WriteLine("No employees found.");
            }
        }
        catch (MySqlException e)
        {
            Console.WriteLine($"An error has occurred while trying to show the table of employees: {e.Message}");
        }
    }
}
