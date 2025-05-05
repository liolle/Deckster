class GameClient {
    matchServiceReference = null;

    connection = null

    initializeMatchService(dotNetReference) {
        this.matchServiceReference = dotNetReference;
        setUpConnection()
    }

    setUpConnection() {
        // Create SignalR hub
        // Need a way to have the URL as env variable.

        if (this.connection != null) { return }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${window.location.origin}/${window.env["HUB_ENDPOINT"]}`)
            .build();

        this.connection.on("Join_game", (match, player) => {
            // Call the .NET method to trigger the event
            if (matchServiceReference) {
                matchServiceReference.invokeMethodAsync("NotifyJoinGame", match, player);
            }
        });

        this.connection.on("game_has_changed", () => {
            if (matchServiceReference) {
                matchServiceReference.invokeMethodAsync("GameHasChanged");
            }
        });

        this.connection.on("leave_game", () => {
            if (matchServiceReference) {
                matchServiceReference.invokeMethodAsync("NotifyLeftGame");
            }
        });

    }

    startConnection() {
        this.connection.start().catch(err => console.error(err.toString()));
    }

    async searchGame(playerId) {
        if (this.connection == null) { return }
        this.connection.invoke("SearchGameAsync", playerId, this.connection.connection.connectionId)
    }

    async leaveGame(playerId) {
        if (this.connection == null) { return }
        this.connection.invoke("LeaveGameAsync", playerId)
    }
}

const GAME_CLIENT = new GameClient();
window.GAME_CLIENT = GAME_CLIENT
GAME_CLIENT.setUpConnection()
GAME_CLIENT.startConnection()

window.initializeMatchService = (dotNetReference) => GAME_CLIENT.initializeMatchService(dotNetReference);
window.getConnectionId = () => GAME_CLIENT.connection.connection.connectionId;
window.searchGame = async (playerId) => GAME_CLIENT.searchGame(playerId)
window.leaveGame = async (playerId) => GAME_CLIENT.leaveGame(playerId)