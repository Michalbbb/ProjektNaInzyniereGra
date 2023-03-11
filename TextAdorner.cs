using System;

public class TextAdorner: Adorner
{
    private readonly string _text;
    private readonly Typeface _typeface;
    private readonly Brush _brush;
    private readonly Pen _pen;
    private readonly double _fontSize;
    private readonly double _padding;

    public TextAdorner(UIElement adornedElement, string text, double fontSize, Brush brush, double padding)
        : base(adornedElement)
    {
        _text = text;
        _typeface = new Typeface(adornedElement.FontFamily, adornedElement.FontStyle, adornedElement.FontWeight, adornedElement.FontStretch);
        _fontSize = fontSize;
        _brush = brush;
        _pen = new Pen(brush, 1);
        _padding = padding;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var formattedText = new FormattedText(_text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, _fontSize, _brush);
        var x = AdornedElement.RenderSize.Width / 2 - formattedText.Width / 2 - _padding;
        var y = -formattedText.Height - _padding;
        var rect = new Rect(new Point(x, y), new Size(formattedText.Width + _padding * 2, formattedText.Height + _padding * 2));
        drawingContext.DrawRoundedRectangle(Brushes.White, _pen, rect, 5, 5);
        drawingContext.DrawText(formattedText, new Point(x + _padding, y + _padding));
    }
}
