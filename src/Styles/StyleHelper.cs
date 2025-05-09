namespace CssInCSharp
{
    public static class StyleHelper
    {
        public static void Register(StyleInfo style)
        {
            StyleService.Instance.Register(style);
        }

        public static string Register(CSSObject style)
        {
            return StyleService.Instance.Register(style);
        }

        public static string Register(string className, CSSObject style)
        {
            return StyleService.Instance.Register(className, style);
        }

        public static string Register(string style)
        {
            return StyleService.Instance.Register(style);
        }

        public static void UseStyleRegister(StyleInfo css)
        {
            StyleService.Instance.Register(css);
        }

        public static string CSS(string css)
        {
            return StyleService.Instance.Register(css);
        }

        public static string CX(string className, CSSObject css)
        {
            return StyleService.Instance.Register(className, css);
        }
    }
}
