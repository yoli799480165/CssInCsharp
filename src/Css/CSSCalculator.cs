using CssInCSharp.Colors;

namespace CssInCSharp.Css
{
    public static class CSSUtil
    {
        public static string Unit(CSSProperties num)
        {
            return "";
        }

        public static CSSCalculator GenCalc()
        {
            return new CSSCalculator();
        }
    }

    public class CSSCalculator
    {
        public CSSCalculator Add(CSSProperties num)
        {
            return this;
        }

        public CSSCalculator Sub(CSSProperties num)
        {
            return this;
        }

        public CSSCalculator Mul(CSSProperties num)
        {
            return this;
        }

        public CSSCalculator Div(CSSProperties num)
        {
            return this;
        }

        public string GetResult(bool force)
        {
            return "";
        }

        public string Equal()
        {
            return "";
        }
    }
}
