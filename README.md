# MoviesKnowledgeGraphAkademia - Migrador de Películas (3 OCs)

Aplicación de consola en .NET 6 para migrar información de películas desde archivos JSON (formato OMDB) hacia una plataforma GNOSS basada en grafos de conocimiento.

## 📋 Características

- Carga de películas, personas (actores, directores, escritores) y géneros
- Gestión automática de GUIDs para evitar duplicados
- Procesamiento en lotes con manejo de errores
- Cache local en archivos CSV
- Soporte para carga incremental o desde cero

## 🚀 Requisitos Previos

- .NET 6.0 SDK o superior
- Acceso a una instancia de GNOSS
- Archivos JSON con información de películas en formato OMDB
- Token de autenticación OAuth para GNOSS

## 📦 Instalación

1. Clona el repositorio:
```bash
git clone https://github.com/tu-usuario/OmdbToGnoss.git
cd OmdbToGnoss
```

2. Restaura los paquetes NuGet:
```bash
dotnet restore
```

3. Configura OAuth:
   - Copia `Config/ConfigOAuth/OAuth_V3.config.template` a `Config/ConfigOAuth/OAuth_V3.config`
   - Edita el archivo con tus credenciales:
```xml
<?xml version="1.0" encoding="utf-8"?>
<config>
  <communityShortName>tu-comunidad</communityShortName>
  <developerEmail>tu-email@ejemplo.com</developerEmail>
  <accessToken>tu-token-aqui</accessToken>
</config>
```

4. Prepara los datos:
   - Coloca los archivos JSON de películas en la carpeta `Data/`
   - Asegúrate de que los archivos OWL estén en la carpeta `OWLs/`

## 🎯 Uso

1. Ejecuta la aplicación:
```bash
dotnet run --project OmdbToGnoss.CORE
```

2. Sigue las instrucciones en consola:
   - Decide si quieres hacer una carga desde cero o incremental
   - Selecciona cómo obtener los nombres de las ontologías
   - Especifica el número de películas a cargar

### Ejemplo de ejecución:
```
¡Bienvenido!

¿Quieres realizar una carga desde cero de películas? (Y/n) -> Y
¿Quieres realizar una carga desde cero de personas? (Y/n) -> Y
¿Quieres realizar una carga desde cero de generos? (Y/n) -> Y

Selecciona cómo obtener las ontologías:
1. Obtenerlos de la comunidad
2. Obtenerlos desde tu equipo (Directorio: OWLs)
3. Introducir los nombres de las ontologías de manera manual

> 2

Introduce el número de películas que quieres cargar (Por defecto: 1 / Máximo: 600)
> 50
```

## 📁 Estructura del Proyecto

```
OmdbToGnoss.CORE/
├── Config/
│   └── ConfigOAuth/
│       └── OAuth_V3.config.template
├── Data/
│   ├── *.json (archivos de películas)
│   ├── Peliculas.csv (cache de GUIDs)
│   ├── Personas.csv (cache de GUIDs)
│   └── Generos.csv (cache de GUIDs)
├── OWLs/
│   ├── urnpln25pelicula.owl
│   ├── urnpln25persona.owl
│   └── urnpln25genero.owl
├── Cargador.cs
├── Extractor.cs
├── Utils.cs
├── Program.cs
└── OmdbToGnoss.CORE.csproj
```

## 🔧 Configuración Avanzada

### Cache CSV
Los archivos CSV en `Data/` mantienen la relación entre nombres de recursos y sus GUIDs. Esto permite:
- Evitar duplicados en cargas sucesivas
- Mantener referencias consistentes
- Realizar cargas incrementales

### Limpieza de Cache
Si necesitas comenzar desde cero, la aplicación te preguntará si deseas limpiar el cache para cada tipo de recurso.

## 🐛 Solución de Problemas

### Error: "The process cannot access the file"
Si obtienes este error al limpiar archivos CSV, la aplicación reintentará automáticamente hasta 3 veces.

### Error: "No se encontraron ontologías"
Asegúrate de que los archivos OWL estén en la carpeta correcta y sigan el patrón de nombres esperado.

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Fork el proyecto
2. Crea tu Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la Branch (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📧 Contacto

Para preguntas o soporte, contacta a: sergiodedios@gnoss.com

---
Desarrollado por GNOSS en el marco de GNOSS Akademia