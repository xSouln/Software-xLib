using System.Windows.Media;

namespace xLibV100.UI
{
    public class UIProperty
    {

        public static Brush RED = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        public static Brush GREEN = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        public static Brush YELLOW = (Brush)new BrushConverter().ConvertFrom("#FF724C21");
        public static Brush TRANSPARENT = null;

        public static Brush GetBrush(string request)
        {
            Brush brush = null;
            try { brush = (Brush)new BrushConverter().ConvertFrom(request); }
            catch { }
            return brush;
        }
    }
}
