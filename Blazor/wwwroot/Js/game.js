export  class GameHub {
    connection = null
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${window.location.origin}/${window.env["HUB_ENDPOINT"]}`)
            .build(); 
        
    }
    async start() {
        try {
            await this.connection.start();
            console.info("SignalR Connected.");
        } catch (err) {
            console.error(err);
        }
    }
}

export class DotNetRef {
   table = {} 
    
    register(name,reference){
       this.table[name] = reference;
    }
    
   get(name){
       return this.table[name];
   } 
}

export  class GameClient {
    #refTable = null
    #hub = null
    
    #clearHubListenersCb = []
    
    constructor(hub,refTable) {
       if (!hub.connection) {
           console.error("Missing connection");
           return;
       }
       this.#hub = hub;
       this.#refTable = refTable;
       this.setUpConnection()
    }
    
    initializeMatchService(dotNetReference) {
        if (!dotNetReference) {
            console.error("Missing dotNetReference");
            return;
        }
        this.#refTable.register("match",dotNetReference) 
    }

    setUpConnection() {
        let conn = this.#hub.connection

        let join = this.#JoinGame.bind(this)
        let game_change = this.#GameHasChanged.bind(this)
        let leave = this.#LeaveGame.bind(this)
        let OnGameTurnTick = this.#OnGameTurnTick.bind(this)
        let OnGamePlayerTurn = this.#OnGamePlayerTurn.bind(this)
        
        conn.on("Join_game", join);
        conn.on("game_has_changed", game_change);
        conn.on("leave_game", leave);
        conn.on("GameTurnTick",OnGameTurnTick)
        conn.on("GamePlayerTurn",OnGamePlayerTurn)

        this.#clearHubListenersCb.push(()=>{
            conn.off("Join_game", join)
        })
        
        this.#clearHubListenersCb.push(()=>{
            conn.off("game_has_changed", game_change)
        })
        
        this.#clearHubListenersCb.push(()=>{
            conn.off("leave_game", leave)
        })
        
        this.#clearHubListenersCb.push(()=>{
            conn.off("GameTurnTick", OnGameTurnTick)
        })

        this.#clearHubListenersCb.push(()=>{
            conn.off("GamePlayerTurn", OnGamePlayerTurn)
        })
    }
    
    #OnGamePlayerTurn(args){
        let board = window.GAME_BOARD
        board.updateTurnButton(args[0])
    }

    #OnGameTurnTick(args){
        let board = window.GAME_BOARD
        board.updateTurnTimer(args[1])
    }

    #JoinGame(match, player){
        let ref = this.#refTable.table["match"]
        if (ref) {
            ref.invokeMethodAsync("NotifyJoinGame", match, player);
        }
    }

    #GameHasChanged() {
        let ref = this.#refTable.table["match"]
        if (ref) {
            ref.invokeMethodAsync("GameHasChanged");
        }
    }

    #LeaveGame() {
        let ref = this.#refTable.table["match"]
        if (ref) {
            ref.invokeMethodAsync("NotifyLeftGame");
        }
    }
   
    clearListeners (){
        for (const cb in this.#clearHubListenersCb) {
           cb() 
        } 
    }

    async searchGame() {
        let conn = this.#hub.connection
        if (conn == null) { return }
        await conn.invoke("SearchGameAsync")
    }

    async leaveGame() {
        let conn = this.#hub.connection
        if (conn == null) { return }
        await conn.invoke("LeaveGameAsync")
    }

    async getPlayerState() {
        let conn = this.#hub.connection
        if (conn == null) { return }
        return await conn.invoke("GetPlayerStateAsync")
    }

    async getGameState() {
        let conn = this.#hub.connection
        if (conn == null) { return }
        return await conn.invoke("GetGameStateAsync")
    }
    
    async readyToPlay()
    {
        let conn = this.#hub.connection
        if (conn == null) { return }
        return await conn.invoke("ReadyToPlayAsync")
    }
    
    async endTurn()
    {
        let conn = this.#hub.connection
        if (conn == null) { return }
        return await conn.invoke("EndTurnAsync")
    }
}