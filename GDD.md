# LOD2 — Game Design Document

Visão definida pelo Leo (14/07/2026), com notas de análise. Este é o documento
de referência do jogo — atualizar conforme as decisões evoluírem.

## Visão geral
- **Gênero:** ARPG de mundo aberto com dungeons, exploração, monstros, animais e PvP
- **Visual:** pixel-art **16-bit**, tema **dark-fantasy com cores vibrantes**, **2.5D**
  com visão de cima inclinada (estilo League of Legends / Diablo 4)
- **Biomas:** gelo, terra, sombra, floresta, fogo

## Classes (3)
| Classe | Perfil | Fonte de dano |
|---|---|---|
| **Mago** | Feitiços e magia; usa **mana** | Magia |
| **Lutador** | Tanque: muita vida, armadura e resistência; curto alcance | Itens/equipamento (melee) |
| **Atirador** | Pouca vida/resistência; muito dano e velocidade de ataque; longo alcance | Auto-attack do equipamento (arco, pistola…) |

> Nota de design: a diferença Lutador × Atirador é **alcance + sobrevivência**
> (ambos dependem de equipamento). Lutador = linha de frente constante;
> Atirador = vidro-canhão à distância.

- Cada classe tem **habilidades no Q/W/E/R** (kit próprio por classe)

## Progressão
- **Level** por XP; mobs/bosses dropam **ouro**, **XP** e **chance de item**
- **Aprimoramento de itens** (ferreiro), **de armadura** e **de habilidades** (feiticeiro)
- **Árvore de habilidades por classe:** 1 ponto por level; máx **5 pontos por
  habilidade**; mais pontos = habilidade mais forte

> Nota de design (matemática da árvore): com 4 habilidades × 5 pontos = 20
> pontos, no level 20 tudo maximiza e a escolha some. Opções pra manter escolha:
> (a) adicionar **passivas** na árvore, (b) custo crescente nos níveis 4-5,
> (c) cap de level < 20 no início. — decidir quando implementar a árvore.

## Mundo
- **Mundo aberto** com vilas, cidades e NPCs
- **Missões de NPC** (mate X / colete Y no início)
- **Ferreiro** = aprimorar itens · **Feiticeiro** = aprimorar habilidades
- **Dungeons** temáticas por bioma (herda as "criptas" do LOD)
- **Animais** (fauna passiva; drop de recursos)

> Nota de escopo: começar com **2 biomas** (hub = floresta; 1ª dungeon = sombra,
> que é a cripta atual) e adicionar os outros um a um.

## PvP
- Meta: PvP entre jogadores.

> Nota de escopo: PvP depende do multiplayer (motivo da migração pra Unity).
> Ordem segura: **co-op primeiro** (2 players juntos, valida a rede) →
> **PvP como arena/duelo** em área específica → só depois avaliar PvP aberto.

## Roadmap de implementação
1. **Fundação RPG** — XP + level + ouro dropando dos inimigos (HUD), chance de item
2. **3 classes de verdade** — seleção de classe, stats por classe, Q/W/E/R por classe
3. **Árvore de habilidades** — pontos por level, tela da árvore, skills níveis 1-5
4. **Mundo/biomas** — hub floresta expandido, 2ª dungeon temática, animais
5. **Vila + NPCs** — ferreiro, feiticeiro, missões simples
6. **Multiplayer co-op (POC)** → **PvP arena**

## Estado atual (14/07/2026)
Unity 6 URP, tudo montado por código (GameBootstrap): mago com auto-attack +
Q/W/E/R, cripta com 4 ondas (ghoul/sprinter/boss/boss veloz), HUD com orbes/
hotbar/cooldowns, dano flutuante, portas hub↔cripta, pausa e inventário básicos.
