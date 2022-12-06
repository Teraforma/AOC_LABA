// See https://aka.ms/new-console-template for more information
using ClientV2;
using System.Reflection.Metadata.Ecma335;

Console.WriteLine("Hello, World!");
Client_Controller controller = new Client_Controller();
while (true) {
    try
    {   
        Console.WriteLine("\nguess/continue/who/number/exit");
        string input = Console.ReadLine();
        input = input.ToLower();

       
        if (input == "continue")
        {

            Console.Write("milisecs given for guessing:");
            if (Int32.TryParse(Console.ReadLine(), out int milisecs))
            {

                controller.ContinueGuessing(milisecs);
            }
            else
            {
                Console.WriteLine("Not an int");
            }

        }
        else if (input == "guess")
        {

            Console.Write("milisecs given for guessing:");
            if ( Int32.TryParse( Console.ReadLine(), out int milisecs))
            {
                controller.LaunchGuessing(milisecs);
            }
            else
            {
                Console.WriteLine("Not an int");
            }
            

        }
        else if (input == "who")
        {
            controller.AskWho();
        } else if (input == "number")
        {
            controller.AskSecretNumber();

        } else if (input == "exit") 
        {
            Console.WriteLine("exit");
            break;
        }
        else
        {
            continue;
        }
        
        
    }
    catch (Exception ex)
    {
        {  Console.WriteLine(ex.ToString()); }
    }
    
}