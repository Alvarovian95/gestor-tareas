# Gestor de Tareas

> Organiza tu día, prioriza lo importante y que no se te escape nada.

App full-stack de gestión de tareas con autenticación JWT, categorías, prioridades y filtros. Desarrollada con Angular 19, .NET 9 y SQL Server.

🚧 **Proyecto en desarrollo activo** 🚧

## Stack Tecnológico

- **Frontend:** Angular 19 (standalone components, signals), Tailwind CSS, Angular Material
- **Backend:** .NET 9 Web API, Entity Framework Core, JWT + Refresh Tokens
- **Base de datos:** SQL Server 2022
- **Infraestructura:** Docker, Docker Compose
- **Testing:** xUnit, FluentAssertions, Jasmine

## Arquitectura

Backend organizado en capas siguiendo principios de Clean Architecture:

```
GestorTareas.Api            → Endpoints y configuración
GestorTareas.Application    → Casos de uso y lógica de aplicación
GestorTareas.Domain         → Entidades y reglas de negocio
GestorTareas.Infrastructure → Acceso a datos y servicios externos
GestorTareas.Tests          → Tests unitarios
```

## Cómo levantar el proyecto

Requisitos: Docker Desktop instalado.

```bash
git clone https://github.com/TU_USUARIO/gestor-tareas.git
cd gestor-tareas
docker-compose up
```

Una vez arrancado:
- **Frontend:** http://localhost:4200
- **API + Swagger:** http://localhost:5000/swagger

## Roadmap

- [x] Configuración inicial del proyecto
- [ ] Modelo de datos y migraciones EF Core
- [ ] Autenticación con JWT y refresh tokens
- [ ] CRUD de tareas
- [ ] Categorías y prioridades
- [ ] Filtros y búsqueda
- [ ] Dashboard con estadísticas
- [ ] Tests unitarios
- [ ] Documentación final y GIF de demo

## Autor

**Álvaro Vian Hernández** — [LinkedIn](https://www.linkedin.com/in/alvaro-vian) · [GitHub](https://github.com/Alvarovian95)

## Licencia

MIT