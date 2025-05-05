class GameClient {
    static matchServiceReference = null;

    static connection = null

    static initializeMatchService(dotNetReference) {
        if (this.matchServiceReference != null) { return }
        matchServiceReference = dotNetReference;

        setUpConnection()
    }

    static setUpConnection() {
        // Create SignalR hub
        // Need a way to have the URL as env variable.

        if (GameClient.connection != null) { return }

        GameClient.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${window.location.origin}/${window.env["HUB_ENDPOINT"]}`)
            .build();

        GameClient.connection.on("Join_game", (match, player) => {
            // Call the .NET method to trigger the event
            if (matchServiceReference) {
                matchServiceReference.invokeMethodAsync("NotifyJoinGame", match, player);
            }
        });

        GameClient.connection.on("game_has_changed", () => {
            if (matchServiceReference) {
                matchServiceReference.invokeMethodAsync("GameHasChanged");
            }
        });

        GameClient.connection.on("leave_game", () => {
            if (matchServiceReference) {
                matchServiceReference.invokeMethodAsync("NotifyLeftGame");
            }
        });

    }

    static startConnection() {
        GameClient.connection.start().catch(err => console.error(err.toString()));
    }

    async searchGame(playerId) {
        if (GameClient.connection == null) { return }
        GameClient.connection.invoke("SearchGameAsync", playerId, connection.connection.connectionId)
    }

    async leaveGame(playerId) {
        if (GameClient.connection == null) { return }
        GameClient.connection.invoke("LeaveGameAsync", playerId)
    }
}

const GAME_CLIENT = new GameClient();
GameClient.startConnection()

window.initializeMatchService = (dotNetReference) => GameClient.initializeMatchService(dotNetReference);
window.getConnectionId = () => GameClient.connection.connection.connectionId;
window.searchGame = async (playerId) => GAME_CLIENT.searchGame(playerId)
window.leaveGame = async (playerId) => GAME_CLIENT.leaveGame(playerId)
