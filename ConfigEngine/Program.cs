ConfigEngine engine = new();

List<ConfigEntry> defaultConfigEntries = new();
engine.LoadFromFile("default_config.txt", defaultConfigEntries);

bool isRunning = true;

while(isRunning)
{
    Console.WriteLine();

    PrintOptions();
    int option = GetInput("Option: ");

    switch(option)
    {
        case 1:
            Console.WriteLine();
            engine.PrintSettings(defaultConfigEntries);
            break;
        case 2:
            CreateFileOption();
            break;
        case 3:
            CreateEntryOption();
            break;
        case 4:
            EditEntryFromFileOption();
            Console.WriteLine();
            break;
        case 5:
            DeleteEntryFromFileOption();
            break;
        case 6:
            isRunning = false;
            break;
    }
}

Console.WriteLine("Done!");

void PrintOptions()
{
    Console.WriteLine("1. Print settings from file");
    Console.WriteLine("2. Create file");
    Console.WriteLine("3. Create entry");
    Console.WriteLine("4. Edit entry from file");
    Console.WriteLine("5. Delete entry from file");
    Console.WriteLine("6. Exit");
}

int GetInput(string message)
{
    Console.Write(message);
    return int.Parse(Console.ReadLine()!);
}

void CreateFileOption()
{
    Console.WriteLine();

    Console.Write("Name: ");
    string filepath = Console.ReadLine()!;
    engine.CreateConfigFile(filepath);
}

void CreateEntryOption()
{
    Console.WriteLine();

    engine.PrintSettings(defaultConfigEntries);
    Console.Write("Key:value to add: ");
    string keyValue = Console.ReadLine()!;

    string[] parts = keyValue.Split('=');
    string key = parts[0];
    string value = parts[1];

    engine.AddSetting(key, value, defaultConfigEntries);
    engine.SaveToFile("default_config.txt", defaultConfigEntries);
    engine.LoadFromFile("default_config.txt", defaultConfigEntries);

    Console.WriteLine("\nSuccessful add!");
    engine.PrintSettings(defaultConfigEntries);
}

void EditEntryFromFileOption()
{
    Console.WriteLine();

    engine.PrintSettings(defaultConfigEntries);

    Console.Write("Key:value to change: ");
    string keyValue = Console.ReadLine()!;

    string[] parts = keyValue.Split('=');
    string key = parts[0];
    string value = parts[1];

    engine.UpdateSetting(key, value, defaultConfigEntries);
    engine.SaveToFile("default_config.txt", defaultConfigEntries);
    engine.LoadFromFile("default_config.txt", defaultConfigEntries);

    Console.WriteLine("\nSuccessful edit!");
    engine.PrintSettings(defaultConfigEntries);
}

void DeleteEntryFromFileOption()
{
    Console.WriteLine();

    engine.PrintSettings(defaultConfigEntries);

    Console.Write("Key to delete: ");
    string key = Console.ReadLine()!;

    engine.DeleteSetting(key, defaultConfigEntries);
    engine.SaveToFile("default_config.txt", defaultConfigEntries);
    engine.LoadFromFile("default_config.txt", defaultConfigEntries);

    Console.WriteLine("\nSuccessful deletion!");
    engine.PrintSettings(defaultConfigEntries);
}