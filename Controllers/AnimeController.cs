using AnimeWeb_App.Models;
using AnimeWeb_App.Services;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnimeWeb_App.Controllers
{
    public class AnimeController : Controller
    {
        private readonly JikanService _jikan;
        private readonly IRazorViewToStringRenderer _renderer;
        private readonly IHtmlPdfService _pdf;



        public AnimeController(JikanService jikan, IHtmlPdfService pdf, IRazorViewToStringRenderer renderer)
        {
            _jikan = jikan;
            _pdf = pdf;
            _renderer = renderer;
        }

        public async Task<IActionResult> Index(string search)
        {
            var animes = await _jikan.GetTopAnimeAsync();
            if (!string.IsNullOrEmpty(search))
            {
                animes = animes
                    .Where(a =>
                        a.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }
            return View(animes);
        }

        public async Task<IActionResult> Show(int id, int pageCharacters = 1, int pageStaff = 1)
        {
            // Cargar anime, personajes y staff
            var anime = await _jikan.GetAnimeAsync(id);
            var characters = await _jikan.GetAnimeCharactersAsync(id);
            var staff = await _jikan.GetAnimeStaffAsync(id);

            // Crear ViewModel con paginación
            var vm = _jikan.PaginateAnimeData(
                anime,
                characters,
                staff,
                pageCharacters,
                pageStaff
            );

            return View(vm);
        }

        public async Task<IActionResult> PdfTop()
        {
            var top = await _jikan.GetTopAnimeAsync();

            string html = await _renderer.RenderViewToStringAsync(
                "Views/Anime/PdfTop.cshtml",
                top
            );


            byte[] pdfBytes = _pdf.GeneratePdf(html);

            return File(pdfBytes, "application/pdf", "top-animes.pdf");
        }

        public async Task<IActionResult> PdfAnime(int id)
        {
            var anime = await _jikan.GetAnimeAsync(id);

            string html = await _renderer.RenderViewToStringAsync(
                 "Views/Anime/PdfAnime.cshtml",
                 anime
             );


            byte[] pdfBytes = _pdf.GeneratePdf(html);

            return File(pdfBytes, "application/pdf", $"anime-{id}.pdf");
        }

    }
}