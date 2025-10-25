# Caritas-Application

Este repositorio contiene una aplicación desarrollada para la organización Cáritas como parte de un proyecto escolar.  
El proyecto está dividido en dos partes principales: un backend en C# (.NET) y un frontend en TypeScript.

---

## Tecnologías utilizadas

- **Backend:** C# (.NET)  
- **Frontend:** TypeScript, HTML, CSS  
- **Control de versiones:** Git  
- **Plataforma de desarrollo:** GitHub

---

## Estructura del proyecto
```
/
├── Backend/
│   |──Backend.Infraestructure
|       ├── Database/
│       ├── Dtos/
│       ├── Extensions/
|       ├── Implementations/
│       ├── Models/
│       ├── Objects/
│       ├── .gitignore
│       └── appsettings.json
|   |──Backend.test
|       ├── Implementations/
│       ├── TestHelpers/
│       └── Backend.Tests.csproj
|   └──Backend/
│       ├── Controllers/
│       ├── Dtos/
│       ├── Implementations/
│       ├── Interfaces/
│       ├── Properties/
│       ├── .gitignore
│       ├── Backend.csproj
│       ├── Backend.csproj.user
│       ├── Backend.http
│       ├── Program.cs
│       ├── appsettings.Development.json
│       └── appsettings.json
├── Fontend/
|   └──Admin Dashboard for Reservations/    
│       ├── src/
│       ├── public/
│       ├── package.json
│       └── tsconfig.json
│   ├──package-lock.json
|   └──package.json
├── .gitignore
├── .gitignore.txt
└── README.md
```
### Descripción de carpetas

- **Backend/**: contiene la lógica del servidor y las API.  
- **Fontend/**: contiene el código del cliente (interfaz web).  
- **.gitignore**: define los archivos y carpetas que Git no debe rastrear.

---

## Instalación

### Requisitos previos

- [.NET SDK](https://dotnet.microsoft.com/) (versión 8 o superior)  
- [Node.js](https://nodejs.org/) (versión LTS recomendada)  
- Git

### Clonar el repositorio

```bash
git clone https://github.com/JesusEduardoEscobar/Caritas-Application.git
cd Caritas-Application
```

## Configuracion del backend
- Entrar en la carpeta del backend:
```bash 
cd Backend
```

- Restaurar dependencias:
```bash 
dotnet restore
```

- Compilar el proyecto:
```bash 
dotnet build
```

- Ejecutar la API:
```bash 
dotnet run
```
**Nota:** usar visual studio community para poder ehercutar con mas comodidad

## Configuracion del frontend
Configuración del Frontend

- Entrar en la carpeta del frontend:
```bash 
cd ../Fontend
```

- Instalar dependencias:
```bash 
npm install
```

- Iniciar el servidor de desarrollo:
```bash 
npm run dev
```

Acceder a la aplicación en (http://localhost:3000)

## Variables de entorno

Configura las variables necesarias en los archivos de entorno:

**Backend**: en appsettings.json o variables del sistema (ConnectionStrings, JWTSettings, etc.).

**Frontend**: en un archivo .env (por ejemplo, REACT_APP_API_URL=http://localhost:5000).

## Créditos y autores

Proyecto desarrollado por Jesús Eduardo Escobar y colaboradores.
Repositorio original: Caritas-Application
