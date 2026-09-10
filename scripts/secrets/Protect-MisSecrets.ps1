param(
    [string]$InputPath = (Join-Path $PSScriptRoot '..\..\secrets.local.json'),
    [string]$OutputPath = (Join-Path $PSScriptRoot '..\..\secrets.enc.json')
)

$keyText = $env:MIS_SECRETS_KEY
if ([string]::IsNullOrWhiteSpace($keyText)) {
    throw 'MIS_SECRETS_KEY must contain a base64-encoded 32-byte key.'
}

$key = [Convert]::FromBase64String($keyText)
if ($key.Length -ne 32) {
    throw 'MIS_SECRETS_KEY must decode to exactly 32 bytes.'
}

if (-not (Test-Path -LiteralPath $InputPath)) {
    throw "Secrets input was not found: $InputPath"
}

$plaintext = [System.IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $InputPath))
$nonce = [byte[]]::new(12)
$tag = [byte[]]::new(16)
$ciphertext = [byte[]]::new($plaintext.Length)
[System.Security.Cryptography.RandomNumberGenerator]::Fill($nonce)

$cipher = [System.Security.Cryptography.AesGcm]::new($key, 16)
try {
    $cipher.Encrypt($nonce, $plaintext, $ciphertext, $tag)
}
finally {
    $cipher.Dispose()
}

$vault = [ordered]@{
    format = 1
    algorithm = 'AES-256-GCM'
    nonce = [Convert]::ToBase64String($nonce)
    ciphertext = [Convert]::ToBase64String($ciphertext)
    tag = [Convert]::ToBase64String($tag)
}

$json = $vault | ConvertTo-Json -Depth 3
[System.IO.File]::WriteAllText($OutputPath, $json + [Environment]::NewLine, [System.Text.UTF8Encoding]::new($false))
Write-Output "Encrypted secrets written to $OutputPath"
