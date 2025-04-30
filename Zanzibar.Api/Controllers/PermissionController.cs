using Microsoft.AspNetCore.Mvc;
using Zanzibar.Core.Interfaces;
using Zanzibar.Core.Models;

namespace Zanzibar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IZanzibarService _zanzibarService;

    public PermissionController(IZanzibarService zanzibarService)
    {
        _zanzibarService = zanzibarService;
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckPermission([FromBody] RelationTuple tuple)
    {
        if (tuple == null)
        {
            return BadRequest("Relation tuple is required");
        }
        var hasPermission = await _zanzibarService.CheckPermissionAsync(tuple);
        return Ok(new { hasPermission });
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddRelation([FromBody] RelationTuple tuple)
    {
        if (tuple == null)
        {
            return BadRequest("Relation tuple is required");
        }
        await _zanzibarService.AddRelationAsync(tuple);
        return Ok();
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRelation([FromBody] RelationTuple tuple)
    {
        if (tuple == null)
        {
            return BadRequest("Relation tuple is required");
        }
        await _zanzibarService.RemoveRelationAsync(tuple);
        return Ok();
    }

    [HttpGet("relations")]
    public async Task<IActionResult> GetRelations(
        [FromQuery] string @namespace,
        [FromQuery] string @object,
        [FromQuery] string relation)
    {
        if (string.IsNullOrEmpty(@namespace) || string.IsNullOrEmpty(@object) || string.IsNullOrEmpty(relation))
        {
            return BadRequest("Namespace, object, and relation are required");
        }
        var relations = await _zanzibarService.GetRelationsAsync(@namespace, @object, relation);
        return Ok(relations);
    }

    [HttpGet("has-permission")]
    public async Task<IActionResult> HasPermission(
        [FromQuery] string @namespace,
        [FromQuery] string @object,
        [FromQuery] string relation,
        [FromQuery] string subject)
    {
        if (string.IsNullOrEmpty(@namespace) || string.IsNullOrEmpty(@object) || 
            string.IsNullOrEmpty(relation) || string.IsNullOrEmpty(subject))
        {
            return BadRequest("Namespace, object, relation, and subject are required");
        }
        var hasPermission = await _zanzibarService.HasPermissionAsync(@namespace, @object, relation, subject);
        return Ok(new { hasPermission });
    }
} 