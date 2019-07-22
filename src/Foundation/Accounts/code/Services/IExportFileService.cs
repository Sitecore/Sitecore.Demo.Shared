namespace Sitecore.Demo.Foundation.Accounts.Services
{
    public interface IExportFileService
    {
        string CreateExportFile();

        void WriteExportedDataIntoFile(string filePath, string exportedData);

        byte[] ReadExportedDataFromFile(string filename);
    }
}