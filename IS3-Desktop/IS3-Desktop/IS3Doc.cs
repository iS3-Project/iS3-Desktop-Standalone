using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Windows;

using IS3.Core;
using IS3.Core.Graphics;
using IS3.ArcGIS.Graphics;

namespace IS3.Desktop
{
    public class GraphicsSerializer
    {
        public static XElement GraphicToXml(IGraphic g)
        {
            XElement node = new XElement("IS3Graphic",
                new XAttribute("DGObjID", g.DGObjID.ToString()),
                new XAttribute("Geometry",g.Geometry.ToJson()));
            return node;
        }

        public static IGraphic GraphicFromXml(XElement root)
        {
            if (root.Name != "IS3Graphic")
                return null;
            string geom = root.Attribute("Geometry").Value;
            string id = root.Attribute("DGObjID").Value;

            IGraphic g = IS3Runtime.GraphicEngine.NewGraphic();
            g.Geometry = IS3Runtime.GeometryEngine.FromJson(geom);
            g.DGObjID = int.Parse(id);
            return g;
        }

        public static XElement GraphicsToXml(IGraphicCollection graphics)
        {
            XElement parent = new XElement("IS3Graphics");
            foreach (IGraphic g in graphics)
            {
                XElement node = GraphicToXml(g);
                parent.Add(node);
            }
            return parent;
        }

        public static IGraphicCollection GraphicsFromXml(XElement root)
        {
            if (root.Name != "IS3Graphics")
                return null;

            IGraphicCollection gc = IS3Runtime.GraphicEngine.NewGraphicCollection();
            foreach (XElement node in root.Descendants())
            {
                IGraphic g = GraphicFromXml(node);
                if (g != null)
                    gc.Add(g);
            }
            if (gc.Count == 0)
                return null;
            else
                return gc;
        }
    }

    public class IS3Doc : IDoc
    {
        #region IDoc interface
        protected IView _view;
        public IView View { 
            get { return _view; } 
        }
        #endregion

        string _corruptedDataFile = "Corrupted data file: {0}.";
        string _drawingLayerName = "DrawingGraphics";
        EngineeringLayer _defaultDrawingELayer;

        protected MainFrame _mainFrame;
        protected string _filePath;

        public string FilePath { get { return _filePath; } }
        public EngineeringLayer DefaultDrawingELayer 
        { get { return _defaultDrawingELayer; } }

        public IS3Doc(MainFrame mainFrame,
            IS3View view, string filePath)
        {
            _mainFrame = mainFrame;
            _view = view;
            _filePath = filePath;
        }

        public void InitializeDoc()
        {
            _defaultDrawingELayer = 
                _view.EMap.GetELayerByName(_drawingLayerName);
            if (_defaultDrawingELayer == null)
            {
                _defaultDrawingELayer =
                    _view.EMap.NewDrawingELayer(_drawingLayerName, "DGGraphic");
                _defaultDrawingELayer.LayerType = EngineeringLayerType.UserDrawing;
            }

            IS3GraphicsLayer gLayer = null;
            gLayer = _view.GetLayer(_drawingLayerName) as IS3GraphicsLayer;
            if (gLayer == null)
            {
                gLayer = _view.NewLayer(_drawingLayerName,
                    _defaultDrawingELayer) as IS3GraphicsLayer;
            }

            if (File.Exists(_filePath))
            {
                StreamReader reader = new StreamReader(_filePath);
                XElement root = XElement.Load(reader);

                if (root.Name != "Map"
                    || root.Attribute("MapID").Value != _view.EMap.MapID)
                {
                    string error = string.Format(_corruptedDataFile, _filePath);
                    MessageBox.Show(error);
                    return;
                }

                DefaultDrawingELayerFromXml(root);
            }
        }

        public void OnClose()
        {
            StreamWriter writer = new StreamWriter(_filePath);
            XElement root = new XElement("Map",
                new XAttribute("MapID", _view.EMap.MapID));
            
            DefaultDrawingELayerToXml(root);

            XDocument doc = new XDocument(root);
            doc.Save(writer);
            writer.Close();
        }

        protected virtual void DefaultDrawingELayerToXml(XElement root)
        {
            IGraphicsLayer gLayer = DefaultDrawingELayer.GraphicsLayer;
            IGraphicCollection gc = IS3Runtime.GraphicEngine.NewGraphicCollection();
            foreach (IGraphic g in gLayer.Graphics)
            {
                gc.Add(g);
            }

            XElement parent = new XElement("DefaultDrawingELayer");
            XElement node = GraphicsSerializer.GraphicsToXml(gc);
            parent.Add(node);
            root.Add(parent);
        }

        protected virtual void DefaultDrawingELayerFromXml(XElement root)
        {
            XElement parent = root.Descendants().First();
            if (parent.Name != "DefaultDrawingELayer")
                return;

            XElement node = parent.Descendants().First();
            IGraphicCollection graphics =
                GraphicsSerializer.GraphicsFromXml(node);
            if (graphics == null)
                return;

            IGraphicsLayer gLayer = DefaultDrawingELayer.GraphicsLayer;
            _view.AddGraphics(gLayer, graphics);
        }
    }
}
