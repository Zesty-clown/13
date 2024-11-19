using System;
using System.IO;

public static class LoginPage
{
    private const string CorrectUsername = "admin";
    private const string CorrectPassword = "password123";

    public static bool AuthenticateUser()
    {
        Console.Clear();
        Console.WriteLine("LOGIN PAGE");
        Console.WriteLine(new string('═', 30));

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = MaskPassword();

        if (username == CorrectUsername && password == CorrectPassword)
        {
            SaveHWIDToDesktop();
            Console.Clear();
            Console.WriteLine("Login successful!");
            Console.WriteLine($"Your HWID has been saved to the desktop as 'user_hwid.txt'.");
            return true;
        }
        else
        {
            Console.WriteLine("\nInvalid credentials. Try again.");
            Thread.Sleep(2000); // Pause to show error message
            return AuthenticateUser();
        }
    }

    private static string MaskPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine(); // Move to next line after Enter
        return password;
    }

    private static string GetHWID()
    {
        // Use machine name or another unique identifier as the HWID
        return Environment.MachineName; // Alternatively: Guid.NewGuid().ToString();
    }

    private static void SaveHWIDToDesktop()
    {
        string hwid = GetHWID();

        // Get the desktop path
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "user_hwid.txt");

        // Create the file and save HWID
        try
        {
            File.WriteAllText(filePath, hwid);
            Console.WriteLine($"HWID saved successfully to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving HWID to file: {ex.Message}");
        }
    }
}
