using GomokuGame.Constants;
using GomokuGame.Interfaces;
using GomokuGame.Models;

namespace GomokuGame.Services
{
    public sealed class GameService : IGameService
    {
        public Board board;

        public GameService() => board = new Board();

        /// <summary>
        /// Create and initialize board setup.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> CreateBoard(int length, int width)
        {
            var responseMessage = new ResponseMessage();
            int[,] boardDimensions = new int[length, width];
            for (int row = 0; row < boardDimensions.GetLength(0); row++)
            {
                for (int col = 0; col < boardDimensions.GetLength(1); col++)
                {
                    boardDimensions[row, col] = 0;
                }
            }

            board.Dimensions = boardDimensions;
            board.Length = length;
            board.Width = width;

            responseMessage.Message = BoardConstants.Created;
            responseMessage.Result = BoardConstants.Successful;

            return responseMessage;
        }

        /// <summary>
        /// Place stone to the board by passing the current player, row, and column position.
        /// </summary>
        /// <param name="currentPlayerTurn"></param>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ResponseMessage> PlaceStone(int currentPlayerTurn, int rowNumber, int columnNumber)
        {
            var responseMessage = new ResponseMessage();

            if (board.Dimensions == null)
            {
                responseMessage.Message = BoardConstants.NoBoardCreated;
                responseMessage.Result = BoardConstants.Invalid;
                return responseMessage;
            }
            try
            {
                var boardRow = board.Dimensions?.GetLength(0);
                var boardColumn = board.Dimensions?.GetLength(1);

                var player = new Player();
                player.Id = currentPlayerTurn;
                player.Name = currentPlayerTurn == 1 ? BoardConstants.PlayerOne : BoardConstants.PlayerTwo;
                player.StoneColor = currentPlayerTurn == 1 ? BoardConstants.ColorWhite : BoardConstants.ColorBlack;
                player.Row = rowNumber;
                player.Column = columnNumber;

                if (rowNumber < boardRow && columnNumber < boardColumn)
                {
                    var currentPosition = board.Dimensions?[rowNumber, columnNumber];
                    if (currentPosition.Equals(1) || currentPosition.Equals(2))
                    {
                        responseMessage.Message = BoardConstants.ExistingStonePlaced;
                        responseMessage.Result = BoardConstants.Invalid;
                    }
                    else
                    {
                        board.Dimensions[rowNumber, columnNumber] = currentPlayerTurn;
                        var result = CheckWinner(board.Dimensions, rowNumber, columnNumber);
                        switch (result)
                        {
                            case 1: //Winner
                                responseMessage.Message = $"Congratulations! {player.Name} with {player.StoneColor} stone won the game!";
                                responseMessage.Result = BoardConstants.Successful;
                                break;
                            case 2: //Draw
                                responseMessage.Message = BoardConstants.DrawGame;
                                responseMessage.Result = BoardConstants.Draw;
                                break;
                            default:
                                responseMessage.Message = $"{player.Name} with {player.StoneColor} stone took his turn and placed the stone in row: {player.Row} and column: {player.Column}.";
                                responseMessage.Result = BoardConstants.Continue;
                                break;
                        }
                    }
                }
                else
                {
                    responseMessage.Message = BoardConstants.InvalidValue;
                    responseMessage.Result = BoardConstants.Invalid;
                }       
            }
            catch (Exception ex)
            {
                responseMessage.Message = $"Application threw an error: {ex.Message}.";
                responseMessage.Result = BoardConstants.Exception;
            }
            return responseMessage;
        }

        #region Private Methods
        private int CheckWinner(int[,] gameBoard, int row, int col)
        {
            if (IsGameWinner(gameBoard, row, col)) return 1;

            if (IsGameDraw(gameBoard)) return 2;

            return 0;
        }

        private bool IsGameWinner(int[,] gameBoard, int row, int col)
        {
            if (CheckHorizontalPattern(gameBoard, row, col)
                || CheckVerticalPattern(gameBoard, row, col)
                || CheckDiagonalLeftToRightPattern(gameBoard, row, col)
                || CheckDiagonalRightToLeftPattern(gameBoard, row, col))
            {
                return true;
            }

            return false;
        }

        private static bool IsGameDraw(int[,] gameBoard)
        {
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    if (gameBoard[row, col].Equals(0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckDiagonalRightToLeftPattern(int[,] gameBoard, int row, int col)
        {
            int connectedStoneNumber = 0;

            for (int i = row + 1, j = col - 1; i < gameBoard.GetLength(0) && j > 0; i++, j--)
            {
                if (ValidateBoardCells(gameBoard, row, col, i, j, ref connectedStoneNumber))
                {
                    continue;
                }

                break;
            }

            if (connectedStoneNumber < BoardConstants.NumberOfConnectedStoneToWin)
            {
                for (int i = row - 1, j = col + 1; i >= 0 && j < gameBoard.GetLength(1); i--, j++)
                {
                    if (ValidateBoardCells(gameBoard, row, col, i, j, ref connectedStoneNumber))
                    {
                        continue;
                    }

                    break;
                }
            }

            return ValidateWinInARow(connectedStoneNumber);
        }

        private bool CheckDiagonalLeftToRightPattern(int[,] gameBoard, int row, int col)
        {
            int connectedStoneNumber = 0;

            for (int i = row + 1, j = col + 1; i < gameBoard.GetLength(0) && j < gameBoard.GetLength(1); i++, j++)
            {
                if (ValidateBoardCells(gameBoard, row, col, i, j, ref connectedStoneNumber))
                {
                    continue;
                }

                break;
            }

            if (connectedStoneNumber < BoardConstants.NumberOfConnectedStoneToWin)
            {
                for (int i = row - 1, j = col - 1; i >= 0 && j >= 0; i--, j--)
                {
                    if (ValidateBoardCells(gameBoard, row, col, i, j, ref connectedStoneNumber))
                    {
                        continue;
                    }

                    break;
                }
            }

            return ValidateWinInARow(connectedStoneNumber);
        }

        private bool CheckVerticalPattern(int[,] gameBoard, int row, int col)
        {
            int connectedStoneNumber = 0;

            for (int i = row + 1; i < gameBoard.GetLength(1); i++)
            {
                if (ValidateBoardCells(gameBoard, row, col, i, col, ref connectedStoneNumber))
                {
                    continue;
                }

                break;
            }

            if (connectedStoneNumber < BoardConstants.NumberOfConnectedStoneToWin)
            {
                for (int i = row - 1; i >= 0; i--)
                {
                    if (ValidateBoardCells(gameBoard, row, col, i, col, ref connectedStoneNumber))
                    {
                        continue;
                    }

                    break;
                }
            }

            return ValidateWinInARow(connectedStoneNumber);
        }

        private bool CheckHorizontalPattern(int[,] gameBoard, int row, int col)
        {
            int connectedStoneNumber = 0;

            for (int i = col + 1; i < gameBoard.GetLength(1); i++)
            {
                if (ValidateBoardCells(gameBoard, row, col, row, i, ref connectedStoneNumber))
                {
                    continue;
                }

                break;
            }

            if (connectedStoneNumber < BoardConstants.NumberOfConnectedStoneToWin)
            {
                for (int i = col - 1; i >= 0; i--)
                {
                    if (ValidateBoardCells(gameBoard, row, col, row, i, ref connectedStoneNumber))
                    {
                        continue;
                    }

                    break;
                }
            }

            return ValidateWinInARow(connectedStoneNumber);
        }

        private bool ValidateBoardCells(int[,] board, int currentRow, int currentCol, int compareRow, int compareCol, ref int nextInARow)
        {
            int currentCell = board[currentRow, currentCol];
            int compareCell = board[compareRow, compareCol];

            if (currentCell.Equals(compareCell) && !currentCell.Equals(0))
            {
                nextInARow++;

                return true;
            }

            return false;
        }

        private bool ValidateWinInARow(int connectedStoneNumber) => connectedStoneNumber == BoardConstants.NumberOfConnectedStoneToWin;
        #endregion
    }
}
