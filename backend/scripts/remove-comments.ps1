# Script para remover todos os comentários dos arquivos .cs

$files = Get-ChildItem -Path . -Include *.cs -Recurse -File | 
    Where-Object { 
        $_.FullName -notmatch '\\bin\\|\\obj\\|\\Migrations\\|\.Designer\.cs$|\.g\.cs$' 
    }

$processed = 0
$errors = 0

foreach ($file in $files) {
    try {
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        $originalContent = $content
        
        # Remove XML documentation comments (///)
        $content = $content -replace '(?m)^\s*///.*$', ''
        
        # Remove block comments (/* */) - including multi-line
        $content = $content -replace '(?s)/\*.*?\*/', ''
        
        # Remove single-line comments (//) but preserve URLs and strings
        $lines = $content -split "`n"
        $newLines = @()
        
        foreach ($line in $lines) {
            $inString = $false
            $inChar = $false
            $result = ""
            $i = 0
            
            while ($i -lt $line.Length) {
                $char = $line[$i]
                $nextChar = if ($i + 1 -lt $line.Length) { $line[$i + 1] } else { '' }
                
                if (-not $inString -and -not $inChar) {
                    if ($char -eq '"') {
                        $inString = $true
                        $result += $char
                    }
                    elseif ($char -eq "'" -and $nextChar -ne "'") {
                        $inChar = $true
                        $result += $char
                    }
                    elseif ($char -eq '/' -and $nextChar -eq '/') {
                        break
                    }
                    else {
                        $result += $char
                    }
                }
                elseif ($inString) {
                    $result += $char
                    if ($char -eq '"' -and ($i -eq 0 -or $line[$i - 1] -ne '\')) {
                        $inString = $false
                    }
                }
                elseif ($inChar) {
                    $result += $char
                    if ($char -eq "'") {
                        $inChar = $false
                    }
                }
                
                $i++
            }
            
            $newLines += $result
        }
        
        $content = $newLines -join "`n"
        
        # Remove empty lines that were comments
        $content = $content -replace '(?m)^\s*$\r?\n', ''
        
        # Only write if content changed
        if ($content -ne $originalContent) {
            Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
            $processed++
            Write-Host "Processed: $($file.FullName)" -ForegroundColor Green
        }
    }
    catch {
        $errors++
        Write-Host "Error processing $($file.FullName): $_" -ForegroundColor Red
    }
}

Write-Host "`nCompleted: $processed files processed, $errors errors" -ForegroundColor Cyan

