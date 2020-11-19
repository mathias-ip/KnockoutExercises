using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataServiceLayer;
using Exercise3_2_StarterProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exercise3_2_StarterProject.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IMapper _mapper;

        public MoviesController(IDataService dataService, IMapper mapper)
        {
            _dataService = dataService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetMovies))]
        public IActionResult GetMovies(int page = 0, int pageSize = 10)
        {
            var movies = _dataService.GetMovies(page, pageSize).Select(CreateDto);
            var total = _dataService.NumberOfMovies();
            var pages = Math.Ceiling(total / (double)pageSize);
            var prev = page > 0 ? Url.Link(nameof(GetMovies), new { page = page - 1, pageSize }) : null;
            var next = page < pages - 1 ? Url.Link(nameof(GetMovies), new { page = page + 1, pageSize }) : null;

            var result = new
            {
                total,
                pages,
                prev,
                next,
                items = movies
            };

            return Ok(result);
        }

        [HttpGet("{id}", Name = nameof(GetMovie))]
        public IActionResult GetMovie(string id)
        {
            var movie = _dataService.GetMovie(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(CreateDto(movie));
        }

        /**
         *
         * Helpers
         *
         */

        private MovieDto CreateDto(Movie movie)
        {
            var dto = _mapper.Map<MovieDto>(movie);
            dto.Url = Url.Link(nameof(GetMovie), new {movie.Id});
            return dto;
        }
    }
}
