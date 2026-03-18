using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SistemaReservaGimnasioBK.Application.DTOs;
using SistemaReservaGimnasioBK.Application.Queries;
using SistemaReservaGimnasioBK.Application.QueryHandlers;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Controllers;

/// <summary>
/// Controlador REST para gestión de espacios reservables.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SpacesController : ControllerBase
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly GetAvailableSpacesQueryHandler _availableSpacesHandler;
    private readonly IValidator<CreateSpaceDto> _validator;
    private readonly ILogger<SpacesController> _logger;

    public SpacesController(
        ISpaceRepository spaceRepository,
        GetAvailableSpacesQueryHandler availableSpacesHandler,
        IValidator<CreateSpaceDto> validator,
        ILogger<SpacesController> logger)
    {
        _spaceRepository = spaceRepository;
        _availableSpacesHandler = availableSpacesHandler;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los espacios activos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SpaceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpaces()
    {
        var spaces = await _spaceRepository.GetActiveSpacesAsync();

        var response = spaces.Select(s => new SpaceDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Capacity = s.Capacity,
            Type = s.Type.ToString(),
            IsActive = s.IsActive
        });

        return Ok(response);
    }

    /// <summary>
    /// Obtiene espacios disponibles en un período específico
    /// </summary>
    /// <param name="startDateTime">Fecha/hora de inicio</param>
    /// <param name="endDateTime">Fecha/hora de fin</param>
    /// <param name="type">Tipo de espacio (opcional)</param>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<SpaceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableSpaces(
        [FromQuery] DateTime startDateTime,
        [FromQuery] DateTime endDateTime,
        [FromQuery] SpaceType? type = null)
    {
        try
        {
            var query = new GetAvailableSpacesQuery(startDateTime, endDateTime, type);
            var result = await _availableSpacesHandler.HandleAsync(query);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un espacio por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpace(Guid id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);

        if (space == null)
            return NotFound(new { message = $"Espacio {id} no encontrado" });

        var response = new SpaceDto
        {
            Id = space.Id,
            Name = space.Name,
            Description = space.Description,
            Capacity = space.Capacity,
            Type = space.Type.ToString(),
            IsActive = space.IsActive
        };

        return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo espacio
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceDto dto)
    {
        // Validar con FluentValidation
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        try
        {
            // Verificar que no exista un espacio con el mismo nombre
            if (await _spaceRepository.ExistsByNameAsync(dto.Name))
            {
                return BadRequest(new { message = "Ya existe un espacio con ese nombre" });
            }

            // Crear la entidad usando el factory method del dominio
            var space = Space.Create(
                dto.Name,
                dto.Description,
                dto.Capacity,
                (SpaceType)dto.Type);

            // Persistir
            var created = await _spaceRepository.AddAsync(space);

            _logger.LogInformation("Espacio creado: {SpaceId} - {SpaceName}", created.Id, created.Name);

            var response = new SpaceDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Capacity = created.Capacity,
                Type = created.Type.ToString(),
                IsActive = created.IsActive
            };

            return CreatedAtAction(nameof(GetSpace), new { id = created.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un espacio
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateSpace(Guid id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);

        if (space == null)
            return NotFound(new { message = $"Espacio {id} no encontrado" });

        space.Deactivate();
        await _spaceRepository.UpdateAsync(space);

        _logger.LogInformation("Espacio desactivado: {SpaceId}", id);

        return NoContent();
    }
}
