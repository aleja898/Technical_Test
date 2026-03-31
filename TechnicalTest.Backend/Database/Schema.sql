-- SCRIPT SQL - ESTRUCTURA DE BASE DE DATOS
-- PRUEBA TÉCNICA INGENIERO DESARROLLADOR - ATI ASISTENCIA TÉCNICA INDUSTRIAL SAS
-- Sistema de Reservas de Hotel

-- =============================================
-- TABLA: Users
-- =============================================
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Apellidos] nvarchar(100) NOT NULL,
    [Mail] nvarchar(255) NOT NULL,
    [Direccion] nvarchar(500) NOT NULL,
    [Status] bit NOT NULL DEFAULT 1,
    [Created] datetime2 NOT NULL DEFAULT GETDATE(),
    [Updated] datetime2 NOT NULL DEFAULT '1900-01-01T00:00:00.0000000',
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

-- Índice único para email
CREATE UNIQUE INDEX [IX_Users_Mail] ON [Users] ([Mail]);

-- =============================================
-- TABLA: Hotels
-- =============================================
CREATE TABLE [Hotels] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(200) NOT NULL,
    [Pais] nvarchar(100) NOT NULL,
    [Latitud] float NOT NULL,
    [Longitud] float NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [Activo] bit NOT NULL DEFAULT 1,
    [NumeroHabitaciones] int NOT NULL,
    [Status] bit NOT NULL DEFAULT 1,
    [Created] datetime2 NOT NULL DEFAULT GETDATE(),
    [Updated] datetime2 NOT NULL DEFAULT '1900-01-01T00:00:00.0000000',
    CONSTRAINT [PK_Hotels] PRIMARY KEY ([Id])
);

-- =============================================
-- TABLA: Reservations
-- =============================================
CREATE TABLE [Reservations] (
    [Id] int NOT NULL IDENTITY,
    [IdUsuario] int NOT NULL,
    [IdHotel] int NOT NULL,
    [IdHabitacion] int NOT NULL,
    [FechaEntrada] datetime2 NOT NULL,
    [FechaSalida] datetime2 NOT NULL,
    [FechaReserva] datetime2 NOT NULL DEFAULT GETDATE(),
    [Estado] int NOT NULL DEFAULT 1, -- 1=Reservado, 2=Cancelado
    [Status] bit NOT NULL DEFAULT 1,
    [Created] datetime2 NOT NULL DEFAULT GETDATE(),
    [Updated] datetime2 NOT NULL DEFAULT '1900-01-01T00:00:00.0000000',
    CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reservations_Users_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Reservations_Hotels_IdHotel] FOREIGN KEY ([IdHotel]) REFERENCES [Hotels] ([Id]) ON DELETE NO ACTION
);

-- Índices para búsquedas eficientes
CREATE INDEX [IX_Reservations_IdHotel] ON [Reservations] ([IdHotel]);
CREATE INDEX [IX_Reservations_IdUsuario] ON [Reservations] ([IdUsuario]);
CREATE INDEX [IX_Reservations_IdHotel_FechaEntrada_FechaSalida] ON [Reservations] ([IdHotel], [FechaEntrada], [FechaSalida]);

-- =============================================
-- DATOS DE EJEMPLO (Opcional - para pruebas)
-- =============================================

-- Insertar usuarios de ejemplo
INSERT INTO [Users] ([Nombre], [Apellidos], [Mail], [Direccion]) VALUES 
('Juan', 'Pérez', 'juan.perez@email.com', 'Calle 123 #45-67'),
('María', 'Gómez', 'maria.gomez@email.com', 'Avenida 89 #12-34'),
('Carlos', 'Rodríguez', 'carlos.rodriguez@email.com', 'Carrera 56 #78-90');

-- Insertar hoteles de ejemplo
INSERT INTO [Hotels] ([Nombre], [Pais], [Latitud], [Longitud], [Descripcion], [NumeroHabitaciones]) VALUES 
('Hotel Plaza Central', 'Colombia', 4.6097, -74.0817, 'Hotel céntrico con todas las comodidades', 50),
('Hotel Playa Dorada', 'Colombia', 10.4694, -73.2538, 'Hotel frente al mar con vistas espectaculares', 30),
('Hotel Montaña Alta', 'Colombia', 5.0689, -75.5174, 'Hotel en la montaña con aire puro', 20);

-- =============================================
-- COMENTARIOS SOBRE EL DISEÑO
-- =============================================
/*
1. TABLAS NECESARIAS:
   - Users: Almacena información de usuarios (id autoincremental, nombre, apellidos, mail único, dirección)
   - Hotels: Almacena información de hoteles (id autoincremental, nombre, país, coordenadas, descripción, activo, número de habitaciones)
   - Reservations: Almacena reservas (id autoincremental, relaciones a usuarios/hoteles, fechas, estado)

2. ¿POR QUÉ SOLO 3 TABLAS?
   - No se necesita tabla de Habitaciones porque "Los hoteles disponen de N habitaciones iguales"
   - No se necesita tabla de TiposHabitacion porque "Todas las reservas serán para 1 habitación y 1 persona"
   - El control de disponibilidad se hace por conteo de reservas vs número de habitaciones

3. RELACIONES:
   - Reservations.IdUsuario → Users.Id (FK)
   - Reservations.IdHotel → Hotels.Id (FK)
   - ON DELETE NO ACTION para mantener integridad referencial

4. ÍNDICES:
   - IX_Users_Mail: Único para validar email único
   - IX_Reservations_IdHotel: Para búsquedas por hotel
   - IX_Reservations_IdUsuario: Para búsquedas por usuario
   - IX_Reservations_IdHotel_FechaEntrada_FechaSalida: Para verificar disponibilidad (overbooking)

5. ESTADOS:
   - Reservations.Estado: 1=Reservado, 2=Cancelado
   - Status en todas las tablas: Para control de auditoría (activo/inactivo)

6. REGLAS DE NEGOCIO IMPLEMENTADAS:
   - Email único en Users
   - Control de overbooking por conteo en la API
   - Fecha de checkout NO cuenta como día de reserva
   - Cancelación marca como cancelado (no elimina)
*/
