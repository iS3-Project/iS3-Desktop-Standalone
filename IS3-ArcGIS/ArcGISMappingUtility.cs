using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;

using IS3.Core.Geometry;

namespace IS3.ArcGIS
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
    public class ArcGISMappingUtility
    {
        static int NUM = 128;

        public static Graphic NewLine(double x1, double y1, double x2, double y2)
        {
            MapPoint p1 = new MapPoint(x1, y1);
            MapPoint p2 = new MapPoint(x2, y2);
            return NewLine(p1, p2);
        }
        public static Graphic NewRectangle(double left, double top, double right, double bottom)
        {
            MapPoint p1 = new MapPoint(left, top);
            MapPoint p2 = new MapPoint(right, top);
            MapPoint p3 = new MapPoint(right, bottom);
            MapPoint p4 = new MapPoint(left, bottom);
            return NewQuadrilateral(p1, p2, p3, p4);
        }
        public static Graphic NewCircle(double x, double y, double r)
        {
            double[] px = new double[NUM];
            double[] py = new double[NUM];
            GeometryAlgorithms.CircleToPoints(x, y, r, NUM, px, py, AngleDirection.CounterClockwise);

            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            for (int i = 0; i < NUM; i++)
            {
                MapPoint p = new MapPoint(px[i], py[i]);
                pc.Add(p);
            }
            pc.Add(pc[0]);

            return NewPolygon(pc);
        }
        public static Graphic NewDonut(double x, double y, double innerRadius, double outerRadius)
        {
            double[] px = new double[NUM];
            double[] py = new double[NUM];
            GeometryAlgorithms.CircleToPoints(x, y, outerRadius, NUM, px, py, AngleDirection.CounterClockwise);

            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            for (int i = 0; i < NUM; i++)
            {
                MapPoint p = new MapPoint(px[i], py[i]);
                pc.Add(p);
            }
            pc.Add(pc[0]);

            PolygonBuilder polygon = new PolygonBuilder(pc);

            GeometryAlgorithms.CircleToPoints(x, y, innerRadius, NUM, px, py, AngleDirection.Clockwise);
            pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            for (int i = 0; i < NUM; i++)
            {
                MapPoint p = new MapPoint(px[i], py[i]);
                pc.Add(p);
            }
            pc.Add(pc[0]);
            polygon.AddPart(pc);

            Graphic g = new Graphic();
            g.Geometry = polygon.ToGeometry();
            return g;
        }
        public static Graphic NewText(string text, double x, double y, Color color, string fontName, double fontSize)
        {
            MapPoint p = new MapPoint(x, y);
            return NewText(text, p, color, fontName, fontSize);
        }

        public static Graphic NewLine(MapPoint p1, MapPoint p2)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            return NewPolyline(pc);
        }
        public static Graphic NewTriangle(MapPoint p1, MapPoint p2, MapPoint p3)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            //pc.Add(p1);
            return NewPolygon(pc);
        }
        public static Graphic NewQuadrilateral(MapPoint p1, MapPoint p2, MapPoint p3, MapPoint p4)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            pc.Add(p4);
            //pc.Add(p1);
            return NewPolygon(pc);
        }
        public static Graphic NewPentagon(MapPoint p1, MapPoint p2, MapPoint p3, MapPoint p4, MapPoint p5)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            pc.Add(p4);
            pc.Add(p5);
            //pc.Add(p1);
            return NewPolygon(pc);
        }
        public static Graphic NewPolygon(Esri.ArcGISRuntime.Geometry.PointCollection pc)
        {
            Esri.ArcGISRuntime.Geometry.Polygon polygon = new Esri.ArcGISRuntime.Geometry.Polygon(pc);
            Graphic g = new Graphic();
            g.Geometry = polygon;
            return g;
        }
        public static Graphic NewPolyline(Esri.ArcGISRuntime.Geometry.PointCollection pc)
        {
            Esri.ArcGISRuntime.Geometry.Polyline polyline = new Esri.ArcGISRuntime.Geometry.Polyline(pc);
            Graphic g = new Graphic();
            g.Geometry = polyline;
            return g;
        }
        public static Graphic NewText(string text, MapPoint p, Color color, string fontName, double fontSize)
        {
            TextSymbol textSymbol = new TextSymbol();
            textSymbol.Text = text;
            textSymbol.Color = color;

            SymbolFont font = new SymbolFont(fontName, fontSize);
            textSymbol.Font = font;

            Graphic graphicText = new Graphic();
            graphicText.Symbol = textSymbol;
            graphicText.Geometry = p;

            return graphicText;
        }
        public static Graphic NewText(string text, MapPoint p, Color color, SymbolFont font)
        {
            TextSymbol textSymbol = new TextSymbol();
            textSymbol.Text = text;
            textSymbol.Color = color;
            textSymbol.Font = font;

            Graphic graphicText = new Graphic();
            graphicText.Symbol = textSymbol;
            graphicText.Geometry = p;

            return graphicText;
        }

        public static Envelope GetGraphicsEnvelope(GraphicCollection gc)
        {
            if (gc.Count == 0)
                return null;

            EnvelopeBuilder envBuilder = new EnvelopeBuilder(gc[0].Geometry.Extent);
            Envelope env = envBuilder.ToGeometry();
            foreach (Graphic g in gc)
            {
                env.Union(g.Geometry.Extent);
            }
            return env;
        }

        public static MapPoint Center(Esri.ArcGISRuntime.Geometry.Polygon polygon)
        {
            IEnumerable<MapPoint> pc = polygon.Parts[0].GetPoints();
            double x = 0;
            double y = 0;

            // The polygon's first point is coincide with the last point
            //for (int i = 0; i < pc.Count - 1; ++i)
            //{
            //    MapPoint p = pc[i];
            //    x += p.X;
            //    y += p.Y;
            //}
            //x /= (pc.Count - 1);
            //y /= (pc.Count - 1);
            foreach (MapPoint p in pc)
            {
                x += p.X;
                y += p.Y;
            }
            x /= pc.Count();
            y /= pc.Count();
            MapPoint center = new MapPoint(x, y);
            return center;
        }
        public static double Length(Esri.ArcGISRuntime.Geometry.PointCollection pts)
        {
            double len = 0;
            MapPoint p1, p2;
            for (int i = 0; i < pts.Count - 1; ++i)
            {
                p1 = pts[i];
                p2 = pts[i + 1];
                len += Distance(p1, p2);
            }
            return len;
        }
        public static double Distance(MapPoint p1, MapPoint p2)
        {
            return GeometryAlgorithms.PointDistanceToPoint(p1.X, p1.Y, p2.X, p2.Y);
        }
        public static MapPoint MidPoint(MapPoint p1, MapPoint p2)
        {
            MapPointBuilder p = new MapPointBuilder(p1.SpatialReference);
            p.X = (p1.X + p2.X) / 2.0;
            p.Y = (p1.Y + p2.Y) / 2.0;
            return p.ToGeometry();
        }
        public static MapPoint InterpolatePointOnLine(MapPoint p1, MapPoint p2, double scale)
        {
            MapPointBuilder p = new MapPointBuilder(p1.SpatialReference);
            double x = 0.0, y = 0.0;
            GeometryAlgorithms.InterpolatePointOnLine(p1.X, p1.Y, p2.X, p2.Y, scale, ref x, ref y);
            p.X = x;
            p.Y = y;
            return p.ToGeometry();
        }

        // Project a point to polyline.
        // point: (p), polyline: (part)
        // output distance relative to the start point of the polyline: (distance)
        // output projection point: (outPnt)
        // return value:
        //      true: the point is projected on the polyline without extending the polyline
        //      false: the point is projected on the polyline through extending the polyline
        //
        public static bool ProjectPointToPolyline(MapPoint p,
            IEnumerable<MapPoint> part,
            ref double distance, ref MapPoint outPnt)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection pts =
                new Esri.ArcGISRuntime.Geometry.PointCollection(part);

            distance = 0.0;
            SpatialReference sf = pts[0].SpatialReference;

            double outx = 0.0, outy = 0.0;
            MapPoint p0, p1;
            for (int i = 0; i < pts.Count - 1; ++i)
            {
                p0 = pts[i];
                p1 = pts[i + 1];

                bool canProject = GeometryAlgorithms.ProjectPointToLine(p.X, p.Y,
                    p0.X, p0.Y, p1.X, p1.Y, ref outx, ref outy);

                if (canProject == true)
                {
                    distance += GeometryAlgorithms.PointDistanceToPoint(outx, outy, p0.X, p0.Y);
                    outPnt = new MapPoint(outx, outy, sf);
                    return true;
                }
                distance += GeometryAlgorithms.PointDistanceToPoint(p0.X, p0.Y, p1.X, p1.Y);
            }

            // Project the point by extending the polyline
            p0 = pts[0];
            p1 = pts[pts.Count - 1];
            double d0p = GeometryAlgorithms.PointDistanceToPoint(p.X, p.Y, p0.X, p0.Y);
            double d1p = GeometryAlgorithms.PointDistanceToPoint(p.X, p.Y, p1.X, p1.Y);
            if (d0p < d1p)
            {
                // the map point is closer to the beginning of the polyline,
                // then extend the beginning segment.
                p1 = pts[1];
                GeometryAlgorithms.ProjectPointToLine(p.X, p.Y,
                    p0.X, p0.Y, p1.X, p1.Y, ref outx, ref outy);
                distance = GeometryAlgorithms.PointDistanceToPoint(outx, outy, p0.X, p0.Y);
                distance *= -1.0;
                outPnt = new MapPoint(outx, outy, sf);
            }
            else
            {
                // the map point is closer to the endding of the polyline,
                // since the loop is ended on the last segment, just use the result is OK.
                distance += GeometryAlgorithms.PointDistanceToPoint(outx, outy, p1.X, p1.Y);
                outPnt = new MapPoint(outx, outy, sf);
            }

            return false;
        }

        public static void Reverse(Esri.ArcGISRuntime.Geometry.PointCollection pts)
        {
            IEnumerable<MapPoint> reversedPts = pts.Reverse();

            Esri.ArcGISRuntime.Geometry.PointCollection newPts =
                new Esri.ArcGISRuntime.Geometry.PointCollection();
            foreach (MapPoint pt in reversedPts)
                newPts.Add(pt);

            pts.Clear();
            foreach (MapPoint pt in newPts)
                pts.Add(pt);
        }

        //public static void SetGraphicAttr(Graphic g, int ID, object obj)
        //{
        //    g.Attributes["ID"] = ID;
        //    g.Attributes["_local"] = true;
        //    g.Attributes["_obj"] = obj;
        //}

        /*
         * When you draw a arrow, you need to calculate the points of arrows.
         * The function do this job.
         */
        public static Esri.ArcGISRuntime.Geometry.PointCollection
            VectorArrowPoints(MapPoint beginPnt, MapPoint endPnt)
        {
            double x = beginPnt.X - endPnt.X;
            double y = beginPnt.Y - endPnt.Y;
            double angle = Math.Atan2(y, x);

            double alpha = Math.PI / 6;                         // arrow is 30 degree by each side of original line
            double length = Math.Sqrt(x * x + y * y) * 0.25;      // arrow is a quarter of original length 

            MapPointBuilder p1 = new MapPointBuilder(endPnt);
            MapPointBuilder p2 = new MapPointBuilder(endPnt);
            p1.X += length * Math.Cos(angle + alpha);
            p1.Y += length * Math.Sin(angle + alpha);
            p2.X += length * Math.Cos(angle - alpha);
            p2.Y += length * Math.Sin(angle - alpha);

            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1.ToGeometry());
            pc.Add(endPnt);
            pc.Add(p2.ToGeometry());

            return pc;
        }
        public static Esri.ArcGISRuntime.Geometry.PointCollection
            VectorArrowPoints(double beginPntX, double beginPntY, MapPoint endPnt)
        {
            double x = beginPntX - endPnt.X;
            double y = beginPntY - endPnt.Y;
            double angle = Math.Atan2(y, x);

            double alpha = Math.PI / 6;                         // arrow is 30 degree by each side of original line
            double length = Math.Sqrt(x * x + y * y) * 0.25;      // arrow is a quarter of original length 

            MapPointBuilder p1 = new MapPointBuilder(endPnt);
            MapPointBuilder p2 = new MapPointBuilder(endPnt);
            p1.X += length * Math.Cos(angle + alpha);
            p1.Y += length * Math.Sin(angle + alpha);
            p2.X += length * Math.Cos(angle - alpha);
            p2.Y += length * Math.Sin(angle - alpha);

            Esri.ArcGISRuntime.Geometry.PointCollection pc = new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1.ToGeometry());
            pc.Add(endPnt);
            pc.Add(p2.ToGeometry());

            return pc;
        }

        // Draw vertical distributed load
        // Note: y1<y2, (x1,y1)-(x1,y2) is a vertical line, (x3,y1)-(x2,y2) is a oblique line
        //
        public static GraphicCollection DistributedLoad_Vertical(double x1, double x2, double x3, double y1, double y2,
            Symbol backgroundFillSymbol, Symbol arrowFillSymbol, Symbol lineSymbol)
        {
            GraphicCollection gc = new GraphicCollection();

            MapPoint p1 = new MapPoint(x1, y1);
            MapPoint p2 = new MapPoint(x1, y2);
            MapPoint p3 = new MapPoint(x2, y2);
            MapPoint p4 = new MapPoint(x3, y1);
            Graphic g = ArcGISMappingUtility.NewQuadrilateral(p1, p2, p3, p4);
            g.Symbol = backgroundFillSymbol;
            gc.Add(g);

            Esri.ArcGISRuntime.Geometry.PointCollection pc =
                new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            pc.Add(p4);
            pc.Add(p1);
            g = ArcGISMappingUtility.NewPolyline(pc);
            g.Symbol = lineSymbol;
            gc.Add(g);

            double x00, x01, y00;
            for (int i = 0; i <= 10; ++i)
            {
                x00 = x1;
                x01 = x2 + i * (x3 - x2) / 10.0;
                y00 = y2 - i * (y2 - y1) / 10.0;
                MapPoint p00 = new MapPoint(x00, y00);
                MapPoint p01 = new MapPoint(x01, y00);
                g = ArcGISMappingUtility.NewLine(p00, p01);
                g.Symbol = lineSymbol;
                gc.Add(g);

                pc = ArcGISMappingUtility.VectorArrowPoints(x2, y00, p00);
                g = ArcGISMappingUtility.NewPolygon(pc);
                g.Symbol = arrowFillSymbol;
                gc.Add(g);
            }

            return gc;
        }

        // Draw horizontal distributed load
        // Note: x1<x2, (x1,y1)-(x2,y1) is a horizontal line, (x1,y2)-(x2,y3) is a oblique line
        //
        public static GraphicCollection DistributedLoad_Horizontal(double x1, double x2, double y1, double y2, double y3,
            Symbol backgroundFillSymbol, Symbol arrowFillSymbol, Symbol lineSymbol)
        {
            GraphicCollection gc = new GraphicCollection();

            MapPoint p1 = new MapPoint(x1, y1);
            MapPoint p2 = new MapPoint(x1, y2);
            MapPoint p3 = new MapPoint(x2, y3);
            MapPoint p4 = new MapPoint(x2, y1);
            Graphic g = ArcGISMappingUtility.NewQuadrilateral(p1, p2, p3, p4);
            g.Symbol = backgroundFillSymbol;
            gc.Add(g);

            Esri.ArcGISRuntime.Geometry.PointCollection pc =
                new Esri.ArcGISRuntime.Geometry.PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            pc.Add(p4);
            pc.Add(p1);
            g = ArcGISMappingUtility.NewPolyline(pc);
            g.Symbol = lineSymbol;
            gc.Add(g);

            double x00, y00, y01;
            for (int i = 0; i <= 10; ++i)
            {
                x00 = x1 + i * (x2 - x1) / 10.0;
                y00 = y1;
                y01 = y2 + i * (y3 - y2) / 10.0;
                MapPoint p00 = new MapPoint(x00, y00);
                MapPoint p01 = new MapPoint(x00, y01);
                g = ArcGISMappingUtility.NewLine(p00, p01);
                g.Symbol = lineSymbol;
                gc.Add(g);

                pc = ArcGISMappingUtility.VectorArrowPoints(x00, y2, p00);
                g = ArcGISMappingUtility.NewPolygon(pc);
                g.Symbol = arrowFillSymbol;
                gc.Add(g);
            }

            return gc;
        }

        // Summary:
        //     Change the spatial reference of a gemoetry without re-projection.
        //     If re-projection is needed, use the GeometryEngine.Project function.
        //
        public static MapPoint ChangeSpatialReference(
            MapPoint p, SpatialReference sr)
        {
            return new MapPoint(p.X, p.Y, p.Z, p.M, sr);
        }
        public static IEnumerable<MapPoint> ChangeSpatialReference(
            IEnumerable<MapPoint> part,
            SpatialReference sr)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection points = 
                new Esri.ArcGISRuntime.Geometry.PointCollection(sr);
            foreach (MapPoint p in part)
                points.Add(ChangeSpatialReference(p, sr));
            return points;
        }
        public static PartCollection ChangeSpatialReference(
            ReadOnlyPartCollection parts,
            SpatialReference sr)
        {
            PartCollection newParts = new PartCollection(sr);
            foreach (IEnumerable<MapPoint> part in parts.GetPartsAsPoints())
            {
                IEnumerable<MapPoint> points = ChangeSpatialReference(part, sr);
                newParts.AddPart(points);
            }
            return newParts;
        }

        public static Esri.ArcGISRuntime.Geometry.Geometry ChangeSpatailReference(
            Esri.ArcGISRuntime.Geometry.Geometry geom, SpatialReference sr)
        {
            if (geom.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Point)
            {
                MapPoint p = geom as MapPoint;
                return ChangeSpatailReference(p, sr);
            }
            else if (geom.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Multipoint)
            {
                Multipoint mp = geom as Multipoint;
                IEnumerable<MapPoint> newMP = ChangeSpatialReference(mp.Points, sr);
                return new Multipoint(newMP, sr);
            }
            else if (geom.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polyline)
            {
                Esri.ArcGISRuntime.Geometry.Polyline pl = geom as Esri.ArcGISRuntime.Geometry.Polyline;
                PartCollection newPart = ChangeSpatialReference(pl.Parts, sr);
                return new Esri.ArcGISRuntime.Geometry.Polyline(newPart, sr);
            }
            else if (geom.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polygon)
            {
                Esri.ArcGISRuntime.Geometry.Polygon pg = geom as Esri.ArcGISRuntime.Geometry.Polygon;
                PartCollection newPart = ChangeSpatialReference(pg.Parts, sr);
                return new Esri.ArcGISRuntime.Geometry.Polygon(newPart, sr);
            }
            else if (geom.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Envelope)
            {
                Envelope ev = geom as Envelope;
                return new Envelope(ev.XMin, ev.YMin, ev.XMax, ev.YMax, sr);
            }
            return null;
        }

    }
}
