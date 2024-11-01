using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Stock;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/comments")] // Base URL
    [ApiController] // API Controller marker
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() {
            var comments = await _commentRepo.GetCommentsAsync();
            var commentsList = comments.Select(x => x.ToCommentDTO());
            return Ok(commentsList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id) {
            var comment = await _commentRepo.GetCommentByIdAsync(id);

            if (comment == null) {
                return NotFound();
            } else {
                return Ok(comment.ToCommentDTO());
            }
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> CreateCommentForStock([FromRoute] int stockId, CreateCommentDTO commentDTO) {

            if (!await _stockRepo.StockExists(stockId)) {
                return BadRequest("Stock does not exist");
            } else {
                var commentModel = commentDTO.ToCreateCommentDTO(stockId);

                await _commentRepo.CreateCommentAsync(commentModel);

               return Ok(commentModel.ToCommentDTO());
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDTO commentDTO) {
            var comment = await _commentRepo.UpdateCommentAsync(id, commentDTO.ToUpdateCommentDTO());

            if (comment == null) {
                return NotFound();
            } 

            return Ok(comment.ToCommentDTO());
        }
    }
}