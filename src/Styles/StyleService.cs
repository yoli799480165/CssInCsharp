using System.Collections.Generic;
using System.Text;
using System;
using CssInCSharp.Extensions;

namespace CssInCSharp
{
    public class StyleInfo
    {
        public string TokenKey { get; set; }
        public string HashId { get; set; }
        public string[] Path { get; set; }
        public Func<CSSInterpolation> StyleFn { get; set; }
    }

    public class StyleService
    {
        private const string CACHE_KEY = "css";
        internal static StyleRegistry StyleRegistry { get; } = new StyleRegistry();
        internal static StyleService Instance { get; } = new StyleService();
        // scoped styles
        internal Dictionary<string, StyleInfo> Styles { get; } = new Dictionary<string, StyleInfo>();

        public void Register(StyleInfo style)
        {
            var path = GetPath(style.TokenKey, style.Path);
            Styles.TryAdd(path, style);
            var provider = StyleRegistry.GetCurrentProviderContentOrDefault(StyleOutlet.InternalStyeSectionOutletName);
            if (provider != null)
            {
                StyleRegistry.NotifyContentProviderChanged(StyleOutlet.InternalStyeSectionOutletName, provider);
            }
        }

        public string Register(string className, CSSObject style)
        {
            string[] path;
            if (className == null)
            {
                var (hashId, item) = SerializeStyle(style);
                className = $"{CACHE_KEY}-{hashId}";
                item.StyleStr = $".{className}{{{item.StyleStr}}}";
                path = new string[] { CACHE_KEY, className };
                StyleCache.Instance.TryAdd(GetPath("", path), item);
            }
            else
            {
                path = new[] { CACHE_KEY, className };

            }
            var css = new CSSObject()
            {
                [$".{className}"] = style,
            };
            Register(new StyleInfo
            {
                HashId = "",
                Path = path,
                StyleFn = () => css
            });
            return className;
        }

        public string Register(CSSObject style)
        {
            return Register(null, style);
        }

        public string Register(string style)
        {
            var css = new CSSString(style);
            var (hashId, item) = SerializeStyle(css);
            var className = $"{CACHE_KEY}-{hashId}";
            item.StyleStr = $".{className}{{{item.StyleStr}}}";
            var path = new string[] { CACHE_KEY, className };
            StyleCache.Instance.TryAdd(GetPath("", path), item);
            Register(new StyleInfo
            {
                HashId = "",
                Path = path,
                StyleFn = () => css
            });
            return className;
        }

        private static string GetPath(string tokenKey, string[] path)
        {
            return $"{tokenKey}|{string.Join("|", path)}";
        }

        internal static (string, StyleCache.Item) SerializeStyle(CSSInterpolation style, string? tokenKey = null, string? hashId = null)
        {
            string styleStr;
            List<(string, string)> effects = null;
            if (style.IsT3)
            {
                styleStr = style.AsT3.ToString();
            }
            else
            {
                var csses = style.ToCssArray();
                var sb = new StringBuilder();
                effects = new List<(string, string)>();
                foreach (var css in csses)
                {
                    sb.Append(css?.SerializeCss(hashId, effects));
                }
                styleStr = sb.ToString();
            }

            var item = new StyleCache.Item
            {
                StyleStr = styleStr,
                TokenKey = tokenKey,
                StyleId = "",
                Effects = new Dictionary<string, string>(),
            };
            if (effects != null && effects.Count > 0)
            {
                foreach (var (effectName, effect) in effects)
                {
                    if (!StyleCache.Instance.HasEffect(effectName))
                    {
                        item.Effects.TryAdd(effectName, effect);
                    }
                }
            }

            if (string.IsNullOrEmpty(hashId))
            {
                hashId = styleStr.Hash();
            }
            return (hashId, item);
        }
    }
}
