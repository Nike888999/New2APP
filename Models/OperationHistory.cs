using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAPP.Models
{
    public class OperationHistory
    {
        public int Id { get; set; }              // Уникальный номер записи
        public int NomenclatureId { get; set; }  // ID товара (связь с таблицей nomenclature)
        public string OperationType { get; set; } // Тип операции: "Добавление", "Списание", "Удаление"
        public int Quantity { get; set; }         // Количество (сколько добавили/списали)
        public DateTime OperationDate { get; set; } // Дата и время операции
        public string UserName { get; set; }      // Имя пользователя, который выполнил операцию
    }
}
