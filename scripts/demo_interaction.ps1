# Demonstra a interatividade do jogo: seleção em caixa + comando de movimento,
# capturando screenshots de cada etapa.
Add-Type -AssemblyName System.Windows.Forms, System.Drawing

Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extra);
    public struct RECT { public int Left, Top, Right, Bottom; }
}
"@

$LEFTDOWN = 0x02; $LEFTUP = 0x04; $RIGHTDOWN = 0x08; $RIGHTUP = 0x10

$proc = Get-Process ModularGameEngine -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { Write-Output "ERRO: jogo nao encontrado"; exit 1 }

[Win32]::SetForegroundWindow($proc.MainWindowHandle) | Out-Null
Start-Sleep -Milliseconds 600

$rect = New-Object Win32+RECT
[Win32]::GetWindowRect($proc.MainWindowHandle, [ref]$rect) | Out-Null

# Offset da área cliente (bordas da janela): 8px laterais, ~31px barra de título
$clientX = $rect.Left + 8
$clientY = $rect.Top + 31

function Game-Point([int]$gx, [int]$gy) {
    return @(($clientX + $gx), ($clientY + $gy))
}

function Take-Shot([string]$path) {
    $r = New-Object Win32+RECT
    [Win32]::GetWindowRect($proc.MainWindowHandle, [ref]$r) | Out-Null
    $w = $r.Right - $r.Left; $h = $r.Bottom - $r.Top
    $bmp = New-Object System.Drawing.Bitmap $w, $h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen($r.Left, $r.Top, 0, 0, (New-Object System.Drawing.Size $w, $h))
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose(); $bmp.Dispose()
}

# --- ETAPA 1: Seleção em caixa (arrasta do canto superior esquerdo até o centro-direita) ---
$start = Game-Point 60 60
$end   = Game-Point 900 640

[Win32]::SetCursorPos($start[0], $start[1]) | Out-Null
Start-Sleep -Milliseconds 150
[Win32]::mouse_event($LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 100

# Movimento gradual para o jogo registrar o arrasto
$steps = 20
for ($i = 1; $i -le $steps; $i++) {
    $x = $start[0] + (($end[0] - $start[0]) * $i / $steps)
    $y = $start[1] + (($end[1] - $start[1]) * $i / $steps)
    [Win32]::SetCursorPos([int]$x, [int]$y) | Out-Null
    Start-Sleep -Milliseconds 25
}

Take-Shot "C:\dev\cursor\ModularGameEngine\demo_1_caixa_selecao.png"

[Win32]::mouse_event($LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 400

Take-Shot "C:\dev\cursor\ModularGameEngine\demo_2_selecionadas.png"

# --- ETAPA 2: Comando de movimento (clique direito no canto inferior direito) ---
$moveTo = Game-Point 1100 550
[Win32]::SetCursorPos($moveTo[0], $moveTo[1]) | Out-Null
Start-Sleep -Milliseconds 150
[Win32]::mouse_event($RIGHTDOWN, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32]::mouse_event($RIGHTUP, 0, 0, 0, [UIntPtr]::Zero)

# Screenshot no meio do caminho (unidades em movimento, marcadores visíveis)
Start-Sleep -Milliseconds 900
Take-Shot "C:\dev\cursor\ModularGameEngine\demo_3_movendo.png"

# Screenshot final (formação em grade no destino)
Start-Sleep -Milliseconds 4000
Take-Shot "C:\dev\cursor\ModularGameEngine\demo_4_chegaram.png"

Write-Output "OK: 4 screenshots capturados"
