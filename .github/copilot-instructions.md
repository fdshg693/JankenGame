# GitHub Copilot Instructions for JankenGame

## Project Overview

This is a Blazor Server web application built with ASP.NET Core 9.0 and C# 13, featuring multiple casual games including Rock-Paper-Scissors (Janken) and Blackjack.

## Architecture

### Framework & Technology Stack
- **Framework**: ASP.NET Core 9.0
- **UI Framework**: Blazor Interactive Server Rendering
- **Language**: C# 13
- **Target Framework**: .NET 9.0

### Project Structure

```
Components/Pages/     - Razor pages with @rendermode InteractiveServer
Models/              - Game logic models (Janken, BlackJack)
Services/Janken/     - Game services (logic, state management)
wwwroot/             - Static assets (CSS, Bootstrap)
```

## Key Components

### Janken (Rock-Paper-Scissors) Games

1. **Multi-player Janken** (`/janken`)
   - Supports 2-6 players (1 human + computers)
   - Uses `JankenGameService` for multi-player winner determination
   - Models: `JankenPlayer`, `MultiPlayerGameRecord`
   - Logic: `JankenLogicService.GetWinningHands()` for winner calculation

2. **Janken Challenge** (`/janken-challenge`)
   - Score-based game: +10 for correct, -5 for wrong
   - Uses `JankenChallengeGame` model for state management
   - Tracks accuracy, play count, correct/wrong answers

### Blackjack (`/blackjack`)
   - Classic 21 card game
   - Models: `Card`, `Deck`, `Hand`
   - Simple hit/stand mechanics

## Important Patterns & Conventions

### Blazor Component Patterns
- All game pages use `@rendermode InteractiveServer`
- State management within component `@code` blocks
- Use `StateHasChanged()` when needed for UI updates

### Service Registration
- **Note**: Services are NOT registered in `Program.cs`
- Services are instantiated directly in components
- Consider this when suggesting dependency injection

### Model Design
- **Enums**: `JankenHand` (Rock/Paper/Scissors), `JankenResultEnum` (Win/Lose/Draw)
- **Records**: Use record types for immutable game results (`MultiPlayerGameRecord`, `JankenGameResult`)
- **Classes**: Use classes for mutable game state (`JankenPlayer`, `JankenChallengeGame`)

### Game Logic Layer
- `JankenLogicService`: Pure logic methods (stateless)
  - `GetWinningHands()`: Determines winning hand(s) from played hands
  - `DetermineWinner()`: Calculates result between two players
- `JankenGameService`: Game state and multi-player support
- `JankenChallengeService`: Challenge mode logic

## Styling Conventions
- Use scoped CSS files (`.razor.css`)
- Follow existing Bootstrap-based styling
- Use semantic class names (e.g., `challenge-container`, `player-hand`, `score-board`)

## When Adding New Features

1. **New Game**: Create in `Components/Pages/` with corresponding models in `Models/`
2. **Services**: Add to `Services/` folder (remember: not auto-registered)
3. **Navigation**: Update `NavMenu.razor`
4. **Models**: 
   - Use records for immutable game results
   - Use classes for mutable state
   - Keep enums simple and descriptive

## Common Pitfalls to Avoid

- Don't assume services are registered in DI container
- Remember to use `@rendermode InteractiveServer` for interactive components
- Janken logic uses 3-way comparison (Rock < Paper < Scissors < Rock)
- Multi-player games support draws (all same or all three types played)

## Testing Notes

- Run with: `dotnet run` or `dotnet watch`
- Default URL: `https://localhost:5001`
- No unit tests currently implemented

## Language & Localization

- **UI Language**: Japanese (all user-facing text)
- **Code Comments**: Minimal, code should be self-documenting
- **Documentation**: Japanese for README.md, English for technical docs
