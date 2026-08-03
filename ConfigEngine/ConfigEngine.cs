public class ConfigEngine
{
    public void LoadFromFile(string filepath, List<ConfigEntry> _entries)
    {
        try
        {
            _entries.Clear();

            string[] lines = File.ReadAllLines(filepath);

            foreach(string line in lines)
            {
                if(string.IsNullOrEmpty(line) || !line.Contains('=')) continue;

                string[] parts = line.Split('=');
                string key = parts[0];
                string value = parts[1];

                _entries.Add(new ConfigEntry(key, value));
            }

            Console.WriteLine($"Successfully loaded {_entries.Count} entries");
        } 
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file '{filepath}' cannot be found!");
        }
    }

    public void UpdateSetting(string key, string value, List<ConfigEntry> _entries)
    {
        int index = _entries.FindIndex(e => e.Key == key);

        if(index != -1)
        {
            _entries[index] = new ConfigEntry(key, value);
        }
    }

    public void AddSetting(string key, string value, List<ConfigEntry> _entries)
    {
        _entries.Add(new ConfigEntry(key, value));
    }

    public void SaveToFile(string filepath, List<ConfigEntry> _entries)
    {
        try
        {
            List<string> linesToWrite = new();

            foreach(var entry in _entries)
            {
                linesToWrite.Add($"{entry.Key}={entry.Value}");
            }    

            File.WriteAllLines(filepath, linesToWrite);   
        } 
        catch (IOException)
        {
            Console.WriteLine("Failed to write to file");
        }
    }

    public void PrintSettings(List<ConfigEntry> _entries)
    {
        foreach (var entry in _entries)
        {
            Console.WriteLine($"{entry.Key}={entry.Value}");
        }
    }

    public void CreateConfigFile(string filepath)
    {
        try
        {
            using(File.Create(filepath)) {}
        } 
        catch(IOException)
        {
            Console.WriteLine("Failed to create file");
        }
    }

    public void DeleteSetting(string key, List<ConfigEntry> entries)
    {
        entries.RemoveAll(e => e.Key == key);
    }
}