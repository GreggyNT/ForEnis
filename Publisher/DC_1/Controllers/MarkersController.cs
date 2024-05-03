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
    [Route("api/v1.0/markers")]
    public class MarkersController : ControllerBase
    {
        private IBaseService<MarkerRequestDto,MarkerResponseDto> authorService;
        private IValidator<MarkerRequestDto> _authorValidator;
        private IDistributedCache _redis;
        public MarkersController(IBaseService<MarkerRequestDto,MarkerResponseDto> authorService,IValidator<MarkerRequestDto> authorValidator, IDistributedCache redis)
        {
            this.authorService = authorService;
            _authorValidator = authorValidator;
            _redis = redis;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<MarkerResponseDto>> GetAuthors() => Ok(authorService.GetAll());

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<MarkerResponseDto> CreateAuthor([FromBody]MarkerRequestDto dto)  { try
            {
                if (_authorValidator.Validate(dto).IsValid)
                    return CreatedAtAction("CreateAuthor", authorService.Create(dto));
            }
            catch (DbUpdateException e)
            {                   
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteAuthor(long id)
        {
            return authorService.Delete(id)?NoContent():NotFound(); 
        }

        [HttpPut]
        public ActionResult<MarkerResponseDto> UpdateAuthor([FromBody] MarkerRequestDto dto)
        {

            return authorService.Update(dto) == null ? NotFound(dto) : Ok(dto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MarkerResponseDto>> GetAuthor(long id)
        {
            var res = await _redis.GetStringAsync($"markers/{id}");
            if (String.IsNullOrEmpty(res))
            {
                res = JsonConvert.SerializeObject(authorService.Read(id));
                await _redis.SetStringAsync($"markers/{id}", res);
            }
            return Ok(JsonConvert.DeserializeObject<MarkerResponseDto>(res));
        }
    }
}
