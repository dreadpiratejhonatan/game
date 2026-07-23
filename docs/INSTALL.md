# Guia de Instalação - Modular Game Engine

## 📦 Instalar o .NET SDK

### Windows

1. **Download do .NET 8 SDK:**
   - Acesse: https://dotnet.microsoft.com/download/dotnet/8.0
   - Baixe o instalador "SDK x64" para Windows
   - Execute o instalador e siga as instruções

2. **Verificar instalação:**
   ```bash
   dotnet --version
   ```
   Deve mostrar: `8.0.x`

### Linux (Ubuntu/Debian)

```bash
# Adicionar repositório Microsoft
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instalar SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### macOS

```bash
# Usando Homebrew
brew install dotnet-sdk
```

## 🎮 Compilar e Executar o Jogo

### 1. Restaurar Dependências
```bash
cd ModularGameEngine
dotnet restore
```

Isso irá baixar:
- MonoGame.Framework.DesktopGL
- MonoGame.Content.Builder.Task
- System.Text.Json

### 2. Compilar
```bash
dotnet build
```

Você verá mensagens como:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 3. Executar
```bash
dotnet run
```

### 4. Executar em Release (Melhor Performance)
```bash
dotnet run --configuration Release
```

## 🐛 Solução de Problemas

### Erro: "No .NET SDKs were found"
- Certifique-se de que o .NET 8 SDK está instalado
- Reinicie o terminal após a instalação
- Verifique com `dotnet --version`

### Erro: "The type or namespace name 'MonoGame' could not be found"
- Execute `dotnet restore` para baixar os pacotes NuGet
- Verifique sua conexão com a internet

### Erro: "Could not load file or assembly 'System.Text.Json'"
- Atualize o SDK: `dotnet --list-sdks` deve mostrar 8.0 ou superior

### Janela não abre / Crash no início
- **Linux**: Instale as dependências do MonoGame:
  ```bash
  sudo apt-get install libsdl2-dev libopenal-dev
  ```
- **macOS**: Instale XQuartz se necessário

## 🔧 Desenvolvimento no Visual Studio / Rider / VS Code

### Visual Studio 2022
1. Abra `ModularGameEngine.csproj`
2. Pressione F5 para compilar e executar

### JetBrains Rider
1. File → Open → Selecione a pasta `ModularGameEngine`
2. Clique em Run (Shift+F10)

### VS Code
1. Instale a extensão "C# Dev Kit"
2. Abra a pasta do projeto
3. Pressione F5

## 📊 Estrutura de Arquivos Gerados

Após a compilação, você verá:

```
ModularGameEngine/
├── bin/
│   └── Debug/
│       └── net8.0/
│           ├── ModularGameEngine.exe (Windows)
│           ├── ModularGameEngine (Linux/Mac)
│           └── data/ (copiado automaticamente)
└── obj/
    └── Debug/
        └── net8.0/
```

O arquivo `data/base_mod/units.json` é automaticamente copiado para `bin/Debug/net8.0/data/` graças à configuração no `.csproj`.

## ✅ Checklist de Instalação

- [ ] .NET 8 SDK instalado (`dotnet --version` funciona)
- [ ] Projeto restaurado (`dotnet restore`)
- [ ] Projeto compilado (`dotnet build`)
- [ ] Jogo executado (`dotnet run`)
- [ ] Janela abriu mostrando formas coloridas
- [ ] Console mostra logs de carregamento

## 🎯 Primeira Execução - O que esperar

Quando executar pela primeira vez:

1. **Console mostra:**
   ```
   === MODULAR GAME ENGINE ===
   Inicializando sistemas...
   [ModManager] Iniciando carregamento de mods...
   [ModManager] 4 unidades carregadas de data/base_mod/units.json
   [ModManager] Carregamento concluído. 4 unidades registradas.
     - Spawned: Soldado Básico at (234, 456)
     - Spawned: Soldado Básico at (789, 123)
     ...
   Conteúdo carregado. 12 entidades criadas.
   ```

2. **Janela mostra:**
   - Fundo azul (CornflowerBlue)
   - 12 retângulos/círculos coloridos espalhados
   - Cores: azul, verde, vermelho, laranja

3. **Controles:**
   - `ESC`: Fecha o jogo

## 🚀 Próximo Passo

Após confirmar que o jogo está funcionando, você pode:
- Editar `data/base_mod/units.json` para adicionar novas unidades
- Modificar cores, tamanhos e stats
- Reiniciar o jogo para ver as mudanças

---

**Precisa de ajuda?** Verifique se todas as dependências estão instaladas.
