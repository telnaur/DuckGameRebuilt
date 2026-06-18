using DuckGame;

namespace DuckGame.PokemonHatPack
{
    public class PokemonHatPack : Mod
    {
      protected override void OnPostInitialize()
      {
        Teams.core.teams.Add(new Team("Piplup", Mod.GetPath<PokemonHatPack>("piplup"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Chimchar", Mod.GetPath<PokemonHatPack>("chimchar"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Machamp", Mod.GetPath<PokemonHatPack>("machamp"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Seviper", Mod.GetPath<PokemonHatPack>("seviper"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Makuhita", Mod.GetPath<PokemonHatPack>("makuhita"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Eevee", Mod.GetPath<PokemonHatPack>("eevee"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Raikou", Mod.GetPath<PokemonHatPack>("raikou"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Blastoise", Mod.GetPath<PokemonHatPack>("blastoise"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Pichu", Mod.GetPath<PokemonHatPack>("pichu"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Sealeo", Mod.GetPath<PokemonHatPack>("sealeo"), false, false, new Vec2()));
        Teams.core.teams.Add(new Team("Noctowl", Mod.GetPath<PokemonHatPack>("noctowl"), false, false, new Vec2()));

        base.OnPostInitialize();
      }
    }
  }
