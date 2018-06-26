using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

// Summary:
//     All classes in this file are intended for symbol definiton only,
//     e.g., in XML definition.

namespace IS3.Core.Graphics
{
    public class SymbolDef { }

    public class LineSymbolDef : SymbolDef { }

    public class FillSymbolDef : SymbolDef { }

    public class MarkerSymbolDef : SymbolDef { }

    public class SimpleLineSymbolDef : LineSymbolDef
    {
        //  Gets or sets the color
        public Color Color { get; set; }
        //  Gets or sets the Line symbol style
        public SimpleLineStyle Style { get; set; }
        //  Gets or sets the line symbol width
        public double Width { get; set; }

        public SimpleLineSymbolDef() { }
        public SimpleLineSymbolDef(Color color, SimpleLineStyle style,
            double width)
        {
            Color = color;
            Style = style;
            Width = width;
        }
    }

    public class SimpleFillSymbolDef : FillSymbolDef
    {
        //  Gets or sets the color
        public Color Color { get; set; }
        //  Gets or sets the fill symbol style
        public SimpleFillStyle Style { get; set; }
        //  Gets or sets the line symbol width
        public SimpleLineSymbolDef OutlineDef { get; set; }

        public SimpleFillSymbolDef() { }
        public SimpleFillSymbolDef(Color color, SimpleFillStyle style,
            SimpleLineSymbolDef outlineDef)
        {
            Color = color;
            Style = style;
            OutlineDef = outlineDef;
        }
    }

    public class SimpleMarkerSymbolDef : MarkerSymbolDef
    {
        // Summary:
        //     Gets or sets the rotation angle.
        // Remarks:
        //     Rotates the symbol either relative to the map or the screen, depending on
        //     the AngleAlignment.
        public double Angle { get; set; }
        //
        // Summary:
        //     Gets or sets the rotation angle alignment. Default value is MarkerAngleAlignment.Screen
        //
        // Remarks:
        //     Determines the behavior of the symbol if the map rotates, ie whether the
        //     symbol should rotate with the map or not.
        public MarkerAngleAlignment AngleAlignment { get; set; }
        // Gets or sets the color
        public Color Color { get; set; }
        // Gets or sets the outline color
        public SimpleLineSymbolDef OutlineDef { get; set; }
        // Gets or sets the symbol size
        public double Size { get; set; }
        // Gets or sets the marker symbol style
        public SimpleMarkerStyle Style { get; set; }
        // Gets or sets the X offset.
        public double XOffset { get; set; }
        // Gets or sets the Y offset.
        public double YOffset { get; set; }

        public SimpleMarkerSymbolDef() { }
        public SimpleMarkerSymbolDef(Color color, double size,
            SimpleMarkerStyle style)
        {
            Color = color;
            Size = size;
            Style = style;
        }
    }

    public class SymbolFontDef
    {
        // Summary:
        //     Gets or sets the font family. Default is Arial
        public string FontFamily { get; set; }
        //
        // Summary:
        //     Gets or sets the size of the font. Default is 10.0
        public double FontSize { get; set; }
        //
        // Summary:
        //     Gets or sets the font style.
        public SymbolFontStyle FontStyle { get; set; }
        //
        // Summary:
        //     Gets or sets the font weight.
        public SymbolFontWeight FontWeight { get; set; }
        //
        // Summary:
        //     Gets or sets the text decoration.
        public SymbolTextDecoration TextDecoration { get; set; }

        public SymbolFontDef() { }
        public SymbolFontDef(string fontFamily, double fontSize,
            SymbolFontStyle fontStyle, SymbolFontWeight fontWeight,
            SymbolTextDecoration textDecoration)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
            FontStyle = fontStyle;
            FontWeight = fontWeight;
            TextDecoration = textDecoration;
        }
    }

    public class TextSymbolDef : MarkerSymbolDef
    {
        // Gets or sets the angle.
        public double Angle { get; set; }
        //
        // Summary:
        //     Gets or sets the rotation angle alignment. Default value is MarkerAngleAlignment.Screen
        //
        // Remarks:
        //     Determines the behavior of the symbol if the map rotates, ie whether the
        //     symbol should rotate with the map or not.
        public MarkerAngleAlignment AngleAlignment { get; set; }
        // Gets or sets the color of the background.
        public Color BackgroundColor { get; set; }
        // Gets or sets the color of the border line.
        public Color BorderLineColor { get; set; }
        // Gets or sets the width of the border line.
        public double BorderLineSize { get; set; }
        // Gets or sets the color.
        public Color Color { get; set; }
        // Gets or sets the text.
        public string Text { get; set; }
        // Gets or sets the horizontal text alignment.
        public HorizontalTextAlignment HorizontalTextAlignment { get; set; }
        // Gets or sets the vertical text alignment.
        public VerticalTextAlignment VerticalTextAlignment { get; set; }
        // Gets or sets the font attributes.
        // Exceptions:
        //   System.ArgumentNullException:
        //     Font
        public SymbolFontDef FontDef { get; set; }
        // Gets or sets the X offset.
        public double XOffset { get; set; }
        // Gets or sets the Y offset.
        public double YOffset { get; set; }

        public TextSymbolDef() { }
        public TextSymbolDef(string text, Color color, Color backgroundColor)
        {
            Text = text;
            Color = color;
            BackgroundColor = backgroundColor;
        }
    }
}
