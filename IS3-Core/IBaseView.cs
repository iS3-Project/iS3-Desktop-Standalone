using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    public interface IBaseView
    {
        string ViewName { get; }
        string ViewID { get; }
        bool DefaultShow { get; }
        bool SetData(params object[] objs);
        ViewLocation ViewPos { get; }

    }
    public enum ViewLocation
    {
        Top,
        Left,
        Center,
        Bottom,
        RightCenter,
        RightBottom,
        Floating
    }
}
