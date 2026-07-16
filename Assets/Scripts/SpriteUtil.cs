using UnityEngine;

/// Fatiamento de spritesheets e sprites de Assets/Resources.
public static class SpriteUtil
{
    /// Fatia um spritesheet em [linhas, colunas]; pivô na base (pés no chão).
    public static Sprite[,] Slice(string resourceName, int cols, int rows, float ppu)
    {
        var tex = Resources.Load<Texture2D>(resourceName);
        if (tex == null)
        {
            Debug.LogWarning($"SpriteUtil: '{resourceName}' não encontrado em Assets/Resources.");
            return null;
        }
        int cw = tex.width / cols;
        int ch = tex.height / rows;
        var frames = new Sprite[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                // linha 0 = topo da imagem; textura Unity tem y=0 embaixo
                var rect = new Rect(c * cw, tex.height - (r + 1) * ch, cw, ch);
                frames[r, c] = Sprite.Create(tex, rect, new Vector2(0.5f, 0f), ppu);
            }
        return frames;
    }

    /// Textura inteira como sprite de mundo (pivô na base).
    public static Sprite Whole(string resourceName, float ppu)
    {
        var tex = Resources.Load<Texture2D>(resourceName);
        if (tex == null)
        {
            Debug.LogWarning($"SpriteUtil: '{resourceName}' não encontrado em Assets/Resources.");
            return null;
        }
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0f), ppu);
    }

    /// Sprite circular gerado em pixel (moedas/poções sem arte dedicada).
    public static Sprite Circle(Color c, int size = 10, float ppu = 16f)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        float r = size / 2f - 0.5f;
        var dark = new Color(c.r * 0.45f, c.g * 0.45f, c.b * 0.45f, 1f);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - (size - 1) / 2f;
                float dy = y - (size - 1) / 2f;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                Color px = d <= r ? (d > r - 1.3f ? dark : c) : Color.clear;
                tex.SetPixel(x, y, px);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), ppu);
    }

    /// Textura inteira como sprite de UI (pivô central).
    public static Sprite Ui(string resourceName)
    {
        var tex = Resources.Load<Texture2D>(resourceName);
        if (tex == null)
        {
            Debug.LogWarning($"SpriteUtil: '{resourceName}' não encontrado em Assets/Resources.");
            return null;
        }
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
    }
}
