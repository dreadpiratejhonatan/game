# Roadmap de Desenvolvimento - Modular Game Engine

## 🎯 Fase 1: Mecânicas Básicas (1-2 semanas)

### ✅ Completado
- [x] Estrutura ECS base
- [x] ModManager (data-driven)
- [x] Sistema de renderização placeholder
- [x] Carregamento de unidades do JSON
- [x] Compilação e estrutura do projeto

### 🔨 Próximos Passos Imediatos

#### 1.1 - Sistema de Input
- [ ] Criar `InputSystem`
- [ ] Capturar teclado (WASD) e mouse
- [ ] Implementar câmera 2D com movimento
- [ ] Zoom in/out com scroll

**Arquivos a criar:**
- `src/Game/Systems/InputSystem.cs`
- `src/Game/Components/CameraComponent.cs`

#### 1.2 - Seleção de Unidades
- [ ] Criar `SelectionSystem`
- [ ] Clicar em unidade para selecionar
- [ ] Desenhar retângulo de seleção múltipla (drag)
- [ ] Highlight visual nas unidades selecionadas

**Arquivos a criar:**
- `src/Game/Systems/SelectionSystem.cs`
- `src/Game/Components/SelectableComponent.cs`

#### 1.3 - Movimento Básico
- [ ] Criar `MovementSystem` (exemplo já fornecido)
- [ ] Clicar com botão direito para mover unidades
- [ ] Pathfinding básico (linha reta primeiro, A* depois)
- [ ] Formação de grupo (unidades não sobrepõem)

**Arquivos a criar:**
- `src/Game/Systems/MovementSystem.cs`
- `src/Game/Components/VelocityComponent.cs`
- `src/Game/Components/PathComponent.cs`

---

## 🎮 Fase 2: Combate e Interação (2-3 semanas)

### 2.1 - Sistema de Combate
- [ ] Criar `CombatSystem`
- [ ] Detecção de inimigos em range
- [ ] Auto-ataque quando em alcance
- [ ] Cálculo de dano e HP
- [ ] Morte de unidades

**Arquivos a criar:**
- `src/Game/Systems/CombatSystem.cs`
- `src/Game/Components/TeamComponent.cs` (facções)
- `src/Game/Components/TargetComponent.cs`

### 2.2 - Sistema de Colisão
- [ ] Criar `CollisionSystem`
- [ ] Bounding boxes ou circle collision
- [ ] Unidades não atravessam umas às outras
- [ ] Colisão com obstáculos (futuro)

**Arquivos a criar:**
- `src/Game/Systems/CollisionSystem.cs`
- `src/Game/Components/ColliderComponent.cs`

### 2.3 - AI Básica
- [ ] Criar `AISystem`
- [ ] Estados: Idle, Patrol, Chase, Attack, Flee
- [ ] Inimigos detectam jogador em raio
- [ ] Perseguição e ataque automático

**Arquivos a criar:**
- `src/Game/Systems/AISystem.cs`
- `src/Game/Components/AIComponent.cs`

**Adicionar ao JSON:**
```json
"ai": {
  "detection_range": 300,
  "attack_when_health_below": 30
}
```

---

## 🖼️ Fase 3: UI e Feedback Visual (1-2 semanas)

### 3.1 - UI de Gameplay
- [ ] Barra de HP acima das unidades
- [ ] Minimapa no canto
- [ ] Painel de unidade selecionada (stats, portrait)
- [ ] Botões de habilidades (placeholder)

**Arquivos a criar:**
- `src/Game/Systems/UISystem.cs`
- `src/Game/Components/UIComponent.cs`

### 3.2 - Efeitos Visuais
- [ ] Partículas de dano (números flutuando)
- [ ] Rastro de projéteis
- [ ] Animação de morte (fade out)
- [ ] Indicadores de seleção (anel no chão)

**Arquivos a criar:**
- `src/Game/Systems/ParticleSystem.cs`
- `src/Game/Components/ParticleEmitterComponent.cs`

---

## 📦 Fase 4: Expansão de Mods (1 semana)

### 4.1 - Novos Tipos de Dados
- [ ] `items.json` (armas, armaduras)
- [ ] `skills.json` (habilidades, magias)
- [ ] `factions.json` (alianças, reputação)
- [ ] `biomes.json` (tipos de terreno)

**Arquivos a criar:**
- `data/base_mod/items.json`
- `data/base_mod/skills.json`
- `data/base_mod/factions.json`

### 4.2 - Sistema de Inventário
- [ ] Criar `InventorySystem`
- [ ] Equipar/desequipar itens
- [ ] Modificar stats baseado em equipment
- [ ] Loot de inimigos mortos

**Arquivos a criar:**
- `src/Game/Systems/InventorySystem.cs`
- `src/Game/Components/InventoryComponent.cs`

### 4.3 - Sistema de Skills
- [ ] Criar `SkillSystem`
- [ ] Cooldowns e mana/energia
- [ ] Habilidades ativas (hotkeys 1-5)
- [ ] Efeitos de área (AOE)

**Arquivos a criar:**
- `src/Game/Systems/SkillSystem.cs`
- `src/Game/Components/SkillComponent.cs`

---

## 🎨 Fase 5: Arte e Polimento (2-4 semanas)

### 5.1 - Sistema de Sprites
- [ ] Carregar sprite sheets
- [ ] Animações (idle, walk, attack, death)
- [ ] Direções (8 direções ou flip horizontal)
- [ ] Layers de renderização (chão, unidades, UI)

**Arquivos a criar:**
- `src/Engine/Graphics/SpriteRenderer.cs`
- `src/Engine/Graphics/AnimationController.cs`

**Adicionar ao JSON:**
```json
"visual": {
  "sprite_sheet": "units/soldier.png",
  "frame_width": 32,
  "frame_height": 32,
  "animations": {
    "idle": [0, 1, 2, 3],
    "walk": [4, 5, 6, 7],
    "attack": [8, 9, 10]
  }
}
```

### 5.2 - Áudio
- [ ] Carregar sons (JSON)
- [ ] SFX (ataque, morte, seleção)
- [ ] Música de fundo (loop)
- [ ] Volume settings

**Arquivos a criar:**
- `src/Engine/Audio/AudioManager.cs`

### 5.3 - Juice e Polimento
- [ ] Screen shake em explosões
- [ ] Slow-motion em mortes importantes
- [ ] Trail effects em movimento rápido
- [ ] Feedback tátil em ações

---

## 🌍 Fase 6: Mundo e Campanha (3-5 semanas)

### 6.1 - Sistema de Mapa
- [ ] Tile-based map
- [ ] Carregar mapas de arquivos (Tiled JSON)
- [ ] Colisão com terreno
- [ ] Fog of war (exploração)

**Arquivos a criar:**
- `src/Game/Systems/MapSystem.cs`
- `data/base_mod/maps/level1.json`

### 6.2 - Missões e Objetivos
- [ ] Sistema de quests
- [ ] Triggers de eventos
- [ ] Diálogo simples (JSON)
- [ ] Cutscenes básicas

### 6.3 - Progresso e Save
- [ ] Salvar/carregar jogo (JSON)
- [ ] Persistência de inventário
- [ ] Unlock de habilidades
- [ ] Estatísticas (kills, tempo jogado)

---

## 🔧 Fase 7: Otimização e Ferramentas (1-2 semanas)

### 7.1 - Performance
- [ ] Profiling e benchmarks
- [ ] Spatial hashing para colisões
- [ ] Object pooling (projéteis, partículas)
- [ ] Culling (não renderizar fora da tela)

### 7.2 - Editor de Mods
- [ ] Hot-reload de JSON (F5 recarrega mods)
- [ ] Console de debug (F12)
- [ ] Spawn de unidades (comandos)
- [ ] God mode e cheats

**Arquivos a criar:**
- `src/Tools/DebugConsole.cs`
- `src/Tools/ModHotReloader.cs`

### 7.3 - Documentação de Modding
- [ ] Tutorial de criação de mods
- [ ] Exemplos de mods prontos
- [ ] API reference para scripters
- [ ] Workshop/comunidade (futuro)

---

## 🚀 Fase 8: Release e Comunidade (Ongoing)

### 8.1 - Build e Distribuição
- [ ] Builds para Windows, Linux, macOS
- [ ] Instalador/launcher
- [ ] Auto-update system
- [ ] Crash reporting

### 8.2 - Multiplayer (Opcional, futuro distante)
- [ ] Co-op local (split-screen)
- [ ] Co-op online (lobby)
- [ ] Sincronização de estado
- [ ] Anti-cheat básico

---

## 📊 Estimativas de Tempo

| Fase | Duração Estimada | Complexidade |
|------|------------------|--------------|
| Fase 1 | 1-2 semanas | ⭐⭐ |
| Fase 2 | 2-3 semanas | ⭐⭐⭐ |
| Fase 3 | 1-2 semanas | ⭐⭐ |
| Fase 4 | 1 semana | ⭐⭐ |
| Fase 5 | 2-4 semanas | ⭐⭐⭐⭐ |
| Fase 6 | 3-5 semanas | ⭐⭐⭐⭐ |
| Fase 7 | 1-2 semanas | ⭐⭐⭐ |
| Fase 8 | Ongoing | ⭐⭐⭐⭐⭐ |

**Total (Prototype MVP)**: ~3-5 meses de desenvolvimento solo

---

## 🎯 Milestones

### Milestone 1: "Playable Prototype" (Fim da Fase 2)
- Unidades se movem
- Combate funcional
- Seleção e controle de grupo
- **Demo jogável de 5 minutos**

### Milestone 2: "Content Ready" (Fim da Fase 4)
- Sistema de mods completo
- Inventário e skills
- 10+ unidades, 20+ items
- **Mods da comunidade possíveis**

### Milestone 3: "Alpha Release" (Fim da Fase 6)
- Campanha com 3 missões
- Arte pixel art básica
- Som e música
- **Versão pública para testers**

### Milestone 4: "Beta Release" (Fim da Fase 7)
- Otimizado e polido
- Editor de mods integrado
- Documentação completa
- **Early Access no itch.io**

---

## 💡 Dicas de Priorização

1. **Sempre teste cada feature isoladamente** antes de integrar
2. **Faça commits frequentes** (a cada feature completa)
3. **Mantenha o jogo sempre executável** (não deixe quebrado por dias)
4. **Prototype rápido, polish depois** (funcional > bonito)
5. **Documente enquanto desenvolve** (README.md atualizado)

---

**Este roadmap é flexível!** Ajuste conforme necessidade do projeto.
