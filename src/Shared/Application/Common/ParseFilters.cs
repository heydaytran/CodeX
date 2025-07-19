using System.Text.RegularExpressions;

namespace Application.Common;

public static class FilterExtensions
{
    public static List<QueryFilter> ParseFilters(string filters)
    {
        var filterList = new List<QueryFilter>();
        if (string.IsNullOrEmpty(filters)) return filterList;

        var parts = filters.Split(",");
        foreach (var part in parts)
        {
            var match = Regex.Match(part, @"(?<field>\w+)(?<op>[=><@]+)(?<value>.+)");
            if (match.Success)
            {
                filterList.Add(new QueryFilter
                {
                    PropertyName = match.Groups["field"].Value,
                    Operator = match.Groups["op"].Value,
                    Value = match.Groups["value"].Value
                });
            }
        }

        return filterList;
    }
}
