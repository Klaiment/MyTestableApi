using Microsoft.AspNetCore.Mvc.Testing;
using MyTestableApi.Api;

namespace MyTestableApi.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class PaysControllerTests : IClassFixture<WebApplicationFactory<MyTestableApi.Api.Program>>
    {
        private readonly WebApplicationFactory<MyTestableApi.Api.Program> _factory;

        public PaysControllerTests(WebApplicationFactory<MyTestableApi.Api.Program> factory)
        {
            _factory = factory;
        }
        /*
        Scénario 1 : Le pays se trouve dans la base de données et est écrit correctement 
        GIVEN : Le pays que je demande est "France"
        WHEN : Je demande les coordonnées géographiques de la mairie de la ville la plus peuplé du pays
        THEN : Je récupère les coordonnées de la mairie  en JSON
        EX : {48.8566; 2.3522}
         */
        [Fact]
        public async Task TestGetCountryInfo_Success()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Pays?nom=France";
            var expectedResponse = "{\"France\":{\"latitude\":48.8566,\"longitude\":2.3522}}";

            // Act
            var response = await client.GetAsync(url);
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(expectedResponse, stringResponse);
        }
        /*
        Scénario 2 : Erreur d’écriture & Pays non disponible dans la base de donnée
        GIVEN : Le pays que je demande est "fransse"
        WHEN : Je demande les coordonnées géographiques de la mairie de la ville la plus peuplé du pays
        THEN : Je récupère les coordonnées de la mairie  en JSON
        EX : {"Le pays 'fransse' n'a pas été trouvé dans la liste."}

         */
        [Fact]
        public async Task TestGetCountryInfo_NotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Pays?nom=fransse";
            var expectedResponse = "Le pays 'fransse' n'a pas été trouvé dans la liste.";

            // Act
            var response = await client.GetAsync(url);
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expectedResponse, stringResponse);
        }
        /*
        Scénario 3 : L'input est vide
        GIVEN : Le pays que je demande est ""
        WHEN : Je demande les coordonnées géographiques de la mairie de la ville la plus peuplé du pays
        THEN : Je récupère les coordonnées de la mairie  en JSON
        EX : "The nom field is required."

         */
        [Fact]
        public async Task TestGetCountryInfo_ValidationError()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Pays?nom=";
            var expectedError = "The nom field is required.";

            // Act
            var response = await client.GetAsync(url);
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains(expectedError, stringResponse);
        }

    }
}
