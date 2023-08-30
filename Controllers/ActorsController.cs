using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/actors")]
    public class ActorsController : CustomBaseController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "actors";

        public ActorsController(ApplicationDbContext dbContext, IMapper mapper, IFileStorage fileStorage) :
            base(dbContext, mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            return await Get<Actor, ActorDTO>(paginationDTO);
        }

        [HttpGet("{id:int}", Name = "getActorById")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            return await Get<Actor, ActorDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {

            var entity = mapper.Map<Actor>(actorCreationDTO);
            if (actorCreationDTO.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreationDTO.Photo.CopyToAsync(memoryStream);

                    var photoContent = memoryStream.ToArray();

                    var extension = Path.GetExtension(actorCreationDTO.Photo.FileName);

                    entity.Photo = await fileStorage.SaveFile(photoContent, extension, container, actorCreationDTO.Photo.ContentType);
                }
            }

            dbContext.Add(entity);

            await dbContext.SaveChangesAsync();

            var actor = mapper.Map<ActorDTO>(entity);

            return CreatedAtRoute("getActorById", new { Id = actor.Id }, actor);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromForm] ActorCreationDTO actorCreationDTO, int id)
        {


            var actor = await dbContext.Actors.FirstOrDefaultAsync(actorDb => actorDb.Id == id);

            if (actor == null) return NotFound("404, you already know klk");

            actor = mapper.Map(actorCreationDTO, actor);

            if (actorCreationDTO.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreationDTO.Photo.CopyToAsync(memoryStream);

                    var photoContent = memoryStream.ToArray();

                    var extension = Path.GetExtension(actorCreationDTO.Photo.FileName);

                    actor.Photo = await fileStorage.EditFile(photoContent, extension, container, actor.Photo, actorCreationDTO.Photo.ContentType);
                }
            }

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);

        }
    }
}
