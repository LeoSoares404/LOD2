# LOD2 — Legends of Darkness (Unity / C#)

Reimplementação em **Unity + C#** do LOD (originalmente feito em Godot 4).
ARPG **2.5D dark-fantasy isométrico**, com o objetivo de **multiplayer online** e
**personagens salvos**.

- Projeto original (Godot 4 / GDScript): https://github.com/LeoSoares404/LOD
- Engine: **Unity** (recomendado template **3D URP** ou **2D**, ver abaixo)
- Linguagem: **C#**

## Como começar
1. **Unity Hub** → *New project* → escolha o template (ver "Decisões" abaixo) →
   **Location = esta pasta `LOD2`** (Unity vai gerar `Assets/`, `ProjectSettings/`,
   `Packages/`).
2. Abra o projeto e comece pelo core (câmera iso + player que anda).
3. `Assets/` deve ser versionado; `Library/`, `Temp/`, `obj/` etc. já estão no
   `.gitignore`.

## Decisões em aberto (definir no começo)
- **2D vs 2.5D real:** o LOD virou 2.5D (sprites billboard em mundo 3D). No Unity dá
  pra fazer igual (3D + billboards) ou puro 2D. Recomendo espelhar o LOD: **3D URP +
  sprites**.
- **Rede:** o motivo da migração. Avaliar **Mirror**, **Netcode for GameObjects** ou
  **Photon** logo cedo, com um POC de 2 players sincronizados antes de portar tudo.
- **Save de personagem:** classe/atributos/inventário — definir formato (JSON local
  primeiro, depois backend).

## O que portar do LOD (referência de lógica)
Os **assets** (PNGs em `LOD/assets` e `LOD/image`) reaproveitam direto. A **lógica**
(GDScript) vira C#:
- Movimento click-to-move + WASD, câmera isométrica (rig com suavização)
- Combate: skills Q/W/E/R, auto-attack por classe, cooldown/mana
- Inimigos + `WaveManager` (ondas, boss)
- Progressão: inventário, gems, armadura, XP/nível
- UI: HUD (orbes, hotbar), moldura dark-fantasy, fonte 8-bit, menus
- Mundo: overworld (hub) ↔ criptas via porta; classes (Mago/Arqueiro/Lutador)

## Status
🚧 Recém-criado — scaffold do repositório. Projeto Unity ainda não gerado.
