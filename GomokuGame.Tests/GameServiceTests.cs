using GomokuGame.Constants;
using GomokuGame.Interfaces;
using GomokuGame.Models;
using GomokuGame.Services;

namespace GomokuGame.Tests
{
    [TestFixture]
    public sealed class GameServiceTests
    {
        private readonly IGameService _gameService;
        private const int ConnectedFiveRows = 4;

        public GameServiceTests()
        {
            _gameService = new GameService();
        }

        [Test]
        public void ShouldSuccessWhenBoardIsCreated()
        {
            var response = _gameService.CreateBoard(15, 15).Result;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo(BoardConstants.Created));
            Assert.That(response.Result, Is.EqualTo(BoardConstants.Successful));
        }

        [Test]
        public void ShouldFailWhenPlacingStoneWithoutCreatedBoard()
        {
            var board = new GameService();
            var response = board.PlaceStone(1, 1, 1).Result;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo(BoardConstants.NoBoardCreated));
            Assert.That(response.Result, Is.EqualTo(BoardConstants.Invalid));
        }

        [Test]
        public void ShouldFailWhenPlacingStoneOutsideBoardDimensions()
        {
            _ = _gameService.CreateBoard(5, 5).Result;

            var response = _gameService.PlaceStone(1, 6, 6).Result;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo(BoardConstants.InvalidValue));
            Assert.That(response.Result, Is.EqualTo(BoardConstants.Invalid));
        }

        [Test]
        [TestCase(1, 1, 1)]
        [TestCase(2, 7, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 4, 8)]
        [TestCase(1, 6, 2)]
        [TestCase(2, 1, 8)]
        public void ShouldSuccessWhenPlacingStoneWithValidMove(int playerId, int rowNumber, int columnNumber)
        {
            _ = _gameService.CreateBoard(15, 15).Result;

            var response = _gameService.PlaceStone(playerId, rowNumber, columnNumber).Result;

            Assert.That(response.Message, Is.Not.Null);
            Assert.That(response.Message, Contains.Substring("stone took his turn and placed the stone"));
            Assert.That(response.Result, Is.EqualTo(BoardConstants.Continue));
        }

        [Test]
        public void ShouldFailWhenPlacingStoneInExisitingPosition()
        {
            _ = _gameService.CreateBoard(15, 15).Result;

            _ = _gameService.PlaceStone(1, 6, 6).Result;
            var response = _gameService.PlaceStone(1, 6, 6).Result;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo(BoardConstants.ExistingStonePlaced));
            Assert.That(response.Result, Is.EqualTo(BoardConstants.Invalid));
        }

        [Test]
        public void ShouldSuccessWhenThereIsWinner()
        {
            _ = _gameService.CreateBoard(15, 15).Result;
            _ = _gameService.PlaceStone(1, 1, 1).Result;
            _ = _gameService.PlaceStone(1, 1, 2).Result;
            _ = _gameService.PlaceStone(1, 1, 3).Result;
            _ = _gameService.PlaceStone(1, 1, 4).Result;
            var response = _gameService.PlaceStone(1, 1, 5).Result;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo("Congratulations! Player One with White stone won the game!"));
            Assert.That(response.Result, Is.EqualTo(BoardConstants.Successful));
        }
    }
}
