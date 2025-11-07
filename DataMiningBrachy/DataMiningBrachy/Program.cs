using DataBaseFileManager;
using FindingHighModulationPatients;
using FindingHAPatients;
namespace DataMiningBrachy
{
    class Program
    {
        static void QueryAria()
        {
            AriaQueryActivities.AriaToCSV();
        }
        static void Main(string[] args)
        {
            DataMiningImaging.MainImaging();
            //QueryAria();
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