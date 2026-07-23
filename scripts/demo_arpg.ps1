# Demonstra o controle ARPG: personagem principal movendo-se com cliques esquerdo
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

$LEFTDOWN = 0x02; $LEFTUP = 0x04

$proc = Get-Process ModularGameEngine -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { Write-Output "ERRO: jogo nao encontrado"; exit 1 }

[Win32]::SetForegroundWindow($proc.MainWindowHandle) | Out-Null
Start-Sleep -Milliseconds 600

$rect = New-Object Win32+RECT
[Win32]::GetWindowRect($proc.MainWindowHandle, [ref]$rect) | Out-Null

$clientX = $rect.Left + 8
$clientY = $rect.Top + 31

function Game-Click([int]$gx, [int]$gy) {
    $sx = $clientX + $gx
    $sy = $clientY + $gy
    [Win32]::SetCursorPos($sx, $sy) | Out-Null
    Start-Sleep -Milliseconds 100
    [Win32]::mouse_event($LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 80
    [Win32]::mouse_event($LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
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

# Estado inicial: personagem no centro, NPCs vagando
Take-Shot "C:\dev\cursor\ModularGameEngine\arpg_1_inicio.png"

# Clique 1: canto superior direito
Game-Click 1000 150
Start-Sleep -Milliseconds 1200
Take-Shot "C:\dev\cursor\ModularGameEngine\arpg_2_movendo_direita.png"

# Aguarda chegar
Start-Sleep -Milliseconds 2500

# Clique 2: canto inferior esquerdo
Game-Click 200 550
Start-Sleep -Milliseconds 1200
Take-Shot "C:\dev\cursor\ModularGameEngine\arpg_3_movendo_esquerda.png"

Start-Sleep -Milliseconds 2500

# Clique 3: centro-direita
Game-Click 800 360
Start-Sleep -Milliseconds 800
Take-Shot "C:\dev\cursor\ModularGameEngine\arpg_4_movendo_centro.png"

Write-Output "OK: 4 screenshots capturados"
