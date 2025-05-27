namespace Blazor.services.game.state;

public class GamePlayerPlaying : GameState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();
    }


    public override async Task<bool> EndTurn(string playerId)
    {
        return await base.EndTurn(playerId);
    }
}