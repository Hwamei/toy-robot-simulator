using ToyRobotSimulator.Core.Services;

Console.Title = "TOY ROBOT SIMULATOR";
var isRunning = true;

while (isRunning)
{
    Console.WriteLine("PLEASE ENTER COMMAND:");

    var command = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(command))
        continue;

    Console.WriteLine($"EXECUTING COMMAND ...");

    try
    {
        var processor = new RobotProcessor(new CommandParser(), new CommandExecutor());
        var messages = processor.Run(command);

        if (messages != null && messages.Any())
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            messages.ForEach(m => Console.WriteLine(m));
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("INVALID COMMAND");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }
    finally 
    {
        Console.WriteLine();
        Console.ResetColor();
    }
}

Console.Read();


