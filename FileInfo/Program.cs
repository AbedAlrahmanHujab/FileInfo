namespace ConsoleApp1
{
    class Program
    {
        public static string[] args;
        Program(string[] args)
        {
            Program.args = args;
        }
        private void Run()
        {
            //Read the file with helping of FileControllar that return object contain file infomation
            var fileinfo = FileControllar.ReadFile(args[0]);

            //search for the file name if it's exist inside the file text
            var wordCounts = fileinfo.WordCounter(fileinfo.FileName);

            //List all words in the doc
            fileinfo.WordsViewer();
            Console.WriteLine();

            //Print the final massage
            var massageToPrint = wordCounts > 0 ? "The file name " + fileinfo.FileName + " is existed in file text " + wordCounts + " time(s)." : "The word is not exist";
            Console.WriteLine(massageToPrint);
        }
        static void Main(string[] args)
        {
            Program program = new Program(new string[] { @"c:\abed.txt" });
            program.Run();
        }
    }
}

/// <summary>
/// FileInfo class used to present property of a file 
/// </summary>
public class FileInfo
{
    private readonly byte[]? _fileData;
    private readonly bool _isEmptyFile;
    private readonly string _fileName = "";
    private readonly string _fileExt = "";

    public FileInfo()
    {

    }
    public FileInfo(byte[] filedata, string filename)
    {
        if (filedata != null)
        {
            _fileData = filedata;
            _isEmptyFile = _fileData.Length == 0;
            _fileExt = Path.GetExtension(filename);
            _fileName = Path.GetFileNameWithoutExtension(filename);
        }
    }

    public string FileName { get { return _fileName; } }
    public string FileExt { get { return _fileExt; } }

    public bool IsEmptyFile
    {
        get
        {
            return _isEmptyFile;
        }
    }

    public bool IsTextFile
    {
        get
        {
            //check empty file
            if (IsEmptyFile || _fileData == null)
            {
                Console.WriteLine("No contant to check.The file is empty or not exist!");
                return false;
            }

            //check contant of the file text or binary
            if (_fileData != null)
                for (int i = 1; i < 512 && i < _fileData.Length; i++)
                    if (_fileData[i] == 0x00 && _fileData[i - 1] == 0x00)
                        return false;

            //It's a text file
            return true;
        }

    }

    public int WordCounter(string wordToCount)
    {
        if (!_isEmptyFile && _fileData != null && IsTextFile)
        {
            //Get string from the file bytes
            var fileAsString = System.Text.Encoding.Default.GetString(_fileData);

            //split text into words
            var words = fileAsString.Split(new string[] { "\r\n", "\r", "\n", " " }, StringSplitOptions.None);

            //group words into groups
            var wordGroups =
              from word in words
              group word by word into groupByWord
              select new { word = groupByWord.Key, wordCounts = groupByWord.Count() };

            //get count of the wanted word
            var result = wordGroups.FirstOrDefault(w => w.word == wordToCount);

            if (result != null)
                return result.wordCounts;
        }
        //0 if the file is empty or not a text file or the word is not found in the file
        return 0;
    }

    public void WordsViewer()
    {
        if (!_isEmptyFile && _fileData != null && IsTextFile)
        {
            var fileAsString = System.Text.Encoding.Default.GetString(_fileData)
                .Replace("."," ")
                .Replace(","," ");
            var words = fileAsString.Split(new string[] { "\r\n", "\r", "\n", " " }, StringSplitOptions.None);

            var wordGroups =
              from word in words
              group word by word into groupByWord
              select new { word = groupByWord.Key, wordCounts = groupByWord.Count() };
             
            // Test the results
            foreach (var wordGroup in wordGroups)
                Console.WriteLine("(" + wordGroup.wordCounts + ") of " + wordGroup.word);
        }
    }
}

/// <summary>
/// Class helper to read text from the hard disk.....etc
/// </summary>
public static class FileControllar
{
    public static FileInfo ReadFile(string filepath)
    {
        //check the file if existing 
        if (!File.Exists(filepath))
        {
            Console.WriteLine("The file is not found check the path & name of the file");
            return null;
        }
        var filebytes = File.ReadAllBytes(filepath);
        return new FileInfo(filebytes, filepath);
    }
}