🛠️ Sistema de Gestión para Taller de Carpintería
Sistema integral desarrollado para la administración, control y seguimiento de operaciones en un taller de carpintería. La solución está diseñada bajo una arquitectura moderna de microservicios y APIs REST, conectada a múltiples clientes (aplicación web administrativa y aplicación móvil híbrida).

🚀 Módulos Principales
Catálogo de Materiales y Acabados: Administración centralizada de insumos (melaminas, maderas, herrajes, etc.) organizados por categorías, marcas, espesores y soporte de imágenes dinámicas para previsualización rápida.

Gestión de Proyectos y Taller: Seguimiento del flujo de trabajo, control de stock y asociación de insumos a proyectos de carpintería activos.

Relevamiento Técnico: Módulo enfocado en la toma de datos y requerimientos en campo o visitas a clientes.

Clientes y Accesos: Control de cartera de clientes y autenticación robusta basada en tokens JWT.

Auditoría y Notificaciones: Registro automático de acciones mediante bitácora interna y sistema de notificaciones para los usuarios del taller.

💻 Tecnologías y Stack
Backend (Carpinteria.API)
Lenguaje: C# (.NET Core / ASP.NET Web API)

Base de Datos: MySQL / Entity Framework Core

Seguridad: Autenticación JWT y roles de usuario.

Servicios: Bitácoras, notificaciones y control de transacciones de base de datos.

Frontend Web (FrontendCarpinteriaWeb)
Framework: Blazor / ASP.NET Core con componentes de interfaz modernos.

Diseño: Bootstrap / MudBlazor para la interfaz administrativa y modales de gestión de materiales/categorías.

Frontend Móvil (FrontendCarpinteria / .NET MAUI Blazor Hybrid)
Plataforma: Aplicación híbrida orientada a dispositivos móviles (Android).

Interfaz: MudBlazor Mobile adaptado con navegación inferior fluida y grillas responsivas para consulta rápida en el taller.

📂 Estructura del Repositorio
Plaintext
├── backend/          # API REST en .NET Core, servicios y modelos de base de datos
├── frontend-web/     # Panel de administración web para gestión y altas
└── frontend-movil/   # Aplicación móvil para consulta en taller y catálogo visual

⚙️ Despliegue y Configuración
Base de Datos: Configurar la cadena de conexión en el archivo appsettings.json del backend apuntando a tu instancia de MySQL (ej. administrada mediante HeidiSQL).
Backend: Compilar y publicar la API (dotnet publish -c Release) para entornos de producción (como MonsterASP).
Frontends: Configurar la URL base del cliente HTTP (HttpClient) para que apunte a la API REST desplegada.

### Diagrama ER

<img width="2575" height="1845" alt="Diagrama ER Diciembre Carpinteria" src="https://github.com/user-attachments/assets/986f4ac5-2995-4b63-9c3d-194408568b3a" />
