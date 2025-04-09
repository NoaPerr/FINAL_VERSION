using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Hadasim_A
{
    class Program
    {
        static void Main()
        {
           
            Console.Write("Enter file path including file name: ");
            string filePathFromUser = Console.ReadLine();
            //File valiation
            if (!File.Exists(filePathFromUser))//File valiation
            {
                Console.WriteLine("Error: File not found");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Enter the number of common errors:");
            string n_text = Console.ReadLine();
            int N = 0;

            Int32.TryParse(n_text, out N);
            //num valiation
            if (N == 0)
            {
                Console.WriteLine(" Error: Please enter a valid positive number greater than 0.");
                Console.ReadKey();
                return;
            }

            try
            {
                var result = GetTopErrorCodes(filePathFromUser, N);
                Console.WriteLine("Top error codes:");
                foreach (var error in result)
                {
                    Console.WriteLine($"Error: {error.Key}, Count: {error.Value}");
                }

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("");
            Console.ReadKey();
        }
        public static List<KeyValuePair<string, int>> GetTopErrorCodes(string filefullPath, int N)
        {
            // Opens the file as a stream for reading
            using (FileStream fileStream = new FileStream(filefullPath, FileMode.Open, FileAccess.Read))
            {
                var splitFiles = SplitFile(fileStream, 100000); 

                var errorCounts = ErrorCounter(splitFiles);

                // Sort by error count and create a list of the top N most common errors
                return errorCounts.OrderByDescending(kv => kv.Value).Take(N).ToList();
            }
        }

        // splits a file into chunks of specified line count
        public static List<MemoryStream> SplitFile(Stream inputFileStream, int lines)
        {
            List<MemoryStream> ListSplitFiles = new List<MemoryStream>();
            StreamReader reader = new StreamReader(inputFileStream, Encoding.UTF8);
            string line;
            MemoryStream memoryStream = null;
            StreamWriter writer = null;
            int lineCounter = 0;

            while ((line = reader.ReadLine()) != null)
            {
                if (lineCounter % lines == 0)
                {
                    /// If there's a previous writer, finish it and start a new chunk
                    if (writer != null)
                    {
                        writer.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin); /// Reset the position to the beginning
                    }

                    // Create a new MemoryStream to store the chunk
                    memoryStream = new MemoryStream();
                    writer = new StreamWriter(memoryStream, Encoding.UTF8);
                    ListSplitFiles.Add(memoryStream);  
                }

               
                writer.WriteLine(line);
                lineCounter++;
            }
            //Ensure the last content is written 
            if (writer != null)
            {
                writer.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);  // Reset to the beginning 
            }

            return ListSplitFiles;
        }


        // Counts the errors in all chunks
        public static Dictionary<string, int> ErrorCounter(List<MemoryStream> fileStreams)
        {
            var allErrors = new Dictionary<string, int>();

            
            foreach (var stream in fileStreams)
            {
                stream.Seek(0, SeekOrigin.Begin);  // For each chunk, count the errors
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string errorCode = codeName(line);
                        if (errorCode != null)
                        {
                            if (!allErrors.ContainsKey(errorCode))
                            {
                                allErrors[errorCode] = 0;
                            }
                            allErrors[errorCode]++; 
                        }
                    }
                }
            }

            return allErrors;
        }

        // Extracts the error code from the log line
        public static string codeName(string line)
        {
            int errorIndex = line.IndexOf("Error:");
            if (errorIndex != -1)
            {
                return line.Substring(errorIndex + 7).Trim();  // Assuming the error code starts after "Error: "
            }
            return null;
        }

    }
}

//סיבוכיות זמן : O(L + K log K).
//פירוט:
//   O(K log K) GetTopErrorCodes
//K הוא מספר השגיאות 
//הפונקציה קוראת ל:
//  O(L) SplitFile
// כאשר L הוא מספר השורות בקובץ
// O(L) ErrorCounter
// כאשר L הוא מספר השורות בקובץ
// codeName O(1)



//סיבוכיות מקום כוללת: O(L + E)
//L הוא מספר השורות בקובץ
//E הוא מספר השגיאות הייחודיות
