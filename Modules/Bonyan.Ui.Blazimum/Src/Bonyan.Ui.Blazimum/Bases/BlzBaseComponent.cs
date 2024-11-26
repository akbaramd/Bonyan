using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Bonyan.Ui.Blazimum.Bases
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

        /// <summary>
        /// A unique identifier to assist with debugging or testing scenarios.
        /// </summary>
        public string UniqueId => Id ?? Guid.NewGuid().ToString();

        /// <summary>
        /// Allows logging of component parameters for debugging purposes.
        /// </summary>
        public void LogParameters()
        {
            var parameterData = new
            {
                Id,
                Class,
                Style,
                AdditionalAttributes
            };
            Console.WriteLine(JsonSerializer.Serialize(parameterData, new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        /// Merges additional attributes into the existing properties, handling id, class, and style specially.
        /// </summary>
        protected override void OnInitialized()
        {
            // Extract "id" from AdditionalAttributes if set
            if (AdditionalAttributes.TryGetValue("id", out var additionalId))
            {
                Id = additionalId?.ToString();
            }

            // If no id is set, generate a GUID
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = Guid.NewGuid().ToString();
            }

            // Extract "class" from AdditionalAttributes if set
            if (AdditionalAttributes.TryGetValue("class", out var additionalClass))
            {
                // Merge AdditionalAttributes class with explicitly set Class
                Class = $"{Class} {additionalClass}".Trim();
                AdditionalAttributes.Remove("class"); // Prevent duplicate rendering
            }

            // Extract "style" from AdditionalAttributes if set
            if (AdditionalAttributes.TryGetValue("style", out var additionalStyle))
            {
                // Merge AdditionalAttributes style with explicitly set Style
                Style = $"{Style}; {additionalStyle}".Trim(';', ' ');
                AdditionalAttributes.Remove("style"); // Prevent duplicate rendering
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Dynamically adds or updates a custom attribute.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        public void SetAttribute(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Attribute key cannot be null or whitespace.", nameof(key));

            AdditionalAttributes[key] = value;
            StateHasChanged();
        }

        /// <summary>
        /// Dynamically removes a custom attribute.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        public void RemoveAttribute(string key)
        {
            if (AdditionalAttributes.ContainsKey(key))
            {
                AdditionalAttributes.Remove(key);
                StateHasChanged();
            }
        }

        /// <summary>
        /// Checks if a specific attribute exists.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <returns>True if the attribute exists; otherwise, false.</returns>
        public bool HasAttribute(string key)
        {
            return AdditionalAttributes.ContainsKey(key);
        }
    }
}
