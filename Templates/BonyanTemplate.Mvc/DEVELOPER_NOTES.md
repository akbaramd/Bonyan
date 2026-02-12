# Developer notes

## Build and run (no need to clean every time)

- **Normal workflow**: Run **Build** (or `dotnet build`). Then run the app. You do **not** need to run Clean every time.
- **When to Clean**: Only if the app still shows old behavior after a build (e.g. old view or old code). Then run **Clean Solution** once, then **Build**, then run again.
- **RCL / library changes**: If you change views or code in referenced projects (e.g. `Bonyan.Ui.BonWeb.Mvc`, Identity module), a normal **Build** will recompile them. Run the app from this project so it uses the latest build output.
- **This project’s views (Debug)**: In Debug configuration, Razor Runtime Compilation is enabled. Changes to `.cshtml` files in this project’s `Views` folder are picked up on **browser refresh** without rebuilding. Changes in RCL projects still require a build.
