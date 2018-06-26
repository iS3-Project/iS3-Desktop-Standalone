using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Markup;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.Serialization;

using IS3.Core;
using IS3.Core.Graphics;

namespace IS3.Core.Serialization
{
    public class IS3XmlProjectSerializer
    {
        public static void LoadDefinition(Project prj, string filepath)
        {
            StreamReader reader = new StreamReader(filepath);

            XElement root = XElement.Load(reader);
            if (root.Name != "Project")
                return;

            XNamespace is3 = "clr-namespace:IS3.Core;assembly=IS3.Core";
            XElement node = root.Element(is3 + "ProjectDefinition");
            if (node != null)
            {
                object obj = XamlReader.Parse(node.ToString());
                prj.projDef = (ProjectDefinition)obj;
            }

            reader.Close();
        }

        public static void WriteProject(Project project, string filepath)
        {
            StreamWriter writer = new StreamWriter(filepath);
            XElement xe = ProjectToXml(project);
            XDocument doc = new XDocument(xe);
            doc.Save(writer);
            writer.Close();
        }

        public static void ReadProject(Project project, string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            XElement root = XElement.Load(reader);
            if (root.Name != "Project")
                return;

            ProjectFromXml(project, root);
        }

        static XElement ProjectToXml(Project prj)
        {
            XElement root = new XElement("Project");
            return root;
        }

        static void ProjectFromXml(Project prj, XElement el)
        {
        }
    }

    public class IS3XmlSerializer2
    {
        public static XElement EngineeringMapToXml(EngineeringMap eMap)
        {
            XElement root = new XElement("EngineeringMap",
                new XAttribute("MapID", eMap.MapID),
                new XAttribute("LocalTileFileName1", eMap.LocalTileFileName1),
                new XAttribute("LocalTileFileName2", eMap.LocalTileFileName2),
                new XAttribute("LocalMapFileName", eMap.LocalMapFileName),
                new XAttribute("LocalGeoDbFileName", eMap.LocalGeoDbFileName),
                new XAttribute("MapUrl", eMap.MapUrl),
                new XAttribute("XMax", eMap.XMax),
                new XAttribute("XMin", eMap.XMin),
                new XAttribute("YMax", eMap.YMax),
                new XAttribute("YMin", eMap.YMin),
                new XAttribute("MinimumResolution", eMap.MinimumResolution),
                new XAttribute("MapType", eMap.MapType.ToString()),
                new XAttribute("Calibrated", eMap.Calibrated),
                new XAttribute("Scale", eMap.Scale),
                new XAttribute("ScaleX", eMap.ScaleX),
                new XAttribute("ScaleY", eMap.ScaleY),
                new XAttribute("ScaleZ", eMap.ScaleZ)
                );

            //XElement parent = new XElement("ELayers");
            //foreach (EngineeringLayer eLayer in eMap.ELayers)
            //{
            //    XElement node = EngineeringLayerToXml(eLayer);
            //    parent.Add(node);
            //}
            //root.Add(parent);

            return root;
        }

        public static EngineeringMap EngineeringMapFromXml(XElement el)
        {
            if (el.Name.ToString() == "EngineeringMap")
            {
                EngineeringMap eMap = new EngineeringMap();
                
                XAttribute attr = el.Attribute("MapID");
                if (attr != null)
                    eMap.MapID = (string)attr;
                attr = el.Attribute("LocalTileFileName1");
                if (attr != null)
                    eMap.LocalTileFileName1 = (string)attr;
                attr = el.Attribute("LocalTileFileName2");
                if (attr != null)
                    eMap.LocalTileFileName2 = (string)attr;
                attr = el.Attribute("LocalMapFileName");
                if (attr != null)
                    eMap.LocalMapFileName = (string)attr;
                attr = el.Attribute("LocalGeoDbFileName");
                if (attr != null)
                    eMap.LocalGeoDbFileName = (string)attr;
                attr = el.Attribute("MapUrl");
                if (attr != null)
                    eMap.MapUrl = (string)attr;
                attr = el.Attribute("XMax");
                if (attr != null)
                    eMap.XMax = (double)attr;
                attr = el.Attribute("XMin");
                if (attr != null)
                    eMap.XMin = (double)attr;
                attr = el.Attribute("YMax");
                if (attr != null)
                    eMap.YMax = (double)attr;
                attr = el.Attribute("YMin");
                if (attr != null)
                    eMap.YMin = (double)attr;
                attr = el.Attribute("MinimumResolution");
                if (attr != null)
                    eMap.MinimumResolution = (double)attr;
                attr = el.Attribute("MapType");
                if (attr != null)
                {
                    EngineeringMapType mapType = EngineeringMapType.FootPrintMap;
                    Enum.TryParse(attr.Value, out mapType);
                    eMap.MapType = mapType;
                }
                attr = el.Attribute("Calibrated");
                if (attr != null)
                    eMap.Calibrated = (bool)attr;
                attr = el.Attribute("Scale");
                if (attr != null)
                    eMap.Scale = (double)attr;
                attr = el.Attribute("ScaleX");
                if (attr != null)
                    eMap.ScaleX = (double)attr;
                attr = el.Attribute("ScaleY");
                if (attr != null)
                    eMap.ScaleY = (double)attr;
                attr = el.Attribute("ScaleZ");
                if (attr != null)
                    eMap.ScaleZ = (double)attr;

                //XElement eLayers = el.Element("ELayers");
                //if (eLayers != null)
                //{
                //    IEnumerable<XElement> eles = eLayers.Elements("ELayer");
                //    foreach (XElement iter in eles)
                //    {
                //        EngineeringLayer eLayer = EngineeringLayerFromXml(iter);
                //        if (eLayer != null)
                //            eMap.ELayers.Add(eLayer);
                //    }
                //}
                return eMap;
            }
            return null;
        }

        public static XElement ProjectInformationToXml(ProjectInformation pi)
        {
            XElement root = new XElement(pi.ToString());

            return root;

        }
    }
}
