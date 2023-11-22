using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace MyTestableApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class paysList : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCountryInfo()
        {
            try
            {
                // Chemin du fichier infos.json (assurez-vous que le chemin est correct)
                string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "infos.json");

                // Vérifie si le fichier existe
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    return NotFound("Le fichier infos.json n'a pas été trouvé.");
                }

                // Lit le contenu du fichier JSON en tant que chaîne de caractères
                string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

                // Retourne le contenu brut du fichier
                return Content(jsonContent, "application/json");
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}