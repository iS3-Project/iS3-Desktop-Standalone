using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Windows;

namespace iS3.Config
{
    public class DbHelper
    {
        static List<string> _tableNames = null;
        public static char[] Separator = new char[] { ',' };
        public static string TablePrefix = "dbo_";

        // Load MDB file table names, skip system tables
        //
        static bool LoadMDBTableNames(string file, List<string> tableNames)
        {
            string connStr = "DSN=MS Access Database;DBQ=" + file;
            OdbcConnection con = new OdbcConnection(connStr);

            try
            {
                con.Open();
                DataTable dt = con.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    string tableName = row[2].ToString();
                    string sys = tableName.Substring(0, 4);
                    // Skip Access system tables
                    if (sys == "MSys")
                        continue;
                    tableNames.Add(tableName);
                }
                con.Close();
                return true;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return false;
            }
        }

        public static List<string> GetDbTablenames(string file)
        {
            if (_tableNames == null)
            {
                _tableNames = new List<string>();
                LoadMDBTableNames(file, _tableNames);
            }

            return _tableNames;
        }

        public static DataSet LoadTable(string file, string tableNameSQL, string conditionSQL, string orderSQL)
        {
            string connStr = "DSN=MS Access Database;DBQ=" + file;
            OdbcConnection con = new OdbcConnection(connStr);
            DataSet dataset = new DataSet();

            try
            {
                con.Open();

                // tableNameSQL,orderSQL,conditionSQL may contain
                // multiple table names speratored by comma
                string[] names = tableNameSQL.Split(Separator);
                string[] orders = null;
                string[] conditions = null;
                if (orderSQL != null)
                    orders = orderSQL.Split(Separator);
                if (conditionSQL != null)
                    conditions = conditionSQL.Split(Separator);

                for (int i = 0; i < names.Count(); ++i)
                {
                    string tableName = names[i];
                    string strCmd = "SELECT * FROM " + tableName + "";

                    if (conditions != null && i < conditions.Count())
                        strCmd += WhereSQL(null, conditions[i]);
                    if (orders != null && i < orders.Count())
                        strCmd += OrderSQL(orders[i]);

                    DbDataAdapter adapter = new OdbcDataAdapter(strCmd, con);
                    adapter.Fill(dataset, tableName);
                }

                con.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }

            return dataset;
        }

        // Convert names and contitionSQL to SQL WHERE clause, such as
        //      WHERE [LineNo]=1 AND ([Name]='1' OR [Name]='2')
        public static string WhereSQL(string[] names, string conditionSQL)
        {
            string where = "";
            bool bWhereAdded = false;
            if (conditionSQL != null && conditionSQL.Length > 0)
            {
                if (conditionSQL.Contains("WHERE") == false)
                    where += " WHERE ";
                where += conditionSQL;
                bWhereAdded = true;
            }

            bool IsNameEmpty = false;
            if (names == null)
                IsNameEmpty = true;
            else if (names.Length == 1 && names[0].Length == 0)
                IsNameEmpty = true;

            if (!IsNameEmpty)
            {
                if (bWhereAdded == false)
                    where += " WHERE ";
                else
                    where += " AND ";
                where += NamesToSQL(names);
            }

            return where;
        }

        // Convert names to SQL format, such as:
        //      ([Name]='1' OR [Name]='2')
        public static string NamesToSQL(string[] names)
        {
            bool first = true;
            string sql = "(";
            foreach (string name in names)
            {
                if (first == false)
                    sql += " OR ";
                sql += "[Name]='" + name + "'";
                first = false;
            }
            sql += ")";
            return sql;
        }


        public static string OrderSQL(string orderSQL)
        {
            if (orderSQL != null && orderSQL.Length > 0)
                return " ORDER BY " + orderSQL;
            else
                return "";
        }

    }
}
