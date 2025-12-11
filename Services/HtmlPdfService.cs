using DinkToPdf;
using DinkToPdf.Contracts;

namespace AnimeWeb_App.Services  // ← FALTABA ESTO
{
    public interface IHtmlPdfService
    {
        byte[] GeneratePdf(string html);
    }

    public class HtmlPdfService : IHtmlPdfService
    {
        private readonly IConverter _converter;

        public HtmlPdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdf(string html)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    ColorMode = ColorMode.Color,
                    Margins = new MarginSettings { Top = 20, Bottom = 20 }
                },
                Objects =
            {
                new ObjectSettings()
                {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                }
            }
            };

            return _converter.Convert(doc);
        }
    }
}
