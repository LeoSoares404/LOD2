using UnityEditor;
using UnityEngine;

/// Mantém a arte pixel nítida: toda textura em Assets/Resources entra com
/// filtro Point (sem borrão), sem compressão e sem mipmap.
public class PixelArtImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Replace('\\', '/').Contains("/Resources/"))
            return;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Default;  // carregado via Resources.Load<Texture2D>
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
    }
}
