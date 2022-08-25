using System.IO;
using System.Text.RegularExpressions;

namespace Libx
{
    public class CsvReader
    {
        public static void Read(string pfname, Func<string[], bool> callback)
        {
            //https://www.nuget.org/packages/CsvHelper/
            //1,"North Las Vegas Airport","Las Vegas","United States","VGT","KVGT",36.2106944,-115.1944444,2205,-8,"U","America/Los_Angeles"
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            using (StreamReader reader = new StreamReader(pfname))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    if (line.Length > 0)
                    {
                        string[] fields = CSVParser.Split(line);

                        bool continueNextLine = callback(fields);
                        if (!continueNextLine)
                        {
                            break;
                        }
                    }
                    line = reader.ReadLine();
                }
            }
        }
        //===================================================================================================
    }
}
