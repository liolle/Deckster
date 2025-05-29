import {GameBoard} from "./board.js";
import { GameClient,GameHub, DotNetRef} from "./game.js";

try {
    const REF_TABLE = new DotNetRef()
    const GAME_HUB = new GameHub();
    window.GAME_HUB = GAME_HUB;
    await GAME_HUB.start();

    const GAME_BOARD = new GameBoard(GAME_HUB,REF_TABLE)
    window.GAME_BOARD = GAME_BOARD;
    
    const GAME_CLIENT = new GameClient(GAME_HUB,REF_TABLE);
    window.GAME_CLIENT = GAME_CLIENT;


    window.initializeBoard = async (game_board_container) => GAME_BOARD.init(game_board_container);
    window.drawBoard = (gameState,playerId) => GAME_BOARD.drawBoard(gameState,playerId);

    window.initializeMatchService = (dotNetReference) => GAME_CLIENT.initializeMatchService(dotNetReference);
    window.getConnectionId = () => GAME_HUB.connection.connection.connectionId;
    window.getPlayerState = async () => GAME_CLIENT.getPlayerState();
    window.getGameState = async () => GAME_CLIENT.getGameState();
    window.searchGame = async (playerId) => GAME_CLIENT.searchGame(playerId);
    window.leaveGame = async (playerId) => GAME_CLIENT.leaveGame(playerId);
    window.readyToPlay = async (playerId) => GAME_BOARD.readyToPlay(playerId);
}catch (e){
    console.error(e);
}
