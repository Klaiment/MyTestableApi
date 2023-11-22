using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace MyTestableApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaysController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PaysController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetCountryInfo([FromQuery] string nom)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nom))
                {
                    return BadRequest("Le champ 'nom' est vide.");
                }

                var paysTrouve = await GetCountryByNameAsync(nom);

                if (paysTrouve != null)
                {
                    var response = new Dictionary<string, Coordonnees>
                    {
                        { paysTrouve.Nom, new Coordonnees { Latitude = paysTrouve.Coordonnees.Latitude, Longitude = paysTrouve.Coordonnees.Longitude } }
                    };

                    return Ok(response);
                }
                else
                {
                    return NotFound($"Le pays '{nom}' n'a pas été trouvé dans la liste.");
                }
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, "Une erreur s'est produite lors de la récupération des données.");
            }
            catch (JsonException)
            {
                return StatusCode(500, "Une erreur s'est produite lors de la désérialisation des données.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        private async Task<Pays> GetCountryByNameAsync(string nom)
        {
            var paysListUrl = "http://localhost:5065/paysList";
            var paysList = await GetPaysListAsync(paysListUrl);

            return paysList.Pays.Find(p => string.Equals(p.Nom, nom, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<PaysList> GetPaysListAsync(string url)
        {
            var jsonContent = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<PaysList>(jsonContent);
        }

        private class PaysList
        {
            public List<Pays> Pays { get; set; }
        }

        private class Pays
        {
            public string Nom { get; set; }
            public string Ville { get; set; }
            public int Population { get; set; }
            public Coordonnees Coordonnees { get; set; }
        }

        private class Coordonnees
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}
