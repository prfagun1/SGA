

namespace SGA.Interfaces
{
    public interface IDataImport
    {
        void ImportAll();
        bool ImportUserHRList();

        bool ImportNewEmployees();
    }
}
