using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;

using IS3.Core;
using IS3.Core.Geometry;
using IS3.Core.Graphics;
using IS3.ArcGIS.Geometry;

namespace IS3.ArcGIS.Graphics
{
    #region Copyright Notice
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************
    #endregion

    // Instances of this class represent graphics.
    public class IS3Graphic : Graphic, IGraphic
    {
        // Constructor:
        public IS3Graphic()
        { }
        //   Construct from a Esri.ArcGISRuntime.Geometry.Geometry instance.
        public IS3Graphic(Esri.ArcGISRuntime.Geometry.Geometry g)
        {
            IGeometry ig = IS3GeometryEngine.fromGeometry(g);
            base.Geometry = ig as Esri.ArcGISRuntime.Geometry.Geometry;
        }

        // Gets the interface of the symbol for the graphic
        // It delegates operations to ArcGIS Runtime.
        public new ISymbol Symbol
        {
            get { return base.Symbol as ISymbol; }
            set { base.Symbol = (Symbol) value;}
        }

        // Gets and sets the geometry for the graphic.
        // It delegates operations to ArcGIS Runtime.
        public new IGeometry Geometry
        {
            get { return base.Geometry as IGeometry; }
            set { base.Geometry = value as Esri.ArcGISRuntime.Geometry.Geometry; }
        }
    }

    public class IS3GraphicCollection : List<IS3Graphic>, IGraphicCollection
    {
        public new IEnumerator<IGraphic> GetEnumerator()
        {
            return base.GetEnumerator();
        }
        public void Add(IGraphic item)
        {
            base.Add(item as IS3Graphic);
        }

        public void Add(IEnumerable items)
        {
            foreach (IGraphic g in items)
                Add(g);
        }

        public new IGraphic this[int index]
        {
            get { return base[index]; }
            set { base[index] = value as IS3Graphic; }
        }
    }
}
