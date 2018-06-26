using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
    public class IS3SimpleRenderer : SimpleRenderer, ISimpleRenderer
    {
        public new ISymbol Symbol
        {
            get { return base.Symbol as ISymbol; }
            set { base.Symbol = value as Symbol; }
        }
    }

    public class IS3UniqueValueInfo : IUniqueValueInfo
    {
        public ISymbol Symbol { get; set; }
        public ObservableCollection<object> Values { get; set; }

        public IS3UniqueValueInfo()
        {
            Values = new ObservableCollection<object>();
        }
    }

    public class IS3UniqueValueRenderer : UniqueValueRenderer, IUniqueValueRenderer
    {
        public new ISymbol DefaultSymbol
        {
            get { return base.DefaultSymbol as ISymbol; }
            set { base.DefaultSymbol = value as Symbol; }
        }
        public new ObservableCollection<string> Fields 
        {
            get { return base.Fields; }
            set { base.Fields = value; }
        }

        public new ObservableCollection<IUniqueValueInfo> Infos 
        {
            get
            {
                return new ObservableCollection<IUniqueValueInfo>(
                    base.Infos.Select(obj => new IS3UniqueValueInfo
                    {
                        Values = obj.Values,
                        Symbol = obj.Symbol as ISymbol
                    })
                    );
            }
            set
            {
                base.Infos = new UniqueValueInfoCollection(
                    value.Select(obj => new UniqueValueInfo 
                    { 
                        Symbol = obj.Symbol as Symbol,
                        Values = obj.Values
                    })
                    );
            }
        }
    }

    public class IS3ClassBreakInfo : IClassBreakInfo
    {
        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public ISymbol Symbol { get; set; }
    }

    public class IS3ClassBreaksRenderer: ClassBreaksRenderer, IClassBreaksRenderer
    {
        public new ISymbol DefaultSymbol
        {
            get { return base.DefaultSymbol as ISymbol; }
            set { base.DefaultSymbol = value as Symbol; }
        }
        public new ObservableCollection<IClassBreakInfo> Infos 
        {
            get
            {
                return new ObservableCollection<IClassBreakInfo>(
                    base.Infos.Select(obj => new IS3ClassBreakInfo
                    {
                        Maximum = obj.Maximum,
                        Minimum = obj.Minimum,
                        Symbol = obj.Symbol as ISymbol
                    })
                    );
            }
            set
            {
                base.Infos = new ClassBreakInfoCollection(
                    value.Select(obj => new ClassBreakInfo 
                    { 
                        Maximum = obj.Maximum,
                        Minimum = obj.Minimum,
                        Symbol = obj.Symbol as Symbol
                    })
                    );
            }
        }
    }
}
