using IS3.Core.Graphics;
using IS3.Core.Geometry;

namespace IS3.Core
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

    // Summary:
    //     LayerDef is a local layer definition which corresponds to
    //     a local geodatabase feature layer or a shape file feature layer
    // Remarks:
    //     Name:
    //          layer id
    //     GeometryType:
    //          geometry type of the layer
    //     IsVisible:
    //          visibility of the layer
    //
    //     LayerDef also includes display options, labelling options.
    //     
    //     RendererDef:
    //           definition for the layer renderer, it overrides the display
    //           options and labelling options.
    //
    public class LayerDef
    {
        public string Name { get; set; }
        public GeometryType GeometryType { get; set; }
        public bool IsVisible { get; set; }

        // Layer display options
        public System.Windows.Media.Color Color { get; set; }
        public System.Windows.Media.Color SelectionColor { get; set; }
        public double MarkerSize { get; set; }
        public SimpleMarkerStyle MarkerStyle { get; set; }
        public SimpleLineStyle LineStyle { get; set; }
        public SimpleFillStyle FillStyle { get; set; }
        public System.Windows.Media.Color OutlineColor { get; set; }
        public double LineWidth { get; set; }

        // Renderer definition: override layer display options if specified.
        public RendererDef RendererDef { get; set; }

        // Layer labelling options
        public bool EnableLabel { get; set; }
        public string LabelTextExpression { get; set; }
        public string LabelWhereClause { get; set; }
        public System.Windows.Media.Color LabelColor { get; set; }
        public System.Windows.Media.Color LabelBackgroundColor { get; set; }
        public System.Windows.Media.Color LabelBorderLineColor { get; set; }
        public double LabelBorderLineWidth { get; set; }
        public string LabelFontFamily { get; set; }
        public double LabelFontSize { get; set; }
        public SymbolFontStyle LabelFontStyle { get; set; }
        public SymbolFontWeight LabelFontWeight { get; set; }
        public SymbolTextDecoration LabelTextDecoration { get; set; }

        public LayerDef()
        {
            IsVisible = true;

            // Layer display options
            Color = System.Windows.Media.Colors.Black;
            SelectionColor = System.Windows.Media.Colors.Red;
            OutlineColor = System.Windows.Media.Colors.Black;
            MarkerSize = 12.0;
            LineWidth = 1.0;
            MarkerStyle = SimpleMarkerStyle.Circle;
            LineStyle = SimpleLineStyle.Solid;
            FillStyle = SimpleFillStyle.Null;

            // Layer labelling options
            LabelColor = System.Windows.Media.Colors.Black;
            LabelFontFamily = "Arial";
            LabelFontSize = 12.0;
        }
    }
}
