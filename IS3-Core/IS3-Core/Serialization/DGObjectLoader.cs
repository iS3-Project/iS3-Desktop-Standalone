using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core.Serialization
{
    // This is a default object loader for DGObject.
    // You are encouraged to write your object loader for each 
    // new class that derived from DGObject
    //
    public class DGObjectLoader
    {
        protected DbDataLoader _dbLoader;

        public DGObjectLoader(DbContext dbContext)
        {
            _dbLoader = new DbDataLoader(dbContext);
        }

        public bool Load(DGObjects objs)
        {
            DGObjectsDefinition def = objs.definition;
            if (def == null)
                return false;

            bool success = _dbLoader.ReadDGObjects(objs, 
                def.TableNameSQL, def.DefNamesSQL, def.OrderSQL, def.ConditionSQL);

            return success;
        }
    }
}
