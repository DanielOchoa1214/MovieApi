using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;

namespace MovieApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoriesController: ControllerBase
	{
        private readonly ICategoryRepository _catRepo;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository catRepo, IMapper mapper)
        {
            _catRepo = catRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var categoriesList = _catRepo.GetCategories();
            var categoiesListDTO = new List<CategoryDTO>();

            foreach (var category in categoriesList)
            {
                categoiesListDTO.Add(_mapper.Map<CategoryDTO>(category));
            }
            return Ok(categoiesListDTO);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId:int}", Name = "GetCategory")]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategory(int categoryId)
        {
            var categoryItem = _catRepo.GetCategory(categoryId);

            if (categoryItem == null) return NotFound();

            var categoryItemDTO = _mapper.Map<CategoryDTO>(categoryItem);
            return Ok(categoryItemDTO);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CategoryCreationDTO categoryCreationDTO)
        {
            if (!ModelState.IsValid || categoryCreationDTO == null) return BadRequest(ModelState);

            if (_catRepo.CategoryExists(categoryCreationDTO.Name))
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(404, ModelState);
            }

            var category = _mapper.Map<Category>(categoryCreationDTO);
            if (!_catRepo.CreateCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong, I broke :( {category.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategory", new { categoryId = category.Id }, category);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{categoryId:int}", Name = "UpdatePatchCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult UpdatePatchCategory(int categoryId, [FromBody] CategoryDTO categoryDTO)
        {

            if (!ModelState.IsValid || categoryDTO == null || categoryId != categoryDTO.Id) return BadRequest(ModelState);

            var category = _mapper.Map<Category>(categoryDTO);

            if (!_catRepo.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong updating, I broke :( {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{categoryId:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCategory(int categoryId)
        {

            if (!_catRepo.CategoryExists(categoryId)) return NotFound();

            var category = _catRepo.GetCategory(categoryId);


            if (!_catRepo.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong deleting, I broke :( {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}

