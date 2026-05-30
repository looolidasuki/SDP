using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp.Fonts;

namespace FurnitureERP.Helpers
{
    /// <summary>
    /// PDFsharp 6.x 需要显式提供字体解析器，避免运行环境缺失 Segoe UI 时导出失败。
    /// </summary>
    public sealed class PdfSharpFontResolver : IFontResolver
    {
        private static readonly object _lock = new object();
        private static bool _registered;

        private static readonly Dictionary<string, byte[]> _fontDataByFaceName =
            new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        private const string SegoeFamily = "Segoe UI";
        private const string ArialFamily = "Arial";

        public string DefaultFontName => SegoeFamily;

        public static void EnsureRegistered()
        {
            if (_registered) return;
            lock (_lock)
            {
                if (_registered) return;
                if (GlobalFontSettings.FontResolver == null)
                    GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
                _registered = true;
            }
        }

        public byte[] GetFont(string faceName)
        {
            if (string.IsNullOrWhiteSpace(faceName))
                faceName = DefaultFontName;

            lock (_lock)
            {
                if (_fontDataByFaceName.TryGetValue(faceName, out var data))
                    return data;

                data = TryLoadFontBytes(faceName);
                if (data == null)
                    throw new FileNotFoundException("Font file not found for faceName: " + faceName);

                _fontDataByFaceName[faceName] = data;
                return data;
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string family = string.IsNullOrWhiteSpace(familyName) ? DefaultFontName : familyName;

            // 尽量匹配 Segoe UI；如果系统没有，就回退 Arial。
            if (!SystemHasFontFamily(family))
                family = SystemHasFontFamily(ArialFamily) ? ArialFamily : DefaultFontName;

            string faceName = BuildFaceName(family, isBold, isItalic);

            // 如果具体 style 找不到，则降级到 regular。
            if (TryLoadFontBytes(faceName) == null)
                faceName = BuildFaceName(family, false, false);

            // PDFsharp 通过 faceName 再回调 GetFont(faceName) 获取字节。
            return new FontResolverInfo(faceName);
        }

        private static bool SystemHasFontFamily(string familyName)
        {
            try
            {
                using (var fonts = new System.Drawing.Text.InstalledFontCollection())
                {
                    return fonts.Families.Any(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));
                }
            }
            catch
            {
                return false;
            }
        }

        private static string BuildFaceName(string family, bool bold, bool italic)
        {
            if (family.Equals(SegoeFamily, StringComparison.OrdinalIgnoreCase))
            {
                if (bold && italic) return "SegoeUI#BoldItalic";
                if (bold) return "SegoeUI#Bold";
                if (italic) return "SegoeUI#Italic";
                return "SegoeUI#Regular";
            }

            if (family.Equals(ArialFamily, StringComparison.OrdinalIgnoreCase))
            {
                if (bold && italic) return "Arial#BoldItalic";
                if (bold) return "Arial#Bold";
                if (italic) return "Arial#Italic";
                return "Arial#Regular";
            }

            // 其他字体：尽量用 family 本名 + style
            string style = bold && italic ? "BoldItalic" : bold ? "Bold" : italic ? "Italic" : "Regular";
            return family + "#" + style;
        }

        private static byte[] TryLoadFontBytes(string faceName)
        {
            // 优先从 Windows Fonts 目录加载常见字体文件
            string fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            if (string.IsNullOrWhiteSpace(fontsDir) || !Directory.Exists(fontsDir))
                fontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");

            string file = null;
            // Segoe UI
            if (string.Equals(faceName, "SegoeUI#Regular", StringComparison.OrdinalIgnoreCase)) file = "segoeui.ttf";
            else if (string.Equals(faceName, "SegoeUI#Bold", StringComparison.OrdinalIgnoreCase)) file = "segoeuib.ttf";
            else if (string.Equals(faceName, "SegoeUI#Italic", StringComparison.OrdinalIgnoreCase)) file = "segoeuii.ttf";
            else if (string.Equals(faceName, "SegoeUI#BoldItalic", StringComparison.OrdinalIgnoreCase)) file = "segoeuiz.ttf";
            // Arial
            else if (string.Equals(faceName, "Arial#Regular", StringComparison.OrdinalIgnoreCase)) file = "arial.ttf";
            else if (string.Equals(faceName, "Arial#Bold", StringComparison.OrdinalIgnoreCase)) file = "arialbd.ttf";
            else if (string.Equals(faceName, "Arial#Italic", StringComparison.OrdinalIgnoreCase)) file = "ariali.ttf";
            else if (string.Equals(faceName, "Arial#BoldItalic", StringComparison.OrdinalIgnoreCase)) file = "arialbi.ttf";

            if (!string.IsNullOrWhiteSpace(file))
            {
                string path = Path.Combine(fontsDir, file);
                if (File.Exists(path))
                    return File.ReadAllBytes(path);
            }

            // 最后尝试：按 familyName 粗略匹配（可能找不到，留给调用方降级）
            return null;
        }
    }
}

