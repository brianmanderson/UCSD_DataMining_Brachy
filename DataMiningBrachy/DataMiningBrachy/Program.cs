using DataBaseFileManager;
using FindingHighModulationPatients;
namespace DataMiningBrachy
{
    class Program
    {
        static void Main(string[] args)
        {
            //MainRun.Update();
            //FileSync.SyncFilesParallel(@"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases", @"C:\Users\BRA008\Modular_Projects\LocalDatabases", maxDegreeOfParallelism: -1);
            //FindModulationPatientsClass findModulationPatients = new FindModulationPatientsClass();
            //findModulationPatients.Main(args);
            DataMiningReTreat.MainRun();
            return;
            DataMiningCylinder dataMiningCylinder = new DataMiningCylinder();
            dataMiningCylinder.Main(args);
            DataMiningSkin dataMiningSkin = new DataMiningSkin();
            dataMiningSkin.Main(args);
        }
    }
}