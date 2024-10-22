using CMS.DataEngine;

using Kentico.Xperience.Admin.Base.Filters;

namespace XperienceCommunity.CSP.UIPages.CspViolationReport;

internal sealed class DateFromWhereConditionBuilder : IWhereConditionBuilder
{
    public Task<IWhereCondition> Build(string columnName, object value)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            throw new ArgumentException($"{nameof(columnName)} cannot be a null or empty string.");
        }

        var whereCondition = new WhereCondition();

        if (value == null)
        {
            return Task.FromResult<IWhereCondition>(whereCondition);
        }

        if (value is not DateTime dateTime)
        {
            throw new ArgumentException(
                $"Invalid type of the {nameof(value)} argument passed to the {nameof(DateFromWhereConditionBuilder)}. "
                + $"The expected type is collection of {nameof(DateTime)}.");
        }


        whereCondition.WhereGreaterOrEquals(columnName, dateTime);

        return Task.FromResult<IWhereCondition>(whereCondition);
    }
}