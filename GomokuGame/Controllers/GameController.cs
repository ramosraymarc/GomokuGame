using GomokuGame.Constants;
using GomokuGame.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GomokuGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        [Route("CreateBoard")]
        public async Task<IActionResult> Board()
        {
            var response = _gameService.CreateBoard(15, 15);

            return new OkObjectResult(response);
        }

        [HttpPost]
        [Route("PlaceStone")]
        public async Task<IActionResult> Stone(int playerId, int row, int column)
        {
            var response = _gameService.PlaceStone(playerId, row, column).Result;
            switch (response.Result)
            {
                case BoardConstants.Successful:
                case BoardConstants.Draw:
                case BoardConstants.Continue:
                    return new OkObjectResult(response);
                    break;
                case BoardConstants.Invalid:
                case BoardConstants.Exception:
                    return new BadRequestObjectResult(response);
                    break;
                default:
                    return new OkObjectResult(response);
                    break;
            }
            
        }
    }
}
