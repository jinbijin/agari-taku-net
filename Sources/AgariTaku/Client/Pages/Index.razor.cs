using AgariTaku.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace AgariTaku.Client.Pages
{
    public partial class Index
    {
        [Inject]
        public GameStateService GameState { get; set; }

        protected override Task OnInitializedAsync()
        {
            GameState.OnChange += HandleChange;
            return base.OnInitializedAsync();
        }

        private async Task Connect()
        {
            await GameState.StartConnection();
        }

        private void HandleChange()
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
