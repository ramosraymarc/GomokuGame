using GomokuGame.Models;

namespace GomokuGame.Interfaces
{
    public interface IGameService
    {
        Task<ResponseMessage> CreateBoard(int length, int width);
        Task<ResponseMessage> PlaceStone(int playerId, int rowNumber, int columnNumber);
    }
}
