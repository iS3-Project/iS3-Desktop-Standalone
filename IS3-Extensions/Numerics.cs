using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using IS3.Core;

namespace IS3.Core.Numerics
{
    public class NumericsDef : EngineeringDef
    {
        public List<MeshLayer> MeshLayers { get; set; }
        public List<NumericLayer> NumericLayers { get; set; }
        public NumericsDef()
        {
            MeshLayers = new List<MeshLayer>();
            NumericLayers = new List<NumericLayer>();
        }

        public MeshLayer GetMeshLayer(string meshLayerName)
        {
            foreach (MeshLayer mLayer in MeshLayers)
            {
                if (mLayer.Name == meshLayerName)
                    return mLayer;
            }

            return null;
        }
        public NumericLayer GetNumericLayer(string numericLayerNmae)
        {
            foreach (NumericLayer nLayer in NumericLayers)
            {
                if (nLayer.Name == numericLayerNmae)
                    return nLayer;
            }

            return null;
        }

        public MeshLayer GetMeshLayerByNodeFeatureLayer(object nodeFeatureLayer)
        {
            foreach (MeshLayer mLayer in MeshLayers)
            {
                if (mLayer.NodeFeatureLayer == nodeFeatureLayer)
                    return mLayer;
            }
            return null;
        }
        public MeshLayer GetMeshLayerByElementFeatureLayer(object elementFeatureLayer)
        {
            foreach (MeshLayer mLayer in MeshLayers)
            {
                if (mLayer.ElementFeaturelayer == elementFeatureLayer)
                    return mLayer;
            }
            return null;
        }

        public override EngineeringLayer GetELayerByName(string layerName)
        {
            foreach (EngineeringLayer el in MeshLayers)
            {
                if (el.Name == layerName)
                    return el;
            }
            foreach (EngineeringLayer el in NumericLayers)
            {
                if (el.Name == layerName)
                    return el;
            }
            return null;
        }

        public override List<EngineeringLayer> GetELayers()
        {
            List<EngineeringLayer> eLayers = new List<EngineeringLayer>();
            foreach (EngineeringLayer item in MeshLayers)
                eLayers.Add(item);
            foreach (EngineeringLayer item in NumericLayers)
                eLayers.Add(item);
            return eLayers;
        }
    }

    public class MeshLayer : EngineeringLayer
    {
        public string NodeFeatureUrl { get; set; }
        public string ElementFeatureUrl { get; set; }
        public object NodeFeatureLayer { get; set; }        // internal use: don't assign value
        public object ElementFeaturelayer { get; set; }     // internal use: don't assign value
        public object Nodes { get; set; }                   // internal use: don't assign value
        public object Elements { get; set; }                // internal use: don't assign value
    }

    public class NumericLayer : EngineeringLayer
    {
        public string MeshName { get; set; }
        public string Description { get; set; }
        public List<BreakInfo> BreakInfos { get; set; }
        public double Opacity { get; set; }
        public DateTime Time { get; set; }                  // analysis time, set by program automatically.

        public NumericLayer()
        {
            Opacity = 0.6;
            BreakInfos = new List<BreakInfo>();
        }
    }

    public class BreakInfo
    {
        double _minValue;
        double _maxValue;
        Brush _fill;

        public double MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public Brush Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }

        public BreakInfo()
        {
        }

        public BreakInfo(double minValue, double maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
    }
}
