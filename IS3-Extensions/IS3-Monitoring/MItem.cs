using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.Monitoring.Serialization;

namespace IS3.Monitoring
{
    // Summary:
    //    Monitoring Point
    public class MonPoint: DGObject
    {
        // Summary:
        //    reference point name
        public string refPointName { get; set; }
        // Summary:
        //    distance to the reference point
        public double? distanceX { get; set; }
        public double? distanceY { get; set; }
        public double? distanceZ { get; set; }
        // Summary:
        //    Installation date and time
        public DateTime? time { get; set; }
        // Summary:
        //    Instrument detail
        public string instrumentDetail { get; set; }
        // Summary:
        //    Bearing of monitoring axis (A, B, C): in degree
        public double? bearingA { get; set; }
        public double? bearingB { get; set; }
        public double? bearingC { get; set; }
        // Summary:
        //    Inclination of instrument axis (A, B, C): in degree
        public double? inclinationA { get; set; }
        public double? inclinationB { get; set; }
        public double? inclinationC { get; set; }
        // Summary:
        //    Reading sign convention in direction (A, B, C)
        public string readingSignA { get; set; }
        public string readingSignB { get; set; }
        public string readingSignC { get; set; }
        // Summary:
        //    componennt count
        public int componentCount { get; set; }
        // Summary:
        //    Component names
        public string componentNames { get; set; }
        // Summary:
        //    Remarks
        public string remarks { get; set; }
        // Summary:
        //    Contractor who installed monitoring instrument
        public string contractor { get; set; }
        // Summary:
        //    Associated file reference
        public string fileName { get; set; }

        // Summary:
        //    readings dictionary - reading component indexed 
        public Dictionary<string, List<MonReading>> readingsDict;

        public List<MonReading> this[string key]
        {
            get { return readingsDict[key]; }
        }

        public MonPoint()
        {
            readingsDict = new Dictionary<string, List<MonReading>>();
        }
        public MonPoint(DataRow rawData)
            : base(rawData)
        {
            readingsDict = new Dictionary<string, List<MonReading>>();
        }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            MonitoringDGObjectLoader loader =
                new MonitoringDGObjectLoader(dbContext);
            bool success = loader.LoadMonPoints(objs);
            return success;
        }

        public override string ToString()
        {
            string str = base.ToString();

            ICollection<string> keys = readingsDict.Keys;
            string strKeys = ", Keys=";
            foreach (string key in keys)
            {
                strKeys += key + ",";
            }
            str += strKeys;

            return str;
        }

        public override List<DataView> detailViews(IEnumerable<DGObject> objs)
        {
            List<DataView> dataViews = base.detailViews(objs);

            DataTable table = parent.rawDataSet.Tables[1];
            string filter = nameFilter(objs);
            DataView view = new DataView(table, filter, "[monPointName]",
                DataViewRowState.CurrentRows);
            dataViews.Add(view);

            return dataViews;
        }

        string nameFilter(IEnumerable<DGObject> objs)
        {
            string sql = "monPointName in (";
            foreach (var obj in objs)
            {
                sql += '\'' + obj.name + '\'';
                sql += ",";
            }
            sql += ")";
            return sql;
        }

    }

    // Summary:
    //     Monitoring Readering
    public class MonReading
    {
        // Summary:
        //     monitoring point name
        public string monPointName { get; set; }
        // Summary:
        //     reading date and time
        public DateTime time { get; set; }
        // Summary:
        //     reading component
        public string component { get; set; }
        // Summary:
        //    raw reading
        public string reading { get; set; }
        // Summary:
        //    reading value
        public double value { get; set; }
        // Summary:
        //    reading unit
        public string unit { get; set; }

        public override string ToString()
        {
            return string.Format(
                "pnt={0}, time={1}, comp={2}, read={3}, val={4}, unit={5}",
                monPointName, time, component, reading, value, unit);
        }
    }

    // Summary:
    //    Monitoring Group
    public class MonGroup: DGObject
    {
        // Summary:
        //    Shape of the group: linear, bezier
        public string groupShape { get; set; }
        // Summary:
        //    Monitoring points names in the group
        public string[] monPntNames { get; set; }
        // Summary:
        //   Monitoring points
        public Dictionary<string, MonPoint> monPntDict { get; set; }

        public MonPoint this[string key]
        {
            get { return monPntDict[key]; }
        }

        public MonGroup()
        {
            monPntDict = new Dictionary<string, MonPoint>();
        }

        public MonGroup(DataRow row)
            : base(row)
        {
            monPntDict = new Dictionary<string, MonPoint>();
        }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            MonitoringDGObjectLoader loader =
                new MonitoringDGObjectLoader(dbContext);
            bool success = loader.LoadMonGroups(objs);
            return success;
        }

        public override string ToString()
        {
            string str = base.ToString();

            ICollection<string> keys = monPntDict.Keys;
            string strKeys = ", Keys=";
            foreach (string key in keys)
            {
                strKeys += key + ",";
            }
            str += strKeys;

            return str;
        }
    }


}
