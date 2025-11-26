using System.Net.Http;
using System.Text.Json;
using AnimeWeb_App.Models;

public class JikanService
{
    private readonly HttpClient _http;

    public JikanService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.jikan.moe/v4/");
    }

    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<List<Anime>> GetTopAnimeAsync()
    {
        var response = await _http.GetStringAsync("top/anime");
        var data = JsonSerializer.Deserialize<TopAnimeResponse>(response, _jsonOptions);
        return data.Data;
    }

    public async Task<Anime> GetAnimeAsync(int id)
    {
        var response = await _http.GetStringAsync($"anime/{id}");
        var data = JsonSerializer.Deserialize<AnimeDetailResponse>(response, _jsonOptions);
        return data.Data;
    }

    public async Task<List<AnimeCharacterWrapper>> GetAnimeCharactersAsync(int id)
    {
        var response = await _http.GetStringAsync($"anime/{id}/characters");
        var data = JsonSerializer.Deserialize<AnimeCharacterResponse>(response, _jsonOptions);
        return data.Data;
    }

    public async Task<List<AnimeStaff>> GetAnimeStaffAsync(int id)
    {
        var response = await _http.GetStringAsync($"anime/{id}/staff");
        var data = JsonSerializer.Deserialize<AnimeStaffResponse>(response, _jsonOptions);
        return data.Data;
    }

    public AnimeViewModel PaginateAnimeData(
    Anime anime,
    List<AnimeCharacterWrapper> characters,
    List<AnimeStaff> staff,
    int pageCharacters,
    int pageStaff)
    {
        int pageSize = 20;

        var vm = new AnimeViewModel();
        vm.Anime = anime;

        // CHARACTERS PAGINATION
        vm.Characters = characters
            .Skip((pageCharacters - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        vm.CharactersPage = pageCharacters;
        vm.CharactersTotalPages = (int)Math.Ceiling(characters.Count / (double)pageSize);

        // STAFF PAGINATION
        vm.Staff = staff
            .Skip((pageStaff - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        vm.StaffPage = pageStaff;
        vm.StaffTotalPages = (int)Math.Ceiling(staff.Count / (double)pageSize);

        return vm;
    }
}
