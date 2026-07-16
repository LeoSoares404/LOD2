/// Estado global do jogo (porte do game_state.gd, autoload do Godot).
public static class GameState
{
    public static string SelectedClass = "mago";
    /// "mouse" = Clássico (andar com botão direito) · "wasd" = Moderno (F1 troca)
    public static string ControlScheme = "mouse";
    public static int CurrentWave = 0;

    // — progressão (GDD, passo 1: fundação RPG) —
    public static int Level = 1;
    public static int Xp = 0;
    public static int Gold = 0;
    public static int SkillPoints = 0;   // +1 por level (árvore de habilidades = passo 3)

    public static int XpToNext() => 20 + (Level - 1) * 12;

    /// Soma XP e sobe de nível quando passa do limiar. Retorna níveis ganhos.
    public static int AddXp(int amount)
    {
        Xp += amount;
        int gained = 0;
        while (Xp >= XpToNext())
        {
            Xp -= XpToNext();
            Level++;
            SkillPoints++;
            gained++;
        }
        return gained;
    }

    public static void AddGold(int amount) => Gold += amount;
}
