using DataBaseFileManager;
using FindingHighModulationPatients;
using FindingHAPatients;
using SendEmailCSharp;
namespace DataMiningBrachy
{
    class Program
    {
        static void QueryAria()
        {
            List<PatientAriaQuery> patients = AriaQueryActivities.QueryAria();
            string csv_path = ".\\Results.csv";
            string directoryPath = Path.GetDirectoryName(csv_path);
            if (string.IsNullOrEmpty(directoryPath))
                directoryPath = ".";
            PatientListToCSV(issuePatients, csv_path);
        }
        static void Main(string[] args)
        {
            //string[] k = { "BrianAnderson@health.ucsd.edu" };
            //SendEmailClass.SendEmail("rmanger@health.ucsd.edu", k, "Great email!", "I love it, great job");
            //DataMiningImaging.MainImaging();
            QueryAria();
            return;
            //MainRun.Update();
            //FileSync.SyncFilesParallel(@"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases", @"C:\Users\BRA008\Modular_Projects\LocalDatabases", maxDegreeOfParallelism: -1);
            //return;
            //FindModulationPatientsClass findModulationPatients = new FindModulationPatientsClass();
            //findModulationPatients.Main(args);
            FindHAPatientsClass.MainRun();
            //DataMiningReTreat.MainRun();
            return;
            DataMiningCylinder dataMiningCylinder = new DataMiningCylinder();
            dataMiningCylinder.Main(args);
            DataMiningSkin dataMiningSkin = new DataMiningSkin();
            dataMiningSkin.Main(args);
        }
    }
}