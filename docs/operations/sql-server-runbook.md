# Runbook de SQL Server

## Respaldo

Antes de una actualización o de mover el sistema a otro equipo, crear un respaldo de la base `MadridIntelligenceStudio`:

```sql
BACKUP DATABASE [MadridIntelligenceStudio]
TO DISK = 'C:\Backups\MadridIntelligenceStudio.bak'
WITH COPY_ONLY, CHECKSUM, COMPRESSION;
```

Verificar el archivo con `RESTORE VERIFYONLY FROM DISK = 'C:\Backups\MadridIntelligenceStudio.bak';`.

## Restauración

Restaurar en una instancia de prueba primero. Aplicar después las migraciones de EF Core y comprobar `/health/ready`, la lista de fixtures y una sincronización autorizada.

## Recuperación

Si una sincronización falla, los fixtures ya importados continúan disponibles. Revisar el historial de sincronizaciones, corregir la causa y repetir la actualización manual; las claves canónicas y los mapeos del proveedor evitan crear duplicados.

## Secretos

No respaldar ni versionar claves en texto plano. Transportar `secrets.enc.json` junto con la clave de descifrado mediante un canal seguro y establecer las variables de entorno en el equipo destino.
