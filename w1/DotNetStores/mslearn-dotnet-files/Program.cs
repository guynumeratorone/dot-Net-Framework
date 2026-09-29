using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(
    Path.Combine(salesTotalDir, "totals.txt"),
    $"{salesTotal}{Environment.NewLine}"
);

GenerateSalesSummary(
    salesFiles,
    Path.Combine(salesTotalDir, "sales-summary.txt")
);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(
        folderName,
        "*",
        SearchOption.AllDirectories
    );

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);

        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, string outputFile)
{
    double totalSales = 0;
    StringBuilder report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");

    StringBuilder details = new StringBuilder();

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        double fileTotal = data?.Total ?? 0;

        totalSales += fileTotal;

        string fileName = Path.GetRelativePath(
            Directory.GetCurrentDirectory(),
            file
        );

        details.AppendLine($" {fileName}: {fileTotal:C}");
    }

    report.AppendLine($" Total Sales: {totalSales:C}");
    report.AppendLine();
    report.AppendLine(" Details:");
    report.Append(details);

    File.WriteAllText(outputFile, report.ToString());
}

record SalesData(double Total);