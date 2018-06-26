using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Windows;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.Geology.Serialization;
using IS3.Geology.UserControls;

namespace IS3.Geology
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
    public class BoreholeGeology
    {
        public double Top { get; set; }
        public double Base { get; set; }
        public int StratumID { get; set; }
    }

    public class Borehole : DGObject
    {
        public double Top { get; set; }
        public double Base { get; set; }
        public double? Mileage { get; set; }
        public string Type { get; set; }
        public List<BoreholeGeology> Geologies { get; set; }

        public Borehole()
        {
            Geologies = new List<BoreholeGeology>();
        }

        public Borehole(DataRow rawData)
            :base(rawData)
        {
            Geologies = new List<BoreholeGeology>();
        }

        public override bool LoadObjs(DGObjects objs, DbContext dbContext)
        {
            GeologyDGObjectLoader loader = new GeologyDGObjectLoader(dbContext);
            bool success = loader.LoadBoreholes(objs);
            return success;
        }

        public override string ToString()
        {
            string str = base.ToString();

            string str1 = string.Format(
                ", Top={0}, Base={1}, Mileage={2}, Type={3}, Geo=",
                Top, Base, Mileage, Type);
            str += str1;

            foreach (var geo in Geologies)
            {
                str += geo.StratumID + ",";
            }

            return str;
        }

        public override List<DataView> tableViews(IEnumerable<DGObject> objs)
        {
            List<DataView> dataViews = base.tableViews(objs);

            if (parent.rawDataSet.Tables.Count > 1)
            {
                DataTable table = parent.rawDataSet.Tables[1];
                string filter = idFilter(objs);
                DataView view = new DataView(table, filter, "[BoreholeID]",
                    DataViewRowState.CurrentRows);
                dataViews.Add(view);
            }

            return dataViews;
        }

        string idFilter(IEnumerable<DGObject> objs)
        {
            string sql = "BoreholeID in (";
            foreach (var obj in objs)
            {
                sql += obj.id.ToString();
                sql += ",";
            }
            sql += ")";
            return sql;
        }

        public override List<FrameworkElement> chartViews(
            IEnumerable<DGObject> objs, double width, double height)
        {
            List<FrameworkElement> charts = new List<FrameworkElement>();

            List<Borehole> bhs = new List<Borehole>();
            foreach (Borehole bh in objs)
            {
                if (bh != null && bh.Geologies.Count > 0)
                    bhs.Add(bh);
            }

            Domain geologyDomain = Globals.project.getDomain(DomainType.Geology);
            DGObjectsCollection strata = geologyDomain.getObjects("Stratum");

            BoreholeCollectionView bhsView = new BoreholeCollectionView();
            bhsView.Name = "Geology";
            bhsView.Boreholes = bhs;
            bhsView.Strata = strata;
            bhsView.ViewerHeight = height;
            bhsView.RefreshView();
            bhsView.UpdateLayout();
            charts.Add(bhsView);

            return charts;
        }
    }
}