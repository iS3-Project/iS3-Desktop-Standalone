using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;

using IS3.Core;

namespace IS3.Desktop
{
    class IS3ProfileView : IS3View
    {
        public IS3ProfileView(UserControl parent, MapView mapView)
            : base (parent, mapView)
        { }

        public override void setCoord(MapPoint mapPt)
        {
            string coord = string.Format("X = {0}, Z = {1}",
                Math.Round(mapPt.X, 2), Math.Round(mapPt.Y, 2));
            IViewHolder viewHolder = _parent as IViewHolder;
            if (viewHolder != null)
                viewHolder.setCoord(coord);
        }

    }
}
