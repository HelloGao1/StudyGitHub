using System.Configuration;

namespace Partner.Data.Integration.Utils
{
    /// <summary>
    /// Represents a class to strore settings for easy access.
    /// </summary>
    public static class AppSettings
    {
        public static readonly string FhirAPIBaseUrl;
        public static readonly string LiveDataBaseUrl;
        public static readonly string HistoricBaseUrl;
        public static readonly string DIImageBaseUrl;
        public static readonly string DICSImageUserName;
        public static readonly string DICSImagePassword;
        public static readonly string DICSImageStudyInstanceUID;

        public static readonly bool StoredPatientInAzure;
        public static readonly string AzureStorageContainerName;
        public static readonly string AzureStorageConnectionName;
        public static readonly string StorageConnectionString;

        static AppSettings()
        {
            FhirAPIBaseUrl = ConfigurationManager.AppSettings["FhirAPIBaseUrl"];
            LiveDataBaseUrl = ConfigurationManager.AppSettings["LiveDataBaseUrl"];
            HistoricBaseUrl = ConfigurationManager.AppSettings["HistoricBaseUrl"];
            DIImageBaseUrl = ConfigurationManager.AppSettings["DIImageBaseUrl"];
            DICSImageUserName = ConfigurationManager.AppSettings["DICSImageUserName"];
            DICSImagePassword = ConfigurationManager.AppSettings["DICSImagePassword"];
            DICSImageStudyInstanceUID = ConfigurationManager.AppSettings["DICSImageStudyInstanceUID"];

            StoredPatientInAzure = ConfigurationManager.AppSettings["StoredPatientInAzure"].ToLower() == "true";
            AzureStorageContainerName = ConfigurationManager.AppSettings["AzureStorageContainerName"];
            AzureStorageConnectionName = ConfigurationManager.AppSettings["AzureStorageConnectionName"];
            StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
        }
    }
}