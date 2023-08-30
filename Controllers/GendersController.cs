using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/genders")]
    public class GendersController : CustomBaseController
    {
        public GendersController(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderDTO>>> GetGenders()
        {
            return await Get<Gender, GenderDTO>();
        }

        [HttpGet("{id:int}", Name = "getGenderById")]
        public async Task<ActionResult<GenderDTO>> Get(int id)
        {
            return await Get<Gender, GenderDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(GenderCreationDTO genderCreationDTO)
        {
            return await Post<Gender, GenderCreationDTO, GenderDTO>(genderCreationDTO, "getGenderById");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, GenderCreationDTO genderCreationDTO)
        {
            return await Put<Gender, GenderCreationDTO>(genderCreationDTO, id);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Gender>(id);
        }

    }
}
