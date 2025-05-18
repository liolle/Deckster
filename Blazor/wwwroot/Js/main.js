import {GameBoard} from "./board.js";
import {GameHub,GameClient} from "./game.js";

try {
    const GAME_HUB = new GameHub();
    window.GAME_HUB = GAME_HUB;
    await GAME_HUB.start();

    const GAME_CLIENT = new GameClient(GAME_HUB);
    window.GAME_CLIENT = GAME_CLIENT;

    const GAME_BOARD = new GameBoard(GAME_HUB)
    window.GAME_BOARD = GAME_BOARD;

    window.initializeBoard = async (game_board_container) => GAME_BOARD.init(game_board_container);
    window.drawBoard = (gameState) => GAME_BOARD.drawBoard(gameState)

    window.initializeMatchService = (dotNetReference) => GAME_CLIENT.initializeMatchService(dotNetReference);
    window.getConnectionId = () => GAME_HUB.connection.connection.connectionId;
    window.getPlayerState = async () => GAME_CLIENT.getPlayerState();
    window.getGameState = async () => GAME_CLIENT.getGameState();
    window.searchGame = async (playerId) => GAME_CLIENT.searchGame(playerId);
    window.leaveGame = async (playerId) => GAME_CLIENT.leaveGame(playerId);

}catch (e){
    console.error(e);
}
