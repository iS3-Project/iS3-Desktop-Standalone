using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using IS3.Core.Geometry;

namespace IS3.Core.Graphics
{
    // interface for graphic
    public interface IGraphic
    {
        // Gets the attributes for the graphic.
        IDictionary<string, object> Attributes { get; }

        // Gets or sets whether this graphic is selected.
        bool IsSelected { get; set; }

        // Gets the interface of the symbol for the graphic
        ISymbol Symbol { get; set; }

        // Gets and sets the geometry for the graphic.
        IGeometry Geometry { get; set; }
    }

    // interface for graphic collection
    public interface IGraphicCollection : IEnumerable<IGraphic>
    {
        // Adds an object to the end.
        void Add(IGraphic item);

        // Adds another collection
        void Add(IEnumerable items);

        // Gets the number of elements actually contained.
        int Count { get; }

        // Gets or sets the element at the specified index.
        IGraphic this[int index] { get; set; }
    }
}
