class GameHub {
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

class DotNetRef {
   refTable = {} 
    
    register(name,reference){
       this.refTable[name] = reference;
    }
    
   get(name){
       return this.refTable[name];
   } 
}

class GameClient {
    #refTable = null;
    #hub = null
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
        if (this.#hub != null) { 
            this.clearListeners()
        }

        this.#hub.connection.on("Join_game", this.#JoinGame);
        this.#hub.connection.on("game_has_changed", this.#GameHasChanged);
        this.#hub.connection.on("leave_game", this.#LeaveGame);
    }
    
    #JoinGame(match, player){
        let ref = this.#refTable("match")
        if (ref) {
            ref.invokeMethodAsync("NotifyJoinGame", match, player);
        }
    }
    
    #GameHasChanged() {
        let ref = this.#refTable("match")
        if (ref) {
            ref.invokeMethodAsync("GameHasChanged");
        }
    }
    
    #LeaveGame() {
        let ref = this.#refTable("match")
        if (ref) {
            ref.invokeMethodAsync("NotifyLeaveGame");
        }
    }
    
    clearListeners (){
        if (this.#hub.connection == null) { return }
        this.#hub.connection.off("Join_game");
        this.#hub.connection.off("game_has_changed");
        this.#hub.connection.off("leave_game");
    }

    async searchGame(playerId) {
        if (this.#hub.connection == null) { return }
        await this.#hub.connection.invoke("SearchGameAsync", playerId, this.connection.connection.connectionId)
    }

    async leaveGame(playerId) {
        if (this.#hub.connection == null) { return }
        await this.#hub.connection.invoke("LeaveGameAsync", playerId)
    }

    async getPlayerState() {
        if (this.#hub.connection == null) { return }
        return await this.#hub.connection.invoke("GetPlayerStateAsync")
    }

    async getGameState() {
        if (this.#hub.connection == null) { return }
        return await this.#hub.connection.invoke("GetGameStateAsync")
    }
}

export {GameHub, GameClient};