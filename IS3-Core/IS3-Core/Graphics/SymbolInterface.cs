using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IS3.Core.Graphics
{
    // Represents the style of a ISimpleLineSymbol
    public enum SimpleLineStyle
    {
        Dash = 0,
        DashDot = 1,
        DashDotDot = 2,
        Dot = 3,
        Null = 4,
        Solid = 5,
    }

    // Represents the style of a ISimpleFillSymbol
    public enum SimpleFillStyle
    {
        BackwardDiagonal = 0,
        Cross = 1,
        DiagonalCross = 2,
        ForwardDiagonal = 3,
        Horizontal = 4,
        Null = 5,
        Solid = 6,
        Vertical = 7,
    }

    // Represents the style of a ISimpleMarkerSymbol
    public enum SimpleMarkerStyle
    {
        Circle = 0,
        Cross = 1,
        Diamond = 2,
        Square = 3,
        X = 4,
        Triangle = 5,
    }

    // Defines the way the rotation angle is used by the marker symbol
    public enum MarkerAngleAlignment
    {
        // Summary:
        //     Rotate the marker relative to the screen, usually 'Up'.
        //
        // Remarks:
        //     If the map rotates, the marker symbol will not rotate with the map but stay
        //     'upright'.
        Screen = 0,
        //
        // Summary:
        //     Rotate the marker relative to the map, usually 'North'.
        //
        // Remarks:
        //     If the map rotates, the marker symbol will rotate with the map.
        Map = 1,
    }

    // Summary:
    //     Horizontal alignment of TextSymbol text.
    public enum HorizontalTextAlignment
    {
        // Summary:
        //     Align text to the left
        Left = 0,
        //
        // Summary:
        //     Align text to the center
        Center = 1,
        //
        // Summary:
        //     Align text to the right
        Right = 2,
        //
        // Summary:
        //     Align text to be justified.
        Justify = 3,
    }

    // Summary:
    //     Vertical alignment for TextSymbol text.
    public enum VerticalTextAlignment
    {
        // Summary:
        //     Align text to the bottom.
        Bottom = 0,
        //
        // Summary:
        //     Align text to the middle.
        Middle = 1,
        //
        // Summary:
        //     Align text to the top.
        Top = 2,
        //
        // Summary:
        //     Align text to the baseline.
        Baseline = 3,
    }

    // Summary:
    //     Describes the font style to be used when drawing text symbol.  The default
    //     value is SymbolFontStyle.Normal.
    public enum SymbolFontStyle
    {
        // Summary:
        //     font-style : normal (default)
        Normal = 0,
        //
        // Summary:
        //     font-style : italic
        Italic = 1,
    }

    // Summary:
    //     Describes the font weight to be used when drawing text symbol.  The default
    //     value is SymbolFontWeight.Normal.
    public enum SymbolFontWeight
    {
        // Summary:
        //     font-wegiht : normal (default)
        Normal = 0,
        //
        // Summary:
        //     font-weight : bold
        Bold = 1,
    }

    // Summary:
    //     Describes the text decoration to be used when drawing text symbol.  The default
    //     value is SymbolTextDecoration.None.
    public enum SymbolTextDecoration
    {
        // Summary:
        //     text-decoration : none (default)
        None = 0,
        //
        // Summary:
        //     text-decoration : line-through
        LineThrough = 1,
        //
        // Summary:
        //     text-decoration : underline
        Underline = 2,
    }

    // interface for symbol
    public interface ISymbol
    { }

    public interface ILineSymbol : ISymbol
    { }

    public interface IFillSymbol : ISymbol
    { }
    public interface IMarkerSymbol : ISymbol
    { }

    public interface ISimpleLineSymbol : ILineSymbol
    {
        //  Gets or sets the color
        Color Color { get; set; }
        //  Gets or sets the Line symbol style
        SimpleLineStyle Style { get; set; }
        //  Gets or sets the line symbol width
        double Width { get; set; }
    }

    public interface ISimpleFillSymbol : IFillSymbol
    {
        //  Gets or sets the color
        Color Color { get; set; }
        //  Gets or sets the fill symbol style
        SimpleFillStyle Style { get; set; }
        //  Gets or sets the line symbol width
        ISimpleLineSymbol Outline { get; set; }
    }

    public interface ISimpleMarkerSymbol : IMarkerSymbol
    {
        // Summary:
        //     Gets or sets the rotation angle.
        // Remarks:
        //     Rotates the symbol either relative to the map or the screen, depending on
        //     the AngleAlignment.
        double Angle { get; set; }
        //
        // Summary:
        //     Gets or sets the rotation angle alignment. Default value is MarkerAngleAlignment.Screen
        //
        // Remarks:
        //     Determines the behavior of the symbol if the map rotates, ie whether the
        //     symbol should rotate with the map or not.
        MarkerAngleAlignment AngleAlignment { get; set; }
        // Gets or sets the color
        Color Color { get; set; }
        // Gets or sets the outline color
        ISimpleLineSymbol Outline { get; set; }
        // Gets or sets the symbol size
        double Size { get; set; }
        // Gets or sets the marker symbol style
        SimpleMarkerStyle Style { get; set; }
        // Gets or sets the X offset.
        double XOffset { get; set; }
        // Gets or sets the Y offset.
        double YOffset { get; set; }
    }

    public interface ISymbolFont
    {
        // Summary:
        //     Gets or sets the font family. Default is Arial
        string FontFamily { get; set; }
        //
        // Summary:
        //     Gets or sets the size of the font. Default is 10.0
        double FontSize { get; set; }
        //
        // Summary:
        //     Gets or sets the font style.
        SymbolFontStyle FontStyle { get; set; }
        //
        // Summary:
        //     Gets or sets the font weight.
        SymbolFontWeight FontWeight { get; set; }
        //
        // Summary:
        //     Gets or sets the text decoration.
        SymbolTextDecoration TextDecoration { get; set; }
    }

    public interface ITextSymbol : IMarkerSymbol
    {
        // Gets or sets the angle.
        double Angle { get; set; }
        //
        // Summary:
        //     Gets or sets the rotation angle alignment. Default value is MarkerAngleAlignment.Screen
        //
        // Remarks:
        //     Determines the behavior of the symbol if the map rotates, ie whether the
        //     symbol should rotate with the map or not.
        MarkerAngleAlignment AngleAlignment { get; set; }
        // Gets or sets the color of the background.
        Color BackgroundColor { get; set; }
        // Gets or sets the color of the border line.
        Color BorderLineColor { get; set; }
        // Gets or sets the width of the border line.
        double BorderLineSize { get; set; }
        // Gets or sets the color.
        Color Color { get; set; }
        // Gets or sets the text.
        string Text { get; set; }
        // Gets or sets the horizontal text alignment.
        HorizontalTextAlignment HorizontalTextAlignment { get; set; }
        // Gets or sets the vertical text alignment.
        VerticalTextAlignment VerticalTextAlignment { get; set; }
        // Gets or sets the font attributes.
        // Exceptions:
        //   System.ArgumentNullException:
        //     Font
        ISymbolFont Font { get; set; }
        // Gets or sets the X offset.
        double XOffset { get; set; }
        // Gets or sets the Y offset.
        double YOffset { get; set; }
    }

}
