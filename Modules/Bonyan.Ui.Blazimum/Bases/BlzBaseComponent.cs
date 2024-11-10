using Microsoft.AspNetCore.Components;
using System;

namespace Bonyan.Ui.Blazimum
{
    public class BlzBaseComponent : ComponentBase
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Class { get; set; } = string.Empty;

        [Parameter]
        public string Style { get; set; } = string.Empty;

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

        protected override void OnInitialized()
        {
            // Set Id to a new GUID if it has not been set by the user
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = Guid.NewGuid().ToString();
            }
            base.OnInitialized();
        }
    }
}