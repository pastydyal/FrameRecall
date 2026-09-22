using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FrameRecall.Services;
using FrameRecall.Services.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrameRecall.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmsController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Film>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Film>>> GetAll(
        [FromQuery] string? title,
        [FromQuery] FilmRating? rating,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            return Ok(await _filmService.GetByTitleAsync(title, cancellationToken));
        }

        if (rating.HasValue)
        {
            return Ok(await _filmService.GetByRatingAsync(rating.Value, cancellationToken));
        }

        return Ok(await _filmService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Film), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Film>> GetById(Guid id, CancellationToken cancellationToken)
    {
        Film? film = await _filmService.GetByIdAsync(id, cancellationToken);
        return film is null ? NotFound() : Ok(film);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Film), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Film>> Create(Film film, CancellationToken cancellationToken)
    {
        Film? created = await _filmService.CreateAsync(film, cancellationToken);
        if (created is null)
        {
            return BadRequest("Не удалось создать фильм. Проверьте название и рейтинг.");
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Film), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Film>> Update(
        Guid id,
        Film film,
        CancellationToken cancellationToken)
    {
        film.Id = id;
        Film? updated = await _filmService.UpdateAsync(film, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Film), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Film>> Delete(Guid id, CancellationToken cancellationToken)
    {
        Film? deleted = await _filmService.DeleteAsync(id, cancellationToken);
        return deleted is null ? NotFound() : Ok(deleted);
    }
}
