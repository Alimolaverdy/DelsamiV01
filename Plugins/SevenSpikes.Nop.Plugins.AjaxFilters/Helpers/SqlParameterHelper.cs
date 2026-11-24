using System;
using System.Data;
using LinqToDB;
using LinqToDB.Data;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public static class SqlParameterHelper
{
    private static DataParameter GetParameter(DataType dbType, string parameterName, object parameterValue)
    {
        return new DataParameter
        {
            Name = parameterName,
            Value = (parameterValue ?? DBNull.Value),
            DataType = dbType
        };
    }

    private static DataParameter GetOutputParameter(DataType dbType, string parameterName)
    {
        return new DataParameter
        {
            Name = parameterName,
            DataType = dbType,
            Direction = ParameterDirection.Output
        };
    }

    public static DataParameter GetStringParameter(string parameterName, string parameterValue)
    {
        return GetParameter((DataType)5, parameterName, parameterValue);
    }

    public static DataParameter GetOutputStringParameter(string parameterName)
    {
        return GetOutputParameter((DataType)5, parameterName);
    }

    public static DataParameter GetInt32Parameter(string parameterName, int? parameterValue)
    {
        return GetParameter((DataType)15, parameterName, parameterValue);
    }

    public static DataParameter GetOutputInt32Parameter(string parameterName)
    {
        return GetOutputParameter((DataType)15, parameterName);
    }

    public static DataParameter GetBooleanParameter(string parameterName, bool? parameterValue)
    {
        return GetParameter((DataType)11, parameterName, parameterValue);
    }

    public static DataParameter GetDecimalParameter(string parameterName, decimal? parameterValue)
    {
        return GetParameter((DataType)23, parameterName, parameterValue);
    }

    public static DataParameter GetDateTimeParameter(string parameterName, DateTime? parameterValue)
    {
        return GetParameter((DataType)29, parameterName, parameterValue);
    }
}
