using Org.BouncyCastle.Security;

public class Menu
{
    private Connection connection = new();
    public void MainMenu()
    {
        string[] menuOptions = {"Add Employee", "Update Current Employee", "Delete Employee", "Delete All Employees", "View Employees", "Exit"};
        for(int i = 0; i < menuOptions.Length; i++)
        {
            Console.WriteLine(i + 1 + ". " + menuOptions[i]);
        } 
        
        Console.WriteLine();
        Console.Write("What would you like to do?");
    }
}