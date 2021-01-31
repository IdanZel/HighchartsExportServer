﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Svg;

namespace HighchartsExportServer
{
    public static class ImageConversion
    {
        public static async Task<byte[]> SvgToPng(int width, string svg)
        {
            var xml = new XmlDocument();
            xml.LoadXml(svg);

            SvgDocument svgDocument = SvgDocument.Open(xml);
            using Bitmap chartBitmap = svgDocument.Draw(width, 0);

            await using var stream = new MemoryStream();
            chartBitmap.Save(stream, ImageFormat.Png);

            return stream.GetBuffer();
        }
    }
}