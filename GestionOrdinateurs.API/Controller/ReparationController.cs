using GestionOrdinateurs.API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace GestionOrdinateurs.API.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReparationController(
    ReparationService reparationService,
    StockService stockService) : ControllerBase
{
    [HttpPost("simuler")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SimulerReparation()
    {
        var ordinateurs = await reparationService.GetOrdinateursAsync(3);
        if (ordinateurs.Count < 3)
        {
            return BadRequest("Au moins trois ordinateurs sont nécessaires.");
        }

        await Task.WhenAll(
            reparationService.SimulerReparationAsync(ordinateurs[0]),
            reparationService.SimulerReparationAsync(ordinateurs[1]),
            reparationService.SimulerReparationAsync(ordinateurs[2]));

        return Ok(new
        {
            Message = "Réparations simulées",
            NombreReparations = ordinateurs.Count // avant on renvoyait ordinateurs mais ca faisait un boucle infinie dans le JSON
        });
    }

    [HttpGet("/api/reparations/historique")]
    public async Task<IActionResult> GetHistorique()
    {
        return Ok(await reparationService.GetHistoriqueAsync());
    }

 

    [HttpGet("stock-ssd")]
    public IActionResult GetStockSSD()
    {
        Parallel.For(0, 10, _ =>
        {
            Task.Delay(10).Wait();
            stockService.ConsommerSSD();
        });

        Console.WriteLine($"Stock SSD restant : {stockService.StockSSD}");
        return Ok(stockService.StockSSD);
    }
}
