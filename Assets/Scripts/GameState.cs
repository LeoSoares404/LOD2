/// Estado global do jogo (porte do game_state.gd, autoload do Godot).
public static class GameState
{
    public static string SelectedClass = "mago";
    /// "mouse" = Clássico (andar com botão direito) · "wasd" = Moderno (F1 troca)
    public static string ControlScheme = "mouse";
    public static int CurrentWave = 0;
}
