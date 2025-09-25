-- Script de actualización de la base de datos para auditoría y gestión de usuarios
-- Ejecutar estos comandos en el orden presentado

-- 1. Actualizar tabla de usuarios (AspNetUsers) para campos adicionales
ALTER TABLE AspNetUsers 
ADD COLUMN Nombre VARCHAR(100) NULL,
ADD COLUMN Apellido VARCHAR(100) NULL,
ADD COLUMN FotoPerfil VARCHAR(500) NULL,
ADD COLUMN FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN FechaUltimaModificacion DATETIME NULL,
ADD COLUMN Estado BOOLEAN NOT NULL DEFAULT TRUE;

-- 2. Actualizar tabla propietarios para auditoría
ALTER TABLE propietarios 
ADD COLUMN FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN CreadoPor VARCHAR(450) NULL,
ADD COLUMN FechaModificacion DATETIME NULL,
ADD COLUMN ModificadoPor VARCHAR(450) NULL,
ADD COLUMN FechaEliminacion DATETIME NULL,
ADD COLUMN EliminadoPor VARCHAR(450) NULL,
ADD FOREIGN KEY (CreadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (ModificadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (EliminadoPor) REFERENCES AspNetUsers(Id);

-- 3. Actualizar tabla inquilinos para auditoría
ALTER TABLE inquilinos 
ADD COLUMN FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN CreadoPor VARCHAR(450) NULL,
ADD COLUMN FechaModificacion DATETIME NULL,
ADD COLUMN ModificadoPor VARCHAR(450) NULL,
ADD COLUMN FechaEliminacion DATETIME NULL,
ADD COLUMN EliminadoPor VARCHAR(450) NULL,
ADD FOREIGN KEY (CreadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (ModificadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (EliminadoPor) REFERENCES AspNetUsers(Id);

-- 4. Actualizar tabla inmuebles para auditoría
ALTER TABLE inmuebles 
ADD COLUMN FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN CreadoPor VARCHAR(450) NULL,
ADD COLUMN FechaModificacion DATETIME NULL,
ADD COLUMN ModificadoPor VARCHAR(450) NULL,
ADD COLUMN FechaEliminacion DATETIME NULL,
ADD COLUMN EliminadoPor VARCHAR(450) NULL,
ADD FOREIGN KEY (CreadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (ModificadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (EliminadoPor) REFERENCES AspNetUsers(Id);

-- 5. Actualizar tabla contratos para auditoría
ALTER TABLE contratos 
ADD COLUMN FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN CreadoPor VARCHAR(450) NULL,
ADD COLUMN FechaModificacion DATETIME NULL,
ADD COLUMN ModificadoPor VARCHAR(450) NULL,
ADD COLUMN FechaEliminacion DATETIME NULL,
ADD COLUMN EliminadoPor VARCHAR(450) NULL,
ADD FOREIGN KEY (CreadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (ModificadoPor) REFERENCES AspNetUsers(Id),
ADD FOREIGN KEY (EliminadoPor) REFERENCES AspNetUsers(Id);

-- 6. Crear índices para mejorar rendimiento de consultas de auditoría
CREATE INDEX idx_propietarios_creadoPor ON propietarios(CreadoPor);
CREATE INDEX idx_propietarios_fechaCreacion ON propietarios(FechaCreacion);
CREATE INDEX idx_inquilinos_creadoPor ON inquilinos(CreadoPor);
CREATE INDEX idx_inquilinos_fechaCreacion ON inquilinos(FechaCreacion);
CREATE INDEX idx_inmuebles_creadoPor ON inmuebles(CreadoPor);
CREATE INDEX idx_inmuebles_fechaCreacion ON inmuebles(FechaCreacion);
CREATE INDEX idx_contratos_creadoPor ON contratos(CreadoPor);
CREATE INDEX idx_contratos_fechaCreacion ON contratos(FechaCreacion);

-- 7. Actualizar el usuario administrador por defecto con los nuevos campos
UPDATE AspNetUsers 
SET Nombre = 'Administrador',
    Apellido = 'Sistema',
    FechaCreacion = CURRENT_TIMESTAMP,
    Estado = TRUE
WHERE Email = 'admin@inmo.local';

-- 8. Crear vista para reportes de auditoría
CREATE VIEW vista_auditoria AS
SELECT 
    'Propietario' as TipoEntidad,
    p.Id as EntidadId,
    CONCAT(p.Nombre, ' ', p.Apellido) as EntidadNombre,
    p.FechaCreacion,
    CONCAT(u1.Nombre, ' ', u1.Apellido) as CreadoPor,
    p.FechaModificacion,
    CONCAT(u2.Nombre, ' ', u2.Apellido) as ModificadoPor,
    p.FechaEliminacion,
    CONCAT(u3.Nombre, ' ', u3.Apellido) as EliminadoPor,
    p.Estado
FROM propietarios p
LEFT JOIN AspNetUsers u1 ON p.CreadoPor = u1.Id
LEFT JOIN AspNetUsers u2 ON p.ModificadoPor = u2.Id
LEFT JOIN AspNetUsers u3 ON p.EliminadoPor = u3.Id

UNION ALL

SELECT 
    'Inquilino' as TipoEntidad,
    i.Id as EntidadId,
    CONCAT(i.Nombre, ' ', i.Apellido) as EntidadNombre,
    i.FechaCreacion,
    CONCAT(u1.Nombre, ' ', u1.Apellido) as CreadoPor,
    i.FechaModificacion,
    CONCAT(u2.Nombre, ' ', u2.Apellido) as ModificadoPor,
    i.FechaEliminacion,
    CONCAT(u3.Nombre, ' ', u3.Apellido) as EliminadoPor,
    i.Estado
FROM inquilinos i
LEFT JOIN AspNetUsers u1 ON i.CreadoPor = u1.Id
LEFT JOIN AspNetUsers u2 ON i.ModificadoPor = u2.Id
LEFT JOIN AspNetUsers u3 ON i.EliminadoPor = u3.Id

UNION ALL

SELECT 
    'Inmueble' as TipoEntidad,
    im.Id as EntidadId,
    im.Direccion as EntidadNombre,
    im.FechaCreacion,
    CONCAT(u1.Nombre, ' ', u1.Apellido) as CreadoPor,
    im.FechaModificacion,
    CONCAT(u2.Nombre, ' ', u2.Apellido) as ModificadoPor,
    im.FechaEliminacion,
    CONCAT(u3.Nombre, ' ', u3.Apellido) as EliminadoPor,
    im.Estado
FROM inmuebles im
LEFT JOIN AspNetUsers u1 ON im.CreadoPor = u1.Id
LEFT JOIN AspNetUsers u2 ON im.ModificadoPor = u2.Id
LEFT JOIN AspNetUsers u3 ON im.EliminadoPor = u3.Id

UNION ALL

SELECT 
    'Contrato' as TipoEntidad,
    c.Id as EntidadId,
    CONCAT('Contrato #', c.Id) as EntidadNombre,
    c.FechaCreacion,
    CONCAT(u1.Nombre, ' ', u1.Apellido) as CreadoPor,
    c.FechaModificacion,
    CONCAT(u2.Nombre, ' ', u2.Apellido) as ModificadoPor,
    c.FechaEliminacion,
    CONCAT(u3.Nombre, ' ', u3.Apellido) as EliminadoPor,
    c.Estado
FROM contratos c
LEFT JOIN AspNetUsers u1 ON c.CreadoPor = u1.Id
LEFT JOIN AspNetUsers u2 ON c.ModificadoPor = u2.Id
LEFT JOIN AspNetUsers u3 ON c.EliminadoPor = u3.Id;

-- NOTA: Ejecutar este script después de hacer backup de la base de datos
-- Las columnas CreadoPor, ModificadoPor y EliminadoPor quedarán NULL para registros existentes
-- Los nuevos registros tendrán la información de auditoría completa
