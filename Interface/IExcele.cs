using Microsoft.Win32;
using NewAPP.Models;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NewAPP.Services.Excele;
using System.Windows;

namespace NewAPP2.Interface
{
    public interface IExcele
    {
        bool ReportToExcele ( List<NomenclatureUnit> data, DateTime startDate, DateTime endDate,
                   string searchName, bool allProducts, ReportPeriodType periodType );
        bool ExportToExele ( List<NomenclatureUnit> data, DateTime startDate, DateTime endDate,
                                  string searchName, bool allProducts, ReportPeriodType periodType, int? prodictId = null );
        List<string> GetAllProductNames ( List<NomenclatureUnit> allData );
        List<DateTime> GetPeriods ( DateTime startDate, DateTime endDate, ReportPeriodType periodType );
        IEnumerable<NomenclatureUnit> GetOperationsForPeriod (
           IGrouping<object, NomenclatureUnit> nomGroup,
           DateTime period,
           ReportPeriodType periodType );
         string GetPeriodTypeName ( ReportPeriodType periodType );
         string GetPeriodLabel ( DateTime period, ReportPeriodType periodType );
        
    }
}
