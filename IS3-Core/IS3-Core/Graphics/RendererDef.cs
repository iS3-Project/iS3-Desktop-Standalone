using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

// Summary:
//     All classes in this file are intended for renderer definiton only,
//     e.g., in XML definition.

namespace IS3.Core.Graphics
{
    public class RendererDef
    {
    }

    public class SimpleRendererDef : RendererDef
    {
        //
        // Summary:
        //     Gets or sets the symbol this renderer will use.
        public SymbolDef SymbolDef { get; set; }

        public SimpleRendererDef() { }
        public SimpleRendererDef(SymbolDef symbolDef)
        {
            SymbolDef = symbolDef;
        }
    }

    public class UniqueValueInfoDef
    {
        // Summary:
        //     Symbol used to represent the value or values
        public SymbolDef SymbolDef { get; set; }
        //
        // Summary:
        //     The values that will be represented by the symbol. Multiple values represent
        //     a unique combination. The UniqueValueRenderer.Fields property must has a
        //     matching number of entries.
        public ObservableCollection<object> Values { get; set; }

        public UniqueValueInfoDef()
        {
            Values = new ObservableCollection<object>();
        }
        public UniqueValueInfoDef(SymbolDef symbolDef, ObservableCollection<object> values)
        {
            SymbolDef = symbolDef;
            Values = values;
        }
    }

    public class UniqueValueRendererDef : RendererDef
    {
        //
        // Summary:
        //     Gets or sets the symbol this renderer will use.
        public SymbolDef DefaultSymbolDef { get; set; }
        // Summary:
        //     Returns one or more field names, these correspond to keys on Graphic attributes.
        public ObservableCollection<string> Fields { get; set; }
        //
        // Summary:
        //     Returns each value and related symbol used to render a layer
        public ObservableCollection<UniqueValueInfoDef> InfosDef { get; set; }

        public UniqueValueRendererDef()
        {
            Fields = new ObservableCollection<string>();
            InfosDef = new ObservableCollection<UniqueValueInfoDef>();
        }
        public UniqueValueRendererDef(SymbolDef defaultSymbolDef,
            ObservableCollection<string> fields,
            ObservableCollection<UniqueValueInfoDef> infosDef)
        {
            DefaultSymbolDef = defaultSymbolDef;
            Fields = fields;
            InfosDef = infosDef;
        }
    }

    public class ClassBreakInfoDef
    {
        //
        // Summary:
        //     The maximum value of the class break
        public double Maximum { get; set; }
        //
        // Summary:
        //     The minimum value of the class break
        public double Minimum { get; set; }
        //
        // Summary:
        //     Symbol used to represent the class break specified between the min and max
        //     values
        public SymbolDef SymbolDef { get; set; }

        public ClassBreakInfoDef() { }
        public ClassBreakInfoDef(double max, double min, SymbolDef symbolDef)
        {
            Maximum = max;
            Minimum = min;
            SymbolDef = symbolDef;
        }
    }

    public class ClassBreaksRendererDef : RendererDef
    {
        //
        // Summary:
        //     Gets or sets the default symbol that will be used by the ClassBreaksRendererDef
        //     when there is no group specified by the ClassBreakInfoDef
        //     objects for a particular observation.
        public SymbolDef DefaultSymbolDef { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the Field that will be symbolized via
        //     groups using ClassBreakInfo objects in this instance.
        public string Field { get; set; }
        //
        // Summary:
        //     Gets the collection of ClassBreakInfo objects
        //     which define each group of numerical observations being symbolized in a layer.
        public ObservableCollection<ClassBreakInfoDef> InfosDef { get; set; }

        public ClassBreaksRendererDef()
        {
            InfosDef = new ObservableCollection<ClassBreakInfoDef>();
        }
        public ClassBreaksRendererDef(SymbolDef defaultSymbolDef,
            string field, ObservableCollection<ClassBreakInfoDef> infosDef)
        {
            DefaultSymbolDef = defaultSymbolDef;
            Field = field;
            InfosDef = infosDef;
        }
    }
}
