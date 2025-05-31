import {GameBoard} from "./board.js";
import { GameClient,GameHub, DotNetRef} from "./game.js";
import {APIRequest} from "./api.js";

try {
    
    // API

    window.setURL = (url) => {
        APIRequest.URL = url
    }

    window.logout = () => APIRequest.logout();
    window.login = (username, password) => APIRequest.login(username, password);
    window.register = (username, password, email, nickname) => APIRequest.register(username, password, email, nickname);

    // GAME HUB
    
    const REF_TABLE = new DotNetRef()
    const GAME_HUB = new GameHub();
    window.GAME_HUB = GAME_HUB;
    await GAME_HUB.start();
    
    // GAME BOARD
    const GAME_BOARD = new GameBoard(GAME_HUB,REF_TABLE)
    window.GAME_BOARD = GAME_BOARD;

    window.initializeBoard = async (game_board_container) => GAME_BOARD.init(game_board_container);
    window.drawBoard = (gameState,playerId) => GAME_BOARD.drawBoard(gameState,playerId);

    // GAME CLIENT
    
    const GAME_CLIENT = new GameClient(GAME_HUB,REF_TABLE);
    window.GAME_CLIENT = GAME_CLIENT;
   
    window.initializeMatchService = (dotNetReference) => GAME_CLIENT.initializeMatchService(dotNetReference);
    window.getConnectionId = () => GAME_HUB.connection.connection.connectionId;
    window.getPlayerState = async () => GAME_CLIENT.getPlayerState();
    window.getGameState = async () => GAME_CLIENT.getGameState();
    window.searchGame = async (playerId) => GAME_CLIENT.searchGame();
    window.leaveGame = async (playerId) => GAME_CLIENT.leaveGame();
    window.readyToPlay = async () => GAME_CLIENT.readyToPlay();
    window.endTurn = async  () => GAME_CLIENT.endTurn();
}catch (e){
    console.error(e);
}
