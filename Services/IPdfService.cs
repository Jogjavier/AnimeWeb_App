using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AnimeWeb_App.Models;

namespace AnimeWeb_App.Services
{
    public interface IPdfService
    {
        byte[] GenerateAnimeListPdf(List<Anime> animes);
        byte[] GenerateAnimeDetailPdf(AnimeViewModel model);
    }

    public class PdfService : IPdfService
    {
        public byte[] GenerateAnimeListPdf(List<Anime> animes)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    page.Header().Text("Listado de Animes")
                        .FontSize(22).Bold().AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);
                                c.RelativeColumn(2);
                                c.RelativeColumn(1);
                                c.ConstantColumn(80);
                                c.RelativeColumn(1);
                                c.ConstantColumn(60);
                            });

                            // Cabecera
                            table.Header(header =>
                            {
                                header.Cell().Background("#333").Padding(5).AlignCenter().Text("#").FontColor("#fff").Bold();
                                header.Cell().Background("#333").Padding(5).AlignCenter().Text("Título").FontColor("#fff").Bold();
                                header.Cell().Background("#333").Padding(5).AlignCenter().Text("Tipo").FontColor("#fff").Bold();
                                header.Cell().Background("#333").Padding(5).AlignCenter().Text("Episodios").FontColor("#fff").Bold();
                                header.Cell().Background("#333").Padding(5).AlignCenter().Text("Estado").FontColor("#fff").Bold();
                                header.Cell().Background("#333").Padding(5).AlignCenter().Text("Año").FontColor("#fff").Bold();
                            });

                            // Filas
                            int i = 1;
                            foreach (var anime in animes)
                            {
                                table.Cell().Element(CellStyle).Text(i++.ToString());
                                table.Cell().Element(CellStyle).Text(anime.Title ?? "N/A");
                                table.Cell().Element(CellStyle).Text(anime.Type ?? "N/A");
                                table.Cell().Element(CellStyle).Text(anime.Episodes?.ToString() ?? "N/A");
                                table.Cell().Element(CellStyle).Text(anime.Status ?? "N/A");
                                table.Cell().Element(CellStyle).Text(anime.Year?.ToString() ?? "N/A");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10).Italic();
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateAnimeDetailPdf(AnimeViewModel model)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    page.Header().Text(model.Anime.Title ?? "Anime")
                        .FontSize(24).Bold().AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Información general
                        col.Item().PaddingBottom(10)
                            .Text("Información General").FontSize(18).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(140);
                                c.RelativeColumn();
                            });

                            InfoRow(table, "Tipo", model.Anime.Type);
                            InfoRow(table, "Estado", model.Anime.Status);
                            InfoRow(table, "Episodios", model.Anime.Episodes?.ToString());
                            InfoRow(table, "Duración", model.Anime.Duration);
                            InfoRow(table, "Rating", model.Anime.Rating);
                            InfoRow(table, "Año", model.Anime.Year?.ToString());
                        });

                        

                        // Compañías
                        col.Item().PaddingTop(15)
                            .Text("Compañías").FontSize(18).Bold();

                        col.Item().PaddingTop(5).Column(c =>
                        {
                            c.Item().Text($"• Productores: {JoinList(model.Anime.Producers)}");
                            c.Item().Text($"• Licenciatarios: {JoinList(model.Anime.Licensors)}");
                            c.Item().Text($"• Estudios: {JoinList(model.Anime.Studios)}");
                        });

                        // Personajes
                        if (model.Characters?.Any() == true)
                        {
                            col.Item().PaddingTop(15)
                                .Text("Personajes Principales").FontSize(18).Bold();

                            col.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background("#333").Padding(5).AlignCenter().Text("Nombre").FontColor("#fff").Bold();
                                    header.Cell().Background("#333").Padding(5).AlignCenter().Text("Rol").FontColor("#fff").Bold();
                                });

                                foreach (var character in model.Characters.Take(10))
                                {
                                    table.Cell().Element(CellStyle).Text(character.Character?.Name ?? "N/A");
                                    table.Cell().Element(CellStyle).Text(character.Role ?? "N/A");
                                }
                            });
                        }

                        // Staff
                        if (model.Staff?.Any() == true)
                        {
                            col.Item().PaddingTop(15)
                                .Text("Staff Principal").FontSize(18).Bold();

                            col.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background("#333").Padding(5).AlignCenter().Text("Nombre").FontColor("#fff").Bold();
                                    header.Cell().Background("#333").Padding(5).AlignCenter().Text("Posiciones").FontColor("#fff").Bold();
                                });

                                foreach (var s in model.Staff.Take(10))
                                {
                                    table.Cell().Element(CellStyle).Text(s.Person?.Name ?? "N/A");
                                    table.Cell().Element(CellStyle).Text(
                                        s.Positions != null && s.Positions.Any()
                                            ? string.Join(", ", s.Positions)
                                            : "N/A"
                                    );
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10).Italic();
                });
            });

            return document.GeneratePdf();
        }

        // Helpers
        private static string JoinList(IEnumerable<dynamic>? items)
        {
            if (items == null || !items.Any()) return "N/A";
            try
            {
                return string.Join(", ", items.Select(x => x.Name?.ToString() ?? ""));
            }
            catch
            {
                return "N/A";
            }
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container.Padding(5).BorderBottom(1).BorderColor("#ccc");
        }

        private static void InfoRow(TableDescriptor table, string label, string? value)
        {
            table.Cell().Element(CellStyle).Text(label).Bold();
            table.Cell().Element(CellStyle).Text(value ?? "N/A");
        }
    }
}