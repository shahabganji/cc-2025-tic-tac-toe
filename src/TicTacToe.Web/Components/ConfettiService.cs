using Microsoft.JSInterop;

namespace TicTacToe.Web.Components;

internal sealed class ConfettiService(IJSRuntime Js)
{
    public async Task CelebrateWinner() => await Js.InvokeVoidAsync("showConfetti", args: true);
    public async Task InformLoser() => await Js.InvokeVoidAsync("showConfetti", args: false);
}
