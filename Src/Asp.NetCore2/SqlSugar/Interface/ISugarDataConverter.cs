using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public interface ISugarDataConverter
    {
        SugarParameter ParameterConverter<T>(object columnValue, int columnIndex);

        T QueryConverter<T>(IDataRecord dataRecord, int dataRecordIndex);
    }
}
