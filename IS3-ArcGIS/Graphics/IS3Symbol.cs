using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Esri.ArcGISRuntime.Symbology;

using IS3.Core.Graphics;

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

    // Instances of this class represent SimpleMarkerSymbol
    //     It delegates operations to ArcGIS Runtime.

    public class IS3SimpleLineSymbol : SimpleLineSymbol, ISimpleLineSymbol
    {
        public new IS3.Core.Graphics.SimpleLineStyle Style
        {
            get { return (IS3.Core.Graphics.SimpleLineStyle)base.Style; }
            set { base.Style = (Esri.ArcGISRuntime.Symbology.SimpleLineStyle)value; }
        }
    }

    public class IS3SimpleFillSymbol : SimpleFillSymbol, ISimpleFillSymbol
    { 
        public new IS3.Core.Graphics.SimpleFillStyle Style
        {
            get { return (Core.Graphics.SimpleFillStyle)base.Style; }
            set { base.Style = (Esri.ArcGISRuntime.Symbology.SimpleFillStyle)value; }
        }

        public new ISimpleLineSymbol Outline
        {
            get { return base.Outline as ISimpleLineSymbol; }
            set { base.Outline = value as SimpleLineSymbol; }
        }
    }

    public class IS3SimpleMarkerSymbol : SimpleMarkerSymbol, ISimpleMarkerSymbol
    {
        public new Core.Graphics.MarkerAngleAlignment AngleAlignment
        {
            get { return (Core.Graphics.MarkerAngleAlignment)base.AngleAlignment; }
            set { base.AngleAlignment = (Esri.ArcGISRuntime.Symbology.MarkerAngleAlignment)value; }
        }
        public new ISimpleLineSymbol Outline
        {
            get { return base.Outline as ISimpleLineSymbol; }
            set { base.Outline = value as SimpleLineSymbol; }
        }

        public new Core.Graphics.SimpleMarkerStyle Style
        {
            get { return (Core.Graphics.SimpleMarkerStyle)base.Style; }
            set { base.Style = (Esri.ArcGISRuntime.Symbology.SimpleMarkerStyle)value; }
        }
    }

    public class IS3SymbolFont : SymbolFont, ISymbolFont
    {
        public IS3SymbolFont() : base()
        { }

        public IS3SymbolFont(string fontFamily, double fontSize)
            : base(fontFamily, fontSize)
        { }

        public new Core.Graphics.SymbolFontStyle FontStyle
        {
            get { return (Core.Graphics.SymbolFontStyle)base.FontStyle; }
            set { base.FontStyle = (Esri.ArcGISRuntime.Symbology.SymbolFontStyle)value; }
        }

        public new Core.Graphics.SymbolFontWeight FontWeight
        {
            get { return (Core.Graphics.SymbolFontWeight)base.FontWeight; }
            set { base.FontWeight = (Esri.ArcGISRuntime.Symbology.SymbolFontWeight)value; }
        }

        public new Core.Graphics.SymbolTextDecoration TextDecoration
        {
            get { return (Core.Graphics.SymbolTextDecoration)base.TextDecoration; }
            set { base.TextDecoration = (Esri.ArcGISRuntime.Symbology.SymbolTextDecoration)value; }
        }
    }

    public class IS3TextSymbol : TextSymbol, ITextSymbol
    {
        public new Core.Graphics.MarkerAngleAlignment AngleAlignment
        {
            get { return (Core.Graphics.MarkerAngleAlignment)base.AngleAlignment; }
            set { base.AngleAlignment = (Esri.ArcGISRuntime.Symbology.MarkerAngleAlignment)value; }
        }

        public new Core.Graphics.HorizontalTextAlignment HorizontalTextAlignment
        {
            get { return (Core.Graphics.HorizontalTextAlignment)base.HorizontalTextAlignment;}
            set { base.HorizontalTextAlignment = (Esri.ArcGISRuntime.Symbology.HorizontalTextAlignment)value;}
        }

        public new Core.Graphics.VerticalTextAlignment VerticalTextAlignment
        {
            get { return (Core.Graphics.VerticalTextAlignment)base.VerticalTextAlignment;}
            set { base.VerticalTextAlignment = (Esri.ArcGISRuntime.Symbology.VerticalTextAlignment)value;}
        }

        public new ISymbolFont Font
        {
            get { return base.Font as ISymbolFont; }
            set { base.Font = value as SymbolFont; }
        }
    }
}
