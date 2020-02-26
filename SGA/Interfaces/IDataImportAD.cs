using SGA.Models;
using System.Linq;

namespace SGA.Interfaces
{
    public interface IDataImportAD 
    {
        void ImportADConsultaUsuariosSistema(IQueryable<ApplicationAD> connectionADIQueryable);

    }
}
