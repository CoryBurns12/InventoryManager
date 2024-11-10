using MySql.Data.MySqlClient;

public class Connection
{
    private string _connectionToDatabase = "server=localhost;database=InventorySystem;user=Cburns12;password=Blazers24_!!!";

    public MySqlConnection Connect()
    {
        MySqlConnection connection = new(_connectionToDatabase);

        try
        {
            connection.Open();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e.Message);
        }

        return connection;
    }
}