using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Controllers.Interfaces;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;


namespace MoviesAPI.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IMapper mapper;

        protected CustomBaseController(ApplicationDbContext appDbContext, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entity = await appDbContext.Set<TEntity>().ToListAsync();
            return mapper.Map<List<TDTO>>(entity);

        }

        protected async Task<ActionResult<TDTO>> Get<Tentity, TDTO>(int id) where Tentity : class, IId
        {
            var entity = await appDbContext.Set<Tentity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound();

            return mapper.Map<TDTO>(entity);
        }
        public async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO paginationDTO) where TEntity : class
        {
            var query = appDbContext.Set<TEntity>().AsQueryable();

            await HttpContext.CountTotalOfRecords(query, paginationDTO.PageRecords);

            var entity = await query.Paginate(paginationDTO).ToListAsync();

            return mapper.Map<List<TDTO>>(entity);
        }
        public async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO paginationDTO, IQueryable<TEntity> queryable) where TEntity : class
        {
            //var query = appDbContext.Set<TEntity>().AsQueryable();

            await HttpContext.CountTotalOfRecords(queryable, paginationDTO.PageRecords);

            var entity = await queryable.Paginate(paginationDTO).ToListAsync();

            return mapper.Map<List<TDTO>>(entity);
        }
        protected async Task<ActionResult> Post<TEntity, TDTO, TRead>(TDTO tDTO, string routeName) where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(tDTO);

            appDbContext.Add(entity);

            await appDbContext.SaveChangesAsync();

            var entityDTO = mapper.Map<TRead>(entity);

            return CreatedAtRoute(routeName, new { id = entity.Id }, entityDTO);

        }
        protected async Task<ActionResult> Put<TEntity, TDTO>(TDTO tDTO, int id) where TEntity : class, IId
        {
            var entity = await appDbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound();

            mapper.Map(tDTO, entity);

            await appDbContext.SaveChangesAsync();

            return NoContent();
        }
        public async Task<ActionResult> Patch <TEntity, TDTOPatch>(int id, JsonPatchDocument<TDTOPatch> jsonPatchDocument)
            where TEntity: class, IId where TDTOPatch : class
        {
            if (jsonPatchDocument == null) return BadRequest();

            var entity = await appDbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound();

            var entityDto = mapper.Map<TDTOPatch>(entity);

            jsonPatchDocument.ApplyTo(entityDto, ModelState);

            bool isValid = TryValidateModel(entityDto);

            if (!isValid) return BadRequest();

            mapper.Map(entityDto, entity);

            await appDbContext.SaveChangesAsync();

            return NoContent();
        }
        protected async Task<ActionResult> Delete <TEntity>(int id) where TEntity : class, IId, new()
        {
            var entity = await appDbContext.Set<TEntity>().AnyAsync(x => x.Id == id);

            if (!entity) return NotFound("ma'fella, you got a 404");

            appDbContext.Remove(new TEntity() { Id = id });

            await appDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
