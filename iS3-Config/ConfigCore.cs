using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using IS3.Core;

namespace iS3.Config
{
    public class ConfigCore
    {
        // Load project list from the specified file.
        //
        public static ProjectList LoadProjectList(string fileName)
        {
            try
            {
                // Load ProjectList.xml using XamlReader
                StreamReader reader = new StreamReader(fileName);
                object obj = XamlReader.Load(reader.BaseStream);
                return obj as ProjectList;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return null;
            }
        }

        // Write project list to the specified file.
        //
        public static bool WriteProjectList(string fileName, ProjectList projList)
        {
            try
            {
                // write xml to memory stream at first
                Stream memStream = new MemoryStream();
                XmlAttributeOverrides overide = CreateProjectListOverrides();  // overide some attributes
                XmlSerializer s = new XmlSerializer(typeof(ProjectList), overide);
                s.Serialize(memStream, projList);  // write to memory

                // replace "Locations" with "ProjectList.Locations" so XamlReader would be happy.
                memStream.Position = 0;
                StreamReader reader = new StreamReader(memStream);
                string xml = reader.ReadToEnd();
                string xaml = xml.Replace("Locations", "ProjectList.Locations");

                // overide ProjectList.xml
                FileStream fs = new FileStream(fileName, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(xaml);
                writer.Close();

                return true;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return false;
            }
        }

        // Write ProjectList member variables (XMin, XMax, YMin, Ymax, UseGeographicMap)
        // and ProjectLocation member variables (ID, DefinitionFile, X, Y, Description, Default)
        // as attributes into XML, so XamlReader can work happily.
        //
        static XmlAttributeOverrides CreateProjectListOverrides()
        {
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();

            // add root element namespace
            attrOverrides.Add(typeof(ProjectList), new XmlAttributes()
            {
                XmlRoot = new XmlRootAttribute()
                {
                    ElementName = "ProjectList",
                    Namespace = "clr-namespace:IS3.Core;assembly=IS3.Core"
                }
            });

            // write ProjectList member variables as attributes
            attrOverrides.Add(typeof(ProjectList), "XMax", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("XMax") });
            attrOverrides.Add(typeof(ProjectList), "XMin", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("XMin") });
            attrOverrides.Add(typeof(ProjectList), "YMax", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("YMax") });
            attrOverrides.Add(typeof(ProjectList), "YMin", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("YMin") });
            attrOverrides.Add(typeof(ProjectList), "UseGeographicMap", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("UseGeographicMap") });

            // write ProjectLocation member variables as attributes
            attrOverrides.Add(typeof(ProjectLocation), "ID", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("ID") });
            attrOverrides.Add(typeof(ProjectLocation), "DefinitionFile", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("DefinitionFile") });
            attrOverrides.Add(typeof(ProjectLocation), "X", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("X") });
            attrOverrides.Add(typeof(ProjectLocation), "Y", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("Y") });
            attrOverrides.Add(typeof(ProjectLocation), "Description", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("Description") });
            attrOverrides.Add(typeof(ProjectLocation), "Default", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("Default") });

            return attrOverrides;
        }

        // Load project definition from the specified file
        //
        public static ProjectDefinition LoadProjectDefinition(string projPath, string projID)
        {
            string fileName = projPath + "\\" + projID + ".xml";
            ProjectDefinition projDef = null;
            if (!File.Exists(fileName))
                return null;

            try
            {
                StreamReader reader = new StreamReader(fileName);
                XElement root = XElement.Load(reader);

                if (root == null || root.Name != "Project")
                    return null;

                XNamespace is3 = "clr-namespace:IS3.Core;assembly=IS3.Core";
                XElement node = root.Element(is3 + "ProjectDefinition");
                if (node != null)
                {
                    object obj = XamlReader.Parse(node.ToString());
                    projDef = (ProjectDefinition)obj;

                    if (projDef.LocalFilePath == null || projDef.LocalFilePath.Length == 0)
                        projDef.LocalFilePath = projPath + "\\" + projID;
                    if (projDef.LocalTilePath == null || projDef.LocalTilePath.Length == 0)
                        projDef.LocalTilePath = projPath + "\\" + "TPKs";
                }

                reader.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return null;
            }

            return projDef;
        }

        // Create an instance of ProjectDefinition class, and initalize the member variables
        // according to the specified project path and ID.
        //
        public static ProjectDefinition CreateProjectDefinition(string projPath, string projID)
        {
            ProjectDefinition projDef = new ProjectDefinition();

            projDef.ID = projID;
            projDef.ProjectTitle = projID;
            projDef.LocalFilePath = projPath + "\\" + projID;
            projDef.LocalTilePath = projPath + "\\" + "TPKs";
            projDef.LocalDatabaseName = projPath + "\\" + projID + "\\" + projID + ".MDB";

            return projDef;
        }

        // Convert an instance of EngineeringMap class to XML string
        //
        static string EMap2string(EngineeringMap emap)
        {
            // Convert List<LayerDef> to XML string because XamlWriter is unhappy with the List<>
            string strLyrs = "";
            foreach (LayerDef lyrDef in emap.LocalGdbLayersDef)
            {
                string s = XamlWriter.Save(lyrDef);
                strLyrs = strLyrs + s + "\r\n";
            }

            // Assign LocalGdbLayersDef to null 
            emap.LocalGdbLayersDef = null;

            // Now XamlWriter is happy to write to a string because LocalGdbLayersDef is null
            string strEmap = XamlWriter.Save(emap);

            // Reload the string to XElement
            XElement root = XElement.Parse(strEmap);

            // Find the <EngineeringMap.LocalGdbLayersDef> element
            XNamespace xns = "clr-namespace:IS3.Core;assembly=IS3.Core";
            XName xname = xns + "EngineeringMap.LocalGdbLayersDef";
            XElement xelm = root.Element(xname);

            // Replace the content with List<LayerDef> string,
            // But the '<' and '>' is replace the &lt; and &gt;
            xelm.Value = strLyrs;

            // Relace '<' and '>' back
            string result = root.ToString();
            result = result.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");

            return result;
        }

        // Convert an instance of ProjectDefinition class to XML string
        //
        static string ProjectDefinition2string(ProjectDefinition prjDef)
        {
            // Convert List<EngineeringMap> to XML string because XamlWriter is unhappy with the List<>
            string strEmaps = "\r\n";
            foreach (EngineeringMap emap in prjDef.EngineeringMaps)
            {
                string s = EMap2string(emap);
                strEmaps = strEmaps + s + "\r\n";
            }

            // Assign EngineeringMaps and SubProjectInfos to null 
            prjDef.EngineeringMaps = null;
            prjDef.SubProjectInfos = null;

            // Now XamlWriter is happy to write to a string because LocalGdbLayersDef is null
            string strPrjDef = XamlWriter.Save(prjDef);

            // Reload the string to XElement
            XElement root = XElement.Parse(strPrjDef);

            // Find the <ProjectDefinition.EngineeringMap> element and remove it because we don't use it.
            XNamespace xns = "clr-namespace:IS3.Core;assembly=IS3.Core";
            XName xname = xns + "ProjectDefinition.SubProjectInfos";
            XElement xelm = root.Element(xname);
            xelm.Remove();

            // Find the <ProjectDefinition.EngineeringMap> element
            xname = xns + "ProjectDefinition.EngineeringMaps";
            xelm = root.Element(xname);

            // Replace the content with List<LayerDef> string,
            // But the '<' and '>' is replace the &lt; and &gt;
            xelm.Value = strEmaps;

            // Relace '<' and '>' back
            string result = root.ToString();
            result = result.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");

            return result;
        }

        // Convert an instance of Project class to XElement
        //
        static XElement Project2XElement(Project prj)
        {
            XElement xePrj = new XElement("Project");
            foreach (Domain domain in prj.domains.Values)
            {
                XElement xeDomain = Domain2XElement(domain);
                xePrj.Add(xeDomain);
            }
            return xePrj;
        }

        // Convert an instance of Domain class to XElement
        //
        static XElement Domain2XElement(Domain domain)
        {
            XElement xeDomain = new XElement("Domain");
            xeDomain.Add(new XAttribute("Name", domain.name));
            xeDomain.Add(new XAttribute("Type", domain.type));

            XElement xeObjsDef = ObjsDefinitions2XElement(domain.objsDefinitions);

            XElement xeTreeDef = new XElement("TreeDefinition");
            XElement xeTree = Tree.Tree2Element(domain.root);
            xeTreeDef.Add(xeTree);

            xeDomain.Add(xeObjsDef);
            xeDomain.Add(xeTreeDef);

            return xeDomain;
        }

        // Convert an instance of Dictionary<string, DGObjectsDefinition> to XElement
        //
        public static XElement ObjsDefinitions2XElement(
            Dictionary<string, DGObjectsDefinition> objsDefinitions)
        {
            XElement xe = new XElement("ObjsDefinition");
            foreach (DGObjectsDefinition objDef in objsDefinitions.Values)
            {
                XElement child = DGObjDef2XElement(objDef);
                xe.Add(child);
            }
            return xe;
        }

        // Convert an instance of DGObjecsDefinition class to XElement
        //
        public static XElement DGObjDef2XElement(DGObjectsDefinition objDef)
        {
            XElement xe = new XElement(objDef.Type);

            xe.Add(new XAttribute("Name", objDef.Name));

            // NOTE: remove "dbo_" prefix of the table name!!!
            //
            string tableName = objDef.TableNameSQL.Replace(DbHelper.TablePrefix, "");
            xe.Add(new XAttribute("TableNameSQL", tableName));

            if (objDef.DefNamesSQL != null)
                xe.Add(new XAttribute("DefNamesSQL", objDef.DefNamesSQL));
            if (objDef.ConditionSQL != null)
                xe.Add(new XAttribute("ConditionSQL", objDef.ConditionSQL));
            if (objDef.OrderSQL != null)
                xe.Add(new XAttribute("OrderSQL", objDef.OrderSQL));

            xe.Add(new XAttribute("HasGeometry", objDef.HasGeometry));
            if (objDef.GISLayerName != null)
                xe.Add(new XAttribute("GISLayerName", objDef.GISLayerName));

            xe.Add(new XAttribute("Has3D", objDef.Has3D));
            if (objDef.Layer3DName != null)
                xe.Add(new XAttribute("Layer3DName", objDef.Layer3DName));

            return xe;
        }


        // Write ProjectDefinition to the specfied file.
        //
        public static bool WriteProject(string projPath, string projID,
            ProjectDefinition prjDef, Project prj)
        {
            string fileName = projPath + "\\" + projID + ".xml";

            string strPrjDef = ProjectDefinition2string(prjDef);
            strPrjDef = "\r\n" + strPrjDef + "\r\n";

            XElement xePrj = Project2XElement(prj);
            string strPrj = xePrj.ToString();

            // insert after "<Project>"
            string text = "<Project>";
            strPrj = strPrj.Insert(text.Length, strPrjDef);

            // overide ProjectList.xml
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(strPrj);
            writer.Close();

            return false;
        }

        // Load Project [skeleton only!] from the specified file.
        //
        public static Project LoadProject(string projPath, string projID)
        {
            string fileName = projPath + "\\" + projID + ".xml";
            if (!File.Exists(fileName))
                return null;

            Project proj = new Project();
            try
            {
                StreamReader reader = new StreamReader(fileName);
                XElement root = XElement.Load(reader);

                if (root == null || root.Name != "Project")
                    return null;

                // Load domain definition
                IEnumerable<XElement> nodes = root.Elements("Domain");
                foreach (XElement node in nodes)
                {
                    Domain domain = Domain.loadDefinition(node);
                    if (domain == null)
                        continue;
                    domain.parent = proj;
                    proj.domains.Add(domain.name, domain);
                }
                reader.Close();

                // NOTE: add "dbo_" prefix of the table name!!!
                //
                foreach (Domain domain in proj.domains.Values)
                {
                    foreach (DGObjectsDefinition objsDef in domain.objsDefinitions.Values)
                    {
                        // skip if the prefix already exist
                        if (objsDef.TableNameSQL.Contains(DbHelper.TablePrefix))
                            continue;

                        string str = "";
                        string[] names = objsDef.TableNameSQL.Split(DbHelper.Separator);
                        int count = names.Count();
                        for (int i = 0; i < count; ++i)
                        {
                            str += DbHelper.TablePrefix + names[i];
                            if (i < count - 1)
                                str += ",";
                        }
                        objsDef.TableNameSQL = str;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return null;
            }

            return proj;
        }

        // Write 2d&3d views to the <projectID>.py
        //
        public static void WriteViewsDef(string iS3Path, string projID, ProjectDefinition prjDef)
        {
            // This is a python template for loading views into iS3.
            string template = 
                "# -*- coding:gb2312 -*-\r\n"+
                "import is3\r\n"+
                "is3.mainframe.LoadProject('{0}')\r\n"+
                "is3.prj = is3.mainframe.prj\r\n"+
                "is3.MainframeWrapper.loadDomainPanels()\r\n"+
                "for emap in is3.prj.projDef.EngineeringMaps:\r\n"+
                "    is3.MainframeWrapper.addView(emap)\r\n"+
                "{1}\r\n";

            string pyPath = iS3Path + "\\IS3Py\\";
            string templateFile = "__template__.py";
            string pyFile = pyPath + projID + ".py";
            string unityFile = projID + ".unity3d";
            string xmlFile = projID + ".xml";

            string templateFilePath = pyPath + templateFile;
            if (File.Exists(templateFilePath))
            {
                // read template
                StreamReader reader = new StreamReader(pyPath + templateFile);
                template = reader.ReadToEnd();
                reader.Close();
            }

            string str3d = "is3.addView3d('Map3D', '{0}')";
            str3d = string.Format(str3d, unityFile);

            string unityFilePath = iS3Path + "\\Data\\" + projID + "\\" + unityFile;
            if (!File.Exists(unityFilePath))
            {
                str3d = "#" + str3d;
            }

            // format the template
            string strPy = string.Format(template, xmlFile, str3d);

            // write <projectID>.py
            FileStream fs = new FileStream(pyFile, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(strPy);
            writer.Close();
        }
    }
}
