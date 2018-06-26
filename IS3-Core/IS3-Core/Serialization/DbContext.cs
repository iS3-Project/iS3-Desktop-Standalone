using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;

namespace IS3.Core.Serialization
{
    // Provide a ready-use database context.
    // 
    public class DbContext
    {
        protected DbAdapter _adapter;
        protected DbConnection _connection;
        protected bool _isOpened;

        // option:
        //  0 - odbc connection
        //  1 - oledb connection
        public DbContext(string dbFileName, int option = 0)
        {
            if (option == 0)
                _adapter = new OdbcAdapter(dbFileName);
            else
                _adapter = new OleDbAdapter(dbFileName);

            _connection = _adapter.NewConnection();
        }

        ~DbContext()
        {
            Close();
        }

        public bool Open()
        {
            if (_isOpened)
                return true;
            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                string error = "Open database file failed. Connection string = '" 
                    + _adapter.ConnectionStr + "'";
                ErrorReport.Report(error);
                _isOpened = false;
                return false;
            }

            _isOpened = true;
            return true;
        }

        public bool Close()
        {
            if (!_isOpened )
                return false;

            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _isOpened = false;
            }

            return true;
        }

        public DbDataReader ExecuteCommand(string strCmd)
        {
            bool success = Open();
            if (!success)
                return null;

            return _adapter.ExcuteCommand(_connection, strCmd);
        }

        public DbDataAdapter GetDbDataAdapter(string strCmd)
        {
            return _adapter.GetDbDataAdapter(_connection, strCmd);
        }

        public bool IsTableExist(string tableName)
        {
            bool success = Open();
            if (!success)
                return false;

            DataTable dt = _connection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string currentTable = row["TABLE_NAME"].ToString();
                if (currentTable == tableName)
                    return true;
            }
            return false;
        }

        public string TableNamePrefix
        {
            get { return _adapter.TableNamePrefix; }
        }

        public override string ToString()
        {
            string str = string.Format("DbContext: adapter={0}, isOpened={1}",
                _adapter.GetType().ToString(), _isOpened);
            return str;
        }
    }
}
