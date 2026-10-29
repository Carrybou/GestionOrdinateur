using GestionOrdinateurs.Shared.Models;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Service;
using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API.Event;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdinateursController : ControllerBase, IDisposable
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly ILogger<OrdinateursController> _logger;
    private readonly JournalService _journalService;
    public readonly IConfiguration _configuration;

    public OrdinateursController(IOrdinateurService ordinateurService, ILogger<OrdinateursController> logger, IConfiguration configuration, JournalService journalService)
    {
        _ordinateurService = ordinateurService;
        _logger = logger;
        _configuration = configuration;
        _journalService = journalService;

        _ordinateurService.OrdinateurAjoute += OnOrdinateurAjoute;
        _ordinateurService.OrdinateurSupprime += OnOrdinateurSupprime;
        _ordinateurService.OrdinateurMisAJour += OnOrdinateurMisAJour;
    }


    // GET
    [HttpGet("ordinateurs/{id}")]
    public async Task<IActionResult> GetOrdinateurById(int id)
    {
        var ordinateur = await _ordinateurService.GetByIdAsync(id);
        if (ordinateur is null)
        {
            _logger.LogInformation("null");
            return NotFound();
        }
        _logger.LogInformation(ordinateur.ToString());
        return Ok(ordinateur);
    }

    [HttpGet("ordinateurs/{id}/diagnostic")]
    public async Task<IActionResult> GetDiagnostic(int id) // Ajout de la méthode pour diagnostiquer un ordinateur
    {
        var ordinateur = await _ordinateurService.GetByIdAsync(id);
        if (ordinateur is null)
        {
            return NotFound();
        }

        return Ok(_ordinateurService.Diagnostiquer(ordinateur));
    }

    [HttpGet("ordinateurs")]
    public async Task<IActionResult> GetAllOrdinateur()
    {
        var ordinateurs = await _ordinateurService.GetAllAsync();
        ordinateurs.ForEach((ordinateur) => _logger.LogInformation(ordinateur.ToString()));
        return Ok(ordinateurs);
    }

    [HttpGet("ordinateurs/disponibles")]
    public async Task<IActionResult> GetOrdinateursDisponibles()
    {
        var ordinateurs = await _ordinateurService.GetDisponiblesAsync();
        ordinateurs.ForEach((ordinateur) => _logger.LogInformation(ordinateur.ToString()));
        return Ok(ordinateurs);
    }

    [HttpGet("ordinateurs/marque/{marque}")]
    public async Task<IActionResult> GetOrdinateurByMarque(string marque)
    {
        var ordinateurs = await _ordinateurService.GetByMarqueAsync(marque);
        ordinateurs.ForEach((ordinateur) => _logger.LogInformation(ordinateur.ToString()));
        return Ok(ordinateurs);
    }

    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> ChercherDansInventaire(string nom)
    {
        var inventaire = new InventaireOrdinateurs
        {
            Ordinateurs = await _ordinateurService.GetAllAsync()
        };

        var ordinateur = inventaire[nom];
        return ordinateur is null ? NotFound() : Ok(ordinateur);
    }

    [HttpGet("ordinateurs/prix-dec")]
    public async Task<IActionResult> GetOrdinateursDuPlusCherAuMoinsCher()
    {
        var ordinateurs = await _ordinateurService.GetDuPlusCherAuMoinsCherAsync();
        ordinateurs.ForEach((ordinateur) => _logger.LogInformation(ordinateur.ToString()));
        return Ok(ordinateurs);
    }

    [HttpGet("ordinateurs/info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            Nom = _configuration["Application:Name"],
            Version = _configuration["Application:Version"]
        });
    }

    [HttpGet("ordinateurs/statistiques")]
    public IActionResult GetStatistiques()
    {
        var (total, enStock, valeurStock, nbReparations) =
            _ordinateurService.CalculerStatistiques();

        return Ok(new
        {
            TotalOrdinateur = total,
            OrdiDisponibles = enStock,
            ValeurStock = valeurStock,
            NbReparations = nbReparations
        });
    }
    //POst
    [HttpPost("ordinateurs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PostOrdinateur(Ordinateur ordinateur)
    {
        var resp = await _ordinateurService.AddAsync(ordinateur);
        return resp ? Created() : BadRequest(); 
    }
    //Put
    [HttpPut("ordinateurs/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutOrdinateur(int id, Ordinateur ordinateur)
    {
        var ordinateurPut = await _ordinateurService.UpdateAsync(id, ordinateur);
        return ordinateurPut == null ? NotFound() : Ok(ordinateurPut); 
    }
    //DELETE
    [HttpDelete("ordinateurs/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrdinateur(int id)
    {
        return await _ordinateurService.DeleteAsync(id) ? NoContent() : NotFound();

    }

    // Event
    private void OnOrdinateurAjoute(object? sender, OrdinateurAjouteEventArgs e) =>
        JournaliserOrdinateur($"Ordinateur ajouté -> {e.Ordinateur.Nom}", e.DateAjout);

    private void OnOrdinateurSupprime(object? sender, OrdinateurSupprimeEventArgs e) =>
        JournaliserOrdinateur($"Ordinateur supprimé -> {e.Ordinateur.Nom}", e.DateAjout);

    private void OnOrdinateurMisAJour(object? sender, OrdinateurMisAJourEventArgs e) =>
        JournaliserOrdinateur($"Ordinateur mis à jour -> {e.Ordinateur.Nom}", e.DateAjout);

    private void JournaliserOrdinateur(string message, DateTime date)
    {
        _logger.LogInformation($"Evenement : {message} a {date}");
        _journalService.EcrireOrdinateur(message);
    }

    public void Dispose()
    {
        _ordinateurService.OrdinateurAjoute -= OnOrdinateurAjoute;
        _ordinateurService.OrdinateurSupprime -= OnOrdinateurSupprime;
        _ordinateurService.OrdinateurMisAJour -= OnOrdinateurMisAJour;
    }

}