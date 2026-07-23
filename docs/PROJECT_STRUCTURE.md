# 📁 Estrutura do Projeto - Modular Game Engine

## Árvore de Arquivos Completa

```
ModularGameEngine/
│
├── 📄 Program.cs                      # Ponto de entrada (Main)
├── 📄 ModularGameEngine.csproj        # Arquivo de projeto .NET
├── 📄 .gitignore                      # Git ignore rules
│
├── 📚 DOCUMENTAÇÃO
│   ├── 📖 README.md                   # Visão geral do projeto
│   ├── 📖 INSTALL.md                  # Guia de instalação
│   ├── 📖 ARCHITECTURE.md             # Documentação técnica
│   ├── 📖 ROADMAP.md                  # Plano de desenvolvimento
│   ├── 📖 QUICK_REFERENCE.md          # Comandos e patterns rápidos
│   ├── 📖 EXAMPLE_MOVEMENT.md         # Tutorial prático
│   ├── 📖 DIAGRAMS.md                 # Diagramas visuais
│   ├── 📖 RESOURCES.md                # Links úteis e recursos
│   └── 📖 PROJECT_STRUCTURE.md        # Este arquivo
│
├── 📂 src/                            # Código-fonte
│   │
│   ├── 📂 Engine/                     # 🔧 NÚCLEO DO MOTOR
│   │   │
│   │   ├── 📂 ECS/                    # Sistema Entity-Component-System
│   │   │   ├── 🔹 IComponent.cs       # Interface para componentes
│   │   │   ├── 🔹 Entity.cs           # Container de componentes
│   │   │   ├── 🔹 ISystem.cs          # Interface para sistemas
│   │   │   └── 🔹 World.cs            # Gerenciador de entidades/sistemas
│   │   │
│   │   ├── 📂 Graphics/               # Renderização avançada (futuro)
│   │   │
│   │   └── 📂 Core/                   # Núcleo do jogo
│   │       └── 🔹 GameEngine.cs       # Game Loop principal (Update/Draw)
│   │
│   ├── 📂 Game/                       # 🎮 LÓGICA DO JOGO
│   │   │
│   │   ├── 📂 Components/             # Componentes específicos do jogo
│   │   │   ├── 🔹 TransformComponent.cs    # Posição, rotação, escala
│   │   │   ├── 🔹 RenderComponent.cs       # Cor, tamanho, forma
│   │   │   └── 🔹 UnitStatsComponent.cs    # HP, velocidade, dano
│   │   │
│   │   └── 📂 Systems/                # Sistemas específicos do jogo
│   │       └── 🔹 RenderSystem.cs     # Renderiza entidades na tela
│   │
│   └── 📂 Mods/                       # 🔌 SISTEMA DE MODS
│       └── 🔹 ModManager.cs           # Carregador de dados JSON
│
└── 📂 data/                           # 💾 DADOS (DATA-DRIVEN)
    │
    └── 📂 base_mod/                   # Mod base com conteúdo padrão
        └── 📄 units.json              # Definições de unidades
        │
        └── (futuro)
            ├── 📄 items.json          # Definições de items
            ├── 📄 skills.json         # Definições de skills
            ├── 📄 factions.json       # Definições de facções
            └── 📄 maps/               # Mapas do jogo
```

## 🗂️ Separação de Responsabilidades

### 🔧 `src/Engine/` - O Motor (Reutilizável)
**O que é**: O núcleo técnico que pode ser reutilizado em qualquer jogo.

**Contém**:
- Sistema ECS genérico
- Renderização de baixo nível
- Game loop

**Princípio**: Sem lógica específica do jogo. Apenas infraestrutura.

---

### 🎮 `src/Game/` - A Lógica (Específico do jogo)
**O que é**: Implementação específica deste jogo (ARPG/RTS/Sandbox).

**Contém**:
- Componentes de gameplay (Transform, Stats, Render)
- Sistemas de gameplay (Render, Movement, Combat)
- Regras do jogo

**Princípio**: Usa o Engine, mas não é reutilizável em outros jogos.

---

### 🔌 `src/Mods/` - Sistema de Mods (Extensibilidade)
**O que é**: Camada que carrega dados externos (JSON) em memória.

**Contém**:
- ModManager (carregador)
- Classes de definição (UnitDefinition, ItemDefinition)
- Deserialização JSON

**Princípio**: Zero hardcode. Todo conteúdo vem de arquivos.

---

### 💾 `data/` - Dados (Data-Driven)
**O que é**: Arquivos JSON com todo o conteúdo do jogo.

**Contém**:
- `base_mod/`: Mod padrão do jogo
- (Futuro) `user_mods/`: Mods criados por jogadores
- (Futuro) `workshop_mods/`: Mods baixados da Steam Workshop

**Princípio**: Moddable. Qualquer pessoa pode editar ou adicionar.

---

## 📊 Fluxo de Dados entre Camadas

```
┌─────────────────────────────────────────────────────────┐
│                    data/ (JSON)                         │
│  units.json, items.json, skills.json, etc.              │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ carrega
                     ▼
┌─────────────────────────────────────────────────────────┐
│                  src/Mods/                              │
│  ModManager deserializa e armazena em memória          │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ fornece dados
                     ▼
┌─────────────────────────────────────────────────────────┐
│                  src/Game/                              │
│  Cria Entities com Components baseados nos dados       │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ usa infraestrutura
                     ▼
┌─────────────────────────────────────────────────────────┐
│                  src/Engine/                            │
│  Processa Entities/Systems no game loop                │
└─────────────────────────────────────────────────────────┘
```

## 🎯 Por que essa estrutura?

### ✅ Vantagens

1. **Separação de Concerns**: Engine ≠ Game ≠ Mods ≠ Data
2. **Reutilizabilidade**: Engine pode ser usado em outros projetos
3. **Moddability**: Data é 100% externa e editável
4. **Escalabilidade**: Adicionar novos sistemas/componentes é trivial
5. **Testabilidade**: Cada camada pode ser testada isoladamente

### 📐 Princípios Arquiteturais

- **Engine**: "Como funciona?" (ECS, rendering, input)
- **Game**: "O que faz?" (soldados, ataque, movimento)
- **Mods**: "Onde está?" (JSON, deserialização)
- **Data**: "Quanto?" (100 HP, 120 speed)

## 🔮 Evolução Futura da Estrutura

### Fase 2: Adicionar Sistemas
```
src/Game/Systems/
├── RenderSystem.cs
├── MovementSystem.cs        ← novo
├── CombatSystem.cs           ← novo
├── AISystem.cs               ← novo
└── CollisionSystem.cs        ← novo
```

### Fase 3: Adicionar Dados
```
data/base_mod/
├── units.json
├── items.json                ← novo
├── skills.json               ← novo
├── factions.json             ← novo
└── maps/
    ├── level1.json           ← novo
    └── level2.json           ← novo
```

### Fase 4: Suporte a Mods Externos
```
data/
├── base_mod/                 (conteúdo padrão)
├── user_mods/                ← novo
│   ├── my_custom_mod/
│   │   ├── mod.json          (metadados)
│   │   ├── units.json
│   │   └── items.json
│   └── another_mod/
│       └── ...
└── workshop_mods/            ← novo (Steam Workshop)
    └── downloaded_mod_123/
        └── ...
```

### Fase 5: Ferramentas
```
ModularGameEngine/
├── src/
├── data/
└── tools/                    ← novo
    ├── ModEditor/            (editor visual de mods)
    ├── MapEditor/            (editor de mapas)
    └── UnitTester/           (testar stats de unidades)
```

## 📝 Convenções de Nomenclatura

### Arquivos
- **Classes**: `PascalCase.cs` (ex: `GameEngine.cs`)
- **Interfaces**: `IPascalCase.cs` (ex: `IComponent.cs`)
- **JSON**: `lowercase.json` (ex: `units.json`)

### Código
- **Namespaces**: `ModularGameEngine.Camada.Subcamada`
- **Classes**: `PascalCase`
- **Métodos**: `PascalCase()`
- **Propriedades**: `PascalCase`
- **Campos privados**: `_camelCase`
- **Parâmetros**: `camelCase`

### JSON
- **Keys**: `snake_case` (ex: `"max_health"`)
- **IDs**: `lowercase_underscored` (ex: `"soldier_basic"`)

## 🔗 Dependências entre Arquivos

### Program.cs
```
Program.cs
  └─► GameEngine.cs
```

### GameEngine.cs
```
GameEngine.cs
  ├─► World.cs
  ├─► ModManager.cs
  └─► RenderSystem.cs
```

### World.cs
```
World.cs
  ├─► Entity.cs
  └─► ISystem.cs
```

### Entity.cs
```
Entity.cs
  └─► IComponent.cs
```

### Componentes
```
TransformComponent.cs
RenderComponent.cs     } ──► IComponent.cs
UnitStatsComponent.cs
```

### Sistemas
```
RenderSystem.cs ──► ISystem.cs
```

### ModManager
```
ModManager.cs
  └─► units.json (lê em runtime)
```

## 📦 Build Output

Após compilação (`dotnet build`):

```
bin/
└── Debug/
    └── net8.0/
        ├── ModularGameEngine.exe    # Executável
        ├── ModularGameEngine.dll
        ├── MonoGame.Framework.dll   # Dependências
        ├── System.Text.Json.dll
        └── data/                    # Copiado automaticamente
            └── base_mod/
                └── units.json
```

## ⚙️ Como o .csproj gerencia isso

```xml
<ItemGroup>
  <None Update="data\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

Isso garante que `data/` é sempre copiado para `bin/`.

---

## 🚀 Próximos Passos

1. ✅ Estrutura criada
2. ✅ ECS implementado
3. ✅ ModManager funcional
4. ✅ Renderização básica
5. ⬜ Instalar .NET SDK (veja [INSTALL.md](INSTALL.md))
6. ⬜ Compilar e testar
7. ⬜ Adicionar MovementSystem (veja [EXAMPLE_MOVEMENT.md](EXAMPLE_MOVEMENT.md))
8. ⬜ Seguir [ROADMAP.md](ROADMAP.md) para features avançadas

---

**Este arquivo serve como mapa do projeto.** Consulte sempre que precisar entender onde algo está ou deveria estar.
