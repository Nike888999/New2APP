using Microsoft.Data.Sqlite;
using NewAPP.Models;
using NewAPP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAPP2.Interface
{
    public interface IDataBaseService
    {
       
        
         User Authenticate ( string login, string password );

         bool AddUser ( string login, string password, string role );

         List<NomenclatureUnit> AllNum ( );

         bool AddNomenclature ( string name, string internalArticle, string externalArticle, string characteristic, string serialNumber, string unit, string addressCell, int oldQuantity, int operationTypeIn, int operationTypeOut, int newQuantity, int UnitPrice, int WeightUnit );

         bool DeleteNomenclature ( string name );

         List<NomenclatureItems> SearchNomenclature ( string searchText );

         bool DeleteUnit ( string name, int amount );

         bool AddUnit ( string name, int amount );

         List<Terminal> GetTerminals ( );

         ObservableCollection<Sensor> GetSensorsByTerminalId ( int terminalId );

         void SaveTerminal ( Terminal terminal );

         void UpdateTerminal ( Terminal terminal );

         bool DeleteTerminal ( int terminalId );

         List<NomenclatureItems> SearchUnit ( string searchText );

         List<OperationHistory> GetOperationHistory ( int nomenclatureId );

         void SaveNomenclature ( int nomenclatureId, string operationType, int quantity, string userName );
        

    }
}
