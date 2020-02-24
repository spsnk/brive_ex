### Error con roslyn/csc.exe

El error en el que no se puede encontrar `bin/roslyn/csc.exe` se debe a un bug con el metodo build del compilador, especialmente cuando se carga un repositorio y se trata de correr la soluci�n.
Existen dos posibles soluciones:
#### 1. Hacer `unload` -> `reload` -> `rebuild` al proyecto brive_ex
Esto copiar� los archivos del compilador net din�mico roslyn al directorio bin.
#### 2. Desinstalar el paquete NuGet `Microsoft.Net.Compilers` e instalarlo de nuevo, usando  NuGet Package Manager, en el proyecto brive_ex.
Esto tambi�n copiar� los archivos necesarios al directorio bin.