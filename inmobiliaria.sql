-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 07-10-2025 a las 17:29:44
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `MontoMensual` decimal(18,2) NOT NULL,
  `InquilinoId` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `FechaTerminacionAnticipada` date DEFAULT NULL,
  `MotivoTerminacion` varchar(500) DEFAULT NULL,
  `MultaAplicada` decimal(18,2) DEFAULT NULL,
  `FechaAplicacionMulta` date DEFAULT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT current_timestamp(),
  `UsuarioCreador` varchar(100) DEFAULT NULL,
  `FechaTerminacionRegistro` datetime DEFAULT NULL,
  `UsuarioTerminacion` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `FechaInicio`, `FechaFin`, `MontoMensual`, `InquilinoId`, `InmuebleId`, `Estado`, `FechaTerminacionAnticipada`, `MotivoTerminacion`, `MultaAplicada`, `FechaAplicacionMulta`, `FechaCreacion`, `UsuarioCreador`, `FechaTerminacionRegistro`, `UsuarioTerminacion`) VALUES
(1, '2025-10-07', '2026-10-07', 450000.00, 1, 1, 0, NULL, NULL, NULL, NULL, '2025-10-07 12:01:43', NULL, NULL, NULL),
(2, '2025-10-07', '2026-10-07', 1000000.00, 1, 2, 1, NULL, NULL, NULL, NULL, '2025-10-07 12:03:24', 'admin@inmo.test', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(200) NOT NULL,
  `Tipo` varchar(50) NOT NULL,
  `TipoInmuebleId` int(11) DEFAULT NULL,
  `Ambientes` int(11) NOT NULL,
  `Precio` decimal(18,2) NOT NULL,
  `PropietarioId` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `Direccion`, `Tipo`, `TipoInmuebleId`, `Ambientes`, `Precio`, `PropietarioId`, `Estado`) VALUES
(1, 'casagenerica1', 'casa', 1, 4, 450000.00, 1, 1),
(2, 'calleyorch', 'Galpón', 7, 2, 800000.00, 2, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `Domicilio` varchar(200) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `Dni`, `Apellido`, `Nombre`, `Email`, `Telefono`, `Domicilio`, `Estado`) VALUES
(1, '656565655656', 'alpargata', 'eusebio', 'safsaf@gmial.com', '26664898987', 'el baldio de aca a la vuelta', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `NumeroPago` int(11) NOT NULL,
  `FechaPago` date NOT NULL,
  `Detalle` varchar(200) NOT NULL,
  `Importe` decimal(18,2) NOT NULL,
  `ContratoId` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `FechaCreacion` datetime NOT NULL DEFAULT current_timestamp(),
  `UsuarioCreador` varchar(100) DEFAULT NULL,
  `FechaAnulacion` datetime DEFAULT NULL,
  `UsuarioAnulacion` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `NumeroPago`, `FechaPago`, `Detalle`, `Importe`, `ContratoId`, `Estado`, `FechaCreacion`, `UsuarioCreador`, `FechaAnulacion`, `UsuarioAnulacion`) VALUES
(2, 2, '2025-10-07', 'Mensualidad', 98998988.00, 2, 1, '2025-10-07 12:10:58', 'admin@inmo.test', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `Domicilio` varchar(200) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `Dni`, `Apellido`, `Nombre`, `Email`, `Telefono`, `Domicilio`, `Estado`) VALUES
(1, '12345678', 'Pérez', 'Juan', 'juanperez@mail.com', '123456789', 'Calle Falsa 123', 1),
(2, '87654321', 'Gómez', 'María', 'mariagomez@mail.com', '987654321', 'Av. Siempreviva 742', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipos_inmuebles`
--

CREATE TABLE `tipos_inmuebles` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(200) DEFAULT NULL,
  `UsoPermitido` varchar(20) NOT NULL,
  `EsComercial` tinyint(1) NOT NULL DEFAULT 0,
  `Estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos_inmuebles`
--

INSERT INTO `tipos_inmuebles` (`Id`, `Nombre`, `Descripcion`, `UsoPermitido`, `EsComercial`, `Estado`) VALUES
(1, 'Casa', 'Casa unifamiliar con jardín y garage', 'Residencial', 0, 1),
(2, 'Departamento', 'Unidad habitacional en edificio', 'Residencial', 0, 1),
(3, 'PH', 'Propiedad horizontal con entrada independiente', 'Residencial', 0, 1),
(4, 'Local Comercial', 'Espacio para actividades comerciales', 'Comercial', 1, 1),
(5, 'Oficina', 'Espacio para trabajo administrativo', 'Comercial', 1, 1),
(6, 'Depósito', 'Espacio para almacenamiento', 'Mixto', 1, 1),
(7, 'Galpón', 'Edificación amplia para industria o depósito', 'Comercial', 1, 1),
(8, 'Duplex', 'Vivienda de dos plantas', 'Residencial', 0, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `ClaveHash` varchar(255) NOT NULL,
  `Rol` varchar(20) NOT NULL,
  `Apellido` varchar(100) DEFAULT NULL,
  `Nombre` varchar(100) DEFAULT NULL,
  `AvatarUrl` varchar(300) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `FechaAlta` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Email`, `ClaveHash`, `Rol`, `Apellido`, `Nombre`, `AvatarUrl`, `Estado`, `FechaAlta`) VALUES
(1, 'admin@inmo.test', 'Admin123!', 'Administrador', 'Administrador', 'Jefaso', '/uploads/avatars/avatar_1_20251002173758.png', 1, '2025-10-02 19:56:28'),
(6, 'juanjo@gmail.com', 'Pitorrico123', 'Empleado', 'torrico', 'juanpi', NULL, 1, '2025-10-02 18:55:12');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InquilinoId` (`InquilinoId`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`),
  ADD KEY `TipoInmuebleId` (`TipoInmuebleId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Dni` (`Dni`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UniqueNumeroPagoContrato` (`ContratoId`,`NumeroPago`),
  ADD KEY `ContratoId` (`ContratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Dni` (`Dni`);

--
-- Indices de la tabla `tipos_inmuebles`
--
ALTER TABLE `tipos_inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Nombre` (`Nombre`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `tipos_inmuebles`
--
ALTER TABLE `tipos_inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FK_Contratos_Inmuebles` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`),
  ADD CONSTRAINT `FK_Contratos_Inquilinos` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `FK_Inmuebles_Propietarios` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`),
  ADD CONSTRAINT `FK_Inmuebles_TiposInmuebles` FOREIGN KEY (`TipoInmuebleId`) REFERENCES `tipos_inmuebles` (`Id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `FK_Pagos_Contratos` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
