using Parquet;
using ParquetSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter file path including file name: ");
        string filePath = Console.ReadLine();

        // File validation
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: File not found");
            Console.ReadKey();
            return;
        }
      
        // Path to a new file
        string newFilePath = "C:\\Users\\מחשב\\Desktop\\C#\\hadasimB\\hadasimB\\new_time_seriess.csv";// Section B.2

        // Creating a HashSet for detecting duplicates
        HashSet<string> seenLines = new HashSet<string>();

        string[] lines = File.ReadAllLines(filePath);
        if (lines.Length == 0)
        {
            return;
        }

        // Creating a StreamWriter to write to the new file
        using (StreamWriter writer = new StreamWriter(newFilePath))
        {
            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                if (columns.Length < 2)// Section B.1
                    continue;

                string timestamp = columns[0].Trim();
                string value = columns[1].Trim();

                if (!IsDateFormatValid(timestamp))
                {
                    continue;
                }

                // Check if line already exists
                if (seenLines.Contains(line))
                {
                    continue;
                }

                if (!IsDateThisYear(timestamp))
                {
                    continue;
                }

                seenLines.Add(line);
                writer.WriteLine(line);
            }
        }       

        Console.WriteLine("Process completed.");

        Dictionary<string, List<string>> result = null;
        if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Processing CSV file...");
            result = ProcessCsv(newFilePath);  // Section B.2.2
        }
        else if (filePath.EndsWith(".parquet", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Processing Parquet file...");
            result = ProcessParquet(filePath);  // Section B.2.4
        }
        else
        {
            Console.WriteLine("Unsupported file format. Please provide a CSV or Parquet file.");
        }

        string directoryPath = @"C:\Users\מחשב\Desktop\C#\hadasimB\hadasimB";//path to folder
        foreach (var date in result.Keys)
        {
            if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))//path to a new file
            {
                string fileName = Path.Combine(directoryPath, $"{date}.csv");
                AverageForEachHour(fileName);
            }
            else if (filePath.EndsWith(".parquet", StringComparison.OrdinalIgnoreCase))//path to a new file
            {
                string correctedFilePath = date.Replace("-", "_");
                string fileName = Path.Combine(directoryPath, $"{correctedFilePath}.parquet");
                AverageForEachHourParquet(fileName);
            }
        }
        Console.Read();
      
    }

    // Check if date is in valid format
    public static bool IsDateFormatValid(string date)
    {
        DateTime temp;
        return DateTime.TryParseExact(date, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out temp);
    }

    // Check if the date is from the current year
    public static bool IsDateThisYear(string date)
    {
        DateTime temp;
        if (DateTime.TryParseExact(date, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out temp))
        {
            return temp.Year == DateTime.Now.Year;
        }
        return false;
    }


    // Splits the input CSV file into smaller files based on date
    public static Dictionary<string, List<string>> ProcessCsv(string filePath)
    {
        var dateFiles = new Dictionary<string, List<string>>();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            var dateTimeString = parts[0];  // date and time
            var value = parts[1];  // value

            DateTime dateTime = DateTime.Parse(dateTimeString);
            string dateKey = dateTime.ToString("yyyy-MM-dd");  // Store only the date without time

            if (!dateFiles.ContainsKey(dateKey))
            {
                dateFiles[dateKey] = new List<string>();
            }

            dateFiles[dateKey].Add(line);
        }

        // Sort the dictionary by date keys (sorted by date)
        Dictionary<string, List<string>> sortedDateFiles = dateFiles.OrderBy(entry => DateTime.Parse(entry.Key)).ToDictionary(entry => entry.Key, entry => entry.Value);

        string directoryPath = @"C:\Users\מחשב\Desktop\C#\hadasimB\hadasimB";

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        foreach (var date in sortedDateFiles.Keys)
        {
            var fileName = Path.Combine(directoryPath, $"{date}.csv");
            var fileLines = sortedDateFiles[date];

            try
            {
                File.WriteAllLines(fileName, fileLines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file {fileName}: {ex.Message}");
            }
        }

        return sortedDateFiles;
    }


    public static Dictionary<string, List<string>> ProcessParquet(string filePath)
    {
        Dictionary<string, List<string>> dateFiles = new Dictionary<string, List<string>>();

        try
        {
            using var file = new ParquetFileReader(filePath);

            for (int rowGroup = 0; rowGroup < file.FileMetaData.NumRowGroups; ++rowGroup)
            {
                using var rowGroupReader = file.RowGroup(rowGroup);
                int groupNumRows = (int)rowGroupReader.MetaData.NumRows;

                // Reading timestamps (first column - timestamps)
                var groupTimestamps = rowGroupReader.Column(0).LogicalReader<Nullable<ParquetSharp.DateTimeNanos>>().ReadAll(groupNumRows);// Convert the DateTimeNanos (nanosecond precision) timestamps to DateTime (milliseconds precision) 

                List<DateTime> timestamps = groupTimestamps
                                            .Where(dt => dt.HasValue)
                                            .Select(dt => DateTimeOffset.FromUnixTimeMilliseconds(dt.Value.Ticks / 1_000_000).UtcDateTime)
                                            .ToList();


                /// Reading values (second column - values)
                var groupValue = rowGroupReader.Column(1).LogicalReader<Nullable<double>>().ReadAll(groupNumRows);

                for (int i = 0; i < groupNumRows; ++i)
                {
                    if (groupTimestamps[i].HasValue && groupValue[i].HasValue)
                    {
                        string dateKey = timestamps[i].ToString("yyyy-MM-dd");  // Extract date without time
                        string rowValue = $"{timestamps[i].ToString("yyyy-MM-dd HH:mm:ss")}: {groupValue[i].Value}";  // Convert timestamp to string

                        if (!dateFiles.ContainsKey(dateKey))
                            dateFiles[dateKey] = new List<string>();

                        dateFiles[dateKey].Add(rowValue);
                    }
                }
            }

            // sort
            var sortedDateFiles = dateFiles.OrderBy(entry => DateTime.Parse(entry.Key))
                                           .ToDictionary(entry => entry.Key, entry => entry.Value);

            string directoryPath = @"C:\Users\מחשב\Desktop\C#\hadasimB\hadasimB";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            // Create a Parquet file for each date in the dictionary
            foreach (var entry in sortedDateFiles)
            {
                string dateKey = entry.Key;
                List<string> rowData = entry.Value;

                List<DateTime> timestamps = new List<DateTime>();
                List<double> values = new List<double>();

                foreach (var row in rowData)
                {
                    var parts = row.Split(": ");
                    if (parts.Length == 2 && double.TryParse(parts[1], out double val))
                    {
                        if (DateTime.TryParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
                        {
                            timestamps.Add(fullDate);
                            values.Add(val);
                        }
                    }
                }

                string outputPath = Path.Combine(directoryPath, $"{dateKey.Replace("-", "_")}.parquet");

                try
                {
                    // Creating columns for the Parquet file
                    var columns = new Column[] {
                    new Column<DateTime>("Timestamp"),
                    new Column<double>("Value")
                };

                    // Creating the file and row group for the data
                    using var writer = new ParquetFileWriter(outputPath, columns);
                    using var rowGroupWriter = writer.AppendRowGroup();
                    using (var timeWriter = rowGroupWriter.NextColumn().LogicalWriter<DateTime>())
                        timeWriter.WriteBatch(timestamps.ToArray());

                    using (var valueWriter = rowGroupWriter.NextColumn().LogicalWriter<double>())
                        valueWriter.WriteBatch(values.ToArray());

                    writer.Close();
                }
                catch (Exception exFile)
                {
                    Console.WriteLine($"Error creating Parquet file for date {dateKey}: {exFile.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing Parquet file: {ex.Message}");
        }

        return dateFiles;
    }


    public static Dictionary<string, Tuple<int, double>> AverageForEachHour(string filePath)
    {
        Dictionary<string, Tuple<int, double>> result = new Dictionary<string, Tuple<int, double>>();
        try
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var dateAndTime = parts[0].Trim();
                var valueStr = parts[1].Trim();

                //Checking if the value can be converted to a double
                if (double.TryParse(valueStr, out double value) && !double.IsNaN(value))
                {
                    // Round the hour to the nearest full hour
                    var dateTime = DateTime.ParseExact(dateAndTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    var roundedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

                    string key = roundedDateTime.ToString("dd/MM/yyyy HH:00");
                    
                    if (result.ContainsKey(key))
                    {
                        Tuple<int, double> existingValue = result[key];

                        double newSum = existingValue.Item2 + value;
                        int newCount = existingValue.Item1 + 1;

                        result[key] = new Tuple<int, double>(newCount, newSum);

                        double roundedSum = Math.Round(newSum, 2);
                    }
                    else
                    {
                        result[key] = new Tuple<int, double>(1, value);
                        double roundedValue = Math.Round(value, 2);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה: {ex.Message}");
        }
        // sort by key
        var sortedResult = result.OrderBy(entry => DateTime.ParseExact(entry.Key, "dd/MM/yyyy HH:00", CultureInfo.InvariantCulture)).ToList();

        foreach (var entry in sortedResult)
        {
            string key = entry.Key;
            int count = entry.Value.Item1;
            double sum = entry.Value.Item2;

            double avg = Math.Round(sum / count, 2);

            Console.WriteLine($"Timestamp: {key},  avg: {avg}");
        }
        return result;
    }

    public static Dictionary<string, Tuple<int, double>> AverageForEachHourParquet(string filePath)
    {
        Dictionary<string, Tuple<int, double>> result = new Dictionary<string, Tuple<int, double>>();

        try
        {
            using var file = new ParquetFileReader(filePath);

            // Ensure that we have at least one row group in the file
            for (int rowGroup = 0; rowGroup < file.FileMetaData.NumRowGroups; ++rowGroup)
            {
                using var rowGroupReader = file.RowGroup(rowGroup);
                int groupNumRows = (int)rowGroupReader.MetaData.NumRows;

                // Reading timestamps (column 0 - DateTime, not string)
                var groupTimestamps = rowGroupReader.Column(0).LogicalReader<DateTime>().ReadAll(groupNumRows);

                // Reading values (column 1 - values)
                var groupValues = rowGroupReader.Column(1).LogicalReader<double>().ReadAll(groupNumRows);

                for (int i = 0; i < groupNumRows; ++i)
                {
                    if (groupTimestamps[i] != DateTime.MinValue && groupValues[i] != null)
                    {
                        // Round the timestamp to the nearest hour
                        var roundedDateTime = new DateTime(groupTimestamps[i].Year, groupTimestamps[i].Month, groupTimestamps[i].Day, groupTimestamps[i].Hour, 0, 0);
                        string key = roundedDateTime.ToString("yyyy-MM-dd HH:00");

                        if (result.ContainsKey(key))
                        {
                            // Update the existing entry
                            var existingValue = result[key];
                            double newSum = existingValue.Item2 + groupValues[i];
                            int newCount = existingValue.Item1 + 1;

                            result[key] = new Tuple<int, double>(newCount, newSum);
                        }
                        else
                        {
                            result[key] = new Tuple<int, double>(1, groupValues[i]);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing Parquet file: {ex.Message}");
        }

        // Sort the results by date and time (hourly)
        var sortedResult = result.OrderBy(entry => DateTime.ParseExact(entry.Key, "yyyy-MM-dd HH:00", CultureInfo.InvariantCulture)).ToList();

        foreach (var entry in sortedResult)
        {
            string key = entry.Key;
            int count = entry.Value.Item1;
            double sum = entry.Value.Item2;

            double avg = Math.Round(sum / count, 2);
            Console.WriteLine($"Timestamp: {key}, avg: {avg}");
        }

        return result;
    }


}
//Section B.3
//הייתי מתכננת לפי הצורך, במידה ונידרש עידכון מיידי עבור כל נתון
//עבור כל שעה נחשב מחדש ממוצע בכל פעם שיתקבל נתון, כך שבמקום לשמור את כל הנתונים 
// נצרף את הערכים מהשורה שהתקבלה לממוצע שכבר קיים לנו עבור השעה הנוכחית ,ונחשב ממוצע חדש.
//ע"פ צורך גם נשמור את המידע עבור כל שעה שעברה במבנה נתונים



//Section B.3
//הקובץ מאחסן מעמודות, לכן במקרים שלא צריך לקרא את כל העמודות,
//הגישה תהיה מהירה יותר מאשר פורמטים אחרים כמו
//CSV או JSON, שבהם הנתונים נשמרים בצורה שורתית