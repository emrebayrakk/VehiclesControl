using System.Text.Json.Serialization;

namespace VehiclesControl.Web.Components.Pages
{
    public partial class Home
    {
        private string searchQuery { get; set; } = "";
        private List<MovieDto> movies { get; set; } = new();
        private bool loading { get; set; } = false;

        private async Task SearchMovies()
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return;
            }

            loading = true;

            var response = await Http.GetFromJsonAsync<MovieResponse>($"https://search.imdbot.workers.dev/?q={searchQuery}");


            movies = response?.Description?.ToList() ?? new();

            loading = false;
        }

        public class MovieResponse
        {
            public List<MovieDto> Description { get; set; }
        }

        public class MovieDto
        {
            [JsonPropertyName("#TITLE")]
            public string Title { get; set; }

            [JsonPropertyName("#YEAR")]
            public int? Year { get; set; }

            [JsonPropertyName("#IMDB_ID")]
            public string ImdbId { get; set; }

            [JsonPropertyName("#RANK")]
            public int? Rank { get; set; }

            [JsonPropertyName("#ACTORS")]
            public string Actors { get; set; }

            [JsonPropertyName("#AKA")]
            public string Aka { get; set; }

            [JsonPropertyName("#IMDB_URL")]
            public string ImdbUrl { get; set; }

            [JsonPropertyName("#IMG_POSTER")]
            public string ImgPoster { get; set; }

            [JsonPropertyName("photo_width")]
            public int? PhotoWidth { get; set; }

            [JsonPropertyName("photo_height")]
            public int? PhotoHeight { get; set; }
        }
    }
}
