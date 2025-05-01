using System.Data;

namespace Wbskt.Common.Providers
{
    internal static class SqlHelper
    {
        public static DataTable ToDataTable(this IEnumerable<(int Id, int IntValue)> kvs)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("IntValue", typeof(int));

            foreach (var (id, intValue) in kvs)
            {
                table.Rows.Add(id, intValue);
            }

            return table;
        }

        public static DataTable ToDataTable(this IEnumerable<int> ids)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }

            return table;
        }
    }
}
