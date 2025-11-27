using AnimeWeb_App.Models;
using AnimeWeb_App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnimeWeb_App.Controllers
{
    public class AnimeController : Controller
    {
        private readonly JikanService _jikan;
        private readonly IPdfService _pdfService;


        public AnimeController(IPdfService pdfService, JikanService jikan)
        {
            _pdfService = pdfService;
            _jikan = jikan;
        }

        public async Task<IActionResult> Index()
        {
            var animes = await _jikan.GetTopAnimeAsync();
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

        public async Task<IActionResult> PdfTopAnime()
        {
            var animes = await _jikan.GetTopAnimeAsync();
            var pdfBytes = _pdfService.GenerateAnimeListPdf(animes);

            return File(pdfBytes, "application/pdf", "top_animes.pdf");
        }
        public async Task<IActionResult> PdfAnime(int id)
        {
            var anime = await _jikan.GetAnimeAsync(id);
            var characters = await _jikan.GetAnimeCharactersAsync(id);
            var staff = await _jikan.GetAnimeStaffAsync(id);

            var vm = _jikan.PaginateAnimeData(
                anime,
                characters,
                staff,
                pageCharacters: 1,
                pageStaff: 1
            );

            var pdfBytes = _pdfService.GenerateAnimeDetailPdf(vm);

            return File(pdfBytes, "application/pdf", $"{anime.Title}.pdf");
        }
    }
}
