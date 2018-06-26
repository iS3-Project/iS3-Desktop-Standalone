using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace IS3.Core.Graphics
{
    public interface IRenderer
    {
    }

    // Summary:
    //     A custom symbology renderer where the graphics of a layer are displayed as
    //     a single symbol.
    public interface ISimpleRenderer : IRenderer
    {
        //
        // Summary:
        //     Gets or sets the symbol this renderer will use.
        ISymbol Symbol { get; set; }
    }

    // Summary:
    //     Represents a unique value or combination of values and a matching symbol
    //     in the UniqueValueRenderer class.
    public interface IUniqueValueInfo
    {
        //
        // Summary:
        //     Symbol used to represent the value or values
        ISymbol Symbol { get; set; }
        //
        // Summary:
        //     The values that will be represented by the symbol. Multiple values represent
        //     a unique combination. The UniqueValueRenderer.Fields property must has a
        //     matching number of entries.
        ObservableCollection<object> Values { get; set; }
    }

    // Summary:
    //     A custom symbology renderer where the graphics of a layer are displayed via
    //     groups based upon specified values found in an attribute field.
    //     The groups (aka. unique values) are defined as discreet occurrences 
    //     specified as UniqueValueInfo.Values in UniqueValueInfo objects
    //     of the UniqueValueInfoCollection.
    public interface IUniqueValueRenderer : IRenderer
    {
        //
        // Summary:
        //     Gets or sets the symbol this renderer will use.
        ISymbol DefaultSymbol { get; set; }
        //
        // Summary:
        //     Returns one or more field names, these correspond to keys on Graphic attributes.
        ObservableCollection<string> Fields { get; set; }
        //
        // Summary:
        //     Returns each value and related symbol used to render a layer
        ObservableCollection<IUniqueValueInfo> Infos { get; set; }
    }

    // Summary:
    //     Represents a pair of values and a matching symbol
    //     in the ClassBreaksRenderer.
    public interface IClassBreakInfo
    {
        //
        // Summary:
        //     The maximum value of the class break
        double Maximum { get; set; }
        //
        // Summary:
        //     The minimum value of the class break
        double Minimum { get; set; }
        //
        // Summary:
        //     Symbol used to represent the class break specified between the min and max
        //     values
        ISymbol Symbol { get; set; }
    }


    // Summary:
    //     A custom symbology renderer where the graphics of a layer are displayed via
    //     groups based upon numerical data. The groups (aka. class breaks) are defined
    //     by specified ClassBreakInfo.Minimum and ClassBreakInfo.Maximum
    //     values in the ClassBreakInfo objects of the ClassBreakInfoCollection.
    public interface IClassBreaksRenderer : IRenderer
    {
        //
        // Summary:
        //     Gets or sets the default symbol that will be used by the IClassBreaksRenderer
        //     when there is no group specified by the IClassBreakInfo
        //     objects for a particular observation.
        ISymbol DefaultSymbol { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the Field that will be symbolized via
        //     groups using ClassBreakInfo objects in this instance.
        string Field { get; set; }
        //
        // Summary:
        //     Gets the collection of ClassBreakInfo objects
        //     which define each group of numerical observations being symbolized in a layer.
        ObservableCollection<IClassBreakInfo> Infos { get; set; }
    }
}
