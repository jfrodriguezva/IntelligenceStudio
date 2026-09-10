$key = [Environment]::GetEnvironmentVariable('AdminAccess__RefreshKey', 'User')
if ([string]::IsNullOrWhiteSpace($key)) { throw 'AdminAccess__RefreshKey is not configured for this user.' }
Write-Output $key
