﻿using System.Diagnostics;
using System.Net;
using Confluent.Kafka;
using FluentValidation;
using lab_1.Dtos.RequestDtos;
using lab_1.Dtos.ResponseDtos;
using lab_1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace lab_1.Controllers
{
    [ApiController]
    [Route("api/v1.0/comments")]
    public class CommentsController : ControllerBase
    {
        private IAsyncService<CommentRequestDto,CommentResponseDto> authorService;
        private IValidator<CommentRequestDto> _authorValidator;
        private IDistributedCache _redis;
        public CommentsController(IAsyncService<CommentRequestDto,CommentResponseDto> authorService, IValidator<CommentRequestDto> authorValidator,IDistributedCache redis)
        {
            this.authorService = authorService;
            _authorValidator = authorValidator;
            _redis = redis;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CommentResponseDto>>> GetAuthors() => Ok(await authorService.GetAllAsync());

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CommentResponseDto>> CreateAuthor([FromBody]CommentRequestDto dto)  {
            try
            {
                if (_authorValidator.Validate(dto).IsValid)
                    return CreatedAtAction("CreateAuthor", await authorService.CreateAsync(dto));
            }
            catch (DbUpdateException e)
            {                   
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async  Task<ActionResult> DeleteAuthor(long id)
        {
            var res = await authorService.DeleteAsync(id);
           return res?NoContent():NotFound(); 
        }

        [HttpPut]
        public async  Task<ActionResult<CommentResponseDto>> UpdateAuthor([FromBody] CommentRequestDto dto)
        { 
            await authorService.UpdateAsync(dto);
            var res = JsonConvert.SerializeObject(await authorService.ReadAsync(dto.Id));
            await _redis.SetStringAsync($"markers/{dto.Id}", res);
            return  String.IsNullOrEmpty(res) ? NotFound(dto) : Ok(dto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async  Task<ActionResult<CommentResponseDto>> GetAuthor(long id)
        {
            var res = await _redis.GetStringAsync($"comments/{id}");
            if (String.IsNullOrEmpty(res))
            {
                res = JsonConvert.SerializeObject(await authorService.ReadAsync(id));
                await _redis.SetStringAsync($"markers/{id}", res);
            }
            return Ok(JsonConvert.DeserializeObject<CommentResponseDto>(res));
        }
        
    }
}
