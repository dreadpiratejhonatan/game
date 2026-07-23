# ✅ Checklist de Verificação - Modular Game Engine

Use este checklist para verificar se tudo está funcionando corretamente.

## 📋 Fase 1: Verificação da Estrutura

- [x] Estrutura de pastas criada
  - [x] `src/Engine/ECS/`
  - [x] `src/Engine/Core/`
  - [x] `src/Game/Components/`
  - [x] `src/Game/Systems/`
  - [x] `src/Mods/`
  - [x] `data/base_mod/`

- [x] Arquivos do projeto
  - [x] `ModularGameEngine.csproj`
  - [x] `Program.cs`
  - [x] `.gitignore`

- [x] Documentação
  - [x] `README.md`
  - [x] `INSTALL.md`
  - [x] `ARCHITECTURE.md`
  - [x] `ROADMAP.md`
  - [x] `QUICK_REFERENCE.md`
  - [x] `EXAMPLE_MOVEMENT.md`
  - [x] `DIAGRAMS.md`
  - [x] `RESOURCES.md`
  - [x] `PROJECT_STRUCTURE.md`

## 📋 Fase 2: Verificação do Código

### Sistema ECS
- [x] `IComponent.cs` - Interface de componentes
- [x] `Entity.cs` - Entidade com componentes
- [x] `ISystem.cs` - Interface de sistemas
- [x] `World.cs` - Gerenciador ECS

### Componentes do Jogo
- [x] `TransformComponent.cs` - Posição, rotação, escala
- [x] `RenderComponent.cs` - Visual (cor, tamanho, forma)
- [x] `UnitStatsComponent.cs` - Stats (HP, velocidade, dano)

### Sistemas do Jogo
- [x] `RenderSystem.cs` - Renderização de placeholders

### Sistema de Mods
- [x] `ModManager.cs` - Carregador de JSON
- [x] Classes de definição (UnitDefinition, etc.)

### Core
- [x] `GameEngine.cs` - Game loop principal

### Dados
- [x] `data/base_mod/units.json` - 4 unidades definidas

## 📋 Fase 3: Instalação e Compilação

⚠️ **REQUER AÇÃO DO USUÁRIO**

### Instalar .NET SDK
- [ ] Baixar .NET 8 SDK de https://dotnet.microsoft.com/download/dotnet/8.0
- [ ] Instalar o SDK
- [ ] Abrir novo terminal (para atualizar PATH)
- [ ] Verificar com `dotnet --version`
  - Deve mostrar: `8.0.x`

### Compilar o Projeto
- [ ] Navegar até `ModularGameEngine/`
- [ ] Executar: `dotnet restore`
  - Deve baixar MonoGame e System.Text.Json
- [ ] Executar: `dotnet build`
  - Deve compilar sem erros
- [ ] Verificar pasta `bin/Debug/net8.0/` criada

## 📋 Fase 4: Primeira Execução

### Executar o Jogo
- [ ] Executar: `dotnet run`
- [ ] Janela abre (1280x720, fundo azul)
- [ ] Console mostra logs de carregamento
- [ ] 12 formas coloridas aparecem na tela

### Verificar Console Output
- [ ] `=== MODULAR GAME ENGINE ===`
- [ ] `[ModManager] 4 unidades carregadas`
- [ ] `Spawned: Soldado Básico at (...)`
- [ ] `Spawned: Arqueiro Básico at (...)`
- [ ] `Spawned: Tanque Pesado at (...)`
- [ ] `Spawned: Batedor Rápido at (...)`
- [ ] `Conteúdo carregado. 12 entidades criadas.`

### Verificar Janela do Jogo
- [ ] Fundo azul (CornflowerBlue)
- [ ] 3 retângulos azuis (#4A90E2) - Soldados
- [ ] 3 retângulos verdes (#50C878) - Arqueiros
- [ ] 3 retângulos vermelhos (#E74C3C) - Tanques
- [ ] 3 círculos laranjas (#F39C12) - Batedores

### Controles
- [ ] Pressionar `ESC` fecha o jogo

## 📋 Fase 5: Teste de Modificação de Dados

### Editar JSON
- [ ] Abrir `data/base_mod/units.json`
- [ ] Mudar a cor do `soldier_basic` de `#4A90E2` para `#FF0000`
- [ ] Salvar o arquivo
- [ ] Executar `dotnet run` novamente
- [ ] Verificar que soldados agora são vermelhos

### Adicionar Nova Unidade
- [ ] Editar `data/base_mod/units.json`
- [ ] Adicionar nova unidade (copiar e modificar uma existente)
- [ ] Mudar `id`, `name`, `color`
- [ ] Salvar
- [ ] Executar `dotnet run`
- [ ] Verificar que 15 entidades são criadas (3 da nova unidade)

## 📋 Fase 6: Extensão (Opcional)

### Adicionar MovementSystem
- [ ] Seguir tutorial em `EXAMPLE_MOVEMENT.md`
- [ ] Criar `VelocityComponent.cs`
- [ ] Criar `MovementSystem.cs`
- [ ] Modificar `GameEngine.cs`
- [ ] Compilar e testar
- [ ] Verificar que unidades se movem na tela

## 📋 Fase 7: Git (Recomendado)

### Inicializar Repositório
- [ ] `git init`
- [ ] `git add .`
- [ ] `git commit -m "Initial commit - ECS base + ModManager"`

### Criar Repositório Remoto (GitHub/GitLab)
- [ ] Criar repositório no GitHub
- [ ] `git remote add origin <url>`
- [ ] `git push -u origin main`

## 📋 Fase 8: Planejamento

### Definir Próximas Features
- [ ] Ler `ROADMAP.md`
- [ ] Escolher próxima feature a implementar
- [ ] Criar issues/tasks no GitHub (opcional)
- [ ] Começar desenvolvimento

## 🐛 Troubleshooting

### Problema: "No .NET SDKs were found"
**Solução**:
- Instalar .NET 8 SDK
- Reiniciar terminal
- Verificar com `dotnet --version`

### Problema: "The type or namespace name 'MonoGame' could not be found"
**Solução**:
- `dotnet restore`
- Verificar conexão com internet (NuGet precisa baixar pacotes)

### Problema: Janela não abre / Crash
**Solução**:
- **Linux**: `sudo apt-get install libsdl2-dev libopenal-dev`
- **macOS**: Instalar XQuartz se necessário
- Verificar logs no console para erro específico

### Problema: JSON não está sendo lido
**Solução**:
- Verificar que `data/` foi copiado para `bin/Debug/net8.0/`
- Verificar sintaxe do JSON em https://jsonlint.com/
- Verificar logs do console: `[ModManager] ... unidades carregadas`

### Problema: Entidades não aparecem na tela
**Solução**:
- Verificar posições aleatórias (X: 100-1180, Y: 100-620)
- Verificar cores no JSON (formato `#RRGGBB`)
- Adicionar log em `SpawnUnitsFromMods()` para debug

## 📊 Status do Projeto

**Versão Atual**: 0.1.0 (Protótipo Inicial)

**Completado**:
- ✅ Estrutura ECS
- ✅ ModManager (data-driven)
- ✅ Renderização placeholder
- ✅ Carregamento de unidades do JSON

**Próximo**:
- ⬜ Sistema de Input
- ⬜ Sistema de Movimento
- ⬜ Sistema de Seleção

**Última Verificação**: ___/___/_____ (preencher quando testar)

---

## 🎯 Resumo Rápido

Se tudo estiver ✅, você deve conseguir:

1. ✅ Executar `dotnet --version` (mostra 8.0+)
2. ✅ Executar `dotnet build` (compila sem erros)
3. ✅ Executar `dotnet run` (abre janela com formas coloridas)
4. ✅ Editar `units.json` e ver mudanças ao reiniciar

**Se conseguir fazer isso, o projeto está 100% funcional! 🎉**

---

## 📞 Próximas Ações

1. Instalar .NET SDK (veja `INSTALL.md`)
2. Compilar o projeto
3. Testar a execução
4. Editar `units.json` para experimentar
5. Seguir `EXAMPLE_MOVEMENT.md` para adicionar movimento
6. Consultar `ROADMAP.md` para próximas features

**Boa sorte! 🚀**
