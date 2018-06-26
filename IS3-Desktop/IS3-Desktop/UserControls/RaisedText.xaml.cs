using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace IS3.Desktop.UserControls
{
    public partial class RaisedText : UserControl
    {
        public RaisedText()
        {
            InitializeComponent();
            if (!double.IsNaN(this.Width) && this.Width > 0)
            {
                DisplayText.Width = this.Width;
                DisplayTextBlur.Width = this.Width;
            }
            if (!double.IsNaN(this.MaxWidth) && this.MaxWidth > 0)
            {
                DisplayText.MaxWidth = this.MaxWidth;
                DisplayTextBlur.MaxWidth = this.MaxWidth;
            }


        }



        #region Dependency Properties

        #region Text
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
                        DependencyProperty.Register("Text", typeof(string), typeof(RaisedText), new PropertyMetadata("", OnTextPropertyChanged));
        /// <summary>
        /// Gets or sets DisplayText.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            string text = (string)e.NewValue;
            rtext.DisplayText.Text = text;
            rtext.DisplayTextBlur.Text = text;
        }
        #endregion

        #region TextBrush
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register("TextBrush", typeof(Brush), typeof(RaisedText), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTextBrushPropertyChanged));

        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        private static void OnTextBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            Brush brush = e.NewValue as Brush;
            rtext.DisplayText.Foreground = brush;
        }
        #endregion

        #region TextSize
        public static readonly DependencyProperty TextSizeProperty = DependencyProperty.Register("TextSize", typeof(double), typeof(RaisedText), new PropertyMetadata(10d, OnTextSizePropertyChanged));

        public double TextSize
        {
            get { return (double)GetValue(TextSizeProperty); }
            set { SetValue(TextSizeProperty, value); }
        }

        private static void OnTextSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            if (!double.IsNaN((double)e.NewValue))
            {
                double size = (double)e.NewValue;
                rtext.DisplayText.FontSize = size;
                rtext.DisplayTextBlur.FontSize = size;
            }
        }
        #endregion

        #region TextFamily
        public static readonly DependencyProperty TextFamilyProperty = DependencyProperty.Register("TextFamily", typeof(FontFamily), typeof(RaisedText), new PropertyMetadata(null, OnTextFamilyPropertyChanged));

        public FontFamily TextFamily
        {
            get { return (FontFamily)GetValue(TextFamilyProperty); }
            set { SetValue(TextFamilyProperty, value); }
        }

        private static void OnTextFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            FontFamily font = (FontFamily)e.NewValue;
            rtext.DisplayText.FontFamily = font;
            rtext.DisplayTextBlur.FontFamily = font;
        }
        #endregion

        #region TextStyle
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register("TextStyle", typeof(FontStyle), typeof(RaisedText), new PropertyMetadata(FontStyles.Normal, OnTextStylePropertyChanged));

        public FontStyle TextStyle
        {
            get { return (FontStyle)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        private static void OnTextStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            FontStyle style = (FontStyle)e.NewValue;
            rtext.DisplayText.FontStyle = style;
            rtext.DisplayTextBlur.FontStyle = style;
        }
        #endregion

        #region TextWeight
        public static readonly DependencyProperty TextWeightProperty = DependencyProperty.Register("TextWeight", typeof(FontWeight), typeof(RaisedText), new PropertyMetadata(FontWeights.Normal, OnTextWeightPropertyChanged));

        public FontWeight TextWeight
        {
            get { return (FontWeight)GetValue(TextWeightProperty); }
            set { SetValue(TextWeightProperty, value); }
        }

        private static void OnTextWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            FontWeight weight = (FontWeight)e.NewValue;
            rtext.DisplayText.FontWeight = weight;
            rtext.DisplayTextBlur.FontWeight = weight;
        }
        #endregion

        #region TextAlignment
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(RaisedText), new PropertyMetadata(TextAlignment.Left, OnTextAlignmentPropertyChanged));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        private static void OnTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            TextAlignment align = (TextAlignment)e.NewValue;
            rtext.DisplayText.TextAlignment = align;
            rtext.DisplayTextBlur.TextAlignment = align;
        }
        #endregion

        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(RaisedText), new PropertyMetadata(TextWrapping.NoWrap, OnTextWrappingPropertyChanged));

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        private static void OnTextWrappingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            TextWrapping wrap = (TextWrapping)e.NewValue;
            rtext.DisplayText.TextWrapping = wrap;
            rtext.DisplayTextBlur.TextWrapping = wrap;
        }
        #endregion

        #region BlurBrush
        public static readonly DependencyProperty BlurBrushProperty = DependencyProperty.Register("BlurBrush", typeof(Brush), typeof(RaisedText), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnBlurBrushPropertyChanged));

        public Brush BlurBrush
        {
            get { return (Brush)GetValue(BlurBrushProperty); }
            set { SetValue(BlurBrushProperty, value); }
        }

        private static void OnBlurBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            Brush brush = e.NewValue as Brush;
            rtext.DisplayTextBlur.Foreground = brush;
        }
        #endregion

        #region BlurRadius
        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register("BlurRadius", typeof(double), typeof(RaisedText), new PropertyMetadata(5d, OnBlurRadiusPropertyChanged));

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        private static void OnBlurRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            double radius = (double)e.NewValue;
            BlurEffect effect = rtext.DisplayTextBlur.Effect as BlurEffect;
            effect.Radius = radius;
        }
        #endregion

        #region ShadowColor
        public static readonly DependencyProperty ShadowColorProperty = DependencyProperty.Register("ShadowColor", typeof(Color), typeof(RaisedText), new PropertyMetadata(Colors.Black, OnShadowColorPropertyChanged));

        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        private static void OnShadowColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            Color color = (Color)e.NewValue;
            DropShadowEffect effect = rtext.DisplayText.Effect as DropShadowEffect;
            effect.Color = color;

        }
        #endregion

        #region ShadowBlurRadius
        public static readonly DependencyProperty ShadowBlurRadiusProperty = DependencyProperty.Register("ShadowBlurRadius", typeof(double), typeof(RaisedText), new PropertyMetadata(5d, OnShadowBlurRadiusPropertyChanged));

        public double ShadowBlurRadius
        {
            get { return (double)GetValue(ShadowBlurRadiusProperty); }
            set { SetValue(ShadowBlurRadiusProperty, value); }
        }

        private static void OnShadowBlurRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            double radius = (double)e.NewValue;
            DropShadowEffect effect = rtext.DisplayText.Effect as DropShadowEffect;
            effect.BlurRadius = radius;
        }
        #endregion

        #region ShadowDirection
        public static readonly DependencyProperty ShadowDirectionProperty = DependencyProperty.Register("ShadowDirection", typeof(double), typeof(RaisedText), new PropertyMetadata(-45d, OnShadowDirectionPropertyChanged));

        public double ShadowDirection
        {
            get { return (double)GetValue(ShadowDirectionProperty); }
            set { SetValue(ShadowDirectionProperty, value); }
        }

        private static void OnShadowDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            double direction = (double)e.NewValue;
            DropShadowEffect effect = rtext.DisplayText.Effect as DropShadowEffect;
            effect.Direction = direction;
        }
        #endregion

        #region ShadowDepth
        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.Register("ShadowDepth", typeof(double), typeof(RaisedText), new PropertyMetadata(2d, OnShadowDepthPropertyChanged));

        public double ShadowDepth
        {
            get { return (double)GetValue(ShadowDepthProperty); }
            set { SetValue(ShadowDepthProperty, value); }
        }

        private static void OnShadowDepthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            double depth = (double)e.NewValue;
            DropShadowEffect effect = rtext.DisplayText.Effect as DropShadowEffect;
            effect.ShadowDepth = depth;
        }
        #endregion

        #region ShadowOpacity
        public static readonly DependencyProperty ShadowOpacityProperty = DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(RaisedText), new PropertyMetadata(0.85d, OnShadowOpacityPropertyChanged));

        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }

        private static void OnShadowOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaisedText rtext = d as RaisedText;
            double opacity = (double)e.NewValue;
            DropShadowEffect effect = rtext.DisplayText.Effect as DropShadowEffect;
            effect.Opacity = opacity;
        }
        #endregion

        #endregion

    }
}
