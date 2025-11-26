using AnimeWeb_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnimeWeb_App.Controllers
{
    public class AnimeController : Controller
    {
        private readonly JikanService _jikan;

        public AnimeController(JikanService jikan)
        {
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
    }
}
