using MySql.Data.MySqlClient;
public class BoolMethods
{
    private Connection connection = new();
        public bool IDExists(string employeeID)
    {
        string _query = "SELECT COUNT(*) FROM Employee WHERE EmployeeID = @EmployeeID";
        using MySqlCommand cmd = new(_query, connection.Connect());
        cmd.Parameters.Add("@EmployeeID", MySqlDbType.Int32).Value = Convert.ToInt32(employeeID);
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        return count > 0;
    }

    // Helper Method to Check if Phone Number Exists
    public bool PhoneNumberExists(string phoneNumber)
    {
        string _query = "SELECT COUNT(*) FROM Employee WHERE PhoneNumber = @PhoneNumber";
        using MySqlCommand cmd = new(_query, connection.Connect());
        cmd.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar).Value = phoneNumber;
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        return count > 0;
    }

    // Helper Method to Check if Email Exists
    public bool EmailExists(string Email)
    {
        string _query = "SELECT COUNT(*) FROM Employee WHERE Email = @Email";
        try
        {
        using MySqlCommand cmd = new(_query, connection.Connect());
        cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = Email;
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        return count > 0;
        }
        catch(System.FormatException e)
        {
            Console.WriteLine($"Error: {e.Message}");
            return false;
        }
        catch(MySqlException e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine($"State: {e.SqlState}");
            Console.WriteLine($"Error Code: {e.ErrorCode}");
            return false;
        }
    }
}